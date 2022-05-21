# Changelog

## 0.4

* [#2](https://github.com/Tarmil/FSharp.Data.LiteralProviders/issues/2)  
    `TextFile`: Add ``` ``..`` ``` to access the parent directory.
* `TextFile`: Add optional parameter `Encoding`.  
    The possible values are `UTF-8`, `UTF-16-le`, `UTF-16-be`, `UTF-32-le` and `UTF-32-be`.  
    When not provided, the encoding is guessed automatically, as before.
* [#12](https://github.com/Tarmil/FSharp.Data.LiteralProviders/issues/12)  
    `TextFile`: Strip the byte order mark from text content.
* [#14](https://github.com/Tarmil/FSharp.Data.LiteralProviders/issues/14)  
    `Env` and `TextFile`: Add optional parameter `EnsureExists`.  
    When `true`, if the file or environment variable doesn't exist, a compile-time error is raised.  
    Otherwise, `DefaultValue` is used, or an empty string if not provided, as before.

## 0.3

* `Env`: Add ability to load `.env` file from resolution path.  
    Add `LoadEnvFile: bool` static parameter to decide whether to load the `.env` file.

## 0.2

* Remove `*OrDefault` providers and instead add parameterized versions of their base providers.
* Add `BuildDate`.

## 0.1

* Add `Env`, `EnvOrDefault`.
* Add `TextFile`, `TextFileOrDefault`.
