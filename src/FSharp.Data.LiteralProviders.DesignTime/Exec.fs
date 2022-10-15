module internal FSharp.Data.LiteralProviders.DesignTime.ExecProvider

open System
open System.Diagnostics
open System.IO
open System.Text
open ProviderImplementation.ProvidedTypes

type ExecFailedException(exitCode: int, message: string) =
    inherit exn(message)
    member _.ExitCode = exitCode

type Args =
    { Command: string
      Args: string
      Input: string option
      Directory: string
      EnsureSuccess: bool
      Timeout: int }

type Result =
    { Output: string
      Error: string
      ExitCode: int }

let (|Args|_|) baseDir (args: obj[]) =
    match args with
    | [| :? string as command
         :? string as args
         :? string as input
         :? string as dir
         :? bool as ensureSuccess
         :? int as timeout |] ->
        let dir = Path.Combine(baseDir, dir)
        { Command = command
          Args = args
          Input = if String.IsNullOrEmpty input then None else Some input
          Directory = dir
          EnsureSuccess = ensureSuccess
          Timeout = timeout }
        |> Some
    | _ -> None

let execute args =
    let proc = new Process()
    proc.StartInfo.UseShellExecute <- false
    proc.StartInfo.FileName <- args.Command
    proc.StartInfo.Arguments <- args.Args
    proc.StartInfo.WorkingDirectory <- args.Directory
    proc.StartInfo.RedirectStandardInput <- true
    proc.StartInfo.RedirectStandardOutput <- true
    proc.StartInfo.RedirectStandardError <- true
    let output = StringBuilder()
    let error = StringBuilder()
    proc.OutputDataReceived.Add(fun e -> output.Append(e.Data) |> ignore)
    proc.ErrorDataReceived.Add(fun e -> error.Append(e.Data) |> ignore)
    proc.Start() |> ignore
    let startTime = try proc.StartTime with _ -> DateTime.Now
    args.Input |> Option.iter proc.StandardInput.Write
    proc.BeginOutputReadLine()
    proc.BeginErrorReadLine()
    if not (proc.WaitForExit(args.Timeout)) then
        proc.Kill()
        proc.WaitForExit()
        raise (ExecFailedException(-1, sprintf "Process timed out after %A" (proc.ExitTime - startTime)))
    proc.WaitForExit()
    if args.EnsureSuccess && proc.ExitCode <> 0 then
        raise (ExecFailedException(proc.ExitCode, sprintf "Process exited with status code %i" proc.ExitCode))
    else
        { Output = output.ToString()
          Error = error.ToString()
          ExitCode = proc.ExitCode }

let create asm ns baseDir =
    let ty = ProvidedTypeDefinition(asm, ns, "Exec", None)
    ty.DefineStaticParameters(
        [ ProvidedStaticParameter("Command", typeof<string>)
          ProvidedStaticParameter("Arguments", typeof<string>, "")
          ProvidedStaticParameter("Input", typeof<string>, "")
          ProvidedStaticParameter("Directory", typeof<string>, ".")
          ProvidedStaticParameter("EnsureSuccess", typeof<bool>, true)
          ProvidedStaticParameter("Timeout", typeof<int>, 10_000) ],
        fun tyName args ->
            match args with
            | Args baseDir args ->
                let ty = ProvidedTypeDefinition(asm, ns, tyName, None)
                ty.AddMembersDelayed(fun () ->
                    try
                        let res = execute args
                        [ yield! Value.String "Output" res.Output
                          yield! Value.String "Error" res.Error
                          yield ProvidedField.Literal("ExitCode", typeof<int>, res.ExitCode) ]
                    with exn ->
                        let exitCode =
                            match exn with
                            | :? ExecFailedException as exn -> exn.ExitCode
                            | _ -> -1
                        [ ProvidedField.Literal("ExitCode", typeof<int>, exitCode)
                          ProvidedField.Literal("ErrorMessage", typeof<string>, exn.Message)
                          ProvidedField.Literal(exn.Message, typeof<string>, exn.ToString()) ])
                ty
            | _ -> failwithf "Invalid args: %A" args)
    ty
