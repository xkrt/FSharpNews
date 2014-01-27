module FSharpNews.Tests.DataPuller.TestData

open System
open System.Text.RegularExpressions
open FSharpNews.Data
open FSharpNews.Utils

let toLine (str: string) = str.Replace("\r\n", " ")

module StackExchange =
    let emptyJson = """{"items":[],"has_more":false,"quota_max":10000,"quota_remaining":9996}"""

    let soJson = toLine """{
  "items": [
     {
      "tags": [
        "forms",
        "f#"
      ],
      "owner": {
        "display_name": "user2165793",
        "link": "http://stackoverflow.com/users/2165793/user2165793",
        "profile_image": "https://www.gravatar.com/avatar/98853eb68c77f8744e0c0588e6305d68?s=128&d=identicon&r=PG",
        "accept_rate": 100,
        "user_type": "registered",
        "user_id": 2165793,
        "reputation": 46
      },
      "title": "Drawing squares on windows form",
      "link": "http://stackoverflow.com/questions/21007873/drawing-squares-on-windows-form",
      "question_id": 21007873,
      "creation_date": 1389219805,
      "last_activity_date": 1389228256,
      "score": 2,
      "answer_count": 1,
      "accepted_answer_id": 21008015,
      "view_count": 27,
      "is_answered": true
    }
  ],
  "quota_remaining": 9999,
  "quota_max": 10000,
  "has_more": false
}"""

    let soActivity =
        StackExchangeQuestion { Id = 21007873
                                Site = Stackoverflow
                                Title = "Drawing squares on windows form"
                                UserDisplayName = "user2165793"
                                Url = "http://stackoverflow.com/questions/21007873/drawing-squares-on-windows-form"
                                CreationDate = DateTime.unixToUtcDate 1389219805 }

    let progJson = toLine """{
  "items": [
    {
      "tags": [
        "f#",
        "computation-expressions"
      ],
      "owner": {
        "reputation": 4297,
        "user_id": 12902,
        "user_type": "registered",
        "accept_rate": 40,
        "profile_image": "https://www.gravatar.com/avatar/89c3645ed352c9f08fae80da29437236?s=128&d=identicon&r=PG",
        "display_name": "Pete",
        "link": "http://programmers.stackexchange.com/users/12902/pete"
      },
      "is_answered": true,
      "view_count": 171,
      "answer_count": 2,
      "score": 3,
      "last_activity_date": 1390000286,
      "creation_date": 1384788003,
      "last_edit_date": 1384798054,
      "question_id": 218779,
      "link": "http://programmers.stackexchange.com/questions/218779/is-there-a-way-to-created-nested-computation-expressions",
      "title": "Is there a way to created nested computation expressions?"
    }
  ],
  "has_more": false,
  "quota_max": 10000,
  "quota_remaining": 9998
}"""

    let progActivity =
        StackExchangeQuestion { Id = 218779
                                Site = Programmers
                                Title = "Is there a way to created nested computation expressions?"
                                UserDisplayName = "Pete"
                                Url = "http://programmers.stackexchange.com/questions/218779/is-there-a-way-to-created-nested-computation-expressions"
                                CreationDate = DateTime.unixToUtcDate 1384788003 }

module Twitter =
    let json = toLine """{
  "created_at": "Wed Jan 22 14:34:07 +0000 2014",
  "id": 425999826191650816,
  "id_str": "425999826191650816",
  "text": "Just found out about the fdharpx library for #fsharp Now I can haz Validation.",
  "source": "<a href=\"http://twitter.com/download/android\" rel=\"nofollow\">Twitter for Android</a>",
  "truncated": false,
  "in_reply_to_status_id": null,
  "in_reply_to_status_id_str": null,
  "in_reply_to_user_id": null,
  "in_reply_to_user_id_str": null,
  "in_reply_to_screen_name": null,
  "user": {
    "id": 404119861,
    "id_str": "404119861",
    "name": "Owein Reese",
    "screen_name": "OweinReese",
    "location": "NYC",
    "url": "http://staticallytyped.wordpress.com",
    "description": "Scala, Python, JavaScript programmer.\nLikes: Windsurfing, the snooze button, egg & sausage sandwiches, avocado and bacon.",
    "protected": false,
    "followers_count": 82,
    "friends_count": 152,
    "listed_count": 8,
    "created_at": "Thu Nov 03 13:37:43 +0000 2011",
    "favourites_count": 24,
    "utc_offset": null,
    "time_zone": null,
    "geo_enabled": false,
    "verified": false,
    "statuses_count": 1424,
    "lang": "en",
    "contributors_enabled": false,
    "is_translator": false,
    "profile_background_color": "C0DEED",
    "profile_background_image_url": "http://abs.twimg.com/images/themes/theme1/bg.png",
    "profile_background_image_url_https": "https://abs.twimg.com/images/themes/theme1/bg.png",
    "profile_background_tile": false,
    "profile_image_url": "http://pbs.twimg.com/profile_images/3100335637/05dd4e2e54d9bbc3049ffecd404812b8_normal.jpeg",
    "profile_image_url_https": "https://pbs.twimg.com/profile_images/3100335637/05dd4e2e54d9bbc3049ffecd404812b8_normal.jpeg",
    "profile_banner_url": "https://pbs.twimg.com/profile_banners/404119861/1363176309",
    "profile_link_color": "0084B4",
    "profile_sidebar_border_color": "C0DEED",
    "profile_sidebar_fill_color": "DDEEF6",
    "profile_text_color": "333333",
    "profile_use_background_image": true,
    "default_profile": true,
    "default_profile_image": false,
    "following": null,
    "follow_request_sent": null,
    "notifications": null
  },
  "geo": null,
  "coordinates": null,
  "place": null,
  "contributors": null,
  "retweet_count": 0,
  "favorite_count": 0,
  "entities": {
    "hashtags": [
      {
        "text": "fsharp",
        "indices": [
          45,
          52
        ]
      }
    ],
    "symbols": [],
    "urls": [],
    "user_mentions": []
  },
  "favorited": false,
  "retweeted": false,
  "filter_level": "medium",
  "lang": "en"
}"""

    let activity =
        Tweet { Id = 425999826191650816L
                Text = "Just found out about the fdharpx library for #fsharp Now I can haz Validation."
                UserId = 404119861L
                UserScreenName = "OweinReese"
                CreationDate = DateTime(2014, 1, 22, 14, 34, 7, DateTimeKind.Utc) }

module NuGet =
    let emptyXml = """<?xml version="1.0" encoding="utf-8"?>
<feed xml:base="https://www.nuget.org/api/v2/" 
    xmlns="http://www.w3.org/2005/Atom" 
    xmlns:d="http://schemas.microsoft.com/ado/2007/08/dataservices" 
    xmlns:m="http://schemas.microsoft.com/ado/2007/08/dataservices/metadata">
    <id>https://www.nuget.org/api/v2/Packages</id>
    <title type="text">Packages</title>
    <updated>2014-01-27T15:28:09Z</updated>
    <link rel="self" title="Packages" href="Packages" />
    <author>
        <name />
    </author>
</feed>"""

    let xml = """<?xml version="1.0" encoding="utf-8"?>
<feed xml:base="https://www.nuget.org/api/v2/" xmlns="http://www.w3.org/2005/Atom" xmlns:d="http://schemas.microsoft.com/ado/2007/08/dataservices" xmlns:m="http://schemas.microsoft.com/ado/2007/08/dataservices/metadata">
    <id>https://www.nuget.org/api/v2/Packages</id>
    <title type="text">Packages</title>
    <updated>2014-01-27T13:40:37Z</updated>
    <link href="Packages" rel="self" title="Packages"/>
    <entry>
        <id>https://www.nuget.org/api/v2/Packages(Id='FSharp.Formatting',Version='2.3.5-beta')</id>
        <category scheme="http://schemas.microsoft.com/ado/2007/08/dataservices/scheme" term="NuGetGallery.V2FeedPackage"/>
        <link href="Packages(Id='FSharp.Formatting',Version='2.3.5-beta')" rel="edit" title="V2FeedPackage"/>
        <title type="text">FSharp.Formatting</title>
        <summary type="text">A package of libraries for building great F# documentation, samples and blogs</summary>
        <updated>2014-01-27T13:38:12Z</updated>
        <author>
            <name>Tomas Petricek,  Oleg Pestov,  Anh-Dung Phan,  Xiang Zhang</name>
        </author>
        <link href="Packages(Id='FSharp.Formatting',Version='2.3.5-beta')/$value" rel="edit-media" title="V2FeedPackage"/>
        <content src="https://www.nuget.org/api/v2/package/FSharp.Formatting/2.3.5-beta" type="application/zip"/>
        <m:properties>
            <d:Version>2.3.5-beta</d:Version>
            <d:NormalizedVersion>2.3.5-beta</d:NormalizedVersion>
            <d:Copyright>Copyright 2014</d:Copyright>
            <d:Created m:type="Edm.DateTime">2014-01-21T02:22:55.767</d:Created>
            <d:Dependencies>Microsoft.AspNet.Razor:2.0.30506.0:|RazorEngine:3.3.0:|FSharp.Compiler.Service:0.0.11-alpha:</d:Dependencies>
            <d:Description>The package is a collection of libraries that can be used for literate programming with F# (great for building documentation) and for generating library documentation  from inline code comments. The key componments are Markdown parser, tools for formatting  F# code snippets, including tool tip type information and a tool for generating  documentation from library metadata.</d:Description>
            <d:DownloadCount m:type="Edm.Int32">1933</d:DownloadCount>
            <d:GalleryDetailsUrl>https://www.nuget.org/packages/FSharp.Formatting/2.3.5-beta</d:GalleryDetailsUrl>
            <d:IconUrl>https://raw.github.com/tpetricek/FSharp.Formatting/master/docs/files/misc/logo.png</d:IconUrl>
            <d:IsLatestVersion m:type="Edm.Boolean">false</d:IsLatestVersion>
            <d:IsAbsoluteLatestVersion m:type="Edm.Boolean">true</d:IsAbsoluteLatestVersion>
            <d:IsPrerelease m:type="Edm.Boolean">true</d:IsPrerelease>
            <d:Language m:null="true"/>
            <d:Published m:type="Edm.DateTime">2014-01-21T02:22:55.767</d:Published>
            <d:PackageHash>/t8wb1jAI+SLa71rLm0aquJ7iGdwLk9EcKFEtrdqbBUP90qZLnfqqaXZj1j/tHnWZingYC64hpMEqfoiuVGy+g==</d:PackageHash>
            <d:PackageHashAlgorithm>SHA512</d:PackageHashAlgorithm>
            <d:PackageSize m:type="Edm.Int64">432690</d:PackageSize>
            <d:ProjectUrl>http://github.com/tpetricek/FSharp.Formatting</d:ProjectUrl>
            <d:ReportAbuseUrl>https://www.nuget.org/package/ReportAbuse/FSharp.Formatting/2.3.5-beta</d:ReportAbuseUrl>
            <d:ReleaseNotes>Omit non-public members from metadata docs by default.</d:ReleaseNotes>
            <d:RequireLicenseAcceptance m:type="Edm.Boolean">false</d:RequireLicenseAcceptance>
            <d:Tags>F# fsharp formatting markdown code fssnip literate programming</d:Tags>
            <d:Title m:null="true"/>
            <d:VersionDownloadCount m:type="Edm.Int32">142</d:VersionDownloadCount>
            <d:MinClientVersion m:null="true"/>
            <d:LastEdited m:null="true" m:type="Edm.DateTime"/>
            <d:LicenseUrl>http://github.com/tpetricek/FSharp.Formatting/blob/master/LICENSE.md</d:LicenseUrl>
            <d:LicenseNames m:null="true"/>
            <d:LicenseReportUrl m:null="true"/>
        </m:properties>
    </entry>
</feed>"""

    let activity = NugetPackage { Id = "FSharp.Formatting"
                                  Version = "2.3.5-beta"
                                  Url = "https://www.nuget.org/packages/FSharp.Formatting/2.3.5-beta"
                                  PublishedDate = DateTime(2014, 1, 21, 2, 22, 55, 767, DateTimeKind.Utc) }
