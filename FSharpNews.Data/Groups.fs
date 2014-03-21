module FSharpNews.Data.Groups

open System
open System.IO
open System.Xml
open System.Xml.Linq
open System.ServiceModel.Syndication
open FSharpx
open FSharpNews.Utils

type Configuration = { BaseUri: Uri }

let private log = Logger.create "GoogleGroups"

let patch (xdoc: XDocument) =
    let feedUpdatedEl =
        xdoc.Elements()
        |> Seq.find (fun el -> el.Name.LocalName = "feed")
        |> fun el -> el.Elements()
        |> Seq.find (fun el -> el.Name.LocalName = "updated")
    feedUpdatedEl.Value <- DateTime.MinValue.ToIsoString()
    xdoc.ToString()

let private parseId = Strings.split '/' >> Seq.last >> Strings.trim

let fetch conf =
    let atomUrl = Uri(conf.BaseUri, "/forum/feed/fsharp-opensource/topics/atom.xml?num=100").ToString()
    let xdoc = XDocument.Load atomUrl
    let xml = patch xdoc

    use sreader = new StringReader(xml)
    use xreader = XmlReader.Create sreader
    let feed = SyndicationFeed.Load xreader
    let items = feed.Items |> Seq.toList
    do log.Info "Fetched topics: %d" items.Length
    items
    |> List.map (fun i -> GroupTopic { Id = parseId i.Id
                                       Title = Strings.trim i.Title.Text
                                       Starter = i.Authors |> Seq.exactlyOne |> fun a -> a.Name |> Strings.trim
                                       CreationDate = i.LastUpdatedTime.UtcDateTime
                                       Url = i.Links |> Seq.exactlyOne |> fun l -> l.Uri.ToString() }
                          , Serializer.toJson i)
