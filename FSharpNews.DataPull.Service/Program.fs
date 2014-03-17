module FSharpNews.DataPull.Service.Program

open System
open System.Threading
open FSharpNews.Data
open FSharpNews.Utils
open FSharpNews.DataPull.Service.TopShelf

let private log = Logger.create "Program"

do AppDomain.CurrentDomain.UnhandledException.Add UnhandledExceptionLogger.handle

let private repeatForeverArg (interval: TimeSpan) startArg fn =
    async { let rec loop arg =
                let newArg = ref arg
                try newArg := fn arg
                with
                | e -> do log.Error "%O" e
                do Thread.Sleep interval
                loop !newArg
            loop startArg }

let private repeatForever interval fn = repeatForeverArg interval () fn
let private repeatEvery5min = repeatForever (TimeSpan.FromMinutes 5.)

let private stackExchange (conf: Configuration.Type) =
    let fetchNewQuestions site =
        let lastQuestionTime = Storage.getTimeOfLastQuestion site
        let startDate = lastQuestionTime.AddSeconds 1.
        let activitiesWithRaws = StackExchange.fetch conf.StackExchange site startDate
        do Storage.saveAll activitiesWithRaws
    repeatEvery5min (fun () -> StackExchange.allSites |> List.iter fetchNewQuestions)

let private twitter (conf: Configuration.Type) =
    let listen = async { Twitter.listenStream conf.Twitter Storage.save }
    let search = async { let lastId = Storage.getIdOfLastTweet()
                         let tweets = Twitter.searchSince conf.Twitter lastId
                         Storage.saveAll tweets }
    [listen; search] |> Async.Parallel |> Async.Ignore

let private nuget (conf: Configuration.Type) =
    let fetchSince = NuGet.fetch conf.NuGet
    repeatEvery5min (Storage.getTimeOfLastPackage >> fetchSince >> Storage.saveAll)

let private fssnip (conf: Configuration.Type) =
    let fetchSnippets () = Fssnip.fetch conf.FsSnip |> Storage.saveAll
    repeatEvery5min fetchSnippets

let private fpish (conf: Configuration.Type) =
    let fetchQuestions () = FPish.fetch conf.FPish |> Storage.saveAll
    repeatEvery5min fetchQuestions

let private gists (conf: Configuration.Type) =
    let fetch since =
        let gists, lastFetchedDate = GitHub.fetchGists conf.GitHub since
        do Storage.saveAll gists
        lastFetchedDate
    let lastSavedDate = Storage.getDateOfLastGist()
    repeatForeverArg (TimeSpan.FromMinutes 5.) lastSavedDate fetch

let private repos (conf: Configuration.Type) =
    let fetch = Storage.getDateOfLastRepo >> GitHub.fetchNewRepos conf.GitHub >> Storage.saveAll
    repeatEvery5min fetch

[<EntryPoint>]
let main argv =
    do log.Info "Program started"
    let args = Args.Default

    let startPullingData() =
        log.Info "Started with args: %A" args
        let conf = Configuration.build args
        [stackExchange, args.StackExchangeEnabled
         twitter,       args.TwitterEnabled
         nuget,         args.NuGetEnabled
         fssnip,        args.FssnipEnabled
         fpish,         args.FpishEnabled
         gists,         args.GistsEnabled
         repos,         args.ReposEnabled]
        |> Seq.filter snd
        |> Seq.map fst
        |> Seq.map (fun fn -> fn conf)
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
        conf |> addCommandLineDefinition "disableStackExchange" (fun url -> args.StackExchangeEnabled <- false)
        conf |> addCommandLineDefinition "stackExchangeUrl" (fun url -> args.StackExchangeUrl <- Some url)
        conf |> addCommandLineDefinition "disableTwitter" (fun url -> args.TwitterEnabled <- false)
        conf |> addCommandLineDefinition "twitterUrl" (fun url -> args.TwitterUrl <- Some url)
        conf |> addCommandLineDefinition "disableNuget" (fun url -> args.NuGetEnabled <- false)
        conf |> addCommandLineDefinition "nugetUrl" (fun url -> args.NuGetUrl <- Some url)
        conf |> addCommandLineDefinition "disableFssnip" (fun url -> args.FssnipEnabled <- false)
        conf |> addCommandLineDefinition "fssnipUrl" (fun url -> args.FssnipUrl <- Some url)
        conf |> addCommandLineDefinition "disableFpish" (fun url -> args.FpishEnabled <- false)
        conf |> addCommandLineDefinition "fpishUrl" (fun url -> args.FpishUrl <- Some url)
        conf |> addCommandLineDefinition "disableGists" (fun url -> args.GistsEnabled <- false)
        conf |> addCommandLineDefinition "disableRepos" (fun url -> args.ReposEnabled <- false)
        conf |> addCommandLineDefinition "githubUrl" (fun url -> args.GitHubUrl <- Some url)

    let exitCode = configureTopShelf <| fun hostConf ->
        do configureService hostConf
        do runService |> service hostConf
    log.Info "Exiting, code=%A" exitCode
    int exitCode
