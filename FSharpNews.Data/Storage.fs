module FSharpNews.Data.Storage

open System
open System.Configuration
open MongoDB.Bson
open MongoDB.Bson.Serialization.Attributes
open MongoDB.Driver
open MongoDB.Driver.Builders
open FSharpNews.Data
open FSharpNews.Utils

let private log = Logger.create "Storage"

type private ActivityType =
    | StackExchange = 0
    | Tweet = 1
    | NugetPackage = 2

[<Literal>]
let DuplicateKeyError = 11000

#if INTERACTIVE
let connectionString = "mongodb://localhost/fsharpnews"
#else
let connectionString = ConfigurationManager.ConnectionStrings.["MongoDB"].ConnectionString
#endif

// todo extract all copy-pasted strings

let private mongoUrl = MongoUrl.Create connectionString
let private client = new MongoClient(mongoUrl)
let private db = client.GetServer().GetDatabase(mongoUrl.DatabaseName)
let private activities = db.GetCollection("activities")
do activities.EnsureIndex(IndexKeys.Ascending("activity.site").Ascending("activity.questionId"), IndexOptions.SetUnique(true).SetSparse(true))
do activities.EnsureIndex(IndexKeys.Ascending("activity.tweetId"), IndexOptions.SetUnique(true).SetSparse(true))
do activities.EnsureIndex(IndexKeys.Ascending("activity.packageId").Ascending("activity.version"), IndexOptions.SetUnique(true).SetSparse(true))

let private i32 value = BsonInt32 value
let private i64 value = BsonInt64 value
let private str value = BsonString value
let private date (value: DateTime) = BsonDateTime value
let private el (name: string) (value: BsonValue) = BsonElement(name, value)

let private siteToBson = function Stackoverflow -> i32 0 | Programmers -> i32 1 | CodeReview -> i32 2 | CodeGolf -> i32 3
let private bsonToSite = function 0 -> Stackoverflow | 1 -> Programmers | 2 -> CodeReview | 3 -> CodeGolf | x -> failwithf "Unknown %d StackExchange site" x

let private mapToDocument (activity, raw) =
    let activityDoc, descriminator =
        match activity with
        | StackExchangeQuestion q -> BsonDocument [ el "questionId" (i32 q.Id)
                                                    el "site" (siteToBson q.Site)
                                                    el "title" (str q.Title)
                                                    el "userDisplayName" (str q.UserDisplayName)
                                                    el "url" (str q.Url)
                                                    el "date" (date q.CreationDate) ]
                                     , i32 (int ActivityType.StackExchange)
        | Tweet t -> BsonDocument [ el "tweetId" (i64 t.Id)
                                    el "text" (str t.Text)
                                    el "userId" (i64 t.UserId)
                                    el "userScreenName" (str t.UserScreenName)
                                    el "date" (date t.CreationDate) ]
                     , i32 (int ActivityType.Tweet)
        | NugetPackage p -> BsonDocument [ el "packageId" (str p.Id)
                                           el "version" (str p.Version)
                                           el "url" (str p.Url)
                                           el "date" (date p.PublishedDate) ]
                            , i32 (int ActivityType.NugetPackage)
    let rawDoc = BsonValue.Create(raw)
    BsonDocument [ el "descriminator" descriminator
                   el "activity" activityDoc
                   el "raw" rawDoc
                   el "addedDate" (date DateTime.UtcNow)]

let private mapFromDocument (document: BsonDocument) =
    let activityType = enum<ActivityType>(document.["descriminator"].AsInt32)
    let adoc = document.["activity"].AsBsonDocument
    let activity =
        match activityType with
        | ActivityType.StackExchange -> { Id = adoc.["questionId"].AsInt32
                                          Site = bsonToSite adoc.["site"].AsInt32
                                          Title = adoc.["title"].AsString
                                          UserDisplayName = adoc.["userDisplayName"].AsString
                                          Url = adoc.["url"].AsString
                                          CreationDate = adoc.["date"].ToUniversalTime() } |> StackExchangeQuestion
        | ActivityType.Tweet -> { Id = adoc.["tweetId"].AsInt64
                                  Text = adoc.["text"].AsString
                                  UserId = adoc.["userId"].AsInt64
                                  UserScreenName = adoc.["userScreenName"].AsString
                                  CreationDate = adoc.["date"].ToUniversalTime() } |> Tweet
        | ActivityType.NugetPackage -> { Id = adoc.["packageId"].AsString
                                         Version = adoc.["version"].AsString
                                         Url = adoc.["url"].AsString
                                         PublishedDate = adoc.["date"].ToUniversalTime() } |> NugetPackage
        | t -> failwithf "Mapping for %A is not implemented" t
    let added = document.["addedDate"].ToUniversalTime()
    activity, added

let private safeUniq fn description =
    try
        fn() |> ignore
    with
    | :? WriteConcernException as e ->
        match e.CommandResult.Code with
        | Value code when code = DuplicateKeyError -> log.Warn "Duplicate key error while %s, error: %s" (description()) (e.ToString())
        | _ -> reraise()

let private safeInsert doc =
    let fn = fun () -> activities.Insert doc
    let desc = fun () -> sprintf "inserting document %O" doc
    safeUniq fn desc

let private safeInsertBatch (docs: BsonDocument list) =
    let fn = fun () -> activities.InsertBatch docs
    let desc = fun () -> sprintf "inserting documents %A" (docs |> List.map (sprintf "%O"))
    safeUniq fn desc

let save (activity: Activity, raw: string) =
    (activity, raw)
    |> mapToDocument
    |> safeInsert

let saveAll (activitiesWithRaws: (Activity*string) list) =
    match activitiesWithRaws with
    | [] -> ()
    | activitiesToSave -> activitiesToSave
                          |> List.map mapToDocument
                          |> safeInsertBatch

let getTimeOfLastQuestion (site: StackExchangeSite) =
    let quest = activities.Find(Query.And([ Query.EQ("descriminator", i32 (int ActivityType.StackExchange))
                                            Query.EQ("activity.site", siteToBson site) ]))
                         .SetSortOrder(SortBy.Descending("activity.date"))
                         .SetLimit(1)
                |> Seq.map mapFromDocument
                |> Seq.tryHead
    match quest with
    | Some (StackExchangeQuestion quest, _) -> quest.CreationDate
    | _ -> DateTime(2014, 1, 1, 0, 0, 0, DateTimeKind.Utc)

let getTimeOfLastPackage () =
    let pkg = activities.Find(Query.EQ("descriminator", i32 (int ActivityType.NugetPackage)))
                        .SetSortOrder(SortBy.Descending("activity.date"))
                        .SetLimit(1)
              |> Seq.map mapFromDocument
              |> Seq.tryHead
    match pkg with
    | Some (NugetPackage pkg, _) -> pkg.PublishedDate
    | _ -> DateTime(2014, 1, 1, 0, 0, 0, DateTimeKind.Utc) // todo extract to var

let getTopActivitiesByCreation count =
    let cursor = activities
                    .FindAll()
                    .SetSortOrder(SortBy.Descending("activity.date"))
                    .SetLimit(count)
    cursor
    |> Seq.cast<BsonDocument>
    |> Seq.map mapFromDocument
    |> Seq.toList

let getAllActivities () =
    activities.FindAll()
    |> Seq.cast<BsonDocument>
    |> Seq.map mapFromDocument
    |> Seq.toList

let getActivitiesAddedSince (dtExclusive: DateTime) =
    let cursor =
        activities
            .Find(Query.GT("addedDate", BsonDateTime dtExclusive))
            .SetSortOrder(SortBy.Descending "activity.date")
    cursor
    |> Seq.cast<BsonDocument>
    |> Seq.map mapFromDocument
    |> Seq.toList

let getActivitiesAddedEarlier count (dtExclusive: DateTime) =
    let cursor = activities.Find(Query.LT("addedDate", BsonDateTime dtExclusive))
                           .SetSortOrder(SortBy.Descending "activity.date")
                           .SetLimit(count)
    cursor // todo: extract function
    |> Seq.cast<BsonDocument>
    |> Seq.map mapFromDocument
    |> Seq.toList

let deleteAll () =
    do activities.RemoveAll() |> ignore
