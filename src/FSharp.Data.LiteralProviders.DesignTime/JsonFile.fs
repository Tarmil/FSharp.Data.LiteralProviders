module internal FSharp.Data.LiteralProviders.DesignTime.JsonFileProvider

open System.IO
open System.Text.Json
open ProviderImplementation.ProvidedTypes

let rec addElement (ty: ProvidedTypeDefinition) (e: JsonElement) (key: string option) =
    match e.ValueKind with
    | JsonValueKind.Null
    | JsonValueKind.Undefined -> ty.AddMember(ProvidedField.Literal(defaultArg key "Value", typeof<string>, (null: string)))
    | JsonValueKind.False -> ty.AddMember(ProvidedField.Literal(defaultArg key "Value", typeof<bool>, false))
    | JsonValueKind.True -> ty.AddMember(ProvidedField.Literal(defaultArg key "Value", typeof<bool>, true))
    | JsonValueKind.String -> ty.AddMember(ProvidedField.Literal(defaultArg key "Value", typeof<string>, e.GetString()))
    | JsonValueKind.Number ->
        match e.TryGetInt32() with
        | true, v -> ty.AddMember(ProvidedField.Literal(defaultArg key "Value", typeof<int>, v))
        | false, _ -> ty.AddMember(ProvidedField.Literal(defaultArg key "Value", typeof<float>, e.GetDouble()))
    | JsonValueKind.Object ->
        match key with
        | Some key -> ty.AddMemberDelayed(fun () ->
            let t = ProvidedTypeDefinition(key, None)
            for child in e.EnumerateObject() do
                addElement t child.Value (Some child.Name)
            t)
        | None ->
            for child in e.EnumerateObject() do
                addElement ty child.Value (Some child.Name)
    | JsonValueKind.Array ->
        match key with
        | Some key -> ty.AddMemberDelayed(fun () ->
            let t = ProvidedTypeDefinition(key, None)
            for k, v in e.EnumerateArray() |> Seq.indexed do
                addElement t v (Some (string k))
            t)
        | None ->
        for k, v in e.EnumerateArray() |> Seq.indexed do
            addElement ty v (Some (string k))
    | k -> failwith $"Impossible: unknown JsonValueKind {k}"

let addFileMembers (ty: ProvidedTypeDefinition) (path: string) =
    use s = File.OpenRead(path)
    try
        let json = JsonDocument.Parse(s).RootElement
        addElement ty json None
    with e ->
        ty.AddMember(ProvidedField.Literal("Error", typeof<string>, e.Message))

let create asm ns baseDir =
    FileSystem.createFile "JsonFile" asm ns baseDir "*.json" (fun ty path _ -> addFileMembers ty path)