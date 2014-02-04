#r """System.Configuration"""
#r """../packages/mongocsharpdriver.1.8.3/lib/net35/MongoDB.Bson.dll"""
#r """../packages/mongocsharpdriver.1.8.3/lib/net35/MongoDB.Driver.dll"""
#r """bin/Debug/FSharpNews.Data.dll"""
#r """bin/Debug/FSharpNews.Utils.dll"""

#load "Storage.fs"

open System
open FSharpNews.Data

let tweet = { Id = 1L
              Text = "Test tweet"
              UserId = 1L
              UserScreenName = "testuser"
              CreationDate = DateTime.UtcNow }

do Storage.deleteAll()

[1L..200L]
|> List.map (fun i -> { tweet with Id = i
                                   Text = sprintf "Test tweet #%d" i
                                   CreationDate = tweet.CreationDate.AddSeconds(float i) })
|> List.map Tweet
|> List.iter (fun t -> Storage.save(t, "")
                       Threading.Thread.Sleep(10))
