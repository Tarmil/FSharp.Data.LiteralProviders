module Tests.JsonFile

open NUnit.Framework
open FSharp.Data.LiteralProviders
open Swensen.Unquote

[<Test>]
let ``File in base dir exists`` () =
    test <@ JsonFile.``test.json``.x = 1 @>

[<Test>]
let ``File in subdir exists`` () =
    test <@ JsonFile.subdir.``test.json``.y = 2 @>

[<Test>]
let ``Root number`` () =
    test <@ JsonFile.subdir.``rootNumber.json``.Value = 4251 @>

[<Test>]
let ``Root string`` () =
    test <@ JsonFile.subdir.``rootString.json``.Value = "Hello, world!" @>

[<Test>]
let ``Nested object`` () =
    test <@ JsonFile.subdir.``nestedObject.json``.a.b = 1 @>
    test <@ JsonFile.subdir.``nestedObject.json``.a.c = "test" @>
    test <@ JsonFile.subdir.``nestedObject.json``.d = true @>

[<Test>]
let ``Array`` () =
    test <@ JsonFile.subdir.``array.json``.arr.``0`` = "a" @>
    test <@ JsonFile.subdir.``array.json``.arr.``1`` = 1 @>
    test <@ JsonFile.subdir.``array.json``.arr.``2`` = true @>
    test <@ JsonFile.subdir.``array.json``.arr.``3`` = false @>
    test <@ JsonFile.subdir.``array.json``.arr.``4``.x = 1 @>

[<Test>]
let ``Error`` () =
    test <@ JsonFile.subdir.``notActually.json``.Error = "'N' is an invalid start of a value. LineNumber: 0 | BytePositionInLine: 0." @>
