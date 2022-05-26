module FSharp.Data.LiteralProviders.DesignTime.Implementation

open System.Reflection
open FSharp.Core.CompilerServices
open ProviderImplementation.ProvidedTypes

[<TypeProvider>]
type Provider(config) as this =
    inherit TypeProviderForNamespaces(config, assemblyReplacementMap=[("FSharp.Data.LiteralProviders.DesignTime", "FSharp.Data.LiteralProviders.Runtime")], addDefaultProbingLocation=true)

    let ns = "FSharp.Data.LiteralProviders"
    let asm = Assembly.GetExecutingAssembly()

    do
        [ yield EnvProvider.create asm ns config.ResolutionFolder
          yield TextFileProvider.create asm ns config.ResolutionFolder
          yield BuildDateProvider.create asm ns
          yield ExecProvider.create asm ns config.ResolutionFolder
          yield! ConditionalProvider.create asm ns ]
        |> List.groupBy (fun t -> t.Namespace)
        |> List.iter this.AddNamespace

[<assembly:TypeProviderAssembly>]
do ()