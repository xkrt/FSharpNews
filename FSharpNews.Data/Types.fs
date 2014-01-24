namespace FSharpNews.Data

open System

type StackExchangeSite =
    | Stackoverflow
    | Programmers

type StackExchangeQuestion = { Id: int
                               Site: StackExchangeSite
                               Title: string
                               UserDisplayName: string
                               Url: string
                               CreationDate: DateTime }

type Tweet = { Id: int64
               Text: string
               UserId: int64
               UserScreenName: string
               CreationDate: DateTime }

type NugetPackage = { Id: string
                      Version: string
                      Url: string
                      PublishedDate: DateTime }

type Activity =
    | StackExchangeQuestion of StackExchangeQuestion
    | Tweet of Tweet
    | NugetPackage of NugetPackage
    with override x.ToString() = sprintf "%A" x
