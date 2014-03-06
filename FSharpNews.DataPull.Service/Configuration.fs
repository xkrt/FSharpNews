module FSharpNews.DataPull.Service.Configuration

open System.Configuration
open FSharpNews.Data
open FSharpNews.Data.StackExchange
open FSharpNews.Data.Twitter
open FSharpNews.Data.NuGet
open FSharpNews.Utils

type Type = { StackExchange: StackExchange.Configuration
              Twitter: Twitter.Configuration
              NuGet: NuGet.Configuration
              FsSnip: Fssnip.Configuration
              FPish: FPish.Configuration }

let build (seUrl: string option) (twiUrl: string option) (nuUrl: string option) (fssnipUrl: string option) (fpishUrl: string option) =
    let cfg = ConfigurationManager.AppSettings

    let seApiKey = cfg.["StackExchangeApiKey"]
    let twiConsumerKey = cfg.["TwitterConsumerKey"]
    let twiConsumerSecret = cfg.["TwitterConsumerSecret"]
    let twiAccessToken = cfg.["TwitterAccessToken"]
    let twiAccessTokenSecret = cfg.["TwitterAccessTokenSecret"]

    let seUrl = seUrl |> Option.fill "https://api.stackexchange.com"
    let twiUrl = twiUrl |> Option.fill "https://stream.twitter.com/1.1/"
    let nuUrl = nuUrl |> Option.fill "https://www.nuget.org/api/v2"
    let fssnipUrl = fssnipUrl |> Option.fill "http://api.fssnip.net"
    let fpishUrl = fpishUrl |> Option.fill "http://fpish.net/atom/topics/tag/1/f~23"

    { StackExchange = { ApiKey = seApiKey
                        ApiUrl = seUrl }
      Twitter = { ConsumerKey = twiConsumerKey
                  ConsumerSecret = twiConsumerSecret
                  AccessToken = twiAccessToken
                  AccessTokenSecret = twiAccessTokenSecret
                  StreamApiUrl = twiUrl }
      NuGet = { Url = nuUrl }
      FsSnip = { Url = fssnipUrl }
      FPish = { BaseUrl = fpishUrl } }
