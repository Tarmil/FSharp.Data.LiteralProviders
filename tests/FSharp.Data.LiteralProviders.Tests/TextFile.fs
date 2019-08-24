module Tests.TextFile

open System.IO
open NUnit.Framework
open FSharp.Data.LiteralProviders

[<Test>]
let ``File in base dir exists`` () =
    Assert.AreEqual("TextFile.fs", TextFile.``TextFile.fs``.Name)

[<Test>]
let ``File in subdir exists`` () =
    Assert.AreEqual("textFile.txt", TextFile.subdir.``textFile.txt``.Name)

[<Test>]
let ``Full path is correct`` () =
    Assert.AreEqual(Path.Combine(__SOURCE_DIRECTORY__, "subdir", "textFile.txt"), TextFile.subdir.``textFile.txt``.Path)

[<Test>]
let ``Text file is read`` () =
    Assert.AreEqual("This is some\ntext content.", TextFile.subdir.``textFile.txt``.Text)

[<Test>]
let ``Binary file is recognized as not text`` () =
    TextFile.subdir.``binFile.bin``.``Not a text file`` |> ignore

type ``Text file without default`` = TextFile<"subdir/textFile.txt">
type ``Text file with default`` = TextFile<"subdir/textFile.txt", "Invalid">

[<Test>]
let ``Text file without default exists`` () =
    Assert.IsTrue(``Text file without default``.Exists)

[<Test>]
let ``Text file with default exists`` () =
    Assert.IsTrue(``Text file with default``.Exists)

[<Test>]
let ``Text file without default contents`` () =
    Assert.AreEqual("This is some\ntext content.", ``Text file without default``.Text)

[<Test>]
let ``Text file with default contents`` () =
    Assert.AreEqual("This is some\ntext content.", ``Text file with default``.Text)

type ``Non-existent without default`` = TextFile<"NonExistentFile.txt">
type ``Non-existent with default`` = TextFile<"NonExistentFile.txt", "default contents">

[<Test>]
let ``Non-existent without default doesn't exist`` () =
    Assert.IsFalse(``Non-existent without default``.Exists)

[<Test>]
let ``Non-existent without default is empty string`` () =
    Assert.AreEqual("", ``Non-existent without default``.Text)

[<Test>]
let ``Non-existent with default doesn't exist`` () =
    Assert.IsFalse(``Non-existent with default``.Exists)

[<Test>]
let ``Non-existent with default is default value`` () =
    Assert.AreEqual("default contents", ``Non-existent with default``.Text)
