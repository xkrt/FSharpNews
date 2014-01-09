module FSharpNews.Data.Storage

open MongoDB.Driver
open FSharpNews.Data
open FSharpNews.Utils

let private log = Logger.create "Storage"

let private client = new MongoClient("mongodb://localhost")
let private db = client.GetServer().GetDatabase("fsharpnews")
let private stackExchange = db.GetCollection("StackExchange")

let saveAll (questions: StackExchangeQuestion list) =
    stackExchange.InsertBatch(questions) |> ignore
