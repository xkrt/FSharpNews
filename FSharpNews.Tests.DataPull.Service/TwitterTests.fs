module FSharpNews.Tests.DataPull.Service.TwitterTests

open System
open NUnit.Framework
open Suave.Types
open Suave.Http
open Suave.Web
open FSharpNews.Data
open FSharpNews.Utils
open FSharpNews.Tests.Core

let writeTweet json (req: HttpRequest) =
    async { do! TwitterApi.writeHeaderBodyDelimeter req
            do! TwitterApi.writeMessage req json
            do! TwitterApi.writeEmptyInfinite req }

[<SetUp>]
let Setup() = do Storage.deleteAll()

[<Test>]
let ``One tweet in stream => one activity in storage``() =
    use tw = TwitterApi.runServer (POST >>= url TwitterApi.path >>== TwitterApi.handle (writeTweet TestData.Twitter.json))
    use se = StackExchangeApi.runEmpty()
    use nu = NuGetApi.runEmpty()

    use puller = ServiceApplication.start()
    sleep 10

    let activities = Storage.getAllActivities()
    activities
    |> List.map fst
    |> Collection.assertEquiv ([ TestData.Twitter.activity ])

    activities
    |> List.map snd
    |> List.iter (assertEqualDateWithin DateTime.UtcNow (TimeSpan.FromSeconds(15.)))

[<Test>]
let ``Retweets are filtered``() =
    use tw = TwitterApi.runServer (POST >>= url TwitterApi.path >>== TwitterApi.handle (writeTweet TestData.Twitter.retweetJson))
    use se = StackExchangeApi.runEmpty()
    use nu = NuGetApi.runEmpty()

    use puller = ServiceApplication.start()
    sleep 10

    Storage.getAllActivities().Length |> assertEqual 0

[<Test>]
let ``Replies are filtered``() =
    use tw = TwitterApi.runServer (POST >>= url TwitterApi.path >>== TwitterApi.handle (writeTweet TestData.Twitter.replyJson))
    use se = StackExchangeApi.runEmpty()
    use nu = NuGetApi.runEmpty()

    use puller = ServiceApplication.start()
    sleep 10

    Storage.getAllActivities().Length |> assertEqual 0
