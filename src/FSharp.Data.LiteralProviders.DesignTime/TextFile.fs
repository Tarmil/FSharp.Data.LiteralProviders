module internal FSharp.Data.LiteralProviders.DesignTime.TextFileProvider

open System
open System.IO
open System.Reflection
open System.Text
open ProviderImplementation.ProvidedTypes

let private allEncodings : (string * Encoding) list =
    [ "UTF-8", UTF8Encoding(true, true)
      "UTF-16-le", UnicodeEncoding(false, true, true)
      "UTF-16-be", UnicodeEncoding(true, true, true)
      "UTF-32-le", UTF32Encoding(false, true, true)
      "UTF-32-be", UTF32Encoding(true, true, true) ]
    
type private EncodingsResult = Result<(string * Encoding) list, string>

let private getEncodings (name: string) : EncodingsResult =
    if String.IsNullOrEmpty name then
        Ok allEncodings
    else
        match allEncodings |> List.tryFind (fun (n, _) -> n = name) with
        | Some e -> Ok [e]
        | None -> Error ("Unknown encoding: " + name)

let private stripBom (encoding: Encoding) (bytes: byte[]) =
    let bom = encoding.GetPreamble()
    if bytes.Length >= bom.Length && (bytes, bom) ||> Seq.forall2 (=) then
        true, bytes[bom.Length..]
    else
        false, bytes
        
let private errorMember error =
    ProvidedProperty("Not a text file", typeof<string>, (fun _ -> <@@ "" @@>), isStatic = true)

let private addFileMembers (ty: ProvidedTypeDefinition) (path: string) (name: string) (encodings: EncodingsResult) =
    ty.AddMembersDelayed<MemberInfo>(fun () ->
        match encodings with
        | Ok encodings ->
            let byteContent = File.ReadAllBytes path
            let textContent =
                encodings
                |> List.tryPick (fun (name, e) ->
                    try
                        let hasBom, byteContent = stripBom e byteContent
                        (name, hasBom, e.GetString(byteContent)) |> Some
                    with _ -> None)
            [ ProvidedField.Literal("Path", typeof<string>, path)
              ProvidedField.Literal("Name", typeof<string>, name)
              match textContent with
              | Some (encoding, hasBom, textContent) ->
                  ProvidedField.Literal("Encoding", typeof<string>, encoding)
                  ProvidedField.Literal("Text", typeof<string>, textContent)
                  ProvidedField.Literal("HasBom", typeof<bool>, hasBom)
              | None ->
                  errorMember "Not a text file"
            ]
        | Error error ->
            [ errorMember error ])

let createFile asm ns baseDir =
    let createForFile (path: string) : ProvidedTypeDefinition =
        let name = Path.GetFileName path
        let ty = ProvidedTypeDefinition(name, None)
        addFileMembers ty path name (Ok allEncodings)
        ty

    let rec createForDir (path: string) (isRoot: bool) : ProvidedTypeDefinition =
        let ty =
            if isRoot
            then ProvidedTypeDefinition(asm, ns, "TextFile", None)
            else ProvidedTypeDefinition(Path.GetFileName path, None)
        ty.AddMembersDelayed(fun () ->
            [ for f in Directory.GetFiles(path) do createForFile f
              for d in Directory.GetDirectories(path) do createForDir d false ])
        ty

    createForDir baseDir true

let addFileOrDefault asm ns baseDir (ty: ProvidedTypeDefinition) =
    ty.DefineStaticParameters(
        [ ProvidedStaticParameter("Path", typeof<string>)
          ProvidedStaticParameter("DefaultValue", typeof<string>, "")
          ProvidedStaticParameter("Encoding", typeof<string>, "") ],
        fun tyName args ->
            match args with
            | [| :? string as path; :? string as defaultValue; :? string as encoding |] ->
                let ty = ProvidedTypeDefinition(asm, ns, tyName, None)
                let path = Path.Combine(baseDir, path)
                let name = Path.GetFileName path
                let exists = File.Exists(path)
                ProvidedField.Literal("Exists", typeof<bool>, exists) |> ty.AddMember
                if exists then
                    let encodings = getEncodings encoding
                    addFileMembers ty path name encodings
                else
                    ty.AddMembers(
                        [ ProvidedField.Literal("Path", typeof<string>, path)
                          ProvidedField.Literal("Name", typeof<string>, name)
                          ProvidedField.Literal("Text", typeof<string>, defaultValue) ])
                ty
            | _ -> failwithf "Invalid args: %A" args)
    ty

let create asm ns baseDir =
    createFile asm ns baseDir
    |> addFileOrDefault asm ns baseDir
