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
