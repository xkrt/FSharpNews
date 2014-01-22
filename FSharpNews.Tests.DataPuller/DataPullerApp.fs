module FSharpNews.Tests.DataPuller.DataPullerApp

open System
open System.Diagnostics
open FSharpNews.Tests.Core

let private dataPullerExePath =
    let relPath = IO.Path.Combine(Environment.executingAssemblyDirPath(), "../../../FSharpNews.DataPuller/bin/Debug/FSharpNews.DataPuller.exe")
    IO.Path.GetFullPath(relPath)

let private stop (proc: Process) =
    do proc.CloseMainWindow() |> ignore
    let exitTimeout = TimeSpan.FromSeconds(3.).TotalMilliseconds |> int
    if proc.WaitForExit(exitTimeout) = false
    then failwithf "DataPuller exit timeout"

let start () =
    let args = ["-test"; FakeApi.seUrl; FakeApi.twitterUrl] |> String.concat " "
    let info = ProcessStartInfo(dataPullerExePath, args)
    let proc = Process.Start(info)
    { new IDisposable with member this.Dispose() = stop proc }
