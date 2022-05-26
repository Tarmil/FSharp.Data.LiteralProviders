module internal FSharp.Data.LiteralProviders.DesignTime.EnvProvider

open System
open System.Collections
open System.Collections.Generic
open System.IO
open ProviderImplementation.ProvidedTypes

let DefaultEnvFileName = ".env"
let LevelsToSearchForEnvFile = 3

let searchAndLoadEnvFile baseDir =
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
        name, value
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
        thisTy.AddMembers(
            [ yield! Value.String "Value" value
              yield ProvidedField.Literal("Name", typeof<string>, name) ])
        thisTy |> ty.AddMember
    ty

let addEnvOrDefault asm ns (envVars: IDictionary<string, string>) (fileVars: IDictionary<string, string>) (ty: ProvidedTypeDefinition) =
    ty.DefineStaticParameters(
        [
            ProvidedStaticParameter("Name", typeof<string>)
            ProvidedStaticParameter("DefaultValue", typeof<string>, "")
            ProvidedStaticParameter("LoadEnvFile", typeof<bool>, true)
            ProvidedStaticParameter("EnsureExists", typeof<bool>, false)
        ],
        fun tyName args ->
            match args with
            | [| :? string as name; :? string as defaultValue; :? bool as loadEnvFile; :? bool as ensureExists |] ->
                let ty = ProvidedTypeDefinition(asm, ns, tyName, None)
                let variables = if loadEnvFile then mergeDicts [envVars; fileVars] else envVars
                let isSet, envValue = variables.TryGetValue(name)
                if ensureExists && not isSet then
                    failwithf "Environment variable does not exist: %s" name
                else
                    let value = if isSet then envValue else defaultValue
                    ty.AddMembers(
                        [ yield! Value.String "Value" value
                          yield ProvidedField.Literal("Name", typeof<string>, name)
                          yield ProvidedField.Literal("IsSet", typeof<bool>, isSet) ])
                    ty
            | _ -> failwithf "Invalid args: %A" args)
    ty

let create asm ns baseDir =
    let envVars = loadEnvVariables()
    let fileVars = searchAndLoadEnvFile baseDir
    createEnv asm ns (mergeDicts [envVars; fileVars])
    |> addEnvOrDefault asm ns envVars fileVars
