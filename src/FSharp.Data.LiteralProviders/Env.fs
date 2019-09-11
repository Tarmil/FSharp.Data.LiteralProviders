module internal FSharp.Data.LiteralProviders.EnvProvider

open System
open System.Collections
open ProviderImplementation.ProvidedTypes
open ProviderImplementation.ProvidedTypes.Functional

let oneEnv name value =
    ProvidedTypeDefinition(name, None)
    |> withMembers
        [ literal "Name" name
          literal "Value" value ]

let createEnv asm ns =
    ProvidedTypeDefinition(asm, ns, "Env", None)
    |> withMembers
        [ for entry in Environment.GetEnvironmentVariables() do
            let entry = entry :?> DictionaryEntry
            let name = entry.Key :?> string
            let value = entry.Value :?> string
            yield oneEnv name value ]

let addEnvOrDefault asm ns =
    withStaticParameters
        [ ProvidedStaticParameter("Name", typeof<string>)
          ProvidedStaticParameter("DefaultValue", typeof<string>, "") ]
        (fun tyName args ->
            let name = args.[0] :?> string
            let envValue = Environment.GetEnvironmentVariable(name)
            let isSet = not (isNull envValue)
            let value = if isSet then envValue else args.[1] :?> string
            ProvidedTypeDefinition(asm, ns, tyName, None)
            |> withMembers
                [ literal "Name" name
                  literal "Value" value
                  literal "IsSet" isSet ])

let create asm ns =
    createEnv asm ns
    |> addEnvOrDefault asm ns
