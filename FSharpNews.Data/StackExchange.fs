module FSharpNews.Data.StackExchange

open System
open System.Configuration
open FSharp.Data
open HttpClient
open FSharpNews.Utils

let private log = Logger.create "StackExchange"
let private apiKey = ConfigurationManager.AppSettings.["StackExchangeApiKey"]

type private Questions = JsonProvider<"DataSamples/StackExchange/questions.json">

let private toString (qsEntity: Questions.DomainTypes.Entity) = sprintf "Items.Count=%d; HasMore=%b; QuotaRemaining=%d; QuotaMax=%d" qsEntity.Items.Length qsEntity.HasMore qsEntity.QuotaRemaining qsEntity.QuotaMax
let private toQuestion site (q: Questions.DomainTypes.Item) =
    StackExchangeQuestion { Site = site
                            Id = q.QuestionId
                            Title = q.Title
                            UserDisplayName = q.Owner.DisplayName
                            Url = q.Link
                            CreationDate = DateTime.unixToUtcDate q.CreationDate }

let private siteToStr site =
    match site with
    | StackExchangeSite.Stackoverflow -> "stackoverflow"
    | StackExchangeSite.Programmers -> "programmers"

let fetch site startDateInclusive =
    let fromDateUnixInclusive = DateTime.toUnix startDateInclusive
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
                       log.Debug "Site=%A response code %d. %s" site response.StatusCode (toString qsEntity)
                       let result' =
                            qsEntity.Items
                            |> Array.toList
                            |> List.zipWith toQuestion
                            |> List.map (fun (raw,act) -> (act, raw.JsonValue.ToString()))
                            |> List.append result
                       if qsEntity.HasMore
                       then loop (page+1) result'
                       else result'
        | None -> failwithf "Response code %d" response.StatusCode
    loop 1 []
