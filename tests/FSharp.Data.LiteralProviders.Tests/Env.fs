module Tests.Env

open NUnit.Framework
open Swensen.Unquote
open FSharp.Data.LiteralProviders

[<Test>]
let ``OS is either Windows_NT or Unix`` () =
    test <@ [|"Windows_NT"; "Unix"|] |> Seq.contains Env.OS.Value @>

[<Test>]
let ``Random var from env file is available directly`` () =
    test <@ Env.RANDOM_VAR_FROM_ENV_FILE.Value = "some value" @>

type ``OS without default`` = Env<"OS">
type ``OS with default`` = Env<"OS", "Invalid">
type ``Random var from env file`` = Env<"RANDOM_VAR_FROM_ENV_FILE">
type ``Random var not from env file`` = Env<"RANDOM_VAR_FROM_ENV_FILE", LoadEnvFile = false>

[<Test>]
let ``Random var from env file is set`` () =
    test <@ ``Random var from env file``.IsSet @>

[<Test>]
let ``Random var from env file with LoadEnvFile false is not set`` () =
    test <@ not ``Random var not from env file``.IsSet @>

[<Test>]
let ``OS without default is set`` () =
    test <@ ``OS without default``.IsSet @>

[<Test>]
let ``OS with default is set`` () =
    test <@ ``OS with default``.IsSet @>

[<Test>]
let ``OS without default is either Windows_NT or Unix`` () =
    test <@ [|"Windows_NT"; "Unix"|] |> Seq.contains ``OS without default``.Value @>

[<Test>]
let ``OS with default is either Windows_NT or Unix`` () =
    test <@ [|"Windows_NT"; "Unix"|] |> Seq.contains ``OS with default``.Value @>

type ``Garbage without default`` = Env<"SomeGarbageVariableThatShouldntBeSet">
type ``Garbage with default`` = Env<"SomeGarbageVariableThatShouldntBeSet", "some default value">

[<Test>]
let ``Garbage variable without default is not set`` () =
    test <@ not ``Garbage without default``.IsSet @>

[<Test>]
let ``Garbage variable without default is empty string`` () =
    test <@ ``Garbage without default``.Value = "" @>

[<Test>]
let ``Garbage variable with default is not set`` () =
    test <@ not ``Garbage with default``.IsSet @>

[<Test>]
let ``Garbage variable with default is default`` () =
    test <@ ``Garbage with default``.Value = "some default value" @>

// Uncomment below to test EnsureExists
// type EnsureExists = Env<"doesntexist", EnsureExists = true>

[<Test>]
let ``Value as int`` () =
    test <@ Env<"SomeGarbageVariableThatShouldntBeSet", "42">.ValueAsInt = 42 @>
    test <@ Env<"SOME_NUMBER">.ValueAsInt = 57 @>
    test <@ Env.SOME_NUMBER.ValueAsInt = 57 @>

[<Test>]
let ``Value as bool`` () =
    test <@ Env<"SomeGarbageVariableThatShouldntBeSet", "true">.ValueAsBool @>
    test <@ Env<"SOME_BOOLEAN">.ValueAsBool @>
    test <@ Env.SOME_BOOLEAN.ValueAsBool @>
