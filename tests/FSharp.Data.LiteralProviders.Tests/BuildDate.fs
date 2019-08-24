module Tests.BuildDate

open System.Text.RegularExpressions
open NUnit.Framework
open FSharp.Data.LiteralProviders

module Assert =
    let IsMatch(s: string, re: string) =
        Regex.IsMatch(s, re) |> Assert.IsTrue

[<Test>]
let ``Utc build date is in UTC format`` () =
    Assert.IsMatch(BuildDate.Utc, @"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}.\d{7}Z")

[<Test>]
let ``Local build date is in local format`` () =
    Assert.IsMatch(BuildDate.Local, @"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}.\d{7}[-+]\d{2}:\d{2}")

[<Test>]
let ``Formatted build date`` () =
    Assert.IsMatch(BuildDate<"yyyy-MM-dd">.Local, @"\d{4}-\d{2}-\d{2}")
