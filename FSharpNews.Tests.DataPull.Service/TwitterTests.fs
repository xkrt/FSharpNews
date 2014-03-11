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
    do ServiceApplication.startAndSleep Twitter

    Storage.getAllActivities()
    |> List.map fst
    |> Collection.assertEquiv [TestData.Twitter.activity]

[<Test>]
let ``Retweets are filtered``() =
    use tw = TwitterApi.runServer (POST >>= url TwitterApi.path >>== TwitterApi.handle (writeTweet TestData.Twitter.retweetJson))
    do ServiceApplication.startAndSleep Twitter

    Storage.getAllActivities().Length |> assertEqual 0

[<Test>]
let ``Replies are filtered``() =
    use tw = TwitterApi.runServer (POST >>= url TwitterApi.path >>== TwitterApi.handle (writeTweet TestData.Twitter.replyJson))
    do ServiceApplication.startAndSleep Twitter

    Storage.getAllActivities().Length |> assertEqual 0
