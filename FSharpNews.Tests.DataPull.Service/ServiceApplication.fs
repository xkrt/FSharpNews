module FSharpNews.Tests.DataPull.Service.ServiceApplication

open System
open System.Diagnostics
open FSharpNews.Data
open FSharpNews.Tests.Core

#if DEBUG
let private appRelativePath = "../../../FSharpNews.DataPull.Service/bin/Debug/FSharpNews.DataPull.Service.exe"
#else
let private appRelativePath = "../../../FSharpNews.DataPull.Service/bin/Release/FSharpNews.DataPull.Service.exe"
#endif

let private dataPullerExePath =
    let relPath = IO.Path.Combine(Environment.executingAssemblyDirPath(), appRelativePath)
    IO.Path.GetFullPath(relPath)

let private stop (proc: Process) =
    do proc.CloseMainWindow() |> ignore
    let exitTimeout = TimeSpan.FromSeconds(3.).TotalMilliseconds |> int
    if proc.WaitForExit(exitTimeout) = false
    then failwithf "FSharpNews.DataPull.Service exit timeout"

let private disSe = "-disableStackExchange"
let private disTw = "-disableTwitter"
let private disNu = "-disableNuget"
let private disFs = "-disableFssnip"
let private disFp = "-disableFpish"
let private disGi = "-disableGists"
let private disRe = "-disableRepos"
let private disGr = "-disableGroups"
let private allDisabled = [disSe; disTw; disNu; disFs;  disFp; disGi; disRe; disGr]

let private start targetSource =
    let enable s = allDisabled |> List.filter ((<>) s)
    let args = match targetSource with
               | StackExchange -> [sprintf "-stackExchangeUrl:%s" StackExchangeApi.baseUrl] @ (enable disSe)
               | Twitter -> [sprintf "-twitterStreamUrl:%s" TwitterApi.baseUrl
                             sprintf "-twitterSearchUrl:%s" TwitterApi.baseSearchUrl] @ (enable disTw)
               | NuGet -> [sprintf "-nugetUrl:%s" NuGetApi.baseUrl] @ (enable disNu)
               | FsSnip -> [sprintf "-fssnipUrl:%s" FsSnipApi.baseUrl] @ (enable disFs)
               | FPish -> [sprintf "-fpishUrl:%s" FPishFeed.baseUrl] @ (enable disFp)
               | Gists -> [sprintf "-githubUrl:%s" GitHubApi.baseUrl] @ (enable disGi)
               | Repos -> [sprintf "-githubUrl:%s" GitHubApi.baseUrl] @ (enable disRe)
               | Groups -> [sprintf "-groupsUrl:%s" GroupsFeed.baseUrl] @ (enable disGr)
               |> String.concat " "
    let info = ProcessStartInfo(dataPullerExePath, args)
    let proc = Process.Start info
    { new IDisposable with member this.Dispose() = stop proc }

let startAndSleep targetSource =
    use app = start targetSource
    sleep 10
