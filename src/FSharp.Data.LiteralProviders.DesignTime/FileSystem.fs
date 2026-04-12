module FSharp.Data.LiteralProviders.DesignTime.FileSystem

open System.IO
open ProviderImplementation.ProvidedTypes


let createFile (providerName: string) asm ns baseDir (pattern: string) addFileMembers =
    let createForFile (path: string) : ProvidedTypeDefinition =
        let name = Path.GetFileName path
        let ty = ProvidedTypeDefinition(name, None)
        addFileMembers ty path name
        ty

    let rec createForDir (path: string) (name: string option) : ProvidedTypeDefinition =
        let ty =
            match name with
            | None -> ProvidedTypeDefinition(asm, ns, providerName, None)
            | Some name -> ProvidedTypeDefinition(name, None)
        ty.AddMembersDelayed(fun () ->
            [ for f in Directory.GetFiles(path, pattern) do createForFile f
              for d in Directory.GetDirectories(path) do createForDir d (Some (Path.GetFileName d))
              let parent = Path.GetDirectoryName(path)
              if parent <> path then createForDir parent (Some "..") ])
        ty

    createForDir baseDir None
