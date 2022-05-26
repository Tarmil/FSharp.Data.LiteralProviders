module FSharp.Data.LiteralProviders.Tests.Conditional

open NUnit.Framework
open FSharp.Data.LiteralProviders

module String =

    [<Test>]
    let ``if`` () =
        Assert.AreEqual("First", String.IF<true, Then = "First", Else = "Second">.Value)
        Assert.AreEqual("Second", String.IF<false, Then = "First", Else = "Second">.Value)

    [<Test>]
    let equal () =
        Assert.IsTrue(String.EQ<"A", "A">.Value)
        Assert.IsFalse(String.EQ<"A", "B">.Value)

    [<Test>]
    let ``not equal`` () =
        Assert.IsFalse(String.NE<"A", "A">.Value)
        Assert.IsTrue(String.NE<"A", "B">.Value)

module Bool =

    [<Test>]
    let ``if`` () =
        Assert.IsTrue(Bool.IF<true, Then = true, Else = false>.Value)
        Assert.IsFalse(Bool.IF<false, Then = true, Else = false>.Value)

    [<Test>]
    let equal () =
        Assert.IsTrue(Bool.EQ<true, true>.Value)
        Assert.IsFalse(Bool.EQ<true, false>.Value)

    [<Test>]
    let ``not equal`` () =
        Assert.IsFalse(Bool.NE<true, true>.Value)
        Assert.IsTrue(Bool.NE<true, false>.Value)
        
    [<Test>]
    let ``and`` () =
        Assert.IsTrue(Bool.AND<true, true>.Value)
        Assert.IsFalse(Bool.AND<true, false>.Value)

    [<Test>]
    let ``or`` () =
        Assert.IsTrue(Bool.OR<true, false>.Value)
        Assert.IsFalse(Bool.OR<false, false>.Value)

    [<Test>]
    let xor () =
        Assert.IsFalse(Bool.XOR<true, true>.Value)
        Assert.IsTrue(Bool.XOR<true, false>.Value)
        
    [<Test>]
    let not () =
        Assert.IsFalse(Bool.NOT<true>.Value)
        Assert.IsTrue(Bool.NOT<false>.Value)

module Int =

    [<Test>]
    let ``if`` () =
        Assert.AreEqual(4, Int.IF<true, Then = 4, Else = 5>.Value)
        Assert.AreEqual(5, Int.IF<false, Then = 4, Else = 5>.Value)

    [<Test>]
    let equal () =
        Assert.IsTrue(Int.EQ<4, 4>.Value)
        Assert.IsFalse(Int.EQ<4, 5>.Value)

    [<Test>]
    let ``not equal`` () =
        Assert.IsFalse(Int.NE<4, 4>.Value)
        Assert.IsTrue(Int.NE<4, 5>.Value)

    [<Test>]
    let ``less than`` () =
        Assert.IsTrue(Int.LT<4, 5>.Value)
        Assert.IsFalse(Int.LT<4, 4>.Value)
        Assert.IsFalse(Int.LT<5, 4>.Value)

    [<Test>]
    let ``less than or equal`` () =
        Assert.IsTrue(Int.LE<4, 5>.Value)
        Assert.IsTrue(Int.LE<4, 4>.Value)
        Assert.IsFalse(Int.LE<5, 4>.Value)

    [<Test>]
    let ``greater than`` () =
        Assert.IsFalse(Int.GT<4, 5>.Value)
        Assert.IsFalse(Int.GT<4, 4>.Value)
        Assert.IsTrue(Int.GT<5, 4>.Value)

    [<Test>]
    let ``greater than or equal`` () =
        Assert.IsFalse(Int.GE<4, 5>.Value)
        Assert.IsTrue(Int.GE<4, 4>.Value)
        Assert.IsTrue(Int.GE<5, 4>.Value)
