namespace FSharpNews.Data

open System

type StackExchangeSite =
    | Stackoverflow = 0
    | Programmers = 1

type StackExchangeQuestion(site: StackExchangeSite, id: int, title: string, url: string, creationDate: DateTime) =
    member val Id = id with get, set
    member val Site = site with get, set
    member val Title = title with get, set
    member val Url = url with get, set
    member val CreationDate = creationDate with get, set
