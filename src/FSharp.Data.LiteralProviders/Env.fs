module internal FSharp.Data.LiteralProviders.EnvProvider

open System
open System.Collections
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
    | Some envFile -> DotNetEnv.Env.Load(envFile)
    | None -> DotNetEnv.Env.Load()

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

let create asm ns baseDir =
    searchAndLoadEnvFile(baseDir)
    
    createEnv asm ns
    |> addEnvOrDefault asm ns
