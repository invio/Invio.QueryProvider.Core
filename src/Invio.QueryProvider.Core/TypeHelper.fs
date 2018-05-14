module Invio.QueryProvider.TypeHelper

open System
open System.Collections.Generic
open System.Reflection
open Microsoft.FSharp.Reflection

let implements (interfaceType : System.Type) implementerType =
    interfaceType.IsAssignableFrom(implementerType)

let rec findIEnumerable seqType =
    if seqType = null || seqType = typedefof<string> then
        None
    else
        if seqType.IsArray then
            Some (typedefof<IEnumerable<_>>.MakeGenericType(seqType.GetElementType()))
        else
            let assigneable =
                match seqType.GetTypeInfo().IsGenericType with
                | true ->
                    seqType.GetGenericArguments()
                    |> Seq.tryFind(fun (arg) ->
                        typedefof<IEnumerable<_>>.MakeGenericType(arg).IsAssignableFrom(seqType))
                | false -> None
            if assigneable.IsSome then
                Some assigneable.Value
            else
                let iface =
                    let ifaces = seqType.GetInterfaces()
                    if (ifaces <> null) && ifaces.Length > 0 then
                        ifaces |> Seq.tryPick(findIEnumerable)
                    else
                        None
                if iface.IsSome then
                    iface
                else
                    let seqTypeInfo = seqType.GetTypeInfo()
                    if seqTypeInfo.BaseType <> null && seqTypeInfo.BaseType <> typedefof<obj> then
                        findIEnumerable(seqTypeInfo.BaseType)
                    else
                        None

let getElementType seqType =
    let ienum = findIEnumerable(seqType)
    match ienum with
    | None -> seqType
    | Some(ienum) ->  ienum
        //ienum.GetGenericArguments() |> Seq.head

let isValueType (t : System.Type) =
    let ti = t.GetTypeInfo()
    ti.IsValueType || t = typedefof<string>
let isOption (t : System.Type) =
    let ti = t.GetTypeInfo()
    ti.IsGenericType &&
    ti.GetGenericTypeDefinition() = typedefof<Option<_>>
let isNullable (t : System.Type) =
    let ti = t.GetTypeInfo()
    ti.IsGenericType &&
    ti.GetGenericTypeDefinition() = typedefof<Nullable<_>>
let isEnumType (t : System.Type) =
    let ti = t.GetTypeInfo()
    ti.IsEnum

/// <summary>
/// Assert that a union case has exactly one field, then return its Reflection.PropertyInfo
/// </summary>
/// <param name="t"></param>
let unionExactlyOneCaseOneField t =
    let cases = FSharpType.GetUnionCases t
    if cases |> Seq.length > 1 then
        failwith "Multi case unions are not supported"
    else
        let case = cases |> Seq.exactlyOne
        let fields = case.GetFields()
        if fields |> Seq.length > 1 then
            failwith "Only one field allowed on union case"
        else
            fields |> Seq.exactlyOne

/// <summary>
/// Gets the underlying type for fsharp types
/// </summary>
/// <param name="t"></param>
let rec unwrapType (t : System.Type) =
    if isOption t then
        t.GetTypeInfo().GetGenericArguments() |> Seq.head |> unwrapType
    else if isNullable t then
        Nullable.GetUnderlyingType(t) |> unwrapType
    else if t |> FSharpType.IsUnion then
        (unionExactlyOneCaseOneField t).PropertyType |> unwrapType
    else
        t
