module FSharp.Data.LiteralProviders.DesignTime.Value

open System
open System.Reflection
open ProviderImplementation.ProvidedTypes

let String (name: string) (value: string) : seq<MemberInfo> =
    [ ProvidedField.Literal(name, typeof<string>, value)

      match Int32.TryParse(value) with
      | true, value -> ProvidedField.Literal(name + "AsInt", typeof<int>, value)
      | false, _ -> ()

      match Boolean.TryParse(value) with
      | true, value -> ProvidedField.Literal(name + "AsBool", typeof<bool>, value)
      | false, _ -> () ]
