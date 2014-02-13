module FSharpNews.Tests.DataPull.Service.ServiceApplication

open System
open System.Diagnostics
open FSharpNews.Tests.Core

let private dataPullerExePath =
    let relPath = IO.Path.Combine(Environment.executingAssemblyDirPath(), "../../../FSharpNews.DataPull.Service/bin/Debug/FSharpNews.DataPull.Service.exe")
    IO.Path.GetFullPath(relPath)

let private stop (proc: Process) =
    do proc.CloseMainWindow() |> ignore
    let exitTimeout = TimeSpan.FromSeconds(3.).TotalMilliseconds |> int
    if proc.WaitForExit(exitTimeout) = false
    then failwithf "FSharpNews.DataPull.Service exit timeout"

let start () =
    let args = [ sprintf "-stackExchangeUrl:%s" StackExchangeApi.baseUrl
                 sprintf "-twitterUrl:%s" TwitterApi.baseUrl
                 sprintf "-nugetUrl:%s" NuGetApi.baseUrl ] |> String.concat " "
    let info = ProcessStartInfo(dataPullerExePath, args)
    let proc = Process.Start(info)
    { new IDisposable with member this.Dispose() = stop proc }
