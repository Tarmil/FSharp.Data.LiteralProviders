module FSharp.Data.LiteralProviders.Tests.Conditional

open NUnit.Framework
open Swensen.Unquote
open FSharp.Data.LiteralProviders

module String =

    [<Test>]
    let ``if`` () =
        test <@ String.IF<true, Then = "First", Else = "Second">.Value = "First" @>
        test <@ String.IF<false, Then = "First", Else = "Second">.Value = "Second" @>

    [<Test>]
    let equal () =
        test <@ String.EQ<"A", "A">.Value @>
        test <@ not String.EQ<"A", "B">.Value @>

    [<Test>]
    let ``not equal`` () =
        test <@ not String.NE<"A", "A">.Value @>
        test <@ String.NE<"A", "B">.Value @>

module Bool =

    [<Test>]
    let ``if`` () =
        test <@ Bool.IF<true, Then = true, Else = false>.Value @>
        test <@ not Bool.IF<false, Then = true, Else = false>.Value @>

    [<Test>]
    let equal () =
        test <@ Bool.EQ<true, true>.Value @>
        test <@ not Bool.EQ<true, false>.Value @>

    [<Test>]
    let ``not equal`` () =
        test <@ not Bool.NE<true, true>.Value @>
        test <@ Bool.NE<true, false>.Value @>
        
    [<Test>]
    let ``and`` () =
        test <@ Bool.AND<true, true>.Value @>
        test <@ not Bool.AND<true, false>.Value @>

    [<Test>]
    let ``or`` () =
        test <@ Bool.OR<true, false>.Value @>
        test <@ not Bool.OR<false, false>.Value @>

    [<Test>]
    let xor () =
        test <@ not Bool.XOR<true, true>.Value @>
        test <@ Bool.XOR<true, false>.Value @>
        
    [<Test>]
    let not () =
        test <@ not Bool.NOT<true>.Value @>
        test <@ Bool.NOT<false>.Value @>

module Int =

    [<Test>]
    let ``if`` () =
        test <@ Int.IF<true, Then = 4, Else = 5>.Value = 4 @>
        test <@ Int.IF<false, Then = 4, Else = 5>.Value = 5 @>

    [<Test>]
    let equal () =
        test <@ Int.EQ<4, 4>.Value @>
        test <@ not Int.EQ<4, 5>.Value @>

    [<Test>]
    let ``not equal`` () =
        test <@ not Int.NE<4, 4>.Value @>
        test <@ Int.NE<4, 5>.Value @>

    [<Test>]
    let ``less than`` () =
        test <@ Int.LT<4, 5>.Value @>
        test <@ not Int.LT<4, 4>.Value @>
        test <@ not Int.LT<5, 4>.Value @>

    [<Test>]
    let ``less than or equal`` () =
        test <@ Int.LE<4, 5>.Value @>
        test <@ Int.LE<4, 4>.Value @>
        test <@ not Int.LE<5, 4>.Value @>

    [<Test>]
    let ``greater than`` () =
        test <@ not Int.GT<4, 5>.Value @>
        test <@ not Int.GT<4, 4>.Value @>
        test <@ Int.GT<5, 4>.Value @>

    [<Test>]
    let ``greater than or equal`` () =
        test <@ not Int.GE<4, 5>.Value @>
        test <@ Int.GE<4, 4>.Value @>
        test <@ Int.GE<5, 4>.Value @>
