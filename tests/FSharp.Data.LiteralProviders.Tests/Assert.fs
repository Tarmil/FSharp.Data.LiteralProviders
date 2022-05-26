namespace Tests

open System.Text.RegularExpressions
open NUnit.Framework

type Assert =

    static member IsMatch(s: string, re: string) =
        Regex.IsMatch(s, re) |> Assert.IsTrue

    static member StartsWith(prefix: string, string: string) =
        Assert.AreEqual(prefix, string.Substring(0, min string.Length prefix.Length))

    static member EndsWith(prefix: string, string: string) =
        Assert.AreEqual(prefix, string.Substring(max 0 (string.Length - prefix.Length)))
