module Tests.Exec

open NUnit.Framework
open Swensen.Unquote
open FSharp.Data.LiteralProviders

type DotnetListReference = Exec<"dotnet", "list reference">

[<Test>]
let ``with success`` () =
    test <@ DotnetListReference.ExitCode = 0 @>
    test <@ DotnetListReference.Error = "" @>
    test <@ DotnetListReference.Output =
                ([ @"Project reference(s)"
                   @"--------------------"
                   @"..\..\src\FSharp.Data.LiteralProviders.Runtime\FSharp.Data.LiteralProviders.Runtime.fsproj" ]
                |> String.concat System.Environment.NewLine) @>

type DotnetVersion = Exec<"dotnet", "--version">

[<Test>]
let ``single line excludes newline`` () =
    test <@ not <| DotnetVersion.Output.Contains("\n") @>

type DotnetListError = Exec<"dotnet", "list whatever">

[<Test>]
let ``with error code`` () =
    test <@ DotnetListError.ExitCode = 1 @>
    test <@ DotnetListError.ErrorMessage = "Process exited with status code 1" @>

type DotnetListErrorAllowed = Exec<"dotnet", "list whatever", EnsureSuccess = false>

[<Test>]
let ``with EnsureSuccess false`` () =
    test <@  DotnetListErrorAllowed.ExitCode = 1 @>
    test <@ DotnetListErrorAllowed.Error <> "" @>

type DotnetListTimeout = Exec<"dotnet", "list reference", Timeout = 1>

[<Test>]
let ``with timeout`` () =
    test <@ DotnetListTimeout.ExitCode = -1 @>
    test <@ DotnetListTimeout.ErrorMessage.StartsWith("Process timed out after ") @>

type DotnetSln = Exec<"dotnet", "sln list", Directory = "../..">

[<Test>]
let ``with directory`` () =
    test <@ DotnetSln.Output <> "" @>
    test <@ DotnetSln.Error = "" @>
    test <@ DotnetSln.ExitCode = 0 @>

type DotnetSlnError = Exec<"dotnet", "sln list", Directory = ".">

[<Test>]
let ``with directory failure`` () =
    test <@ DotnetSlnError.ExitCode = 1 @>
    test <@ DotnetSlnError.ErrorMessage = "Process exited with status code 1" @>
