namespace FSharp.Data.LiteralProviders

open FSharp.Core.CompilerServices
open ProviderImplementation.ProvidedTypes.Functional

[<TypeProvider>]
type Provider(config) =
    inherit FunctionalProvider(config, fun asm ->
        let ns = "FSharp.Data.LiteralProviders"
        [ EnvProvider.create asm ns
          TextFileProvider.create asm ns config.ResolutionFolder
          BuildDateProvider.create asm ns ])

[<assembly:TypeProviderAssembly>]
do ()