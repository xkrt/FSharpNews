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
                       StreamApiUrl: string }

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
    context.Log <- new LinqToTwitterLog() :> IO.TextWriter
    context

type private CommonMessage = JsonProvider<"DataSamples/Twitter/stream-message.json", SampleList=true>
type private TweetMessage = JsonProvider<"DataSamples/Twitter/tweet.json", SampleList=true>

let private parseDate str =
    let dt = DateTime.ParseExact(str, "ddd MMM dd HH:mm:ss %K yyyy", CultureInfo.InvariantCulture.DateTimeFormat)
    dt.ToUniversalTime()

let rec private processStream config save (stream: StreamContent) =
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
            listenStream config save
        | _ ->
            do log.Warn "Status=Success; Unknown message: %s" content
    | status, content, error ->
        do log.Warn "Status=%A; Error=%O; Content=%s" status error (content.IfNullOrWs("<none>"))

and listenStream config save =
    let context = createContext config
    let q = query { for s in context.Streaming do
                    where (s.Type = StreamingType.Filter && s.Track = targetHashtag && s.Language = "en")
                    select (s) }
    q.StreamingCallback(processStream config save) |> Seq.head |> ignore
