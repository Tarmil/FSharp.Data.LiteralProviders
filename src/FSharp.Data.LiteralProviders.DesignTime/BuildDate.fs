module internal FSharp.Data.LiteralProviders.DesignTime.BuildDateProvider

open System
open ProviderImplementation.ProvidedTypes

let private addFields (ty: ProvidedTypeDefinition) (format: string) =
    ProvidedField.Literal("Utc", typeof<string>, DateTime.UtcNow.ToString(format)) |> ty.AddMember
    ProvidedField.Literal("Local", typeof<string>, DateTime.Now.ToString(format)) |> ty.AddMember

let createBuildDate asm ns =
    let ty = ProvidedTypeDefinition(asm, ns, "BuildDate", None)
    addFields ty "o"
    ty

let addWithFormat asm ns (ty: ProvidedTypeDefinition) =
    ty.DefineStaticParameters(
        [ProvidedStaticParameter("Format", typeof<string>)],
        fun tyName args ->
            let ty = ProvidedTypeDefinition(asm, ns, tyName, None)
            let format = args.[0] :?> string
            addFields ty format
            ty)
    ty

let create asm ns =
    createBuildDate asm ns
    |> addWithFormat asm ns
