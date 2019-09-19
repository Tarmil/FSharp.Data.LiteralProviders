module internal FSharp.Data.LiteralProviders.TextFileProvider

open System.IO
open System.Text
open ProviderImplementation.ProvidedTypes

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
    [ yield ProvidedField.literal "Path" path
      yield ProvidedField.literal "Name" name
      match textContent with
      | Some (encoding, textContent) ->
        yield ProvidedField.literal "Encoding" encoding
        yield ProvidedField.literal "Text" textContent
      | None ->
        yield ProvidedField.literal "Not a text file" "" ]

let create asm ns baseDir =
    let createForFile (path: string) =
        let name = Path.GetFileName path
        ProvidedTypeDefinition(name, None)
        |> ProvidedTypeDefinition.withMembersDelayed (fun () -> fileMembers path name)

    let rec createForDir (path: string) (isRoot: bool) =
        if isRoot
        then ProvidedTypeDefinition(asm, ns, "TextFile", None)
        else ProvidedTypeDefinition(Path.GetFileName path, None)
        |> ProvidedTypeDefinition.withMembersDelayed (fun () ->
            [ for f in Directory.GetFiles(path) do yield createForFile f
              for d in Directory.GetDirectories(path) do yield createForDir d false ])

    createForDir baseDir true
    |> ProvidedTypeDefinition.withTypedStaticParameters
        (Param.mandatory "Path", Param.optional "DefaultValue" "")
        (fun tyName (path, defaultValue) ->
            let path = Path.Combine(baseDir, path)
            let name = Path.GetFileName path
            let exists = File.Exists(path)
            ProvidedTypeDefinition(asm, ns, tyName, None)
            |> ProvidedTypeDefinition.withMember (ProvidedField.literal "Exists" exists)
            |> ProvidedTypeDefinition.withMembersDelayed (fun () -> 
                if exists then
                    fileMembers path name
                else
                    [ ProvidedField.literal "Path" path
                      ProvidedField.literal "Name" name
                      ProvidedField.literal "Text" defaultValue ]))
