module FSharpNews.Tests.Frontend.Tests

open System
open NUnit.Framework
open OpenQA.Selenium
open FSharpNews.Data
open FSharpNews.Utils
open FSharpNews.Tests.Core

[<SetUp>]
let Setup() = do Storage.deleteAll()

let saveQuest q = Storage.save(StackExchangeQuestion q, "")

let soIcoUrl = "http://cdn.sstatic.net/stackoverflow/img/favicon.ico"
let pIcoUrl = "http://cdn.sstatic.net/programmers/img/favicon.ico"

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

let getRowItems (row: IWebElement) =
    match row |> findElements "td" with
    | iconTd::linkTd::dateTd::[] -> iconTd |> findElement "img",
                                    linkTd |> findElement "a",
                                    dateTd
    | _ -> failwithf "Three cells in row expected"

let F x = fun () -> x

let checkMatchRow ((iconSrc,linkText,linkHref,ago), (row: IWebElement)) =
    let img, link, date = getRowItems row
    img?src |> assertEqual iconSrc
    F link |> checkTextIs linkText
    link?href |> assertEqual linkHref
    F date |> checkTextIs ago

let checkMatchText (expectedLinkText, row: IWebElement) =
    let _, link, _ = getRowItems row
    F link |> checkTextIs expectedLinkText

let sleepMs (ms: int) = Threading.Thread.Sleep(ms)
let waitAjax() = sleepMs 5000


module Page =
    let private indexUrl = sprintf "http://%s:4040" Environment.machine
    let go() = go indexUrl

    let hiddenNews = element ".hidden-news-bar"
    let table = element "#news"
    let rows = table |> elementsWithin "tr"
    let loader = element ".loader"
    let noMoreNews = element "#noMoreNews"
    let noNewsAtAll = element "#noNews"


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
    |> List.iter checkMatchRow

[<Test>]
let ``Ajax news hidden with bar``() =
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
    |> List.iter checkMatchRow

[<Test>]
let ``Hidden news count in title``() =
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

[<Test>]
let ``Infinite scroll``() =
    let savedQuests =
        let pageSize = 100
        [1..pageSize*2 + 10]
        |> List.map (fun i -> { soQuest with Id = i
                                             Title = sprintf "Test question #%d" i
                                             CreationDate = soQuest.CreationDate.AddMilliseconds(float i) })
    savedQuests
    |> List.map StackExchangeQuestion
    |> List.iter (fun a -> Storage.save(a, ""); sleepMs 5)

    let takeExpected n =
        savedQuests
        |> List.rev
        |> Seq.map (fun q -> sprintf "%s: %s" q.UserDisplayName q.Title)
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
let ``Loader initially hidden``() =
    do saveQuest soQuest
    do Page.go()
    checkNotDisplayed Page.loader

[<Test>]
let ``Stackexchange question title should be html decoded``() =
    let quest = { soQuest with UserDisplayName = "Jack"; Title = "Converting a Union&lt;&#39;a&gt; to a Union&lt;&#39;b&gt;" }
    do saveQuest quest
    do Page.go()
    let _, link, _ = Page.rows().Head |> getRowItems
    F link |> checkTextIs "Jack: Converting a Union<'a> to a Union<'b>"
