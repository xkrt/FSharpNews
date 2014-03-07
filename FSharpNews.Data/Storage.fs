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
let private minDataDate = DateTime(2014, 3, 1, 0, 0, 0, DateTimeKind.Utc)

type private ActivityType =
    | StackExchange = 0
    | Tweet = 1
    | NugetPackage = 2
    | FsSnippet = 3
    | FPish = 4
    | Gist = 5

[<Literal>]
let private DuplicateKeyError = 11000

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
do activities.EnsureIndex(IndexKeys.Ascending("activity.snippetId"), IndexOptions.SetUnique(true).SetSparse(true))
do activities.EnsureIndex(IndexKeys.Ascending("activity.fpishId"), IndexOptions.SetUnique(true).SetSparse(true))
do activities.EnsureIndex(IndexKeys.Ascending("activity.gistId"), IndexOptions.SetUnique(true).SetSparse(true))

let private doc (elems: BsonElement list) = BsonDocument(elems)
let private el (name: string) (value: BsonValue) = BsonElement(name, value)
let private i32 value = BsonInt32 value
let private i64 value = BsonInt64 value
let private str value = BsonString value
let private optstr opt =
    match opt with
    | Some s -> (str s) :> BsonValue
    | None -> BsonNull.Value :> BsonValue
let private date (value: DateTime) = BsonDateTime value

let (|BNull|_|) (v: BsonValue) =
   if v.IsBsonNull
   then Some ()
   else None

let private siteToBson = function Stackoverflow -> i32 0 | Programmers -> i32 1 | CodeReview -> i32 2 | CodeGolf -> i32 3
let private bsonToSite = function 0 -> Stackoverflow | 1 -> Programmers | 2 -> CodeReview | 3 -> CodeGolf | x -> failwithf "Unknown %d StackExchange site" x

let private mapToDocument (activity, raw) =
    let activityDoc, descriminator =
        match activity with
        | StackExchangeQuestion q -> doc [ el "questionId" (i32 q.Id)
                                           el "site" (siteToBson q.Site)
                                           el "title" (str q.Title)
                                           el "userDisplayName" (str q.UserDisplayName)
                                           el "url" (str q.Url)
                                           el "date" (date q.CreationDate) ]
                                     , i32 (int ActivityType.StackExchange)
        | Tweet t -> doc [ el "tweetId" (i64 t.Id)
                           el "text" (str t.Text)
                           el "userId" (i64 t.UserId)
                           el "userScreenName" (str t.UserScreenName)
                           el "date" (date t.CreationDate) ]
                     , i32 (int ActivityType.Tweet)
        | NugetPackage p -> doc [ el "packageId" (str p.Id)
                                  el "version" (str p.Version)
                                  el "url" (str p.Url)
                                  el "date" (date p.PublishedDate) ]
                            , i32 (int ActivityType.NugetPackage)
        | FsSnippet s -> doc [ el "snippetId" (str s.Id)
                               el "title" (str s.Title)
                               el "author" (str s.Author)
                               el "url" (str s.Url)
                               el "date" (date s.PublishedDate) ]
                         , i32 (int ActivityType.FsSnippet)
        | FPishQuestion q -> doc [ el "fpishId" (i32 q.Id)
                                   el "title" (str q.Title)
                                   el "author" (str q.Author)
                                   el "url" (str q.Url)
                                   el "date" (date q.PublishedDate) ]
                             , i32 (int ActivityType.FPish)
        | Gist g -> doc [ el "gistId" (str g.Id)
                          el "description" (optstr g.Description)
                          el "owner" (str g.Owner)
                          el "url" (str g.Url)
                          el "date" (date g.CreationDate) ]
                    , i32 (int ActivityType.Gist)
    doc [ el "descriminator" descriminator
          el "activity" activityDoc
          el "raw" (str raw)
          el "addedDate" (date DateTime.UtcNow) ]

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
        | ActivityType.FsSnippet -> { FsSnippet.Id = adoc.["snippetId"].AsString
                                      Title = adoc.["title"].AsString
                                      Author = adoc.["author"].AsString
                                      Url = adoc.["url"].AsString
                                      PublishedDate = adoc.["date"].ToUniversalTime() } |> FsSnippet
        | ActivityType.FPish -> { FPishQuestion.Id = adoc.["fpishId"].AsInt32
                                  Title = adoc.["title"].AsString
                                  Author = adoc.["author"].AsString
                                  Url = adoc.["url"].AsString
                                  PublishedDate = adoc.["date"].ToUniversalTime() } |> FPishQuestion
        | ActivityType.Gist -> { Id = adoc.["gistId"].AsString
                                 Description = match adoc.["description"] with
                                               | BNull -> None
                                               | v -> Some v.AsString
                                 Owner = adoc.["owner"].AsString
                                 Url = adoc.["url"].AsString
                                 CreationDate = adoc.["date"].ToUniversalTime() } |> Gist
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
    activities.Find(Query.And([ Query.EQ("descriminator", i32 (int ActivityType.StackExchange))
                                Query.EQ("activity.site", siteToBson site) ]))
             .SetSortOrder(SortBy.Descending("activity.date"))
             .SetLimit(1)
    |> Seq.map mapFromDocument
    |> Seq.tryHead
    |> function | Some (StackExchangeQuestion quest, _) -> quest.CreationDate
                | _ -> minDataDate

let getTimeOfLastPackage () =
    activities.Find(Query.EQ("descriminator", i32 (int ActivityType.NugetPackage)))
                         .SetSortOrder(SortBy.Descending("activity.date"))
                         .SetLimit(1)
    |> Seq.map mapFromDocument
    |> Seq.tryHead
    |> function | Some (NugetPackage pkg, _) -> pkg.PublishedDate
                | _ -> minDataDate

let getDateOfLastGist () =
    activities.Find(Query.EQ("descriminator", i32 (int ActivityType.Gist)))
              .SetSortOrder(SortBy.Descending("activity.date"))
              .SetLimit(1)
    |> Seq.map mapFromDocument
    |> Seq.tryHead
    |> function | Some (Gist gist, _) -> gist.CreationDate
                | _ -> minDataDate

let mapToActivities cursor =
    cursor
    |> Seq.cast<BsonDocument>
    |> Seq.map mapFromDocument
    |> Seq.toList

let getTopActivitiesByCreation count =
    activities
        .FindAll()
        .SetSortOrder(SortBy.Descending("activity.date"))
        .SetLimit(count)
    |> mapToActivities

let getAllActivities () = activities.FindAll() |> mapToActivities

let getActivitiesAddedSince (dtExclusive: DateTime) =
    activities
        .Find(Query.GT("addedDate", BsonDateTime dtExclusive))
        .SetSortOrder(SortBy.Descending "activity.date")
    |> mapToActivities

let getActivitiesAddedEarlier count (dtExclusive: DateTime) =
    activities.Find(Query.LT("addedDate", BsonDateTime dtExclusive))
                        .SetSortOrder(SortBy.Descending "activity.date")
                        .SetLimit(count)
    |> mapToActivities

let deleteAll () =
    do activities.RemoveAll() |> ignore
