module FSharpNews.Tests.Frontend.WebTests

open System
open NUnit.Framework
open OpenQA.Selenium
open FSharpNews.Data
open FSharpNews.Utils
open FSharpNews.Tests.Core

let killChromeDriver() =
    Diagnostics.Process.GetProcessesByName("chromedriver")
    |> Array.iter (fun p -> p.Kill())

[<TestFixtureSetUp>]
let SetupFixture() =
    do killChromeDriver()
    do Browser.start()

[<TestFixtureTearDown>]
let TeardownFixture() =
    do Browser.close()
    do killChromeDriver()

[<SetUp>]
let Setup() = do Storage.deleteAll()

let saveQuest q = Storage.save(StackExchangeQuestion q, "")
let saveTweet t = Storage.save(Tweet t, "")

module Page =
    let indexUrl = sprintf "http://%s:4040" Environment.machine
    let perPageCount = 100

    let go() = go indexUrl

    let hiddenNews = element ".hidden-news-bar"
    let table = element "#news"
    let rows = table |> elementsWithin "tr"
    let loader = element ".loader"
    let noMoreNews = element "#noMoreNews"
    let noNewsAtAll = element "#noNews"

let soIcoUrl = sprintf "%s/Content/Images/so16x16.png" Page.indexUrl
let pIcoUrl = sprintf "%s/Content/Images/programmers16x16.png" Page.indexUrl

let soQuest = { Id = 1
                Site = Stackoverflow
                Title = "Test Stackoverflow question"
                UserDisplayName = "User1"
                Url = "http://stackoverflow.com/questions/1/test-stackoverflow-question"
                CreationDate = DateTime.UtcNow }
let pQuest = { Id = 2
               Site = Programmers
               Title = "Test Programmers question"
               UserDisplayName = "User2"
               Url = "http://programmers.stackexchange.com/questions/2/test-programmers-question"
               CreationDate = DateTime.UtcNow }

let tweet = { Id = 3L
              Text = "Test tweet"
              UserId = 42L
              UserScreenName = "TestUser"
              CreationDate = DateTime.UtcNow
              Urls = []
              Photo = None }

let getRowCells (row: IWebElement) =
    match row |> findElements "td" with
    | iconTd::linkTd::dateTd::[] -> iconTd, linkTd, dateTd
    | _ -> failwithf "Three cells in row expected"

let F x = fun () -> x

let checkMatchSingleLinkRow ((iconSrc,linkText,linkHref,ago), (row: IWebElement)) =
    let icoTd, linksTd, date = getRowCells row
    (icoTd |> findElement "img") ? src |> assertEqual iconSrc
    F linksTd |> checkTextIs linkText
    linksTd |> findElements "a" |> Seq.head |> getAttr "href" |> assertEqual linkHref
    F date |> checkTextIs ago

let checkMatchText (expectedLinkText, row: IWebElement) =
    let _, linksTd, _ = getRowCells row
    F linksTd |> checkTextIs expectedLinkText

let linkTitle q = sprintf "%s: %s" q.UserDisplayName q.Title

let sleepMs (ms: int) = Threading.Thread.Sleep(ms)
let waitAjax() = sleepMs 5500


[<Test>]
let ``Show special message if no news at all``() =
    do Page.go()
    checkNotDisplayed Page.table
    checkDisplayed Page.noNewsAtAll

[<Test>]
let ``Special message hidden if has news``() =
    do saveQuest soQuest
    do Page.go()
    checkDisplayed Page.table
    checkNotDisplayed Page.noNewsAtAll

[<Test>]
let ``Order by creation date descending``() =
    let questOld = { soQuest with Title = "Old"; CreationDate = DateTime.UtcNow.AddHours(-2.); Id = 1 }
    let questMiddle = { soQuest with Title = "Middle"; CreationDate = DateTime.UtcNow.AddHours(-1.); Id = 2 }
    let questNew = { soQuest with Title = "New"; CreationDate = DateTime.UtcNow; Id = 3 }
    [questMiddle; questNew; questOld] |> List.iter saveQuest

    let expected = [soIcoUrl, "User1: New", soQuest.Url, "a few seconds ago"
                    soIcoUrl, "User1: Middle", soQuest.Url, "an hour ago"
                    soIcoUrl, "User1: Old", soQuest.Url, "2 hours ago"]

    do Page.go()
    Page.rows()
    |> List.zip expected
    |> List.iter checkMatchSingleLinkRow

[<Test>]
let ``Preserve order by creation date for newest ajax loaded news``() =
    let questOldest = { soQuest with Title = "Oldest"; CreationDate = DateTime.UtcNow.AddHours(-3.); Id = 1 }
    do saveQuest questOldest
    do Page.go()

    let questOlder = { soQuest with Title = "Older"; CreationDate = DateTime.UtcNow.AddHours(-2.); Id = 2 }
    let questMiddle = { soQuest with Title = "Middle"; CreationDate = DateTime.UtcNow.AddHours(-1.); Id = 3 }
    [questOlder; questMiddle] |> List.iter saveQuest
    waitAjax()

    let questNew = { soQuest with Title = "New"; CreationDate = DateTime.UtcNow; Id = 4 }
    do saveQuest questNew
    waitAjax()

    do click Page.hiddenNews
    let expected = [soIcoUrl, "User1: New", soQuest.Url, "a few seconds ago"
                    soIcoUrl, "User1: Middle", soQuest.Url, "an hour ago"
                    soIcoUrl, "User1: Older", soQuest.Url, "2 hours ago"
                    soIcoUrl, "User1: Oldest", soQuest.Url, "3 hours ago"]

    Page.rows()
    |> List.zip expected
    |> List.iter checkMatchSingleLinkRow

[<Test>]
let ``Newest ajax news hidden with bar``() =
    do saveQuest soQuest
    do Page.go()

    checkNotDisplayed Page.hiddenNews
    Page.rows().Length |> assertEqual 1

    do saveQuest pQuest
    waitAjax()

    checkDisplayed Page.hiddenNews
    Page.hiddenNews |> checkTextIs "1 news"
    do click Page.hiddenNews

    Page.rows().Length |> assertEqual 2

    let expected = [pIcoUrl, (sprintf "%s: %s" pQuest.UserDisplayName pQuest.Title), pQuest.Url, "a few seconds ago"
                    soIcoUrl, (sprintf "%s: %s" soQuest.UserDisplayName soQuest.Title), soQuest.Url, "a few seconds ago"]
    Page.rows()
    |> List.zip expected
    |> List.iter checkMatchSingleLinkRow

[<Test>]
let ``Newest ajax news highlighted with special css class``() =
    do saveQuest soQuest
    do Page.go()

    let checkHighlighted = F >> checkHasClass "newActivity"
    let checkNotHighlighted = F >> checkNoClass "newActivity"

    Page.rows() |> List.exactlyOne |> checkNotHighlighted

    do saveQuest pQuest
    waitAjax()
    do click Page.hiddenNews

    let rows = Page.rows()
    List.nth rows 0 |> checkHighlighted
    List.nth rows 1 |> checkNotHighlighted

    do saveQuest { pQuest with Id = pQuest.Id + 1 }
    waitAjax()
    do click Page.hiddenNews

    let rows = Page.rows()
    List.nth rows 0 |> checkHighlighted
    List.nth rows 1 |> checkNotHighlighted
    List.nth rows 2 |> checkNotHighlighted

[<Test>]
let ``Hidden newest count in title``() =
    do saveQuest { soQuest with Id = 1 }
    do Page.go()
    waitTitle "F# News"

    do saveQuest { soQuest with Id = 2 }
    waitAjax()
    waitTitle "(1) F# News"

    do saveQuest { pQuest with Id = 3 }
    waitAjax()
    waitTitle "(2) F# News"

    do click Page.hiddenNews
    waitTitle "F# News"

let genQuests n =
    [1..n]
    |> List.map (fun i -> { soQuest with Id = i
                                         Title = sprintf "Test question #%d" i
                                         CreationDate = soQuest.CreationDate.AddMilliseconds(float i) })

[<Test>]
let ``Infinite scroll``() =
    let savedQuests = genQuests (Page.perPageCount * 2 + 10)
    savedQuests
    |> List.map StackExchangeQuestion
    |> List.iter (fun a -> Storage.save(a, ""); sleepMs 5)

    let takeExpected n =
        savedQuests
        |> List.rev
        |> Seq.map linkTitle
        |> Seq.take n

    do Page.go()

    Page.rows().Length |> assertEqual 100
    Page.rows()
    |> Seq.zip (takeExpected 100)
    |> Seq.iter checkMatchText
    checkNotDisplayed Page.noMoreNews

    do scrollToBottom()
    Page.rows().Length |> assertEqual 200
    Page.rows()
    |> Seq.zip (takeExpected 200)
    |> Seq.iter checkMatchText
    checkNotDisplayed Page.noMoreNews

    do scrollToBottom()
    Page.rows().Length |> assertEqual 210
    Page.rows()
    |> Seq.zip (takeExpected 210)
    |> Seq.iter checkMatchText
    checkDisplayed Page.noMoreNews

[<Test>]
let ``Earlier news should be requested by creation date``() =
    let initialQuests = genQuests Page.perPageCount |> List.map StackExchangeQuestion
    initialQuests |> Seq.take 50 |> Seq.iter (fun a -> Storage.save(a, ""); sleepMs 5)
    let timeWhileSaving = DateTime.UtcNow
    initialQuests |> Seq.skip 50 |> Seq.iter (fun a -> Storage.save(a, ""); sleepMs 5)
    do Page.go()

    let oldest = { soQuest with Id=999; Title = "Oldest"; CreationDate = soQuest.CreationDate.AddDays(-1.) }
    do Storage.saveWithAdded(StackExchangeQuestion oldest, timeWhileSaving)
    do scrollToBottom()

    let rows = Page.rows()
    rows.Length |> assertEqual (Page.perPageCount + 1)
    rows |> Seq.last |> (fun r -> linkTitle oldest, r) |> checkMatchText

[<Test>]
let ``Loader initially hidden``() =
    do saveQuest soQuest
    do Page.go()
    checkNotDisplayed Page.loader

[<Test>]
let ``Stackexchange question title and twitter tweet should be html decoded``() =
    do saveQuest { soQuest with UserDisplayName = "Jack"; Title = "Converting a Union&lt;&#39;a&gt; to a Union&lt;&#39;b&gt;" }
    do saveTweet { tweet with UserScreenName = "Jack"; Text = "Converting a Union&lt;&#39;a&gt; to a Union&lt;&#39;b&gt;" }
    do Page.go()

    Page.rows()
    |> Seq.map getRowCells
    |> Seq.iter (fun (_, link, _) -> F link |> checkTextIs "Jack: Converting a Union<'a> to a Union<'b>")

[<Test>]
let ``Stackexchange user name should be html decoded``() =
    do saveQuest { soQuest with UserDisplayName = "Alex R&#248;nne Petersen"; Title = "test" }
    do Page.go()

    Page.rows()
    |> Seq.map getRowCells
    |> Seq.iter (fun (_, link, _) -> F link |> checkTextIs "Alex Rønne Petersen: test")

[<Test>]
let ``Tweet with urls and photo``() =
    do saveTweet { Id = 434122673254563840L
                   Text = "Latest #fsharp fractal fun: Kidney! Source at https://t.co/ocJUzydPaU @brandewinder http://t.co/zfdty9LIWv"
                   UserId = 107460704L
                   UserScreenName = "relentlessdev"
                   CreationDate = DateTime(2014, 2, 14, 0, 31, 25, DateTimeKind.Utc)
                   Urls = [ { Url = "https://t.co/ocJUzydPaU"
                              ExpandedUrl = "https://github.com/relentless/FractalFun"
                              DisplayUrl = "github.com/relentless/Fra…"
                              StartIndex = 46
                              EndIndex = 69 } ]
                   Photo = Some { Url = "http://t.co/zfdty9LIWv"
                                  MediaUrl = "http://pbs.twimg.com/media/BgZQRVzIQAAWqJx.jpg"
                                  DisplayUrl = "pic.twitter.com/zfdty9LIWv"
                                  StartIndex = 84
                                  EndIndex = 106 } }
    do Page.go()
    let row = Page.rows() |> Seq.exactlyOne

    let _, linksCell, _ = getRowCells row
    match linksCell |> findElements "a" with
    | tweetUrl1::externalUrl::tweetUrl2::imgUrl::imgAnchorUrl::[] ->
        F tweetUrl1 |> checkTextIs "relentlessdev: Latest #fsharp fractal fun: Kidney! Source at"
        F tweetUrl1 |> checkAttributeIs "href" "https://twitter.com/relentlessdev/status/434122673254563840"

        F externalUrl |> checkTextIs "github.com/relentless/Fra…"
        F externalUrl |> checkAttributeIs "href" "https://github.com/relentless/FractalFun"

        F tweetUrl2 |> checkTextIs "@brandewinder"
        F tweetUrl2 |> checkAttributeIs "href" "https://twitter.com/relentlessdev/status/434122673254563840"

        F imgUrl |> checkTextIs "pic.twitter.com/zfdty9LIWv"
        F imgUrl |> checkAttributeIs "href" "http://t.co/zfdty9LIWv"

        F imgAnchorUrl |> checkDisplayed
        F imgAnchorUrl |> checkAttributeIs "href" "http://pbs.twimg.com/media/BgZQRVzIQAAWqJx.jpg"
    | x -> failwithf "Wrong number of links: %d" x.Length
