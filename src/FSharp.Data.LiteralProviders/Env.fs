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

let create asm ns =
    ProvidedTypeDefinition(asm, ns, "Env", None)
    |> withMembers
        [ for entry in Environment.GetEnvironmentVariables() do
            let entry = entry :?> DictionaryEntry
            let name = entry.Key :?> string
            let value = entry.Value :?> string
            yield oneEnv name value ]
    |> withTypedStaticParameters
        (mandatory "Name", optional "DefaultValue" "")
        (fun tyName (name, defaultValue) ->
            let envValue = Environment.GetEnvironmentVariable(name)
            let isSet = not (isNull envValue)
            let value = if isSet then envValue else defaultValue
            ProvidedTypeDefinition(asm, ns, tyName, None)
            |> withMembers
                [ literal "Name" name
                  literal "Value" value
                  literal "IsSet" isSet ])
