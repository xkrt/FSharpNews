module FSharpNews.Tests.DataPull.Service.TestData

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

    let reviewJson = toLine """{
  "items": [
    {
      "tags": [
        "c#",
        "f#"
      ],
      "owner": {
        "reputation": 365,
        "user_id": 7452,
        "user_type": "registered",
        "accept_rate": 50,
        "profile_image": "https://www.gravatar.com/avatar/016b90d0f548f6f649e24f2916ccf4b6?s=128&d=identicon&r=PG",
        "display_name": "Oxinabox",
        "link": "http://codereview.stackexchange.com/users/7452/oxinabox"
      },
      "is_answered": true,
      "view_count": 309,
      "answer_count": 2,
      "score": 3,
      "last_activity_date": 1391532420,
      "creation_date": 1317564163,
      "question_id": 5115,
      "link": "http://codereview.stackexchange.com/questions/5115/using-a-function-to-emulate-f-match-in-c",
      "title": "Using a Function to emulate F# Match in C#"
    }
  ],
  "has_more": false,
  "quota_max": 10000,
  "quota_remaining": 8811
}"""
    let reviewActivity =
        StackExchangeQuestion { Id = 5115
                                Site = CodeReview
                                Title = "Using a Function to emulate F# Match in C#"
                                UserDisplayName = "Oxinabox"
                                Url = "http://codereview.stackexchange.com/questions/5115/using-a-function-to-emulate-f-match-in-c"
                                CreationDate = DateTime.unixToUtcDate 1317564163 }

    let golfJson = toLine """{
  "items": [
    {
      "tags": [
        "code-golf",
        "tips",
        "f#"
      ],
      "owner": {
        "reputation": 1874,
        "user_id": 9275,
        "user_type": "registered",
        "accept_rate": 100,
        "profile_image": "http://i.stack.imgur.com/iCodV.png?s=128&g=1",
        "display_name": "ProgramFOX",
        "link": "http://codegolf.stackexchange.com/users/9275/programfox"
      },
      "is_answered": false,
      "view_count": 67,
      "answer_count": 0,
      "community_owned_date": 1387685622,
      "score": 0,
      "last_activity_date": 1387618185,
      "creation_date": 1387618185,
      "question_id": 16116,
      "link": "http://codegolf.stackexchange.com/questions/16116/tips-for-golfing-in-f",
      "title": "Tips for golfing in F#"
    }
  ],
  "has_more": false,
  "quota_max": 10000,
  "quota_remaining": 8802
}"""

    let golfActivity =
        StackExchangeQuestion { Id = 16116
                                Site = CodeGolf
                                Title = "Tips for golfing in F#"
                                UserDisplayName = "ProgramFOX"
                                Url = "http://codegolf.stackexchange.com/questions/16116/tips-for-golfing-in-f"
                                CreationDate = DateTime.unixToUtcDate 1387618185 }

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

    let retweetJson = toLine """{
  "created_at": "Wed Jan 22 14:16:39 +0000 2014",
  "id": 425995429973860352,
  "id_str": "425995429973860352",
  "text": "RT @davidalpert: This is impressive: “@dsyme: Seamless integration of code, spreadsheet and data, combining Excel with F#... http://t.co/Ji…",
  "source": "web",
  "truncated": false,
  "in_reply_to_status_id": null,
  "in_reply_to_status_id_str": null,
  "in_reply_to_user_id": null,
  "in_reply_to_user_id_str": null,
  "in_reply_to_screen_name": null,
  "user": {
    "id": 390596620,
    "id_str": "390596620",
    "name": "Søren Engel",
    "screen_name": "soren_engel",
    "location": "Denmark",
    "url": null,
    "description": null,
    "protected": false,
    "followers_count": 30,
    "friends_count": 183,
    "listed_count": 0,
    "created_at": "Fri Oct 14 07:00:18 +0000 2011",
    "favourites_count": 541,
    "utc_offset": null,
    "time_zone": null,
    "geo_enabled": false,
    "verified": false,
    "statuses_count": 489,
    "lang": "en",
    "contributors_enabled": false,
    "is_translator": false,
    "profile_background_color": "C0DEED",
    "profile_background_image_url": "http://abs.twimg.com/images/themes/theme1/bg.png",
    "profile_background_image_url_https": "https://abs.twimg.com/images/themes/theme1/bg.png",
    "profile_background_tile": false,
    "profile_image_url": "http://pbs.twimg.com/profile_images/3490857112/51f09f2e8b0833be7a16113f759bfd9d_normal.jpeg",
    "profile_image_url_https": "https://pbs.twimg.com/profile_images/3490857112/51f09f2e8b0833be7a16113f759bfd9d_normal.jpeg",
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
  "retweeted_status": {
    "created_at": "Wed Jan 22 13:43:01 +0000 2014",
    "id": 425986964681003008,
    "id_str": "425986964681003008",
    "text": "This is impressive: “@dsyme: Seamless integration of code, spreadsheet and data, combining Excel with F#... http://t.co/Jim8wNISZf #fsharp”",
    "source": "<a href=\"http://twitter.com/download/iphone\" rel=\"nofollow\">Twitter for iPhone</a>",
    "truncated": false,
    "in_reply_to_status_id": 424653658673131520,
    "in_reply_to_status_id_str": "424653658673131520",
    "in_reply_to_user_id": 25663453,
    "in_reply_to_user_id_str": "25663453",
    "in_reply_to_screen_name": "dsyme",
    "user": {
      "id": 17907937,
      "id_str": "17907937",
      "name": "David Alpert",
      "screen_name": "davidalpert",
      "location": "Winnipeg, MB, Canada",
      "url": "http://www.spinthemoose.com/",
      "description": "husband, father, UX designer, front-end engineer, and software craftsman.",
      "protected": false,
      "followers_count": 528,
      "friends_count": 399,
      "listed_count": 68,
      "created_at": "Fri Dec 05 21:03:32 +0000 2008",
      "favourites_count": 630,
      "utc_offset": -25200,
      "time_zone": "Mountain Time (US & Canada)",
      "geo_enabled": true,
      "verified": false,
      "statuses_count": 9187,
      "lang": "en",
      "contributors_enabled": false,
      "is_translator": false,
      "profile_background_color": "C0DEED",
      "profile_background_image_url": "http://abs.twimg.com/images/themes/theme1/bg.png",
      "profile_background_image_url_https": "https://abs.twimg.com/images/themes/theme1/bg.png",
      "profile_background_tile": false,
      "profile_image_url": "http://pbs.twimg.com/profile_images/1375779237/David3_cropped_normal.jpg",
      "profile_image_url_https": "https://pbs.twimg.com/profile_images/1375779237/David3_cropped_normal.jpg",
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
    "retweet_count": 2,
    "favorite_count": 2,
    "entities": {
      "hashtags": [
        {
          "text": "fsharp",
          "indices": [
            131,
            138
          ]
        }
      ],
      "symbols": [],
      "urls": [
        {
          "url": "http://t.co/Jim8wNISZf",
          "expanded_url": "http://bit.ly/1cUI4Kh",
          "display_url": "bit.ly/1cUI4Kh",
          "indices": [
            108,
            130
          ]
        }
      ],
      "user_mentions": [
        {
          "screen_name": "dsyme",
          "name": "Don Syme",
          "id": 25663453,
          "id_str": "25663453",
          "indices": [
            21,
            27
          ]
        }
      ]
    },
    "favorited": false,
    "retweeted": false,
    "possibly_sensitive": false,
    "lang": "en"
  },
  "retweet_count": 0,
  "favorite_count": 0,
  "entities": {
    "hashtags": [
      {
        "text": "fsharp",
        "indices": [
          139,
          140
        ]
      }
    ],
    "symbols": [],
    "urls": [
      {
        "url": "http://t.co/Jim8wNISZf",
        "expanded_url": "http://bit.ly/1cUI4Kh",
        "display_url": "bit.ly/1cUI4Kh",
        "indices": [
          139,
          140
        ]
      }
    ],
    "user_mentions": [
      {
        "screen_name": "davidalpert",
        "name": "David Alpert",
        "id": 17907937,
        "id_str": "17907937",
        "indices": [
          3,
          15
        ]
      },
      {
        "screen_name": "dsyme",
        "name": "Don Syme",
        "id": 25663453,
        "id_str": "25663453",
        "indices": [
          38,
          44
        ]
      }
    ]
  },
  "favorited": false,
  "retweeted": false,
  "possibly_sensitive": false,
  "filter_level": "medium",
  "lang": "en"
}"""

    let replyJson = toLine """{
  "created_at": "Sun Feb 23 09:02:11 +0000 2014",
  "id": 437512703427764224,
  "id_str": "437512703427764224",
  "text": "@bradwilson If you ever solve this then please tell the #fsharp community how.",
  "source": "<a href=\"http://twitter.com/download/android\" rel=\"nofollow\">Twitter for Android</a>",
  "truncated": false,
  "in_reply_to_status_id": 437421541031022592,
  "in_reply_to_status_id_str": "437421541031022592",
  "in_reply_to_user_id": 988341,
  "in_reply_to_user_id_str": "988341",
  "in_reply_to_screen_name": "bradwilson",
  "user": {
    "id": 22477880,
    "id_str": "22477880",
    "name": "Steffen Forkmann",
    "screen_name": "sforkmann",
    "location": "Essen / Germany",
    "url": "http://www.navision-blog.de",
    "description": "F# and Dynamics NAV developer, blogger and sometimes speaker. Creator of FAKE - F# Make and NaturalSpec. MVP for F#.",
    "protected": false,
    "followers_count": 1147,
    "friends_count": 665,
    "listed_count": 81,
    "created_at": "Mon Mar 02 12:04:39 +0000 2009",
    "favourites_count": 202,
    "utc_offset": 3600,
    "time_zone": "Berlin",
    "geo_enabled": true,
    "verified": false,
    "statuses_count": 11942,
    "lang": "en",
    "contributors_enabled": false,
    "is_translator": false,
    "is_translation_enabled": false,
    "profile_background_color": "EDECE9",
    "profile_background_image_url": "http://abs.twimg.com/images/themes/theme3/bg.gif",
    "profile_background_image_url_https": "https://abs.twimg.com/images/themes/theme3/bg.gif",
    "profile_background_tile": false,
    "profile_image_url": "http://pbs.twimg.com/profile_images/1280550984/buddy_lueneburg_normal.jpg",
    "profile_image_url_https": "https://pbs.twimg.com/profile_images/1280550984/buddy_lueneburg_normal.jpg",
    "profile_link_color": "088253",
    "profile_sidebar_border_color": "D3D2CF",
    "profile_sidebar_fill_color": "E3E2DE",
    "profile_text_color": "634047",
    "profile_use_background_image": true,
    "default_profile": false,
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
          56,
          63
        ]
      }
    ],
    "symbols": [],
    "urls": [],
    "user_mentions": [
      {
        "screen_name": "bradwilson",
        "name": "Brad Wilson",
        "id": 988341,
        "id_str": "988341",
        "indices": [
          0,
          11
        ]
      }
    ]
  },
  "favorited": false,
  "retweeted": false,
  "filter_level": "medium",
  "lang": "en"
}"""


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

module FsSnip =
    let emptyJson = "[]"

    let json = """[{
  "author": "Michel Caradec",
  "description": "One solution to Rosalind rabbits problem.",
  "likes": 0,
  "link": "http://fssnip.net/lQ",
  "published": "7 hours ago",
  "title": "Rabbits and Recurrence Relations"
}]"""

    let snippet = { FsSnippet.Id = "lQ"
                    Title = "Rabbits and Recurrence Relations"
                    Author = "Michel Caradec"
                    Url = "http://fssnip.net/lQ"
                    PublishedDate = DateTime.UtcNow.Subtract(TimeSpan.FromHours(7.)) }

module FPish =
    let emptyXml = """<?xml version="1.0" encoding="utf-8"?>
<feed xmlns="http://www.w3.org/2005/Atom">
  <title type="text">FPish.net - Questions tagged 'f#'</title>
  <subtitle type="text">Questions asked on FPish.net and tagged 'f#'.</subtitle>
  <id>uuid:ebef6232-7545-4ed6-8fd2-39abcdae4b62;id=21227</id>
  <updated>2014-03-06T04:42:34Z</updated>
</feed>"""

    let xml = """<?xml version="1.0" encoding="utf-8"?>
<feed xmlns="http://www.w3.org/2005/Atom">
  <title type="text">FPish.net - Questions tagged 'f#'</title>
  <subtitle type="text">Questions asked on FPish.net and tagged 'f#'.</subtitle>
  <id>uuid:ebef6232-7545-4ed6-8fd2-39abcdae4b62;id=21227</id>
  <updated>2014-03-06T04:42:34Z</updated>
  <entry>
    <id>http://fpish.net//topic/None/57493</id>
    <title type="text">Record field names as function parameters </title>
    <published>2008-05-22T18:30:21-07:00</published>
    <updated>2014-03-06T04:42:34Z</updated>
    <link href="http://fpish.net//topic/None/57493" rel="alternate"/>
    <category term="f#"/>
    <category term="compiler"/>
    <content type="text">Asked by michaeldavid </content>
  </entry>
</feed>"""

    let question = { FPishQuestion.Id = 57493
                     Title = "Record field names as function parameters"
                     Author = "michaeldavid"
                     Url = "http://fpish.net//topic/None/57493"
                     PublishedDate = DateTime(2008, 5, 23, 1, 30, 21, DateTimeKind.Utc) }
