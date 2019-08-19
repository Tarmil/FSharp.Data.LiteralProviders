module internal FSharp.Data.LiteralProviders.FileProvider

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

let createFile asm ns baseDir =
    let createForFile (path: string) =
        let name = Path.GetFileName path
        let ty = ProvidedTypeDefinition(name, None)
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
        ty

    let rec createForDir (path: string) (isRoot: bool) =
        let ty =
            if isRoot
            then ProvidedTypeDefinition(asm, ns, "File", None)
            else ProvidedTypeDefinition(Path.GetFileName path, None)
        ty.AddMembersDelayed(fun () ->
            [ for f in Directory.GetFiles(path) do yield createForFile f
              for d in Directory.GetDirectories(path) do yield createForDir d false ])
        ty

    createForDir baseDir true
