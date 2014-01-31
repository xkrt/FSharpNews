module FSharpNews.Tests.Acceptance.Tests

open System
open System.Reflection
open System.Diagnostics
open NUnit.Framework
open canopy
open OpenQA.Selenium
open FSharpNews.Data
open FSharpNews.Utils
open FSharpNews.Tests.Core

let soIcoUrl = "http://cdn.sstatic.net/stackoverflow/img/favicon.ico"
let pIcoUrl = "http://cdn.sstatic.net/programmers/img/favicon.ico"
let indexUrl = sprintf "http://%s:4040" Environment.machine

let ajaxInterval = 60
let waitAjax() = sleep (ajaxInterval + 3)

[<SetUp>]
let Setup() =
    do Storage.deleteAll()

let saveQuest q = Storage.save(StackExchangeQuestion q, "")

let (?) (webEl: IWebElement) attr = webEl.GetAttribute(attr)

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

let checkMatch ((iconSrc,linkText,linkHref,ago), row) =
    let img, link, date = match row |> elementsWithin "td" with
                          | iconTd::linkTd::dateTd::[] ->
                              iconTd |> elementWithin "img",
                              linkTd |> elementWithin "a",
                              dateTd
                          | _ -> failwithf "Three cells in row expected"

    img?src |> assertEqual iconSrc
    read link |> assertEqual linkText
    link?href |> assertEqual linkHref
    read date |> assertEqual ago

let table() = element "#news"

[<Test>]
let ``Show special message if no news at all``() =
    do url indexUrl
    notDisplayed "#news"
    displayed "#noNews"

[<Test>]
let ``Special message hidden if has news``() =
    do saveQuest soQuest
    do url indexUrl
    displayed "#news"
    notDisplayed "#noNews"

[<Test>]
let ``Order by creation date descending``() =
    let questOld = { soQuest with Title = "Old"; CreationDate = DateTime.UtcNow.AddHours(-2.); Id = 1 }
    let questMiddle = { soQuest with Title = "Middle"; CreationDate = DateTime.UtcNow.AddHours(-1.); Id = 2 }
    let questNew = { soQuest with Title = "New"; CreationDate = DateTime.UtcNow; Id = 3 }
    [questMiddle; questNew; questOld] |> List.iter saveQuest

    let expected = [soIcoUrl, "User1: New", soQuest.Url, "a few seconds ago"
                    soIcoUrl, "User1: Middle", soQuest.Url, "an hour ago"
                    soIcoUrl, "User1: Old", soQuest.Url, "2 hours ago"]

    do url indexUrl
    element "#news"
    |> elementsWithin "tr"
    |> List.zip expected
    |> List.iter checkMatch

[<Test>]
let ``Ajax news hidden with bar``() =
    do saveQuest soQuest
    do url indexUrl

    notDisplayed ".hidden-news-bar"
    (table() |> elementsWithin "tr").Length |> assertEqual 1

    do saveQuest pQuest
    do waitAjax()

    displayed ".hidden-news-bar"
    ".hidden-news-bar" == "1 news"
    click ".hidden-news-bar"

    let rows = table() |> elementsWithin "tr"
    rows.Length |> assertEqual 2

    let expected = [pIcoUrl, (sprintf "%s: %s" pQuest.UserDisplayName pQuest.Title), pQuest.Url, "a minute ago"
                    soIcoUrl, (sprintf "%s: %s" soQuest.UserDisplayName soQuest.Title), soQuest.Url, "a minute ago"]
    rows
    |> List.zip expected
    |> List.iter checkMatch

[<Test>]
let ``Updated ago``() =
    do saveQuest soQuest
    do url indexUrl

    "#updated" == "Updated a few seconds ago"
    sleep (ajaxInterval - 5)
    "#updated" == "Updated a minute ago"
