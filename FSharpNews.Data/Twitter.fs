module FSharpNews.Data.Twitter

open System
open System.Configuration
open System.Globalization
open FSharp.Data
open LinqToTwitter
open FSharpNews.Data
open FSharpNews.Utils

[<Literal>]
let private targetHashtag = "#fsharp"

let private log = Logger.create "Twitter"

let private creds = SingleUserInMemoryCredentials()
creds.ConsumerKey <- ConfigurationManager.AppSettings.["TwitterConsumerKey"]
creds.ConsumerSecret <- ConfigurationManager.AppSettings.["TwitterConsumerSecret"]
creds.TwitterAccessToken <- ConfigurationManager.AppSettings.["TwitterAccessToken"]
creds.TwitterAccessTokenSecret <- ConfigurationManager.AppSettings.["TwitterAccessTokenSecret"]

let private authorizer = SingleUserAuthorizer()
authorizer.Credentials <- creds

let private context = new TwitterContext(authorizer)

type private CommonMessage = JsonProvider<"DataSamples/Twitter/stream-message.json", SampleList=true>
type private TweetMessage = JsonProvider<"DataSamples/Twitter/tweet.json", SampleList=true>

let private parseDate str =
    let dt = DateTime.ParseExact(str, "ddd MMM dd HH:mm:ss %K yyyy", CultureInfo.InvariantCulture.DateTimeFormat)
    dt.ToUniversalTime()

let rec private processStream save (stream: StreamContent) =
    match stream.Status, stream.Content, stream.Error with
    | TwitterErrorStatus.Success, content, _ when content.IsNullOrWs() ->
        do log.Debug "Status=Success, blank message (keep-alive)"
    | TwitterErrorStatus.Success, content, _ ->
        let msg = CommonMessage.Parse content
        match msg.Id, msg.Disconnect with
        | Some _, _ ->
            let tweet = TweetMessage.Parse content
            do log.Info "Status=Success; User=%s; At %s; Text=%s" tweet.User.ScreenName tweet.CreatedAt tweet.Text
            let activity = (Tweet { Id = tweet.Id
                                    CreationDate = parseDate tweet.CreatedAt
                                    Text = tweet.Text
                                    UserId = tweet.User.Id
                                    UserScreenName = tweet.User.ScreenName })
            do save(activity, content)
        | _, Some disconnect ->
            do log.Debug "Status=Success, disconnect=%O. Reopening stream..." disconnect
            listenStream save
        | _ ->
            do log.Warn "Status=Success; Unknown message: %s" content
    | status, content, error ->
        do log.Debug "Status=%A; Error=%O; Content=%s" status error (content.IfNullOrWs("<none>"))

and listenStream save =
    let q = query { for s in context.Streaming do
                    where (s.Type = StreamingType.Filter && s.Track = targetHashtag)
                    select (s) }
    q.StreamingCallback(processStream save) |> Seq.head |> ignore
