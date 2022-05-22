module FSharp.Data.LiteralProviders.DesignTime.Implementation

open System.Reflection
open FSharp.Core.CompilerServices
open ProviderImplementation.ProvidedTypes

[<TypeProvider>]
type Provider(config) as this =
    inherit TypeProviderForNamespaces(config, assemblyReplacementMap=[("FSharp.Data.LiteralProviders.DesignTime", "FSharp.Data.LiteralProviders.Runtime")], addDefaultProbingLocation=true)

    let ns = "FSharp.Data.LiteralProviders"
    let asm = Assembly.GetExecutingAssembly()

    let createTypes() =
        [ EnvProvider.create asm ns config.ResolutionFolder
          TextFileProvider.create asm ns config.ResolutionFolder
          BuildDateProvider.create asm ns
          ExecProvider.create asm ns config.ResolutionFolder ]

    do this.AddNamespace(ns, createTypes())

[<assembly:TypeProviderAssembly>]
do ()