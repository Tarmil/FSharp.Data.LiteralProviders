namespace Tests

open System.IO
open NUnit.Framework
open FSharp.Data.LiteralProviders

module File =

    [<Test>]
    let ``File in base dir exists`` () =
        Assert.AreEqual("File.fs", File.``File.fs``.Name)

    [<Test>]
    let ``File in subdir exists`` () =
        Assert.AreEqual("textFile.txt", File.subdir.``textFile.txt``.Name)

    [<Test>]
    let ``Full path is correct`` () =
        Assert.AreEqual(Path.Combine(__SOURCE_DIRECTORY__, "subdir", "textFile.txt"), File.subdir.``textFile.txt``.Path)

    [<Test>]
    let ``Text file is read`` () =
        Assert.AreEqual("This is some\r\ntext content.", File.subdir.``textFile.txt``.Text)

    [<Test>]
    let ``Binary file is recognized as not text`` () =
        File.subdir.``binFile.bin``.``Not a text file`` |> ignore

module FileOrDefault =

    type ``Text file without default`` = FileOrDefault<"subdir/textFile.txt">
    type ``Text file with default`` = FileOrDefault<"subdir/textFile.txt", "Invalid">

    [<Test>]
    let ``Text file without default exists`` () =
        Assert.IsTrue(``Text file without default``.Exists)

    [<Test>]
    let ``Text file with default exists`` () =
        Assert.IsTrue(``Text file with default``.Exists)

    [<Test>]
    let ``Text file without default contents`` () =
        Assert.AreEqual("This is some\r\ntext content.", ``Text file without default``.Text)

    [<Test>]
    let ``Text file with default contents`` () =
        Assert.AreEqual("This is some\r\ntext content.", ``Text file with default``.Text)

    type ``Non-existent without default`` = FileOrDefault<"NonExistentFile.txt">
    type ``Non-existent with default`` = FileOrDefault<"NonExistentFile.txt", "default contents">

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
