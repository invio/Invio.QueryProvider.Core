namespace Invio.QueryProvider.Test.Models

open System
open System.ComponentModel
open System.Text.RegularExpressions

[<type: AllowNullLiteral>]
[<type: TypeConverter(typedefof<PhoneNumberTypeConverter>)>]
type PhoneNumber(areaCode : int, prefix : int, suffix : int) =
    static member private phoneRegex =
        new Regex("^\((?'area'[0-9]+)\) (?'prefix'[0-9]{3})-(?'suffix'[0-9]{4})$", RegexOptions.Compiled)

    member public this.AreaCode = areaCode
    member public this.Prefix = prefix
    member public this.Suffix = suffix

    override this.ToString() =
        sprintf "(%d) %d-%d" areaCode prefix suffix

    override this.GetHashCode() =
        hash (areaCode, prefix, suffix)

    override this.Equals(other : obj) =
        match other with
            | :? PhoneNumber as p -> (areaCode, prefix, suffix) = (p.AreaCode, p.Prefix, p.Suffix)
            | _ -> false

    static member public Parse(str : String) : PhoneNumber =
        match PhoneNumber.phoneRegex.Match(str) with
            | m when m.Success ->
                new PhoneNumber(
                   Int32.Parse(m.Groups.["area"].Value),
                   Int32.Parse(m.Groups.["prefix"].Value),
                   Int32.Parse(m.Groups.["suffix"].Value)
                )
            | _ -> failwithf "Unable to parse phone number string: %s" str

and PhoneNumberTypeConverter() =
    inherit TypeConverter()
    override this.CanConvertFrom(_, sourceType: Type) =
        sourceType = typedefof<String>
    override this.ConvertFrom(_, _, value: obj) =
        match value with
            | null -> null
            | :? String as str -> PhoneNumber.Parse(str) :> obj
            | _ -> failwithf "The type %s cannot be converted to the type PhoneNumber." (value.GetType().Name)
    override this.CanConvertTo(_, destinationType: Type) =
        destinationType = typedefof<String>
    override this.ConvertTo(_, _, value: obj, destinationType: Type) =
        match value with
            | null -> null
            | :? PhoneNumber as phone ->
                if destinationType = typedefof<String> then
                    phone.ToString() :> obj
                else
                    failwithf "PhoneNumber cannot be converted to type %s." (destinationType.Name)
            | _ -> failwithf "PhoneNumberTypeConverter does not support values of type %s." (value.GetType().Name)

