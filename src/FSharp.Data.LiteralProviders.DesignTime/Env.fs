module internal FSharp.Data.LiteralProviders.DesignTime.EnvProvider

open System
open System.Collections
open ProviderImplementation.ProvidedTypes

let createEnv asm ns =
    let ty = ProvidedTypeDefinition(asm, ns, "Env", None)
    for entry in Environment.GetEnvironmentVariables() do
        let entry = entry :?> DictionaryEntry
        let name = entry.Key :?> string
        let thisTy = ProvidedTypeDefinition(name, None)
        ProvidedField.Literal("Name", typeof<string>, name) |> thisTy.AddMember
        ProvidedField.Literal("Value", typeof<string>, entry.Value) |> thisTy.AddMember
        thisTy |> ty.AddMember
    ty

let addEnvOrDefault asm ns (ty: ProvidedTypeDefinition) =
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

let create asm ns =
    createEnv asm ns
    |> addEnvOrDefault asm ns
