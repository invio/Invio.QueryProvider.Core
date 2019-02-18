namespace Invio.QueryProvider.Test.Models

open System
open System.ComponentModel
open System.Text.RegularExpressions

[<type: TypeConverter(typedefof<HyperlinkTypeConverter>)>]
type Hyperlink =
    struct
        val Title: String
        val Url: Uri
        new(title: String, url: Uri) =  { Title = title; Url = url; }

        override this.ToString() =
            sprintf "%s#%s#" this.Title (this.Url.ToString())

//        override this.GetHashCode() =
//            hash (this.Title, this.Url)
//
//        override this.Equals(other : obj) =
//            match other with
//                | :? PhoneNumber as p -> (this.Title, this.Url) = (this.Title, this.Url)
//                | _ -> false
        static member op_Equality(h1: Hyperlink, h2: Hyperlink) =
            h1 = h2

        static member op_Inequality(h1: Hyperlink, h2: Hyperlink) =
            h1 <> h2

        static member op_Explicit(h: Hyperlink): String =
            h.ToString()

        static member private hyperlinkRegex =
            new Regex("^(?'title'[^#]+)?#(?'url'[^#]*)#$", RegexOptions.Compiled)

        static member public Parse(hyperlink: String) : Hyperlink =
            match Hyperlink.hyperlinkRegex.Match(hyperlink) with
                | m when m.Success ->
                    new Hyperlink(
                        (if m.Groups.["title"].Success then m.Groups.["title"].Value else null),
                        new Uri(m.Groups.["url"].Value, UriKind.RelativeOrAbsolute)
                     )
                | _ -> failwithf "The specified string '%s' could not be parsed as a Hyperlink." hyperlink
    end

and HyperlinkTypeConverter() =
    inherit TypeConverter()
    override this.CanConvertFrom(_, sourceType: Type) =
        sourceType = typedefof<String>
    override this.ConvertFrom(_, _, value: obj) =
        match value with
            | null -> null
            | :? String as str -> Hyperlink.Parse(str) :> obj
            | _ -> failwithf "The type %s cannot be converted to the type Hyperlink." (value.GetType().Name)
    override this.CanConvertTo(_, destinationType: Type) =
        destinationType = typedefof<String>
    override this.ConvertTo(_, _, value: obj, destinationType: Type) =
        match value with
            | null -> null
            | :? Hyperlink as phone ->
                if destinationType = typedefof<String> then
                    phone.ToString() :> obj
                else
                    failwithf "Hyperlink cannot be converted to type %s." (destinationType.Name)
            | _ -> failwithf "HyperlinkTypeConverter does not support values of type %s." (value.GetType().Name)
