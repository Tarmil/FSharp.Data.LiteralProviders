module internal FSharp.Data.LiteralProviders.BuildDateProvider

open System
open System.Reflection
open ProviderImplementation.ProvidedTypes

let private fields (format: string) : list<MemberInfo> =
    [ ProvidedField.literal "Utc" (DateTime.UtcNow.ToString format)
      ProvidedField.literal "Local" (DateTime.Now.ToString format) ]

let create asm ns =
    ProvidedTypeDefinition(asm, ns, "BuildDate", None)
    |> ProvidedTypeDefinition.withMembers (fields "o")
    |> ProvidedTypeDefinition.withTypedStaticParameters
        (Param.mandatory "Format")
        (fun tyName format ->
            ProvidedTypeDefinition(asm, ns, tyName, None)
            |> ProvidedTypeDefinition.withMembers (fields format))
