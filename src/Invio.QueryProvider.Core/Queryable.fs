namespace Invio.QueryProvider

open System.Reflection
open System.Linq
open System.Linq.Expressions
open System.Collections
open System.Collections.Generic

/// <summary>
/// Type to remove boiler plate when implementing IQueryable
/// </summary>
type Query<'T>
    (
        provider : BaseQueryProvider,
        ?expression : Expression
    ) as this =

    let hardExpression =
        match expression with
        | Some e when e <> null -> e
        | _ -> Expression.Constant(this) :> Expression

    let result : Lazy<IEnumerable<'T>> =
        lazy (provider.PrepareEnumerable(hardExpression) :?> IEnumerable<'T>)

    interface IQueryable<'T> with
        member this.Expression = hardExpression

        member this.ElementType = typedefof<'T>

        member this.Provider = provider :> IQueryProvider

    interface IEnumerable with
        member this.GetEnumerator() =
            (result.Force() :> IEnumerable).GetEnumerator()

    interface IEnumerable<'T> with
        member this.GetEnumerator() =
            result.Force().GetEnumerator()

    override this.ToString () =
        match expression with
        | Some e when e <> null -> e.ToString()
        | _ -> sprintf "value(Query<%s>)" typedefof<'T>.Name

and OrderedQuery<'T>(provider : BaseQueryProvider, expression : Expression) =
    inherit Query<'T>(provider, expression)

    interface IOrderedQueryable<'T>

/// <summary>
/// Type to remove boiler plate when implementing IQueryProvider
/// </summary>
and [<AbstractClass>] BaseQueryProvider() =

    interface IQueryProvider with

        member this.CreateQuery<'S> (expression : Expression) =
            match expression with
            | :? MethodCallExpression as call
                when call.Method.ReturnType |> TypeHelper.implements typedefof<IOrderedQueryable> ->
                    OrderedQuery<'S>(this, expression) :> IQueryable<'S>
            | _ -> Query<'S>(this, expression) :> IQueryable<'S>

        member this.CreateQuery (expression : Expression) =
            let elementType = TypeHelper.getElementType(expression.Type)
            try
                let generic = typedefof<Query<_>>.MakeGenericType(elementType)

                let args : obj array = [|this; expression|]
                System.Activator.CreateInstance(generic, args) :?> IQueryable
            with
            | :? TargetInvocationException as ex -> raise ex.InnerException

        member this.Execute<'S> expression =
            match this.Execute expression with
                | null -> Unchecked.defaultof<'S>
                | x -> x :?> 'S

        member this.Execute (expression) =
            this.Execute expression

    abstract member Execute : Expression -> obj

    abstract member PrepareEnumerable : Expression -> IEnumerable

