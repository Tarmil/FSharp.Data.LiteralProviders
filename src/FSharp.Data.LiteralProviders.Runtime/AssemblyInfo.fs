namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("FSharp.Data.LiteralProviders.Runtime")>]
[<assembly: AssemblyProductAttribute("FSharp.Data.LiteralProviders")>]
[<assembly: AssemblyDescriptionAttribute("This library is for the .NET platform implementing FSharp.Data.LiteralProviders.")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] AssemblyTitle = "FSharp.Data.LiteralProviders.Runtime"
    let [<Literal>] AssemblyProduct = "FSharp.Data.LiteralProviders"
    let [<Literal>] AssemblyDescription = "This library is for the .NET platform implementing FSharp.Data.LiteralProviders."
