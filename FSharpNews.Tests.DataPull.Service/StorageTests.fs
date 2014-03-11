module FSharpNews.Tests.DataPull.Service.StorageTests

open System
open System.Configuration
open NUnit.Framework
open MongoDB.Bson
open MongoDB.Driver
open MongoDB.Driver.Builders
open FSharpNews.Data
open FSharpNews.Utils
open FSharpNews.Tests.Core

let utcnow () = DateTime.UtcNow.Truncate(TimeSpan.FromMilliseconds 1.)
let oneSec = TimeSpan.FromSeconds 1.

let withEmptyRaw a = a, ""

let mongoUrl = ConfigurationManager.ConnectionStrings.["MongoDB"].ConnectionString |> MongoUrl.Create
let client = new MongoClient(mongoUrl)
let collection = client.GetServer().GetDatabase(mongoUrl.DatabaseName).GetCollection("activities")

let tweet = { Id = 424661043634507776L
              Text = "Suave: non-blocking hipster web programming framework for F# - http://t.co/9EFDzlo8zt #fsharp"
              UserId = 880772426L
              UserScreenName = "fsharporg"
              CreationDate = DateTime(2014, 1, 18, 21, 54, 17, DateTimeKind.Utc) }
let tweetA = Tweet tweet

let soquest = { Id = 11227809
                Site = Stackoverflow
                Title = "Why is processing a sorted array faster than an unsorted array?"
                UserDisplayName = "GManNickG"
                Url = "http://stackoverflow.com/questions/11227809/why-is-processing-a-sorted-array-faster-than-an-unsorted-array"
                CreationDate = DateTime(2012, 6, 27, 13, 51, 36, DateTimeKind.Utc) }
let soquestA = StackExchangeQuestion soquest

let progquest = { Id = 46716
                  Site = Programmers
                  Title = "What technical details should a programmer of a web application consider before making the site public?"
                  UserDisplayName = "Joel Coehoorn"
                  Url = "http://programmers.stackexchange.com/users/8057/joel-coehoorn"
                  CreationDate = DateTime(2008, 9, 16, 13, 47, 53, DateTimeKind.Utc) }
let progquestA = StackExchangeQuestion progquest

let package = { Id = "FSharp.Formatting"
                Version = "2.3.2-beta"
                Url = "https://www.nuget.org/packages/FSharp.Formatting/2.3.2-beta"
                PublishedDate = DateTime(2014, 1, 17, 1, 46, 22, DateTimeKind.Utc) }
let packageA = NugetPackage package

let snippet =  { FsSnippet.Id = "lQ"
                 Title = "Rabbits and Recurrence Relations"
                 Author = "Michel Caradec"
                 Url = "http://fssnip.net/lQ"
                 PublishedDate = DateTime(2014, 3, 5, 5, 10, 11, DateTimeKind.Utc) }
let snippetA = FsSnippet snippet

[<SetUp>]
let Setup() = collection.RemoveAll() |> ignore

[<Test>]
let ``save saves activity with raw data and time added``() =
    let tweetJson = """{"key": "value"}"""
    do Storage.save(Tweet tweet, tweetJson)

    let doc = collection.FindAll() |> Seq.toList |> List.exactlyOne
    doc.["raw"].AsString |> assertEqual tweetJson
    doc.["addedDate"].ToUniversalTime() |> assertEqualDateWithin DateTime.UtcNow oneSec

    let act = doc.["activity"].AsBsonDocument
    act.["tweetId"].AsInt64 |> assertEqual tweet.Id
    act.["text"].AsString |> assertEqual tweet.Text
    act.["userId"].AsInt64 |> assertEqual tweet.UserId
    act.["userScreenName"].AsString |> assertEqual tweet.UserScreenName
    act.["date"].ToUniversalTime() |> assertEqual tweet.CreationDate

[<Test>]
let ``saveAll activities => getAllActivities returns same activities``() =
    let toSave = [tweetA; soquestA; progquestA]
    toSave
    |> List.map withEmptyRaw
    |> Storage.saveAll

    let savedActivities = Storage.getAllActivities()
    savedActivities |> List.iter (fun (_,added) -> added |> assertEqualDateWithin (utcnow()) oneSec)
    savedActivities |> List.map fst |> Collection.assertEquiv toSave

[<Test>]
let ``saveAll with empty list => nothing saved``() =
    do Storage.saveAll []
    Storage.getAllActivities() |> assertEqual []

[<Test>]
let ``saveAll partially duplicates => non-duplicates successfully saved``() =
    let alreadySavedTweet = Tweet { tweet with Id = 1L }
    do Storage.saveAll [alreadySavedTweet |> withEmptyRaw]

    let duplicateTweet = alreadySavedTweet
    let newTweet = Tweet { tweet with Id = 2L }
    do Storage.saveAll [ duplicateTweet |> withEmptyRaw
                         newTweet |> withEmptyRaw ]

    Storage.getAllActivities()
    |> List.map fst
    |> Collection.assertEquiv [ alreadySavedTweet; newTweet ]

[<Test>]
let ``getTopActivitiesByCreation``() =
    let middle = Tweet { tweet with CreationDate = utcnow().AddDays(-2.) }
    let newer = StackExchangeQuestion { soquest with CreationDate = utcnow().AddDays(-1.) }
    let older = StackExchangeQuestion { progquest with CreationDate = utcnow().AddDays(-3.) }
    [middle; newer; older] |> List.map withEmptyRaw |> Storage.saveAll

    Storage.getTopActivitiesByCreation 3
    |> List.map fst
    |> Collection.assertEqual [newer; middle; older]

[<Test>]
let ``getActivitiesAddedSince returns activities since exclusive date ordered by creation date``() =
    let saveWithAdded (addedDate: DateTime) (tweet: Tweet) =
        do Storage.save (Tweet tweet, "")
        let doc = collection.FindOne(Query.EQ("activity.tweetId", BsonInt64 tweet.Id ))
        doc.["addedDate"] <- BsonDateTime addedDate
        do collection.Save(doc) |> ignore
        tweet

    let since = utcnow().AddDays(-1.)

    let oldT = { tweet with Text = "Old"; Id = 1L } |> saveWithAdded (since.AddDays(-2.))
    let sinceT = { tweet with Text = "Since"; Id = 2L } |> saveWithAdded since
    let newerT = { tweet with Text = "Little bit newer"; CreationDate = utcnow(); Id = 3L } |> saveWithAdded (since.AddMilliseconds(1.))
    let newT = { tweet with Text = "New"; CreationDate = utcnow().AddDays(-1.); Id = 4L } |> saveWithAdded (since.AddDays(1.))

    Storage.getActivitiesAddedSince since
    |> List.map fst
    |> Collection.assertEqual [Tweet newerT; Tweet newT]

[<Test>]
let ``getTimeOfLastQuestion returns creation time of last question on specific site``() =
    let oldSo = { soquest with Id=1; CreationDate = utcnow().AddDays(-1.) }
    let newSo = { soquest with Id=2; CreationDate = utcnow() }
    let oldProg = { progquest with Id=1; CreationDate = utcnow().AddDays(-1.) }
    let newProg = { progquest with Id=2; CreationDate = utcnow() }

    [oldSo; newSo; oldProg; newProg]
    |> List.map StackExchangeQuestion
    |> List.map withEmptyRaw
    |> Storage.saveAll

    Storage.getTimeOfLastQuestion Stackoverflow |> assertEqual newSo.CreationDate
    Storage.getTimeOfLastQuestion Programmers |> assertEqual newProg.CreationDate

[<Test>]
let ``saveAll do not raise duplicate key exception``() =
    [tweetA; tweetA]
    |> List.map withEmptyRaw
    |> Storage.saveAll

    Storage.getAllActivities()
    |> List.map fst
    |> List.exactlyOne
    |> assertEqual tweetA

[<Test>]
let ``stackexchange questions should be unique``() =
    let id = 42
    let so = { soquest with Id=id }
    let prog = { progquest with Id=id }

    [so; prog; so; prog]
    |> List.map StackExchangeQuestion
    |> List.map withEmptyRaw
    |> List.iter Storage.save

    match Storage.getAllActivities() with
    | (StackExchangeQuestion q1,_)::(StackExchangeQuestion q2,_)::[] -> [q1; q2] |> Collection.assertEquiv [so; prog]
    | x -> failwithf "Expected: %A\r\nBut was: %A" [so; prog] x

let testUniquness activity =
    activity |> withEmptyRaw |> Storage.save
    activity |> withEmptyRaw |> Storage.save

    match Storage.getAllActivities() with
    | (savedActivity,_)::[] -> savedActivity |> assertEqual activity
    | x -> failwithf "Expected: %A\r\nBut was: %A" activity x

[<Test>]
let ``tweets should be unique``() = testUniquness tweetA

[<Test>]
let ``nuget packages should be unique``() = testUniquness packageA

[<Test>]
let ``snippets should be unique``() = testUniquness snippetA

[<Test>]
let ``fpish questions should be unique``() = testUniquness (FPishQuestion TestData.FPish.question)

[<Test>]
let ``gists should be unique``() = testUniquness (Gist TestData.Gist.gist)

[<Test>]
let ``github repositories should be unique``() = testUniquness (Repository TestData.Repositories.repo)
