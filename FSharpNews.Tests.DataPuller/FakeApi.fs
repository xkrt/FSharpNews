module FSharpNews.Tests.DataPuller.FakeApi

open FSharpNews.Tests.Core

let serverPort = 4141

let sePath = "/StackExchange/2.1/questions"
let seUrl = sprintf "http://%s:%d/StackExchange" Environment.machine serverPort

let twitterPath = "/Twitter/1.1/statuses/filter.json"
let twitterUrl = sprintf "http://%s:%d/Twitter/1.1/" Environment.machine serverPort

