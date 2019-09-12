module internal FSharp.Data.LiteralProviders.BuildDateProvider

open System
open System.Reflection
open ProviderImplementation.ProvidedTypes
open ProviderImplementation.ProvidedTypes.Functional

let private fields (format: string) : list<MemberInfo> =
    [ literal "Utc" (DateTime.UtcNow.ToString format)
      literal "Local" (DateTime.Now.ToString format) ]

let create asm ns =
    ProvidedTypeDefinition(asm, ns, "BuildDate", None)
    |> withMembers (fields "o")
    |> withTypedStaticParameters
        (mandatory "Format")
        (fun tyName format ->
            ProvidedTypeDefinition(asm, ns, tyName, None)
            |> withMembers (fields format))
