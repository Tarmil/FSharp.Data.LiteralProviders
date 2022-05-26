module Tests.Exec

open NUnit.Framework
open FSharp.Data.LiteralProviders

type DotnetListReference = Exec<"dotnet", "list reference">

[<Test>]
let ``with success`` () =
    Assert.AreEqual(0, DotnetListReference.ExitCode)
    Assert.IsEmpty(DotnetListReference.Error)
    Assert.StartsWith("Project reference(s)", DotnetListReference.Output)
    Assert.EndsWith(".fsproj", DotnetListReference.Output)

type DotnetListError = Exec<"dotnet", "list whatever">

[<Test>]
let ``with error code`` () =
    Assert.AreEqual(1, DotnetListError.ExitCode)
    Assert.AreEqual("Process exited with status code 1", DotnetListError.ErrorMessage)

type DotnetListErrorAllowed = Exec<"dotnet", "list whatever", EnsureSuccess = false>

[<Test>]
let ``with EnsureSuccess false`` () =
    Assert.AreEqual(1, DotnetListErrorAllowed.ExitCode)
    Assert.IsNotEmpty(DotnetListErrorAllowed.Error)

type DotnetListTimeout = Exec<"dotnet", "list reference", Timeout = 1>

[<Test>]
let ``with timeout`` () =
    Assert.AreEqual(-1, DotnetListTimeout.ExitCode)
    Assert.StartsWith("Process timed out after ", DotnetListTimeout.ErrorMessage)

type DotnetSln = Exec<"dotnet", "sln list", Directory = "../..">

[<Test>]
let ``with directory`` () =
    Assert.IsNotEmpty(DotnetSln.Output)
    Assert.IsEmpty(DotnetSln.Error)
    Assert.AreEqual(0, DotnetSln.ExitCode)

type DotnetSlnError = Exec<"dotnet", "sln list", Directory = ".">

[<Test>]
let ``with directory failure`` () =
    Assert.AreEqual(1, DotnetSlnError.ExitCode)
    Assert.AreEqual("Process exited with status code 1", DotnetSlnError.ErrorMessage)
