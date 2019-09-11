module internal FSharp.Data.LiteralProviders.BuildDateProvider

open System
open System.Reflection
open ProviderImplementation.ProvidedTypes
open ProviderImplementation.ProvidedTypes.Functional

let private fields (format: string) : list<MemberInfo> =
    [ literal "Utc" (DateTime.UtcNow.ToString format)
      literal "Local" (DateTime.Now.ToString format) ]

let createBuildDate asm ns =
    ProvidedTypeDefinition(asm, ns, "BuildDate", None)
    |> withMembers (fields "o")

let addWithFormat asm ns =
    withStaticParameters
        [ ProvidedStaticParameter("Format", typeof<string>) ]
        (fun tyName args ->
            let format = args.[0] :?> string
            ProvidedTypeDefinition(asm, ns, tyName, None)
            |> withMembers (fields format))

let create asm ns =
    createBuildDate asm ns
    |> addWithFormat asm ns
