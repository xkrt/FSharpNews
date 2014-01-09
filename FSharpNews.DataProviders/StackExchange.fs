module FSharpNews.DataProviders.StackExchange

open System
open HttpClient
open FSharp.Data
open FSharpNews.Data
open FSharpNews.Utils

let private log = Logger.create "StackExchange"
let private apiKey = "B))N8RBxzKO)Fv*LmA4azA(("

type private Questions = JsonProvider<"DataSamples/StackExchange/questions.json">

let private unixToUtcDate unixStamp =
    let startEpoch = DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
    startEpoch.AddSeconds(float(unixStamp))

let private utcDateToUnix (dateTime: DateTime) =
    let startEpoch = DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
    dateTime.Subtract(startEpoch).TotalSeconds |> int

let private toString (qsEntity: Questions.DomainTypes.Entity) = sprintf "Items.Count=%d; HasMore=%b; QuotaRemaining=%d; QuotaMax=%d" qsEntity.Items.Length qsEntity.HasMore qsEntity.QuotaRemaining qsEntity.QuotaMax
let private toQuestion site (q: Questions.DomainTypes.Item) = StackExchangeQuestion(site, q.QuestionId, q.Title, q.Link, unixToUtcDate(q.CreationDate))

let private siteToStr site =
    match site with
    | StackExchangeSite.Stackoverflow -> "stackoverflow"
    | StackExchangeSite.Programmers -> "programmers"
    | _ -> failwithf "Unknown stackexchange site %A" site

let fetch site startDateInclusive =
    let fromDateUnixInclusive = utcDateToUnix startDateInclusive
    let toQuestion = toQuestion site

    let rec loop page result =
        let response =
            createRequest Get "https://api.stackexchange.com/2.1/questions"
            |> withQueryStringItem { name="key"; value=apiKey }
            |> withQueryStringItem { name="fromdate"; value=fromDateUnixInclusive.ToString() }
            |> withQueryStringItem { name="page"; value=page.ToString() }
            |> withQueryStringItem { name="site"; value=siteToStr site }
            |> withQueryStringItem { name="sort"; value="creation" }
            |> withQueryStringItem { name="order"; value="desc" }
            |> withQueryStringItem { name="tagged"; value="f#" }
            |> withQueryStringItem { name="pagesize"; value="100" }
            |> withQueryStringItem { name="filter"; value="default" }
            |> withAutoDecompression DecompressionScheme.GZip
            |> getResponse

        match response.EntityBody with
        | Some body -> let qsEntity = Questions.Parse body
                       log.Debug "Response code %d. %s" response.StatusCode (toString qsEntity)
                       let result' =
                            qsEntity.Items
                            |> Array.toList
                            |> List.map toQuestion
                            |> List.append result
                       if qsEntity.HasMore
                       then loop (page+1) result'
                       else result'
        | None -> failwithf "Response code %d" response.StatusCode
    loop 1 []
