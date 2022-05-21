# FSharp.Data.LiteralProviders

[![Build](https://github.com/Tarmil/FSharp.Data.LiteralProviders/workflows/Build/badge.svg?branch=master)](https://github.com/Tarmil/FSharp.Data.LiteralProviders/actions?query=workflow%3ABuild+branch%3Amaster)
[![Nuget](https://img.shields.io/nuget/v/FSharp.Data.LiteralProviders?logo=nuget)](https://nuget.org/packages/FSharp.Data.LiteralProviders)

This is a collection of type providers that provide literals: compile-time constants that can be used in regular code, but also as parameters to other type providers or .NET attributes.

## Env

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

> Tip: to pass `Env` directly to another type provider without binding it as a `let [<Literal>]`, use the `const` operator:
>
> ```fsharp
> type Sql = SqlProvider<Common.DatabaseProviderTypes.MSSQLSERVER,
>                        const(Env<"CONNECTION_STRING",
>                                  "Server=localhost;Integrated Security=true">.Value)>
> ```

## TextFile

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

## BuildDate

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
