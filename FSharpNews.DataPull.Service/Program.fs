﻿module FSharpNews.DataPull.Service.Program

open System
open System.Threading
open FSharpNews.Data
open FSharpNews.Utils
open FSharpNews.DataPull.Service.TopShelf

let private log = Logger.create "Program"

do AppDomain.CurrentDomain.UnhandledException.Add UnhandledExceptionLogger.handle

let private repeatForeverArg (interval: TimeSpan) fn startArg =
    async { let rec loop arg =
                let newArg = ref arg
                try newArg := fn arg
                with
                | e -> do log.Error "%O" e
                do Thread.Sleep interval
                loop !newArg
            loop startArg }

let private repeatForever (interval: TimeSpan) fn = repeatForeverArg interval fn ()

let private stackExchange (conf: Configuration.Type) =
    let fetchNewQuestions site =
        let lastQuestionTime = Storage.getTimeOfLastQuestion site
        let startDate = lastQuestionTime.AddSeconds(1.)
        let activitiesWithRaws = StackExchange.fetch conf.StackExchange site startDate
        do Storage.saveAll activitiesWithRaws
    let repeat = repeatForever (TimeSpan.FromMinutes(5.))
    repeat (fun () -> StackExchange.allSites |> List.iter fetchNewQuestions)

let private twitter (conf: Configuration.Type) =
    async { Twitter.listenStream conf.Twitter Storage.save }

let private nuget (conf: Configuration.Type) =
    let fetchSince = NuGet.fetch conf.NuGet
    let fetchNewPackages () =
        let lastPackagePublishedDate = Storage.getTimeOfLastPackage()
        let pkgsWithRaw = fetchSince lastPackagePublishedDate
        do Storage.saveAll pkgsWithRaw
    repeatForever (TimeSpan.FromMinutes(5.)) fetchNewPackages

let private fssnip (conf: Configuration.Type) =
    let fetchSnippets () =
        let snippets = Fssnip.fetch conf.FsSnip
        do Storage.saveAll snippets
    repeatForever (TimeSpan.FromMinutes(5.)) fetchSnippets

let private fpish (conf: Configuration.Type) =
    let fetchQuestions () =
        let quests = FPish.fetch conf.FPish
        do Storage.saveAll quests
    repeatForever (TimeSpan.FromMinutes(5.)) fetchQuestions

let private gists (conf: Configuration.Type) =
    let fetch since =
        let gists, lastFetchedDate = GitHub.fetchGists conf.GitHub since
        do Storage.saveAll gists
        lastFetchedDate
    let lastSavedDate = Storage.getDateOfLastGist()
    repeatForeverArg (TimeSpan.FromMinutes 5.) fetch lastSavedDate

[<EntryPoint>]
let main argv =
    do log.Info "Program started"
    let seUrl = ref None
    let twiUrl = ref None
    let nuUrl = ref None
    let fssnipUrl = ref None
    let fpishUrl = ref None
    let githubUrl = ref None

    let startPullingData() =
        let conf = Configuration.build !seUrl !twiUrl !nuUrl !fssnipUrl !fpishUrl !githubUrl
        [stackExchange
         twitter
         nuget
         fssnip
         fpish
         gists]
        |> Seq.map (fun f -> f conf)
        |> Async.Parallel
        |> Async.Ignore
        |> Async.Start

    let start _ = startPullingData(); true
    let stop _ = true
    let runService () = serviceControl start stop

    let configureService conf =
        conf |> serviceName "FSharpNews.DataPull.Service"
        conf |> description "FSharp News data pull service"
        conf |> runAsNetworkService
        conf |> startAutomatically
        conf |> dependsOnMongoDB
        conf |> enableServiceRecovery <| restartService 1
        conf |> useNLog
        conf |> addCommandLineDefinition "stackExchangeUrl" (fun url -> seUrl := Some url)
        conf |> addCommandLineDefinition "twitterUrl" (fun url -> twiUrl := Some url)
        conf |> addCommandLineDefinition "nugetUrl" (fun url -> nuUrl := Some url)
        conf |> addCommandLineDefinition "fssnipUrl" (fun url -> fssnipUrl := Some url)
        conf |> addCommandLineDefinition "fpishUrl" (fun url -> fpishUrl := Some url)
        conf |> addCommandLineDefinition "githubUrl" (fun url -> githubUrl := Some url)

    let exitCode = configureTopShelf <| fun conf ->
        do configureService conf
        do runService |> service conf
    log.Info "Exiting, code=%A" exitCode
    int exitCode
