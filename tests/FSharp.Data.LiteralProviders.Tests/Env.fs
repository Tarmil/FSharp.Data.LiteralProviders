namespace Tests

open NUnit.Framework
open FSharp.Data.LiteralProviders

module Env =

    [<Test>]
    let ``OS is either Windows_NT or Unix`` () =
        Assert.Contains(Env.OS.Value, [|"Windows_NT"; "Unix"|])

module EnvOrDefault =

    type ``OS without default`` = EnvOrDefault<"OS">
    type ``OS with default`` = EnvOrDefault<"OS", "Invalid">

    [<Test>]
    let ``OS without default is set`` () =
        Assert.IsTrue(``OS without default``.IsSet)

    [<Test>]
    let ``OS with default is set`` () =
        Assert.IsTrue(``OS with default``.IsSet)

    [<Test>]
    let ``OS without default is either Windows_NT or Unix`` () =
        Assert.Contains(``OS without default``.Value, [|"Windows_NT"; "Unix"|])

    [<Test>]
    let ``OS with default is either Windows_NT or Unix`` () =
        Assert.Contains(``OS with default``.Value, [|"Windows_NT"; "Unix"|])

    type ``Garbage without default`` = EnvOrDefault<"SomeGarbageVariableThatShouldntBeSet">
    type ``Garbage with default`` = EnvOrDefault<"SomeGarbageVariableThatShouldntBeSet", "some default value">

    [<Test>]
    let ``Garbage variable without default is not set`` () =
        Assert.IsFalse(``Garbage without default``.IsSet)

    [<Test>]
    let ``Garbage variable without default is empty string`` () =
        Assert.AreEqual("", ``Garbage without default``.Value)

    [<Test>]
    let ``Garbage variable with default is not set`` () =
        Assert.IsFalse(``Garbage with default``.IsSet)

    [<Test>]
    let ``Garbage variable with default is default`` () =
        Assert.AreEqual("some default value", ``Garbage with default``.Value)
