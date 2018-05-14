namespace Invio.QueryProvider.Test

open System
open System.IO
open System.Text.RegularExpressions

module CsvReader =
    open System.Globalization

    let inline trace (fn : 'a -> string) value =
        #if DEBUG
            if not (Environment.NewLine.StartsWith("\r")) then
                Console.Error.WriteLine(fn(value) + "\r") // Unit test debug output needs CR
            else
                Console.Error.WriteLine(fn(value))
            value
        #else
            value
        #endif

    let private binaryRegex =
        new Regex(@"0x([0-9A-F]{2})+", RegexOptions.Compiled ||| RegexOptions.IgnoreCase);

    let rec private findClosingQuote (line : string) (pos : int) =
        match line.IndexOf('"', pos) with
            | ix when ix >= 0 ->
                if ix < line.Length - 1 && line.[ix + 1] = '"' then
                    findClosingQuote line (ix + 2)
                else
                    ix
            | ix -> ix

    let private readQuotedValue (line : string) (pos : int) (source : TextReader) =
        Seq.unfold
            (fun (line, ix) ->
                if ix < 0 then None
                elif line = null then raise (new InvalidDataException "A quoted string was never closed")
                else
                    match findClosingQuote line ix with
                        | closeQuotePos when closeQuotePos >= 0 ->
                            // Quote Found
                            trace
                                (fun _ -> sprintf "Found closing quote at position %i (...%s...)" closeQuotePos <| line.Substring(Math.Max(0, closeQuotePos - 10), 20))
                                Some ((line.Substring(ix, closeQuotePos - ix), line, closeQuotePos), (null, -1))
                        | _ ->
                            let nextLine = trace (sprintf "Quoted string continues onto next line: %s") <| source.ReadLine()
                            // Quote Not Found, emit the current segment and continue.
                            // Note: this normalizes quoted newlines based on the current environment
                            Some ((line.Substring(ix) + Environment.NewLine, null, -1), (nextLine, 0)))
            (line, pos + 1)
        |> Seq.fold
            (fun (result, _, _) (segment, currentLine, endPos) -> (result + segment, currentLine, endPos))
            (String.Empty, null, -1)

    let private readCsvFieldValue (line : string) (pos : int) (source : TextReader) =
        match line.[pos] with
            | '"' ->
                let (fieldValue, currentLine, endQuoteIx) =
                    readQuotedValue
                        (trace (fun _ -> sprintf "Reading quoted value starting at position %i (...%s...)" (pos + 1) <| line.Substring(Math.Max(0, pos - 10), 20)) line)
                        (pos + 1)
                        source
                let endIx = currentLine.IndexOf(',', endQuoteIx)
                (fieldValue, true, currentLine, if endIx > 0 then endIx + 1 else -1)
            | _ ->
                match line.IndexOf(',', pos) with
                    | ix when ix < 0 -> (line.Substring(pos), false, line, -1)
                    | endIx when endIx > pos -> (line.Substring(pos, endIx - pos), false, line, endIx + 1)
                    // Treat and unquoted blank as null (use ,"", for empty string)
                    | _ -> (null, false, line, pos + 1)

    let private dateTimeFormats = [|
        "yyyy-MM-dd HH:mm:ss"
        "yyyy-MM-dd HH:mm:ss.fff"
        "yyyy-MM-dd'T'HH:mm:ss"
        "yyyy-MM-dd'T'HH:mm:ss.fff"
        "yyyy-MM-dd"
    |]

    let inferType (str : string) : obj =
        let mutable intValue = 0;
        let mutable decimalValue = 0M;
        let mutable doubleValue = 0.0;
        let mutable booleanValue = false;
        let mutable dateTimeValue = DateTime.MinValue;
        match str with
            | null -> null
            | _ when Int32.TryParse(str, &intValue) -> intValue :> obj
            // Prefer Decimal to Double when there are a small number of digits
            | _ when str.Length <= 30 && Decimal.TryParse(str, &decimalValue) -> decimalValue :> obj
            | _ when Double.TryParse(str, &doubleValue) -> doubleValue :> obj
            | _ when Boolean.TryParse(str, &booleanValue) -> booleanValue :> obj
            | _ when DateTime.TryParseExact(str, dateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, &dateTimeValue) ->
                dateTimeValue :> obj
            | _ when binaryRegex.IsMatch(str) ->
                (Seq.unfold
                    (fun pos ->
                        if pos < str.Length then
                            Some (Convert.ToByte(str.Substring(pos, 2), 16), pos + 2)
                            else None)
                    2)
                |> Seq.toArray
                :> obj
            | _ -> str :> obj

    let private readCsvLine (line : string) (source : TextReader) : obj array =
        Seq.unfold
            (fun (currentLine : string, pos : int) ->
                if pos < 0 then
                    trace (fun _ -> "EOL") None
                elif pos >= currentLine.Length then
                    // The last , was at the end of the line, emit a final null value
                    trace (fun _ -> "Read value: null") Some (null, (line, -1))
                else
                    match readCsvFieldValue currentLine pos source with
                        | (value, true, newLine, newPos) ->
                            trace (fun _ -> sprintf "Read value: \"%s\"" value) Some (value :> obj, (newLine, newPos))
                        | (value, false, newLine, newPos) ->
                            trace (fun _ -> sprintf "Read value: %s" value) Some (inferType value, (newLine, newPos)))
            (line, 0)
        |> Seq.toArray
        |> trace (fun result -> sprintf "Read values (%i): [|%s|]" result.Length (String.Join(", ", Seq.map (fun obj -> match obj with null -> "null" | _ -> obj.ToString()) result)))

    let readCsvData (source : TextReader) : seq<obj array> =
        Seq.unfold
            (fun _ ->
                match source.ReadLine() with
                    | null -> None
                    | line -> Some ((trace (fun _ -> sprintf "Reading CSV Line: %s" line) (readCsvLine line source)), ()))
            ()
