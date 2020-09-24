module internal FSharp.Data.LiteralProviders.DesignTime.EnvProvider

open System
open System.Collections
open System.Collections.Generic
open System.IO
open ProviderImplementation.ProvidedTypes

let DefaultEnvFileName = ".env"
let LevelsToSearchForEnvFile = 3

let searchAndLoadEnvFile (baseDir) =
    let envFile =
        seq {
            let mutable currentDirectory = DirectoryInfo(baseDir)

            for _ in [1..LevelsToSearchForEnvFile] do
                yield! currentDirectory.EnumerateFiles(DefaultEnvFileName, SearchOption.TopDirectoryOnly)
                currentDirectory <- currentDirectory.Parent
        }
        |> Seq.map (fun f -> f.FullName)
        |> Seq.tryHead

    match envFile with
    | Some envFile -> DotEnvFile.LoadFile envFile true
    | None -> dict []

let loadEnvVariables () =
    dict [
        for entry in Environment.GetEnvironmentVariables() do
        let entry = entry :?> DictionaryEntry
        let name = entry.Key :?> string
        let value = entry.Value :?> string
        yield name, value
    ]

let mergeDicts (dicts: seq<IDictionary<_, _>>) =
    dict [
        for d in dicts do
            for KeyValue(k, v) in d do
                k, v
    ]

let createEnv asm ns (variables: IDictionary<string, string>) =
    let ty = ProvidedTypeDefinition(asm, ns, "Env", None)
    for KeyValue(name, value) in variables do
        let thisTy = ProvidedTypeDefinition(name, None)
        ProvidedField.Literal("Name", typeof<string>, name) |> thisTy.AddMember
        ProvidedField.Literal("Value", typeof<string>, value) |> thisTy.AddMember
        thisTy |> ty.AddMember
    ty

let addEnvOrDefault asm ns (envVars: IDictionary<string, string>) (fileVars: IDictionary<string, string>) (ty: ProvidedTypeDefinition) =
    ty.DefineStaticParameters(
        [
            ProvidedStaticParameter("Name", typeof<string>)
            ProvidedStaticParameter("DefaultValue", typeof<string>, "")
            ProvidedStaticParameter("LoadEnvFile", typeof<bool>, true)
        ],
        fun tyName args ->
            let ty = ProvidedTypeDefinition(asm, ns, tyName, None)
            let name = args.[0] :?> string
            let variables = if args.[2] :?> bool then mergeDicts [envVars; fileVars] else envVars
            let isSet, envValue = variables.TryGetValue(name)
            let value = if isSet then envValue else args.[1] :?> string
            ProvidedField.Literal("Name", typeof<string>, name) |> ty.AddMember
            ProvidedField.Literal("Value", typeof<string>, value) |> ty.AddMember
            ProvidedField.Literal("IsSet", typeof<bool>, isSet) |> ty.AddMember
            ty)
    ty

let create asm ns baseDir =
    let envVars = loadEnvVariables()
    let fileVars = searchAndLoadEnvFile baseDir
    createEnv asm ns (mergeDicts [envVars; fileVars])
    |> addEnvOrDefault asm ns envVars fileVars
