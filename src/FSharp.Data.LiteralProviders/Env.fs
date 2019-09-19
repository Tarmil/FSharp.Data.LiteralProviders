module internal FSharp.Data.LiteralProviders.EnvProvider

open System
open System.Collections
open ProviderImplementation.ProvidedTypes

let oneEnv name value =
    ProvidedTypeDefinition(name, None)
    |> ProvidedTypeDefinition.withMembers
        [ ProvidedField.literal "Name" name
          ProvidedField.literal "Value" value ]

let create asm ns =
    ProvidedTypeDefinition(asm, ns, "Env", None)
    |> ProvidedTypeDefinition.withMembers
        [ for entry in Environment.GetEnvironmentVariables() do
            let entry = entry :?> DictionaryEntry
            let name = entry.Key :?> string
            let value = entry.Value :?> string
            yield oneEnv name value ]
    |> ProvidedTypeDefinition.withTypedStaticParameters
        (Param.mandatory "Name", Param.optional "DefaultValue" "")
        (fun tyName (name, defaultValue) ->
            let envValue = Environment.GetEnvironmentVariable(name)
            let isSet = not (isNull envValue)
            let value = if isSet then envValue else defaultValue
            ProvidedTypeDefinition(asm, ns, tyName, None)
            |> ProvidedTypeDefinition.withMembers
                [ ProvidedField.literal "Name" name
                  ProvidedField.literal "Value" value
                  ProvidedField.literal "IsSet" isSet ])
