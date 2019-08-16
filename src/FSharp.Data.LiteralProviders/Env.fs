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

    let createEnv() =
        let ty = ProvidedTypeDefinition(asm, ns, "Env", None)
        for entry in Environment.GetEnvironmentVariables() do
            let entry = entry :?> System.Collections.DictionaryEntry
            let name = entry.Key :?> string
            let thisTy = ProvidedTypeDefinition(name, None)
            ProvidedField.Literal("Name", typeof<string>, name) |> thisTy.AddMember
            ProvidedField.Literal("Value", typeof<string>, entry.Value) |> thisTy.AddMember
            thisTy |> ty.AddMember
        ty

    let createEnvOrDefault() =
        let ty = ProvidedTypeDefinition(asm, ns, "EnvOrDefault", None)
        ty.DefineStaticParameters(
            [ProvidedStaticParameter("Name", typeof<string>); ProvidedStaticParameter("DefaultValue", typeof<string>, "")],
            fun tyName args ->
                let ty = ProvidedTypeDefinition(asm, ns, tyName, None)
                let name = args.[0] :?> string
                let envValue = Environment.GetEnvironmentVariable(name)
                let isSet = not (isNull envValue)
                let value = if isSet then envValue else args.[1] :?> string
                ProvidedField.Literal("Name", typeof<string>, name) |> ty.AddMember
                ProvidedField.Literal("Value", typeof<string>, value) |> ty.AddMember
                ProvidedField.Literal("IsSet", typeof<bool>, isSet) |> ty.AddMember
                ty)
        ty

    let createTypes() =
        [createEnv(); createEnvOrDefault()]

    do this.AddNamespace(ns, createTypes())

[<assembly:TypeProviderAssembly>]
do ()