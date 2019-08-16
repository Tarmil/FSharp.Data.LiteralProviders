namespace FSharp.Data.LiteralProviders

open System
open System.Reflection
open FSharp.Core.CompilerServices
open ProviderImplementation.ProvidedTypes

[<TypeProvider>]
type EnvProvider(config) as this =
    inherit TypeProviderForNamespaces(config)

    let ns = "FSharp.Data.LiteralProviders"
    let asm = Assembly.GetExecutingAssembly()

    let createTypes() =
        let ty = ProvidedTypeDefinition(asm, ns, "Env", None)

        for entry in Environment.GetEnvironmentVariables() do
            let entry = entry :?> System.Collections.DictionaryEntry
            let name = entry.Key :?> string
            let thisTy = ProvidedTypeDefinition(name, None)
            ProvidedField.Literal("Name", typeof<string>, name) |> thisTy.AddMember
            ProvidedField.Literal("Value", typeof<string>, entry.Value) |> thisTy.AddMember
            thisTy |> ty.AddMember

        [ty]

    do this.AddNamespace(ns, createTypes())

[<assembly:TypeProviderAssembly>]
do ()