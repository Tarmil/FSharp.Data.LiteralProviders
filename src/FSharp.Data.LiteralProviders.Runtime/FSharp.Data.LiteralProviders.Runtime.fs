namespace MyNamespace

open System

// Put the TypeProviderAssemblyAttribute in the runtime DLL, pointing to the design-time DLL
[<assembly:CompilerServices.TypeProviderAssembly("FSharp.Data.LiteralProviders.DesignTime.dll")>]
do ()
