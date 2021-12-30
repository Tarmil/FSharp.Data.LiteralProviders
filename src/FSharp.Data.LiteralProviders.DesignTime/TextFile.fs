module internal FSharp.Data.LiteralProviders.DesignTime.TextFileProvider

open System.IO
open System.Reflection
open System.Text
open ProviderImplementation.ProvidedTypes

let private encodings : (string * Encoding) list =
    [ "UTF-8", UTF8Encoding(false, true)
      "UTF-16-le", UnicodeEncoding(false, false, true)
      "UTF-16-be", UnicodeEncoding(true, false, true)
      "UTF-32-le", UTF32Encoding(false, false, true)
      "UTF-32-be", UTF32Encoding(true, false, true) ]

let private addFileMembers (ty: ProvidedTypeDefinition) (path: string) (name: string) =
    ty.AddMembersDelayed(fun () ->
        let byteContent = File.ReadAllBytes path
        let textContent =
            encodings
            |> List.tryPick (fun (name, e) ->
                try (name, e.GetString(byteContent)) |> Some
                with _ -> None)
        [ ProvidedField.Literal("Path", typeof<string>, path)
          ProvidedField.Literal("Name", typeof<string>, name)
          match textContent with
          | Some (encoding, textContent) ->
              ProvidedField.Literal("Encoding", typeof<string>, encoding)
              ProvidedField.Literal("Text", typeof<string>, textContent)
          | None ->
              ProvidedProperty("Not a text file", typeof<string>, (fun _ -> <@@ "" @@>), isStatic = true)
        ] : list<MemberInfo>)

let createFile asm ns baseDir =
    let createForFile (path: string) : ProvidedTypeDefinition =
        let name = Path.GetFileName path
        let ty = ProvidedTypeDefinition(name, None)
        addFileMembers ty path name
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
        [ProvidedStaticParameter("Path", typeof<string>); ProvidedStaticParameter("DefaultValue", typeof<string>, "")],
        fun tyName args ->
            let ty = ProvidedTypeDefinition(asm, ns, tyName, None)
            let path = Path.Combine(baseDir, args.[0] :?> string)
            let name = Path.GetFileName path
            let exists = File.Exists(path)
            ProvidedField.Literal("Exists", typeof<bool>, exists) |> ty.AddMember
            if exists then
                addFileMembers ty path name
            else
                ty.AddMembers(
                    [ ProvidedField.Literal("Path", typeof<string>, path)
                      ProvidedField.Literal("Name", typeof<string>, name)
                      ProvidedField.Literal("Text", typeof<string>, args.[1]) ])
            ty)
    ty

let create asm ns baseDir =
    createFile asm ns baseDir
    |> addFileOrDefault asm ns baseDir
