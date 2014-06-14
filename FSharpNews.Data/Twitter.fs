module FSharpNews.Data.Twitter

open System
open System.Configuration
open System.Globalization
open FSharp.Data
open FSharp.Data.Toolbox.Twitter
open FSharpNews.Data
open FSharpNews.Utils

[<Literal>]
let private targetHashtag = "#fsharp"

let private log = Logger.create "Twitter"

type Configuration = { ConsumerKey: string
                       ConsumerSecret: string
                       AccessToken: string
                       AccessTokenSecret: string
                       StreamApiUrl: string
                       SearchApiUrl: string }

let private parseDate str =
    let dt = DateTime.ParseExact(str, "ddd MMM dd HH:mm:ss %K yyyy", CultureInfo.InvariantCulture.DateTimeFormat)
    dt.ToUniversalTime()

let private processTweet save (tweet: TwitterTypes.Tweet.Root) =
    match tweet.RetweetedStatus, tweet.InReplyToStatusId, tweet.User with
    | Some _, _, _ -> do log.Info "Id=%A is retweet, skip" tweet.IdStr
    | _, Some _, _ -> do log.Info "Id=%A is reply, skip" tweet.Id
    | _, _, Some user when user.ScreenName = "fssnip" -> do log.Info "Id=%A tweet by @fssnip, skip" tweet.IdStr
    | None, None, _ ->
        do log.Info "User=%s; At %s; Text=%s" tweet.User.Value.ScreenName tweet.CreatedAt.Value tweet.Text.Value
        let activity = (Tweet { Id = tweet.Id.Value
                                CreationDate = parseDate tweet.CreatedAt.Value
                                Text = tweet.Text.Value
                                UserId = tweet.User.Value.Id
                                UserScreenName = tweet.User.Value.ScreenName
                                Urls = tweet.Entities.Value.Urls
                                        |> Array.map (fun u -> { Url = u.Url
                                                                 ExpandedUrl = u.ExpandedUrl
                                                                 DisplayUrl = u.DisplayUrl
                                                                 StartIndex = u.Indices.[0]
                                                                 EndIndex = u.Indices.[1] })
                                        |> Array.toList
                                Photo = tweet.Entities.Value.Media
                                        |> Seq.tryHead
                                        |> Option.map (fun m -> { Url = m.Url
                                                                  MediaUrl = m.MediaUrl
                                                                  DisplayUrl = m.DisplayUrl
                                                                  StartIndex = m.Indices.[0]
                                                                  EndIndex = m.Indices.[1] }) })
        do save(activity, tweet.JsonValue.AsString())

let private createTwitter config = 
    let context = UserContext { ConsumerKey = config.ConsumerKey
                                ConsumerSecret = config.ConsumerSecret
                                AccessToken = config.AccessToken
                                AccessSecret = config.AccessTokenSecret }
    Twitter(context)

let listenStream (config: Configuration)  save =
    let twitter = createTwitter config
    let stream = twitter.Streaming.FilterTweets [targetHashtag]
    stream.TweetReceived.Add (processTweet save)
    stream.Start()

let searchSince config (lastKnownId: int64) =
    let mapStatus (s: TwitterTypes.SearchTweets.Status) =
        let activity =
            Tweet { Id = s.Id
                    Text = s.Text
                    UserId = int64 s.User.Id
                    UserScreenName = s.User.ScreenName
                    CreationDate = parseDate s.CreatedAt
                    Urls = s.Entities.Urls
                           |> Array.map (fun u -> { Url = u.Url
                                                    ExpandedUrl = u.ExpandedUrl
                                                    DisplayUrl = u.DisplayUrl
                                                    StartIndex = u.Indices.[0]
                                                    EndIndex = u.Indices.[1] })
                           |> Array.toList
                    Photo = s.Entities.Media
                            |> Seq.tryHead
                            |> Option.map (fun m -> { Url = m.Url
                                                      MediaUrl = m.MediaUrl
                                                      DisplayUrl = m.DisplayUrl
                                                      StartIndex = m.Indices.[0]
                                                      EndIndex = m.Indices.[1] }) }
        let json = Serializer.toJson s
        activity, json

    let twitter = createTwitter config
    let requestAll () =
        let maxBatchSize = 100
        let rec requestNextBatches maxId =
            let results = twitter.Search.Tweets(query=targetHashtag,
                                                sinceId=lastKnownId,
                                                maxId=maxId,
                                                count=maxBatchSize,
                                                lang="en")
            checkAndRequestNext results
        and checkAndRequestNext (results: TwitterTypes.SearchTweets.Root) =
            let batch =
                results.Statuses
                |> Seq.filter (fun s -> s.RetweetedStatus.IsNone && s.InReplyToStatusId.IsNone)
                |> Seq.map mapStatus
                |> Seq.toList
            if results.Statuses.Length = maxBatchSize
            then let minId = results.Statuses |> Seq.map (fun s -> s.Id) |> Seq.min
                 batch @ (requestNextBatches (minId - 1L))
            else batch

        let firstResults = twitter.Search.Tweets(query=targetHashtag,
                                                 sinceId=lastKnownId,
                                                 count=maxBatchSize,
                                                 lang="en")
        checkAndRequestNext firstResults

    let results = requestAll()
    do log.Info "Search found: %d" results.Length
    results
