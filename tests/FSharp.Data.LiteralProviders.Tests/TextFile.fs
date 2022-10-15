module Tests.TextFile

open System.IO
open NUnit.Framework
open Swensen.Unquote
open FSharp.Data.LiteralProviders

[<Test>]
let ``File in base dir exists`` () =
    test <@ TextFile.``TextFile.fs``.Name = "TextFile.fs" @>

[<Test>]
let ``File in subdir exists`` () =
    test <@ TextFile.subdir.``textFile.txt``.Name = "textFile.txt" @>

[<Test>]
let ``Full path is correct`` () =
    test <@ TextFile.subdir.``textFile.txt``.Path = Path.Combine(__SOURCE_DIRECTORY__, "subdir", "textFile.txt") @>

[<Test>]
let ``Text file is read`` () =
    test <@ TextFile.subdir.``textFile.txt``.Text = "This is some\ntext content." @>

[<Test>]
let ``Binary file is recognized as not text`` () =
    TextFile.subdir.``binFile.bin``.``Not a text file`` |> ignore

type ``Text file without default`` = TextFile<"subdir/textFile.txt">
type ``Text file with default`` = TextFile<"subdir/textFile.txt", "Invalid">

[<Test>]
let ``Text file without default exists`` () =
    test <@ ``Text file without default``.Exists @>

[<Test>]
let ``Text file with default exists`` () =
    test <@ ``Text file with default``.Exists @>

[<Test>]
let ``Text file without default contents`` () =
    test <@ ``Text file without default``.Text = "This is some\ntext content." @>

[<Test>]
let ``Text file with default contents`` () =
    test <@ ``Text file with default``.Text = "This is some\ntext content." @>

type ``Non-existent without default`` = TextFile<"NonExistentFile.txt">
type ``Non-existent with default`` = TextFile<"NonExistentFile.txt", "default contents">

[<Test>]
let ``Non-existent without default doesn't exist`` () =
    test <@ not ``Non-existent without default``.Exists @>

[<Test>]
let ``Non-existent without default is empty string`` () =
    test <@  ``Non-existent without default``.Text = "" @>

[<Test>]
let ``Non-existent with default doesn't exist`` () =
    test <@ not ``Non-existent with default``.Exists @>

[<Test>]
let ``Non-existent with default is default value`` () =
    test <@ ``Non-existent with default``.Text = "default contents" @>

type WithBom = TextFile<"subdir/textFileWithBom.txt", Encoding = "UTF-16-le">
type WithoutBom = TextFile<"subdir/textFileWithoutBom.txt", Encoding = "UTF-16-le">

[<Test>]
let ``Regression #12 - Strip BOM`` () =
    test <@ WithBom.HasBom @>
    test <@ WithBom.Text = "Test content" @>
    test <@ not WithoutBom.HasBom @>
    test <@ WithoutBom.Text = "Test content" @>

[<Test>]
let ``Parent directory`` () =
    test <@ TextFile.``..``.``parentTextFile.txt``.Text = "This is some\ntext content." @>

[<Test>]
let ``Parent of subdirectory`` () =
    test <@ TextFile.subdir.``..``.``TextFile.fs``.Name = "TextFile.fs" @>

[<Test>]
let ``Parent of parent directory`` () =
    test <@ TextFile.subdir.``..``.``..``.``parentTextFile.txt``.Text = "This is some\ntext content." @>

// Uncomment below to test EnsureExists
// type EnsureExists = TextFile<"doesntexist.txt", EnsureExists = true>

[<Test>]
let ``Text as int`` () =
    test <@ TextFile.subdir.``number.txt``.TextAsInt = 42 @>
    test <@ TextFile<"subdir/number.txt">.TextAsInt = 42 @>

[<Test>]
let ``Text as bool`` () =
    test <@ TextFile.subdir.``boolean.txt``.TextAsBool @>
    test <@ TextFile<"subdir/boolean.txt">.TextAsBool @>
