module internal FSharp.Data.LiteralProviders.TextFileProvider

open System.IO
open System.Reflection
open System.Text
open ProviderImplementation.ProvidedTypes

let private encodings : (string * Encoding) list =
    [ "UTF-8", UTF8Encoding(false, true) :> _
      "UTF-16-le", UnicodeEncoding(false, false, true) :> _
      "UTF-16-be", UnicodeEncoding(true, false, true) :> _
      "UTF-32-le", UTF32Encoding(false, false, true) :> _
      "UTF-32-be", UTF32Encoding(true, false, true) :> _ ]

let private addFileMembers (ty: ProvidedTypeDefinition) (path: string) (name: string) =
    ty.AddMembersDelayed(fun () ->
        let byteContent = File.ReadAllBytes path
        let textContent =
            encodings
            |> List.tryPick (fun (name, e) ->
                try (name, e.GetString(byteContent)) |> Some
                with _ -> None)
        [ yield ProvidedField.Literal("Path", typeof<string>, path) :> _
          yield ProvidedField.Literal("Name", typeof<string>, name) :> _
          match textContent with
          | Some (encoding, textContent) ->
              yield ProvidedField.Literal("Encoding", typeof<string>, encoding) :> _
              yield ProvidedField.Literal("Text", typeof<string>, textContent) :> _
          | None ->
          yield ProvidedProperty("Not a text file", typeof<string>, (fun _ -> <@@ "" @@>), isStatic = true) :> _
        ] : list<MemberInfo>)

let createFile asm ns baseDir =
    let createForFile (path: string) =
        let name = Path.GetFileName path
        let ty = ProvidedTypeDefinition(name, None)
        addFileMembers ty path name
        ty

    let rec createForDir (path: string) (isRoot: bool) =
        let ty =
            if isRoot
            then ProvidedTypeDefinition(asm, ns, "TextFile", None)
            else ProvidedTypeDefinition(Path.GetFileName path, None)
        ty.AddMembersDelayed(fun () ->
            [ for f in Directory.GetFiles(path) do yield createForFile f
              for d in Directory.GetDirectories(path) do yield createForDir d false ])
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
