module FSharpNews.Tests.Frontend.ActivityViewModelTests

open System
open NUnit.Framework
open FSharpNews.Data
open FSharpNews.Utils
open FSharpNews.Tests.Core
open FSharpNews.Web.Frontend.Models

let assertLinks expectedLinks tweet =
    let model = ActivityViewModel.Create(Tweet tweet, DateTime.UtcNow)
    model.Links |> Collection.assertEqual expectedLinks

[<Test>]
let ``Tweet without urls and without photo``() =
    { Id = 434019892908814336L
      Text = "Wow, 105 registered for the deedle #fsharp talk tonight @skillsmatter"
      UserId = 25549086L
      UserScreenName = "colinbul"
      CreationDate = DateTime(2014, 2, 13, 17, 43, 0, DateTimeKind.Utc)
      Urls = []
      Photo = None }
    |> assertLinks [ { Text = "colinbul: Wow, 105 registered for the deedle #fsharp talk tonight @skillsmatter"
                       Url = "https://twitter.com/colinbul/status/434019892908814336" } ]

[<Test>]
let ``Tweet with url``() =
    { Id = 435053513476169728L
      Text = "Two similar projects showing #csharp and #fsharp code us proportionally http://t.co/43d1UmUX7U"
      UserId = 1251118291L
      UserScreenName = "RyanB_DotNet"
      CreationDate = DateTime(2014, 2, 16, 14, 10, 15, DateTimeKind.Utc)
      Urls = [{ Url = "http://t.co/43d1UmUX7U"
                ExpandedUrl = "http://ow.ly/tyzSB"
                DisplayUrl = "ow.ly/tyzSB"
                StartIndex = 72
                EndIndex = 94 }]
      Photo = None }
    |> assertLinks [ { Text = "RyanB_DotNet: Two similar projects showing #csharp and #fsharp code us proportionally"
                       Url = "https://twitter.com/RyanB_DotNet/status/435053513476169728" }
                     { Text = "ow.ly/tyzSB"
                       Url = "http://ow.ly/tyzSB" } ]

[<Test>]
let ``Tweet with few urls``() =
    { Id = 435129343317008385L
      Text = "http://t.co/3ajpL9VpMF isn't just a fantastic resource for learning #fsharp. You'll also find great posts like this: http://t.co/U5pjKIW7Od"
      UserId = 109438744L
      UserScreenName = "askingalot"
      CreationDate = DateTime(2014, 2, 16, 19, 11, 34, DateTimeKind.Utc)
      Urls = [{ Url = "http://t.co/3ajpL9VpMF"
                ExpandedUrl = "http://fsharpforfunandprofit.com"
                DisplayUrl = "fsharpforfunandprofit.com"
                StartIndex = 0
                EndIndex = 22 }
              { Url = "http://t.co/U5pjKIW7Od"
                ExpandedUrl = "http://fsharpforfunandprofit.com/posts/roman-numeral-kata/"
                DisplayUrl = "fsharpforfunandprofit.com/posts/roman-nu…"
                StartIndex = 117
                EndIndex = 139 }]
      Photo = None }
    |> assertLinks [ { Text = "askingalot:"
                       Url = "https://twitter.com/askingalot/status/435129343317008385" }
                     { Text = "fsharpforfunandprofit.com"
                       Url = "http://fsharpforfunandprofit.com" }
                     { Text = "isn't just a fantastic resource for learning #fsharp. You'll also find great posts like this:"
                       Url = "https://twitter.com/askingalot/status/435129343317008385" }
                     { Text = "fsharpforfunandprofit.com/posts/roman-nu…"
                       Url = "http://fsharpforfunandprofit.com/posts/roman-numeral-kata/" }]

[<Test>]
let ``Tweet with photo``() =
    { Id = 435022949070811137L
      Text = "Operators usage highlighting #FSharpVSPoserTools #fsharp http://t.co/GBUo2K5PE3"
      UserId = 115861128L
      UserScreenName = "kot_2010"
      CreationDate = DateTime(2014, 2, 16, 12, 8, 48, DateTimeKind.Utc)
      Urls = []
      Photo = Some { Url = "http://t.co/GBUo2K5PE3"
                     MediaUrl = "http://pbs.twimg.com/media/BgmDET1CMAAUZud.png"
                     DisplayUrl = "pic.twitter.com/GBUo2K5PE3"
                     StartIndex = 57
                     EndIndex = 79 } }
    |> assertLinks [ { Text = "kot_2010: Operators usage highlighting #FSharpVSPoserTools #fsharp"
                       Url = "https://twitter.com/kot_2010/status/435022949070811137" }
                     { Text = "pic.twitter.com/GBUo2K5PE3"
                       Url = "http://t.co/GBUo2K5PE3" } ]

[<Test>]
let ``Tweet with url and photo``() =
    { Id = 434122673254563840L
      Text = "Latest #fsharp fractal fun: Kidney! Source at https://t.co/ocJUzydPaU @brandewinder http://t.co/zfdty9LIWv"
      UserId = 107460704L
      UserScreenName = "relentlessdev"
      CreationDate = DateTime(2014, 2, 14, 0, 31, 25, DateTimeKind.Utc)
      Urls = [ { Url = "https://t.co/ocJUzydPaU"
                 ExpandedUrl = "https://github.com/relentless/FractalFun"
                 DisplayUrl = "github.com/relentless/Fra…"
                 StartIndex = 46
                 EndIndex = 69 } ]
      Photo = Some { Url = "http://t.co/zfdty9LIWv"
                     MediaUrl = "http://pbs.twimg.com/media/BgZQRVzIQAAWqJx.jpg"
                     DisplayUrl = "pic.twitter.com/zfdty9LIWv"
                     StartIndex = 84
                     EndIndex = 106 } }
    |> assertLinks [ { Text = "relentlessdev: Latest #fsharp fractal fun: Kidney! Source at"
                       Url = "https://twitter.com/relentlessdev/status/434122673254563840" }
                     { Text = "github.com/relentless/Fra…"
                       Url = "https://github.com/relentless/FractalFun" }
                     { Text = "@brandewinder"
                       Url = "https://twitter.com/relentlessdev/status/434122673254563840" }
                     { Text = "pic.twitter.com/zfdty9LIWv"
                       Url = "http://t.co/zfdty9LIWv" } ]
