# FSharp.Data.LiteralProviders

[![Build](https://github.com/Tarmil/FSharp.Data.LiteralProviders/workflows/Build/badge.svg?branch=master)](https://github.com/Tarmil/FSharp.Data.LiteralProviders/actions?query=workflow%3ABuild+branch%3Amaster)
[![Nuget](https://img.shields.io/nuget/v/FSharp.Data.LiteralProviders?logo=nuget)](https://nuget.org/packages/FSharp.Data.LiteralProviders)

This is a collection of type providers that provide literals: compile-time constants that can be used in regular code, but also as parameters to other type providers or .NET attributes.

<!-- doctoc --github --notitle README.md -->
<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->


- [Reference](#reference)
  - [Env](#env)
  - [TextFile](#textfile)
  - [Exec](#exec)
  - [Conditionals](#conditionals)
    - [Equality](#equality)
    - [Comparison](#comparison)
    - [Boolean operations](#boolean-operations)
    - [If](#if)
  - [BuildDate](#builddate)
  - [Parsed value](#parsed-value)
- [Tips for combining type providers](#tips-for-combining-type-providers)
- [Packaging](#packaging)
  - [Using NuGet](#using-nuget)
  - [Using Paket](#using-paket)

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

## Reference

### Env

`FSharp.Data.LiteralProviders.Env` contains literals for environment variables during compile time.

```fsharp
open FSharp.Data.LiteralProviders

/// The compile-time value of the "OS" environment variable
let compileOS = Env.OS.Value

match compileOS with
| "Windows_NT" -> printfn "This program was compiled on Windows!"
| "Unix" -> printfn "This program was compiled on OSX or Linux!"
| _ -> printfn "I don't know the platform this program was compiled on :("
```

Here is a more useful example, using it as a parameter to another type provider (namely, [SQLProvider](http://fsprojects.github.io/SQLProvider/)):

```fsharp
open FSharp.Data.Sql
open FSharp.Data.LiteralProviders

type Sql = SqlProvider<Common.DatabaseProviderTypes.MSSQLSERVER,
                       Env.CONNECTION_STRING.Value>
```

Note that when called this way, `Env` fails to compile if the environment variable is not set.

Alternatively, the environment variable's name can be passed as a string parameter.
In this case, `Env` returns the empty string if the variable is not set.

```fsharp
open FSharp.Data.LiteralProviders

let vsVersion = Env<"VisualStudioEdition">.Value

match vsVersion with
| "" -> printfn "This program wasn't compiled with Visual Studio."
| v -> printfn "This program was built with Visual Studio %s." v
```

When used with a parameter, `Env` also provides a value `IsSet : bool`

Additional parameters can be passed:

* `DefaultValue : string` will be used as the value if the environment variable isn't set, instead of the empty string.

    ```fsharp
    open FSharp.Data.Sql
    open FSharp.Data.LiteralProviders

    let [<Literal>] connString =
        Env<"CONNECTION_STRING", "Server=localhost;Integrated Security=true">.Value

    type Sql = SqlProvider<Common.DatabaseProviderTypes.MSSQLSERVER, connString>
    ```

* `EnsureExists : bool` specifies the behavior when the environment variable isn't set.

    If false (the default), then `Value` is an empty string (or `DefaultValue` if provided).

    If true, then the type provider raises a compile-time error.

    ```fsharp
    /// Throws a compile-time error "Environment variable does not exist: CONNECTION_STRING".
    let [<Literal>] connString = Env<"CONNECTION_STRING", EnsureExists = true>.Text
    ```

### TextFile

`FSharp.Data.LiteralProviders.TextFile` contains literals that are read from text files during compilation.

```fsharp
open FSharp.Data.LiteralProviders

/// The compile-time contents of the file <projectFolder>/build/version.txt
let [<Literal>] version = TextFile.build.``version.txt``.Text
```

Alternatively, the file path can be passed as a string parameter.
In this case, `TextFile` returns the empty string if the file doesn't exist.

```fsharp
open FSharp.Data.LiteralProviders

/// The compile-time contents of the file <projectFolder>/build/version.txt
/// or "" if this file doesn't exist.
let [<Literal>] version = TextFile<"build/version.txt">.Text
```

Additional parameters can be passed:

* `DefaultValue : string` will be used as the value if the file doesn't exist, instead of the empty string.

    ```fsharp
    open FSharp.Data.LiteralProviders

    /// The compile-time contents of the file <projectFolder>/build/version.txt
    /// or "1.0" if this file doesn't exist.
    let [<Literal>] version = TextFile<"build/version.txt", DefaultValue = "1.0">.Text
    ```

* `Encoding : string` specifies the text encoding.

    The possible values are `UTF-8`, `UTF-16-le`, `UTF-16-be`, `UTF-32-le` and `UTF-32-be`.

    When not specified, `TextFile` tries to guess the encoding.

    ```fsharp
    open FSharp.Data.LiteralProviders

    let [<Literal>] script = TextFile<"LoadData.sql", Encoding = "UTF-16-le">.Text
    ```

    Note: regardless of the encoding, if the file starts with a byte order mark, then the BOM is stripped from the string.

* `EnsureExists : bool` specifies the behavior when the file doesn't exist.

    If false (the default), then the `Text` value is an empty string (or `DefaultValue` if provided).

    If true, then the type provider raises a compile-time error.

    ```fsharp
    /// Throws a compile-time error "File does not exist: fileThatDoesntExist.txt".
    let [<Literal>] test = TextFile<"fileThatDoesntExist.txt", EnsureExists = true>.Text
    ```

### Exec

`FSharp.Data.LiteralProviders.Exec` executes an external program during compilation and captures its output.

```fsharp
open FSharp.Data.LiteralProviders

let [<Literal>] currentBranch = Exec<"git", "branch --show-current">.Output
```

Additional parameters can be passed:

* `Input: string`: text that is passed to the program's standard output.

* `Directory: string`: the working directory. The default is the project directory.

* `EnsureSuccess: bool`: if true, the provider ensures that the program exits successfully, and fails otherwise.  
    If false, no error is raised.  
    The default is true.

* `Timeout: int`: timeout in milliseconds. Raise an error if the program takes longer to finish.  
    The default is 10_000 (10 seconds).

The following values are provided:

* `Output: string`: the program's standard output.

* `Error: string`: the program's standard error.

* `ExitCode: int`: the program's exit code. Only useful with `EnsureSuccess = false`, otherwise always 0.

### Conditionals

`FSharp.Data.LiteralProviders` contains sub-namespaces `String`, `Int` and `Bool` for conditional operations on these types.

#### Equality

The providers `EQ` and `NE` contain `Value: bool` that checks whether the two parameters are equal / not equal, respectively.

```fsharp
open FSharp.Data.LiteralProviders

let [<Literal>] branch = Exec<"git", "branch --show-current">.Output

let [<Literal>] isMaster = String.EQ<branch, "master">.Value
```

#### Comparison

In sub-namespace `Int`, the providers `LT`, `LE`, `GT` and `GE` contain `Value: bool` that checks whether the first parameter is less than / less than or equal / greater than / greater than or equal to the second parameter, respectively.

```fsharp
open FSharp.Data.LiteralProviders

let [<Literal>] gitStatusCode = Exec<"git", "status", EnsureSuccess = false>.ExitCode

let [<Literal>] notInGitRepo = Int.GT<gitStatusCode, 0>.Value
```

#### Boolean operations

In sub-namespace `Bool`, the providers `AND`, `OR`, `XOR` and `NOT` contain `Value: bool` that performs the corresponding boolean operation on its parameter(s).

```fsharp
open FSharp.Data.LiteralProviders

type GithubAction = Env<"GITHUB_ACTION">

let [<Literal>] isLocalBuild = Bool.NOT<GithubAction.IsSet>.Value
```

#### If

The provider `IF` takes a condition and two values as parameters.
It returns the first value if the condition is true, and the second value if the condition is false.

```fsharp
open FSharp.Data.LiteralProviders

let [<Literal>] versionSuffix = String.IF<isMaster, "", "-pre">.Value
```

Note that even though only one value is returned, both are evaluated.
So if one branch fails, even though the other one is returned, the whole provider will fail.

```fsharp
open FSharp.Data.LiteralProviders

let [<Literal>] isCI = Env<"CI", "false">.ValueAsBool

// The following will fail, because when CI is false, GITHUB_REF_NAME is not defined.
let [<Literal>] badRef =
    String.IF<isCI,
        Env.GITHUB_REF_NAME.Value,
        const Exec<"git", "branch --current">.Value>.Value

// Instead, make sure to use a version that never fails.
// Here, Env returns an empty string if GITHUB_REF_NAME is not defined.
let [<Literal>] goodRef =
    String.IF<isCI,
        Env<"GITHUB_REF_NAME">.Value,
        const Exec<"git", "branch --current">.Value>.Value

// Even better, avoid using IF if you can achieve the same result with default values.
// For example, here, no need to check the CI variable:
// GITHUB_REF_NAME is set iff compiling on Github Actions anyway.
// So you can directly use GITHUB_REF_NAME, with `git branch` as default value.
let [<Literal>] betterRef =
    Env<"GITHUB_REF_NAME", const Exec<"git", "branch --current">.Value>.Value
```

### BuildDate

`FSharp.Data.LiteralProviders.BuildDate` contains the build time as a literal string in ISO-8601 format (["o" format](https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings#Roundtrip)).

```fsharp
open FSharp.Data.LiteralProviders

let utcBuildDate = BuildDate.Utc      // "2019-08-24T19:45:03.2279236Z"
let localBuildDate = BuildDate.Local  // "2019-08-24T21:45:03.2279236+02:00"
```

It can be optionally parameterized by a [date format](https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings).

```fsharp
open FSharp.Data.LiteralProviders

let buildTime = BuildDate<"hh:mm:ss">.Utc  // "21:45:03"
```

### Parsed value

The providers try to parse string values as integer and as boolean. If any of these succeed, a value suffixed with `AsInt` or `AsBool` is provided.

```fsharp
open FSharp.Data.LiteralProviders

let runNumber = Env<"GITHUB_RUN_NUMBER">.Value // eg. "42"

let runNumber = Env<"GITHUB_RUN_NUMBER">.ValueAsInt // eg. 42
```

The following values are parsed this way:

* `Env.Value`
* `TextFile.Text`
* `Exec.Output`
* `Exec.Error`

## Tips for combining type providers

One of the main use cases for FSharp.Data.LiteralProviders is to provide a literal to pass to another type provider. There are several ways to do so:

* Declare each TP with a type alias:

    ```fsharp
    type ConnectionString = Env<"CONNECTION_STRING">
    
    type Sql = SqlProvider<Common.DatabaseProviderTypes.MSSQLSERVER, ConnectionString.Value>
    ```

* Declare a TP's value as Literal then pass it to another TP:

    ```fsharp
    let [<Literal>] ConnectionString = Env<"CONNECTION_STRING">.Value
    
    type Sql = SqlProvider<Common.DatabaseProviderTypes.MSSQLSERVER, ConnectionString>
    ```

* To use a TP entirely inside a parameter of another TP, prefix it with the keyword `const`:

    ```fsharp
    type Sql = SqlProvider<Common.DatabaseProviderTypes.MSSQLSERVER,
                           const Env<"CONNECTION_STRING">.Value>
    ```

## Packaging

FSharp.Data.LiteralProviders is a compile-time only package: all of its provided values are baked into the compiled assembly.
This means that if you are writing a library that uses FSharp.Data.LiteralProviders, your downstream users don't need to depend on it.

Here is how to exclude FSharp.Data.LiteralProviders from your NuGet dependencies.

### Using NuGet

If you are using `dotnet`'s built-in package management, then in your project file, replace the following:

```xml
<PackageReference Include="FSharp.Data.LiteralProviders" Version="..." />
```

with:

```xml
<PackageReference Include="FSharp.Data.LiteralProviders" Version="...">
    <PrivateAssets>All</PrivateAssets>
</PackageReference>
```

### Using Paket

If you are packaging your library with `paket pack`, add the following to your `paket.template`:

```
excludeddependencies
    FSharp.Data.LiteralProviders
```
