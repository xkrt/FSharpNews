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

type TweetUrl = { Url: string
                  ExpandedUrl: string
                  DisplayUrl: string
                  StartIndex: int
                  EndIndex: int }

type TweetPhoto = { Url: string
                    MediaUrl: string
                    DisplayUrl: string
                    StartIndex: int
                    EndIndex: int }

type Tweet = { Id: int64
               Text: string
               UserId: int64
               UserScreenName: string
               CreationDate: DateTime
               Urls: TweetUrl list
               Photo: TweetPhoto option }

type NugetPackage = { Id: string
                      Version: string
                      Url: string
                      PublishedDate: DateTime }

type FsSnippet = { Id: string
                   Title: string
                   Author: string
                   PublishedDate: DateTime
                   Url: string }

type FPishQuestion = { Id: int
                       Title: string
                       Author: string
                       PublishedDate: DateTime
                       Url: string }

type Gist = { Id: string
              Description: string option
              Owner: string
              CreationDate: DateTime
              Url: string }

type Repository = { Id: int
                    Name: string
                    Description: string option
                    Owner: string
                    Url: string
                    CreationDate: DateTime }

type Activity =
    | StackExchangeQuestion of StackExchangeQuestion
    | Tweet of Tweet
    | NugetPackage of NugetPackage
    | FsSnippet of FsSnippet
    | FPishQuestion of FPishQuestion
    | Gist of Gist
    | Repository of Repository
    with override x.ToString() = sprintf "%A" x
