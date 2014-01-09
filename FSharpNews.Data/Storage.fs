module FSharpNews.Data.Storage

open System
open MongoDB.Bson
open MongoDB.Driver
open MongoDB.Driver.Builders
open FSharpNews.Data
open FSharpNews.Utils

let private log = Logger.create "Storage"

let private client = new MongoClient("mongodb://localhost")
let private db = client.GetServer().GetDatabase("fsharpnews")
let private stackExchange = db.GetCollection("StackExchange")

let getLastQuestionTime (site: StackExchangeSite) =
    let q =
        stackExchange.FindAs<StackExchangeQuestion>(Query.EQ("Site", BsonInt32(int site)))
                     .SetSortOrder(SortBy.Descending("CreationDate"))
                     .SetLimit(1)
        |> Seq.head
    q.CreationDate

let saveAll (questions: StackExchangeQuestion list) =
    match questions.Length with
    | 0 -> ()
    | _ -> stackExchange.InsertBatch(questions) |> ignore
