module FSharpNews.DataPuller.Configuration

open System.Configuration
open FSharpNews.Data.StackExchange
open FSharpNews.Data.Twitter
open FSharpNews.Data.NuGet

let get argv =
    let cfg = ConfigurationManager.AppSettings

    let seApiKey = cfg.["StackExchangeApiKey"]
    let twiConsumerKey = cfg.["TwitterConsumerKey"]
    let twiConsumerSecret = cfg.["TwitterConsumerSecret"]
    let twiAccessToken = cfg.["TwitterAccessToken"]
    let twiAccessTokenSecret = cfg.["TwitterAccessTokenSecret"]

    let seApiUrl, twiStreamApiUrl, nugetUrl =
        match Array.toList argv with
        | opt::stackExchangeUrl::twitterUrl::nugetUrl::[] when opt = "-test" -> stackExchangeUrl, twitterUrl, nugetUrl
        | [] -> "https://api.stackexchange.com",
                "https://stream.twitter.com/1.1/",
                "https://www.nuget.org/api/v2"
        | _ -> failwith "Wrong command line parameters"

    let seConfig = { ApiKey = seApiKey
                     ApiUrl = seApiUrl }
    let twiConfig = { ConsumerKey = twiConsumerKey
                      ConsumerSecret = twiConsumerSecret
                      AccessToken = twiAccessToken
                      AccessTokenSecret = twiAccessTokenSecret
                      StreamApiUrl = twiStreamApiUrl }
    let nuConfig = { Url = nugetUrl }
    seConfig, twiConfig, nuConfig
