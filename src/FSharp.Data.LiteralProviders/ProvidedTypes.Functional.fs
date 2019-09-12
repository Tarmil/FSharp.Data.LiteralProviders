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

type Param<'T>(name: string, ?defaultValue: 'T) =
    member this.AsStaticParameter =
        ProvidedStaticParameter(name, typeof<'T>, ?parameterDefaultValue = Option.map box defaultValue)

type WithTypedStaticParameters =
    static member inline WithStaticParameters< ^This, ^Pars, ^Args when (^This or ^Pars or ^Args) : (static member ToUntyped : ^Pars * (string -> ^Args -> ProvidedTypeDefinition) -> list<ProvidedStaticParameter> * (string -> obj[] -> ProvidedTypeDefinition))>
            (pars: ^Pars, func: string -> ^Args -> ProvidedTypeDefinition, ty: ProvidedTypeDefinition) =
        let param, func = ((^This or ^Pars or ^Args) : (static member ToUntyped : ^Pars * (string -> ^Args -> ProvidedTypeDefinition) -> list<ProvidedStaticParameter> * (string -> obj[] -> ProvidedTypeDefinition)) (pars, func))
        ty.DefineStaticParameters(param, func)
        ty

    static member ToUntyped<'A>(pa: Param<'A>, func: string -> 'A -> ProvidedTypeDefinition) =
        [pa.AsStaticParameter],
        fun tyName (args: obj[]) -> func tyName (unbox args.[0])

    static member ToUntyped<'A, 'B>((pa: Param<'A>, pb: Param<'B>), func: string -> ('A * 'B) -> ProvidedTypeDefinition) =
        [pa.AsStaticParameter; pb.AsStaticParameter],
        fun tyName (args: obj[]) -> func tyName (unbox args.[0], unbox args.[1])

    static member ToUntyped<'A, 'B, 'C>((pa: Param<'A>, pb: Param<'B>, pc: Param<'C>), func: string -> ('A * 'B * 'C) -> ProvidedTypeDefinition) =
        [pa.AsStaticParameter; pb.AsStaticParameter; pc.AsStaticParameter],
        fun tyName (args: obj[]) -> func tyName (unbox args.[0], unbox args.[1], unbox args.[2])

    static member ToUntyped<'A, 'B, 'C, 'D>((pa: Param<'A>, pb: Param<'B>, pc: Param<'C>, pd: Param<'D>), func: string -> ('A * 'B * 'C * 'D) -> ProvidedTypeDefinition) =
        [pa.AsStaticParameter; pb.AsStaticParameter; pc.AsStaticParameter; pd.AsStaticParameter],
        fun tyName (args: obj[]) -> func tyName (unbox args.[0], unbox args.[1], unbox args.[2], unbox args.[3])

    static member ToUntyped<'A, 'B, 'C, 'D, 'E>((pa: Param<'A>, pb: Param<'B>, pc: Param<'C>, pd: Param<'D>, pe: Param<'E>), func: string -> ('A * 'B * 'C * 'D * 'E) -> ProvidedTypeDefinition) =
        [pa.AsStaticParameter; pb.AsStaticParameter; pc.AsStaticParameter; pd.AsStaticParameter; pe.AsStaticParameter],
        fun tyName (args: obj[]) -> func tyName (unbox args.[0], unbox args.[1], unbox args.[2], unbox args.[3], unbox args.[4])

    static member ToUntyped<'A, 'B, 'C, 'D, 'E, 'F>((pa: Param<'A>, pb: Param<'B>, pc: Param<'C>, pd: Param<'D>, pe: Param<'E>, pf: Param<'F>), func: string -> ('A * 'B * 'C * 'D * 'E * 'F) -> ProvidedTypeDefinition) =
        [pa.AsStaticParameter; pb.AsStaticParameter; pc.AsStaticParameter; pd.AsStaticParameter; pe.AsStaticParameter; pf.AsStaticParameter],
        fun tyName (args: obj[]) -> func tyName (unbox args.[0], unbox args.[1], unbox args.[2], unbox args.[3], unbox args.[4], unbox args.[5])

    static member ToUntyped<'A, 'B, 'C, 'D, 'E, 'F, 'G>((pa: Param<'A>, pb: Param<'B>, pc: Param<'C>, pd: Param<'D>, pe: Param<'E>, pf: Param<'F>, pg: Param<'G>), func: string -> ('A * 'B * 'C * 'D * 'E * 'F * 'G) -> ProvidedTypeDefinition) =
        [pa.AsStaticParameter; pb.AsStaticParameter; pc.AsStaticParameter; pd.AsStaticParameter; pe.AsStaticParameter; pf.AsStaticParameter; pg.AsStaticParameter],
        fun tyName (args: obj[]) -> func tyName (unbox args.[0], unbox args.[1], unbox args.[2], unbox args.[3], unbox args.[4], unbox args.[5], unbox args.[6])

    static member ToUntyped<'A, 'B, 'C, 'D, 'E, 'F, 'G, 'H>((pa: Param<'A>, pb: Param<'B>, pc: Param<'C>, pd: Param<'D>, pe: Param<'E>, pf: Param<'F>, pg: Param<'G>, ph: Param<'H>), func: string -> ('A * 'B * 'C * 'D * 'E * 'F * 'G * 'H) -> ProvidedTypeDefinition) =
        [pa.AsStaticParameter; pb.AsStaticParameter; pc.AsStaticParameter; pd.AsStaticParameter; pe.AsStaticParameter; pf.AsStaticParameter; pg.AsStaticParameter; ph.AsStaticParameter],
        fun tyName (args: obj[]) -> func tyName (unbox args.[0], unbox args.[1], unbox args.[2], unbox args.[3], unbox args.[4], unbox args.[5], unbox args.[6], unbox args.[7])

let mandatory<'T> name = Param<'T>(name)
let optional<'T> name defaultValue = Param<'T>(name, defaultValue)

/// Pass a tuple of `mandatory` or `optional` values as params, and receive a tuple of typed values.
let inline withTypedStaticParameters pars func ty = WithTypedStaticParameters.WithStaticParameters<WithTypedStaticParameters, _, _>(pars, func, ty)
