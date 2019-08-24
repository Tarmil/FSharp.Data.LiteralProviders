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
        [ EnvProvider.create asm ns
          TextFileProvider.create asm ns config.ResolutionFolder ]

    do this.AddNamespace(ns, createTypes())

[<assembly:TypeProviderAssembly>]
do ()