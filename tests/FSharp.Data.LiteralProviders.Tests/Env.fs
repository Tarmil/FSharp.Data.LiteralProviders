module Tests.Env

open NUnit.Framework
open Swensen.Unquote
open FSharp.Data.LiteralProviders

[<Test>]
let ``HOME is non-empty`` () =
#if WINDOWS
    test <@ Env.HOMEPATH.Value <> "" @>
#else
    test <@ Env.HOME.Value <> "" @>
#endif

[<Test>]
let ``Random var from env file is available directly`` () =
    test <@ Env.RANDOM_VAR_FROM_ENV_FILE.Value = "some value" @>

#if WINDOWS
let [<Literal>] HOMENAME = "HOMEPATH"
#else
let [<Literal>] HOMENAME = "HOME"
#endif

type ``HOME without default`` = Env<HOMENAME>
type ``HOME with default`` = Env<HOMENAME, "Invalid">
type ``Random var from env file`` = Env<"RANDOM_VAR_FROM_ENV_FILE">
type ``Random var not from env file`` = Env<"RANDOM_VAR_FROM_ENV_FILE", LoadEnvFile = false>

[<Test>]
let ``Random var from env file is set`` () =
    test <@ ``Random var from env file``.IsSet @>

[<Test>]
let ``Random var from env file with LoadEnvFile false is not set`` () =
    test <@ not ``Random var not from env file``.IsSet @>

[<Test>]
let ``HOME without default is set`` () =
    test <@ ``HOME without default``.IsSet @>

[<Test>]
let ``HOME with default is set`` () =
    test <@ ``HOME with default``.IsSet @>

[<Test>]
let ``HOME without default is non-empty`` () =
    test <@ ``HOME without default``.Value <> "" @>

[<Test>]
let ``HOME with default is non-empty`` () =
    test <@ ``HOME with default``.Value <> "" @>

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
