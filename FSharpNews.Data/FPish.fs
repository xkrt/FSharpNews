module FSharpNews.Data.FPish

open System
open System.Xml
open System.ServiceModel.Syndication
open FSharpx
open FSharpNews.Utils

type Configuration = { BaseUrl: string }

let private log = Logger.create "FPish"

let private parseId link =
    link
    |> Strings.split '/'
    |> Seq.last
    |> Int32.Parse

let private parseAuthor content =
    match content with
    | Regex.Interpreted.Match "Asked by (.+)" m -> m.GroupValues |> Seq.exactlyOne |> Strings.trim
    | _ -> failwithf "Cant parse author in '%s'" content

let fetch conf =
    let atomUrl = Uri(Uri conf.BaseUrl, "/atom/topics/tag/1/f~23").ToString()
    use reader = XmlReader.Create atomUrl
    let feed = SyndicationFeed.Load reader
    let items = feed.Items |> Seq.toList
    do log.Info "Fetched questions: %d" items.Length
    items
    |> List.map (fun i -> FPishQuestion { Id = parseId i.Id
                                          Title = i.Title.Text.Trim()
                                          Author = parseAuthor ((i.Content :?> TextSyndicationContent).Text)
                                          PublishedDate = i.PublishDate.UtcDateTime
                                          Url = i.Id }
                          , Serializer.toJson i)
