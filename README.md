# FSharp.Data.LiteralProviders

[![Build status](https://ci.appveyor.com/api/projects/status/hteaej04prcnaes7/branch/master?svg=true)](https://ci.appveyor.com/project/Tarmil/fsharp-data-literalproviders/branch/master)
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

Note that `Env` fails to compile if the environment variable is not set.

## EnvOrDefault

Like `Env`, `EnvOrDefault` contains literals for environment variables during compile time.
However, `EnvOrDefault` takes the variable's name as a string parameter, and returns the empty string if the variable is not set.

```fsharp
open FSharp.Data.LiteralProviders

let vsVersion = EnvOrDefault<"VisualStudioEdition">.Value

match vsVersion with
| "" -> printfn "This program wasn't compiled with Visual Studio."
| v -> printfn "This program was built with Visual Studio %s." v
```

You can pass a second parameter that will be used as the default value instead of the empty string.

```fsharp
open FSharp.Data.Sql
open FSharp.Data.LiteralProviders

let [<Literal>] connString =
    EnvOrDefault<"CONNECTION_STRING", "Server=localhost;Integrated Security=true">.Value

type Sql = SqlProvider<Common.DatabaseProviderTypes.MSSQLSERVER, connString>
```

> Tip: to pass `EnvOrDefault` directly to another type provider without binding it as a `let [<Literal>]`, use the `const` operator:
>
> ```fsharp
> type Sql = SqlProvider<Common.DatabaseProviderTypes.MSSQLSERVER,
>                        const(EnvOrDefault<"CONNECTION_STRING",
>                                           "Server=localhost;Integrated Security=true">)>.Value
> ```

## TextFile

`FSharp.Data.LiteralProviders.TextFile` contains literals that are read from text files during compilation.

```fsharp
open FSharp.Data.LiteralProviders

/// The compile-time contents of the file <projectFolder>/build/version.txt
let [<Literal>] version = TextFile.build.``version.txt``.Text
```

## TextFileOrDefault

Like `TextFile`, `TextFileOrDefault` contains literals that are read from text files during compilation.
However, `TextFileOrDefault` takes the file path as a string parameter, and returns the empty string if the file is not found or is not a text file.

```fsharp
open FSharp.Data.LiteralProviders

/// The compile-time contents of the file <projectFolder>/build/version.txt
/// or "" if this file doesn't exist.
let [<Literal>] version = TextFileOrDefault<"build/version.txt">.Text
```

You can pass a second parameter that will be used as the default value instead of the empty string.

```fsharp
open FSharp.Data.LiteralProviders

/// The compile-time contents of the file <projectFolder>/build/version.txt
/// or "1.0" if this file doesn't exist.
let [<Literal>] version = TextFileOrDefault<"build/version.txt", "1.0">.Text
```
