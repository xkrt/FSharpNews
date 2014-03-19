module FSharpNews.Data.Twitter

open System
open System.Configuration
open System.Globalization
open FSharp.Data
open LinqToTwitter
open FSharpNews.Data
open FSharpNews.Utils

// todo use stall_warnings

[<Literal>]
let private targetHashtag = "#fsharp"

let private log = Logger.create "Twitter"

type Configuration = { ConsumerKey: string
                       ConsumerSecret: string
                       AccessToken: string
                       AccessTokenSecret: string
                       StreamApiUrl: string
                       SearchApiUrl: string }

type private LinqToTwitterLog() =
    inherit IO.TextWriter()
    let log = Logger.create "Linq2Twitter"
    override this.Encoding with get () = Text.UTF8Encoding() :> Text.Encoding
    override this.WriteLine(str: string) = log.Debug "%s" str

let private createContext config =
    let creds = SingleUserInMemoryCredentials()
    creds.ConsumerKey <- config.ConsumerKey
    creds.ConsumerSecret <- config.ConsumerSecret
    creds.TwitterAccessToken <- config.AccessToken
    creds.TwitterAccessTokenSecret <- config.AccessTokenSecret

    let authorizer = SingleUserAuthorizer()
    authorizer.Credentials <- creds
    authorizer.UseCompression <- true

    let context = new TwitterContext(authorizer)
    context.StreamingUrl <- config.StreamApiUrl.EnsureEndsWith("/")
    context.SearchUrl <- config.SearchApiUrl.EnsureEndsWith("/")
    context.Log <- new LinqToTwitterLog() :> IO.TextWriter
    context

type private CommonMessage = JsonProvider<"DataSamples/Twitter/stream-message.json", SampleList=true>
type private TweetMessage = JsonProvider<"DataSamples/Twitter/tweet.json", SampleList=true>

let private parseDate str =
    let dt = DateTime.ParseExact(str, "ddd MMM dd HH:mm:ss %K yyyy", CultureInfo.InvariantCulture.DateTimeFormat)
    dt.ToUniversalTime()

let rec private processStream config save (stream: StreamContent) =
    match stream.Status, stream.Content, stream.Error with
    | TwitterErrorStatus.Success, content, _ when content.IsNullOrWs() -> do log.Debug "Status=Success, blank message (keep-alive)"
    | TwitterErrorStatus.Success, content, _ ->
        let msg = CommonMessage.Parse content
        match msg.Id, msg.Disconnect, msg.Warning with
        | Some _, _, _ ->
            do log.Debug "Status=Success; Content=%s" content
            let tweet = TweetMessage.Parse content
            match tweet.RetweetedStatus, tweet.InReplyToStatusId.Number with
            | Some _, _ -> do log.Info "Id=%d is retweet, skip" tweet.Id
            | _, Some _ -> do log.Info "Id=%d is reply, skip" tweet.Id
            | None, None -> do log.Info "User=%s; At %s; Text=%s" tweet.User.ScreenName tweet.CreatedAt tweet.Text
                            let activity = (Tweet { Id = tweet.Id
                                                    CreationDate = parseDate tweet.CreatedAt
                                                    Text = tweet.Text
                                                    UserId = tweet.User.Id
                                                    UserScreenName = tweet.User.ScreenName })
                            do save(activity, content)
        | _, Some disconnect, _ -> do log.Debug "Status=Success, disconnect=%O. Reopening stream..." disconnect
                                   listenStream config save
        | _, _, Some warning -> do log.Warn "Twitter warning: %O" warning
        | _ -> do log.Warn "Status=Success; Unknown message: %s" content
    | status, content, error -> do log.Warn "Status=%A; Error=%O; Content=%s" status error (content.IfNullOrWs("<none>"))
                                listenStream config save

and listenStream config save =
    let context = createContext config
    let q = query { for s in context.Streaming do
                    where (s.Type = StreamingType.Filter && s.Track = targetHashtag && s.Language = "en")
                    select (s) }
    q.StreamingCallback(processStream config save) |> Seq.tryHead |> ignore

let searchSince config (lastKnownId: int64) =
    let context = createContext config
    let sinceId = uint64 lastKnownId
    let maxCount = 100

    let toTweet (status: Status) =
        let tweet = { Id = Int64.Parse status.StatusID
                      Text = status.Text
                      UserId = Int64.Parse status.User.Identifier.ID
                      UserScreenName = status.User.Identifier.ScreenName
                      CreationDate = status.CreatedAt }
        let json = Serializer.toJson status
        (tweet,json)
    let filterRepliesRetweets = Seq.filter (fun (s: Status) -> s.RetweetedStatus.StatusID = null && s.InReplyToStatusID = null)
    let toFilteredTweets = filterRepliesRetweets >> Seq.map toTweet >> Seq.toList
    let getMinId (statuses: Collections.Generic.List<Status>) =
        statuses
        |> Seq.map (fun s -> s.StatusID)
        |> Seq.map Int64.Parse
        |> Seq.min

    let rec loop maxIdExclusive =
        let maxId = uint64 (maxIdExclusive - 1L)
        do log.Debug "SinceId=%d, MaxId=%d" sinceId maxId
        let q = query { for s in context.Search do
                        where (s.SinceID = sinceId
                            && s.MaxID = maxId
                            && s.Query = targetHashtag
                            && s.Count = maxCount
                            && s.SearchLanguage = "en"
                            && s.Type = SearchType.Search
                            && s.ResultType = ResultType.Recent)
                        select (s) }
        match Seq.tryHead q with
        | Some result when result.Statuses.Count = maxCount ->
            let minId = getMinId result.Statuses
            let batch = toFilteredTweets result.Statuses
            batch @ (loop minId)
        | Some result when result.Statuses.Count < maxCount -> toFilteredTweets result.Statuses
        | _ -> []

    let initialQuery = query { for s in context.Search do
                               where (s.SinceID = sinceId
                                   && s.Query = targetHashtag
                                   && s.Count = maxCount
                                   && s.SearchLanguage = "en"
                                   && s.Type = SearchType.Search
                                   && s.ResultType = ResultType.Recent)
                               select (s) }
    let tweets =
        match Seq.tryHead initialQuery with
        | Some result when result.Statuses.Count = maxCount ->
            let minId = getMinId result.Statuses
            let initialTweets = toFilteredTweets result.Statuses
            initialTweets @ (loop minId)
        | Some result when result.Statuses.Count < maxCount -> toFilteredTweets result.Statuses
        | _ -> []
    do log.Info "Search found: %d" tweets.Length
    tweets |> List.map (fun (t,json) -> Tweet t, json)
