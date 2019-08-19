namespace FSharp.Data.LiteralProviders

open System.Reflection
open FSharp.Core.CompilerServices
open ProviderImplementation.ProvidedTypes

[<TypeProvider>]
type Provider(config) as this =
    inherit TypeProviderForNamespaces(config)

    let ns = "FSharp.Data.LiteralProviders"
    let asm = Assembly.GetExecutingAssembly()

    let createTypes() =
        [ EnvProvider.createEnv asm ns
          EnvProvider.createEnvOrDefault asm ns
          FileProvider.createFile asm ns config.ResolutionFolder
          FileProvider.createFileOrDefault asm ns config.ResolutionFolder
        ]

    do this.AddNamespace(ns, createTypes())

[<assembly:TypeProviderAssembly>]
do ()