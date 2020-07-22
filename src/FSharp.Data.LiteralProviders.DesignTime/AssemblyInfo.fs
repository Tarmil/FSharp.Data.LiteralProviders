namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("FSharp.Data.LiteralProviders.DesignTime")>]
[<assembly: AssemblyProductAttribute("FSharp.Data.LiteralProviders")>]
[<assembly: AssemblyDescriptionAttribute("Type providers for literals: compile-time environment variables, text files, etc.")>]
[<assembly: AssemblyVersionAttribute("1.0.0")>]
[<assembly: AssemblyFileVersionAttribute("1.0.0")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] AssemblyTitle = "FSharp.Data.LiteralProviders.DesignTime"
    let [<Literal>] AssemblyProduct = "FSharp.Data.LiteralProviders"
    let [<Literal>] AssemblyDescription = "Type providers for literals: compile-time environment variables, text files, etc."
    let [<Literal>] AssemblyVersion = "1.0.0"
    let [<Literal>] AssemblyFileVersion = "1.0.0"
