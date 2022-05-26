module internal FSharp.Data.LiteralProviders.DesignTime.ConditionalProvider

open ProviderImplementation.ProvidedTypes

let createCond<'T when 'T : equality and 'T : comparison> asm ns name func =
    let ty = ProvidedTypeDefinition(asm, ns, name, None)
    ty.DefineStaticParameters(
        [ ProvidedStaticParameter("Left", typeof<'T>)
          ProvidedStaticParameter("Right", typeof<'T>) ],
        fun tyName args ->
            match args with
            | [| :? 'T as left; :? 'T as right |] ->
                let ty = ProvidedTypeDefinition(asm, ns, tyName, None)
                ProvidedField.Literal("Value", typeof<bool>, func left right)
                |> ty.AddMember
                ty
            | _ -> failwithf "Invalid args: %A" args)
    ty

let createUnary<'T, 'U> asm ns name func =
    let ty = ProvidedTypeDefinition(asm, ns, name, None)
    ty.DefineStaticParameters(
        [ ProvidedStaticParameter("Arg", typeof<'T>) ],
        fun tyName args ->
            match args with
            | [| :? 'T as arg |] ->
                let ty = ProvidedTypeDefinition(asm, ns, tyName, None)
                ProvidedField.Literal("Value", typeof<'U>, func arg)
                |> ty.AddMember
                ty
            | _ -> failwithf "Invalid args: %A" args)
    ty

let createIf<'T> asm ns =
    let ty = ProvidedTypeDefinition(asm, ns, "IF", None)
    ty.DefineStaticParameters(
        [ ProvidedStaticParameter("Condition", typeof<bool>)
          ProvidedStaticParameter("Then", typeof<'T>)
          ProvidedStaticParameter("Else", typeof<'T>) ],
        fun tyName args ->
            match args with
            | [| :? bool as cond; :? 'T as then'; :? 'T as else' |] ->
                let ty = ProvidedTypeDefinition(asm, ns, tyName, None)
                ProvidedField.Literal("Value", typeof<'T>, if cond then then' else else')
                |> ty.AddMember
                ty
            | _ -> failwithf "Invalid args: %A" args)
    ty

let createEquality<'T when 'T : equality and 'T : comparison> asm ns =
    [ createCond<'T> asm ns "EQ" (=)
      createCond<'T> asm ns "NE" (<>) ]

let createBoolOps asm ns =
    [ createCond<bool> asm ns "AND" (&&)
      createCond<bool> asm ns "OR" (||)
      createCond<bool> asm ns "XOR" (<>)
      createUnary<bool, bool> asm ns "NOT" not ]

let createComparison<'T when 'T : equality and 'T : comparison> asm ns =
    [ createCond<'T> asm ns "LT" (<)
      createCond<'T> asm ns "LE" (<=)
      createCond<'T> asm ns "GT" (>)
      createCond<'T> asm ns "GE" (>=) ]

let create asm ns =
    let nsString = ns + ".String"
    let nsBool = ns + ".Bool"
    let nsInt = ns + ".Int"

    [ yield createIf<string> asm nsString
      yield! createEquality<string> asm nsString

      yield createIf<bool> asm nsBool
      yield! createEquality<bool> asm nsBool
      yield! createBoolOps asm nsBool

      yield createIf<int> asm nsInt
      yield! createEquality<int> asm nsInt
      yield! createComparison<int> asm nsInt ]
