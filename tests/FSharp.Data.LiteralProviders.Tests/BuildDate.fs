module Tests.BuildDate

open System.Text.RegularExpressions
open NUnit.Framework
open Swensen.Unquote
open FSharp.Data.LiteralProviders

let matchRegex regex text =
    Regex(regex, RegexOptions.Singleline).IsMatch(text)

[<Test>]
let ``Utc build date is in UTC format`` () =
    test <@ BuildDate.Utc |> matchRegex @"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}.\d{7}Z" @>

[<Test>]
let ``Local build date is in local format`` () =
    test <@ BuildDate.Local |> matchRegex @"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}.\d{7}[-+]\d{2}:\d{2}" @>

[<Test>]
let ``Formatted build date`` () =
    test <@ BuildDate<"yyyy-MM-dd">.Local |> matchRegex @"\d{4}-\d{2}-\d{2}" @>
