namespace Invio.QueryProvider.Test

open System.Linq.Expressions

open Xunit

open Invio.QueryProvider.ExpressionMatching
open Invio.QueryProvider.ExpressionHelper

type ExpressionTest() =
    [<Fact>]
    member this.visit () =
        let source = Expression.Add(Expression.Constant(2), Expression.Constant(2))

        let actual =
            source
            |> visit (fun e ->
                match e with
                | Constant _ -> Replace (Expression.Variable(typedefof<int>, "foo") :> Expression)
                | _ -> Recurse
            )

        let expected = Expression.Add(Expression.Variable(typedefof<int>, "foo"), Expression.Variable(typedefof<int>, "foo"))

        Assert.Equal(expected.ToString(), actual.ToString())

    [<Fact>]
    member this.map () =
        let source = Expression.Add(Expression.Constant(2), Expression.Constant(2))
        ignore()

        let result =
            map(fun e -> Recurse, e.NodeType.ToString() ) source
            |> String.concat(",")
        Assert.Equal("Constant,Constant,Add", result)
