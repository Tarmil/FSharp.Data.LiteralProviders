module internal FSharp.Data.LiteralProviders.TextFileProvider

open System.IO
open System.Text
open ProviderImplementation.ProvidedTypes
open ProviderImplementation.ProvidedTypes.Functional

let private encodings : (string * Encoding) list =
    [ "UTF-8", UTF8Encoding(false, true) :> _
      "UTF-16-le", UnicodeEncoding(false, false, true) :> _
      "UTF-16-be", UnicodeEncoding(true, false, true) :> _
      "UTF-32-le", UTF32Encoding(false, false, true) :> _
      "UTF-32-be", UTF32Encoding(true, false, true) :> _ ]

let private fileMembers (path: string) (name: string) =
    let byteContent = File.ReadAllBytes path
    let textContent =
        encodings
        |> List.tryPick (fun (name, e) ->
            try (name, e.GetString(byteContent)) |> Some
            with _ -> None)
    [ yield literal "Path" path
      yield literal "Name" name
      match textContent with
      | Some (encoding, textContent) ->
        yield literal "Encoding" encoding
        yield literal "Text" textContent
      | None ->
        yield literal "Not a text file" "" ]

let createFile asm ns baseDir =
    let createForFile (path: string) =
        let name = Path.GetFileName path
        ProvidedTypeDefinition(name, None)
        |> withMembersDelayed (fun () -> fileMembers path name)

    let rec createForDir (path: string) (isRoot: bool) =
        if isRoot
        then ProvidedTypeDefinition(asm, ns, "TextFile", None)
        else ProvidedTypeDefinition(Path.GetFileName path, None)
        |> withMembersDelayed (fun () ->
            [ for f in Directory.GetFiles(path) do yield createForFile f
              for d in Directory.GetDirectories(path) do yield createForDir d false ])

    createForDir baseDir true

let addFileOrDefault asm ns baseDir =
    withStaticParameters
        [ ProvidedStaticParameter("Path", typeof<string>)
          ProvidedStaticParameter("DefaultValue", typeof<string>, "") ]
        (fun tyName args ->
            let path = Path.Combine(baseDir, args.[0] :?> string)
            let name = Path.GetFileName path
            let exists = File.Exists(path)
            ProvidedTypeDefinition(asm, ns, tyName, None)
            |> withMember (literal "Exists" exists)
            |> withMembersDelayed (fun () -> 
                if exists then
                    fileMembers path name
                else
                    [ literal "Path" path
                      literal "Name" name
                      literal "Text" (args.[1] :?> string) ]))

let create asm ns baseDir =
    createFile asm ns baseDir
    |> addFileOrDefault asm ns baseDir
