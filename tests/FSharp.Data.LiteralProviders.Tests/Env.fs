namespace Tests

open NUnit.Framework
open FSharp.Data.LiteralProviders

module Env =

    [<Test>]
    let ``OS is either Windows_NT or Unix`` () =
        Assert.Contains(Env.OS.Value, [|"Windows_NT"; "Unix"|])
