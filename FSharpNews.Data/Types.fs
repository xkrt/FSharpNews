namespace FSharpNews.Data

open System

type StackExchangeSite =
    | Stackoverflow
    | Programmers
    | CodeReview
    | CodeGolf

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

type FsSnippet = { Id: string
                   Title: string
                   Author: string
                   PublishedDate: DateTime
                   Url: string }

type Activity =
    | StackExchangeQuestion of StackExchangeQuestion
    | Tweet of Tweet
    | NugetPackage of NugetPackage
    | FsSnippet of FsSnippet
    with override x.ToString() = sprintf "%A" x
