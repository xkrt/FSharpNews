module FSharpNews.Tests.Acceptance.DataPullerApp

open System
open System.Diagnostics

let private dataPullerExePath =
    let relPath = IO.Path.Combine(Utils.executingAssemblyDirPath(), "../../../FSharpNews.DataPuller/bin/Debug/FSharpNews.DataPuller.exe")
    IO.Path.GetFullPath(relPath)

let private stop (proc: Process) =
    do proc.StandardInput.Close()
    let timeout = TimeSpan.FromSeconds(5.).TotalMilliseconds |> int
    if proc.WaitForExit(timeout) = true
    then if proc.ExitCode = 0
            then ()
            else failwithf "DataPuller exit with code %d" proc.ExitCode
    else failwithf "DataPuller exit timeout"

let start () =
    let args = sprintf "-test http://%s:4141/StackExchange" Utils.machine
    let info = ProcessStartInfo(dataPullerExePath, args)
    info.UseShellExecute <- false
    info.RedirectStandardInput <- true
    let proc = Process.Start(info)
    { new IDisposable with member this.Dispose() = stop proc }
