module ProviderImplementation.ProvidedTypes.Functional

open System.Reflection

let withStaticParameters pars make (ty: ProvidedTypeDefinition) =
    ty.DefineStaticParameters(pars, make)
    ty

let withMembers (items: list<#MemberInfo>) (ty: ProvidedTypeDefinition) =
    ty.AddMembers items
    ty

let withMember (items: MemberInfo) (ty: ProvidedTypeDefinition) =
    ty.AddMember items
    ty

let withMembersDelayed (items: unit -> list<#MemberInfo>) (ty: ProvidedTypeDefinition) =
    ty.AddMembersDelayed items
    ty

let withMemberDelayed (items: unit -> #MemberInfo) (ty: ProvidedTypeDefinition) =
    ty.AddMemberDelayed items
    ty

let literal<'T> (name: string) (value: 'T) =
    ProvidedField.Literal(name, typeof<'T>, box value)

type FunctionalProvider(config, build: Assembly -> list<ProvidedTypeDefinition>) as this =
    inherit TypeProviderForNamespaces(config)

    do
        Assembly.GetExecutingAssembly()
        |> build
        |> List.groupBy (fun ty -> ty.Namespace)
        |> List.iter this.AddNamespace
