namespace FSharpNews.DataPull.Service

type Args = { mutable StackExchangeEnabled: bool
              mutable StackExchangeUrl: string option
              mutable TwitterEnabled: bool
              mutable TwitterUrl: string option
              mutable NuGetEnabled: bool
              mutable NuGetUrl: string option
              mutable FssnipEnabled: bool
              mutable FssnipUrl: string option
              mutable FpishEnabled: bool
              mutable FpishUrl: string option
              mutable GistsEnabled: bool
              mutable ReposEnabled: bool
              mutable GitHubUrl: string option }
                with static member Default = { StackExchangeEnabled = true
                                               StackExchangeUrl = None
                                               TwitterEnabled = true
                                               TwitterUrl = None
                                               NuGetEnabled = true
                                               NuGetUrl = None
                                               FssnipEnabled = true
                                               FssnipUrl = None
                                               FpishEnabled = true
                                               FpishUrl = None
                                               GistsEnabled = true
                                               ReposEnabled = true
                                               GitHubUrl = None }

module Configuration = 
    open System
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
                  FPish: FPish.Configuration
                  GitHub: GitHub.Configuration }

    let private uri url = Uri(url)

    let build (args: Args) =
        let seUrl = args.StackExchangeUrl |> Option.fill "https://api.stackexchange.com"
        let twiUrl = args.TwitterUrl |> Option.fill "https://stream.twitter.com/1.1/"
        let nuUrl = args.NuGetUrl |> Option.fill "https://www.nuget.org/api/v2"
        let fssnipUrl = args.FssnipUrl |> Option.fill "http://api.fssnip.net"
        let fpishUrl = args.FpishUrl |> Option.fill "http://fpish.net/atom/topics/tag/1/f~23"
        let githubUri = args.GitHubUrl |> Option.fill "https://api.github.com" |> uri

        let cfg = ConfigurationManager.AppSettings
        let seApiKey = cfg.["StackExchangeApiKey"]
        let twiConsumerKey = cfg.["TwitterConsumerKey"]
        let twiConsumerSecret = cfg.["TwitterConsumerSecret"]
        let twiAccessToken = cfg.["TwitterAccessToken"]
        let twiAccessTokenSecret = cfg.["TwitterAccessTokenSecret"]
        let githubLogin = cfg.["GithubLogin"]
        let githubPassword = cfg.["GithubPassword"]

        { StackExchange = { ApiKey = seApiKey
                            ApiUrl = seUrl }
          Twitter = { ConsumerKey = twiConsumerKey
                      ConsumerSecret = twiConsumerSecret
                      AccessToken = twiAccessToken
                      AccessTokenSecret = twiAccessTokenSecret
                      StreamApiUrl = twiUrl }
          NuGet = { Url = nuUrl }
          FsSnip = { Url = fssnipUrl }
          FPish = { BaseUrl = fpishUrl }
          GitHub = { BaseUri = githubUri
                     Login = githubLogin
                     Password = githubPassword } }
