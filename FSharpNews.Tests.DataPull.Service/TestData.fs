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
    let streamJson = toLine """{
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

    let streamTweet = { Id = 425999826191650816L
                        Text = "Just found out about the fdharpx library for #fsharp Now I can haz Validation."
                        UserId = 404119861L
                        UserScreenName = "OweinReese"
                        CreationDate = DateTime(2014, 1, 22, 14, 34, 7, DateTimeKind.Utc)
                        Urls = []
                        Photo = None }
    let streamActivity = Tweet streamTweet

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

    let searchJson = """{
  "statuses": [
    {
      "metadata": {
        "result_type": "recent",
        "iso_language_code": "en"
      },
      "created_at": "Mon Mar 17 04:39:08 +0000 2014",
      "id": 445419037632528384,
      "id_str": "445419037632528384",
      "text": "For what the TIOBE Index is worth, #fsharp is on a rapid rise.  http://t.co/3eSa03F9Ol",
      "source": "web",
      "truncated": false,
      "in_reply_to_status_id": null,
      "in_reply_to_status_id_str": null,
      "in_reply_to_user_id": null,
      "in_reply_to_user_id_str": null,
      "in_reply_to_screen_name": null,
      "user": {
        "id": 143032843,
        "id_str": "143032843",
        "name": "Josef Koza",
        "screen_name": "JosefKoza",
        "location": "",
        "description": "",
        "url": null,
        "entities": {
          "description": {
            "urls": []
          }
        },
        "protected": false,
        "followers_count": 46,
        "friends_count": 148,
        "listed_count": 0,
        "created_at": "Wed May 12 11:54:52 +0000 2010",
        "favourites_count": 6,
        "utc_offset": -14400,
        "time_zone": "Eastern Time (US & Canada)",
        "geo_enabled": true,
        "verified": false,
        "statuses_count": 127,
        "lang": "en",
        "contributors_enabled": false,
        "is_translator": false,
        "is_translation_enabled": false,
        "profile_background_color": "C0DCF1",
        "profile_background_image_url": "http://pbs.twimg.com/profile_background_images/378800000083362189/a6edfb7e3d68e87264874ca185a65dd1.jpeg",
        "profile_background_image_url_https": "https://pbs.twimg.com/profile_background_images/378800000083362189/a6edfb7e3d68e87264874ca185a65dd1.jpeg",
        "profile_background_tile": false,
        "profile_image_url": "http://pbs.twimg.com/profile_images/344513261566788205/80047a159760ddca71f4106671ce6073_normal.jpeg",
        "profile_image_url_https": "https://pbs.twimg.com/profile_images/344513261566788205/80047a159760ddca71f4106671ce6073_normal.jpeg",
        "profile_link_color": "C0DCF1",
        "profile_sidebar_border_color": "C0DCF1",
        "profile_sidebar_fill_color": "1C1C1C",
        "profile_text_color": "5C91B9",
        "profile_use_background_image": true,
        "default_profile": false,
        "default_profile_image": false,
        "following": false,
        "follow_request_sent": false,
        "notifications": false
      },
      "geo": null,
      "coordinates": null,
      "place": null,
      "contributors": null,
      "retweet_count": 0,
      "favorite_count": 1,
      "entities": {
        "hashtags": [
          {
            "text": "fsharp",
            "indices": [
              35,
              42
            ]
          }
        ],
        "symbols": [],
        "urls": [
          {
            "url": "http://t.co/3eSa03F9Ol",
            "expanded_url": "http://www.tiobe.com/index.php/content/paperinfo/tpci/index.html",
            "display_url": "tiobe.com/index.php/cont…",
            "indices": [
              64,
              86
            ]
          }
        ],
        "user_mentions": []
      },
      "favorited": false,
      "retweeted": false,
      "possibly_sensitive": false,
      "lang": "en"
    }
  ],
  "search_metadata": {
    "completed_in": 0.095,
    "max_id": 445440121127841793,
    "max_id_str": "445440121127841793",
    "next_results": "?max_id=444870422270472191&q=%23fsharp&lang=en&count=100&include_entities=1&result_type=recent",
    "query": "%23fsharp",
    "refresh_url": "?since_id=445440121127841793&q=%23fsharp&lang=en&result_type=recent&include_entities=1",
    "count": 100,
    "since_id": 418099438234894335,
    "since_id_str": "418099438234894335"
  }
}"""
    let searchTweet = { Id = 445419037632528384L
                        Text = "For what the TIOBE Index is worth, #fsharp is on a rapid rise.  http://t.co/3eSa03F9Ol"
                        UserId = 143032843L
                        UserScreenName = "JosefKoza"
                        CreationDate = DateTime(2014, 3, 17, 4, 39, 8, DateTimeKind.Utc)
                        Urls = [{ Url = "http://t.co/3eSa03F9Ol"
                                  ExpandedUrl = "http://www.tiobe.com/index.php/content/paperinfo/tpci/index.html"
                                  DisplayUrl = "tiobe.com/index.php/cont…"
                                  StartIndex = 64
                                  EndIndex = 86 }]
                        Photo = None }
    let searchActivity = Tweet searchTweet


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
    let activity = FPishQuestion question

module Gist =
    let emptyJson = "[]"

    // gist with F# file
    let fsharpJson = """[
  {
    "url": "https://api.github.com/gists/9380680",
    "forks_url": "https://api.github.com/gists/9380680/forks",
    "commits_url": "https://api.github.com/gists/9380680/commits",
    "id": "9380680",
    "git_pull_url": "https://gist.github.com/9380680.git",
    "git_push_url": "https://gist.github.com/9380680.git",
    "html_url": "https://gist.github.com/9380680",
    "files": {
      "p26.fs": {
        "filename": "p26.fs",
        "type": "text/plain",
        "language": "F#",
        "raw_url": "https://gist.githubusercontent.com/drcabana/9380680/raw/bd412a06a5a0f3c51c0ded2a9f2b87aa01b02815/p26.fs",
        "size": 2238
      }
    },
    "public": true,
    "created_at": "2014-03-06T01:56:34Z",
    "updated_at": "2014-03-06T02:00:50Z",
    "description": "euler 26, f# version",
    "comments": 1,
    "user": null,
    "comments_url": "https://api.github.com/gists/9380680/comments",
    "owner": {
      "login": "drcabana",
      "id": 314158,
      "avatar_url": "https://gravatar.com/avatar/2248ec70c680a42550cbc8c8b297e85d?d=https%3A%2F%2Fidenticons.github.com%2F23286291fe49143112b63f52c2f6c23a.png&r=x",
      "gravatar_id": "2248ec70c680a42550cbc8c8b297e85d",
      "url": "https://api.github.com/users/drcabana",
      "html_url": "https://github.com/drcabana",
      "followers_url": "https://api.github.com/users/drcabana/followers",
      "following_url": "https://api.github.com/users/drcabana/following{/other_user}",
      "gists_url": "https://api.github.com/users/drcabana/gists{/gist_id}",
      "starred_url": "https://api.github.com/users/drcabana/starred{/owner}{/repo}",
      "subscriptions_url": "https://api.github.com/users/drcabana/subscriptions",
      "organizations_url": "https://api.github.com/users/drcabana/orgs",
      "repos_url": "https://api.github.com/users/drcabana/repos",
      "events_url": "https://api.github.com/users/drcabana/events{/privacy}",
      "received_events_url": "https://api.github.com/users/drcabana/received_events",
      "type": "User",
      "site_admin": false
    }
  }
]"""

    let gist = { Gist.Id = "9380680"
                 Description = Some "euler 26, f# version"
                 Owner = "drcabana"
                 Url = "https://gist.github.com/9380680"
                 CreationDate = DateTime(2014, 3, 6, 1, 56, 34, DateTimeKind.Utc) }
    let activity = Gist gist

    // gist with non-F# files
    let nonFsharpJson = """[
  {
    "url": "https://api.github.com/gists/9384891",
    "forks_url": "https://api.github.com/gists/9384891/forks",
    "commits_url": "https://api.github.com/gists/9384891/commits",
    "id": "9384891",
    "git_pull_url": "https://gist.github.com/9384891.git",
    "git_push_url": "https://gist.github.com/9384891.git",
    "html_url": "https://gist.github.com/9384891",
    "files": {
      "0_reuse_code.js": {
        "filename": "0_reuse_code.js",
        "type": "application/javascript",
        "language": "JavaScript",
        "raw_url": "https://gist.githubusercontent.com/eakraly/9384891/raw/2b8a5ccf55d94d29328a24ef3913ffb54841429c/0_reuse_code.js",
        "size": 126
      },
      "1_ruby_quicksort.rb": {
        "filename": "1_ruby_quicksort.rb",
        "type": "application/ruby",
        "language": "Ruby",
        "raw_url": "https://gist.githubusercontent.com/eakraly/9384891/raw/b16d3e65da4810d1bf24fd231028f3775f7dcede/1_ruby_quicksort.rb",
        "size": 592
      },
      "2_keyboard_shortcuts.md": {
        "filename": "2_keyboard_shortcuts.md",
        "type": "text/plain",
        "language": "Markdown",
        "raw_url": "https://gist.githubusercontent.com/eakraly/9384891/raw/df51b18a3a36024d04f9348bd4e24f6991e41a02/2_keyboard_shortcuts.md",
        "size": 534
      },
      "3_cheesecake_recipe.md": {
        "filename": "3_cheesecake_recipe.md",
        "type": "text/plain",
        "language": "Markdown",
        "raw_url": "https://gist.githubusercontent.com/eakraly/9384891/raw/f030315106c2b8dc0f52ad861f67223f88d4e35f/3_cheesecake_recipe.md",
        "size": 1821
      }
    },
    "public": true,
    "created_at": "2014-03-06T08:30:32Z",
    "updated_at": "2014-03-06T08:30:32Z",
    "description": "Here are some things you can do with Gists in GistBox.",
    "comments": 0,
    "user": null,
    "comments_url": "https://api.github.com/gists/9384891/comments",
    "owner": {
      "login": "eakraly",
      "id": 2505440,
      "avatar_url": "https://gravatar.com/avatar/bc4d4704c9547f9bad0cf1043b34e8c1?d=https%3A%2F%2Fidenticons.github.com%2F4c0a83bc43bd978408cd4d7a8bbfc6ec.png&r=x",
      "gravatar_id": "bc4d4704c9547f9bad0cf1043b34e8c1",
      "url": "https://api.github.com/users/eakraly",
      "html_url": "https://github.com/eakraly",
      "followers_url": "https://api.github.com/users/eakraly/followers",
      "following_url": "https://api.github.com/users/eakraly/following{/other_user}",
      "gists_url": "https://api.github.com/users/eakraly/gists{/gist_id}",
      "starred_url": "https://api.github.com/users/eakraly/starred{/owner}{/repo}",
      "subscriptions_url": "https://api.github.com/users/eakraly/subscriptions",
      "organizations_url": "https://api.github.com/users/eakraly/orgs",
      "repos_url": "https://api.github.com/users/eakraly/repos",
      "events_url": "https://api.github.com/users/eakraly/events{/privacy}",
      "received_events_url": "https://api.github.com/users/eakraly/received_events",
      "type": "User",
      "site_admin": false
    }
  }
]"""

    // F# gist with japan description
    let japanFsharpJson = """[
  {
    "url": "https://api.github.com/gists/9340629",
    "forks_url": "https://api.github.com/gists/9340629/forks",
    "commits_url": "https://api.github.com/gists/9340629/commits",
    "id": "9340629",
    "git_pull_url": "https://gist.github.com/9340629.git",
    "git_push_url": "https://gist.github.com/9340629.git",
    "html_url": "https://gist.github.com/9340629",
    "files": {
      "TextBox2.cs": {
        "filename": "TextBox2.cs",
        "type": "text/plain",
        "language": "C#",
        "raw_url": "https://gist.githubusercontent.com/sayurin/9340629/raw/866f078e069f88c53761573b24f6d60b8722b51e/TextBox2.cs",
        "size": 1047
      },
      "TextBox2.fs": {
        "filename": "TextBox2.fs",
        "type": "text/plain",
        "language": "F#",
        "raw_url": "https://gist.githubusercontent.com/sayurin/9340629/raw/2a4344b28162b708b8662fdcfad4e226c04c35da/TextBox2.fs",
        "size": 1002
      }
    },
    "public": true,
    "created_at": "2014-03-04T05:12:30Z",
    "updated_at": "2014-03-04T05:14:00Z",
    "description": "WinFormsのTextBoxでfocusを失う時にIMEへの入力文字を確定させる",
    "comments": 0,
    "user": null,
    "comments_url": "https://api.github.com/gists/9340629/comments",
    "owner": {
      "login": "sayurin",
      "id": 3615855,
      "avatar_url": "https://gravatar.com/avatar/3710daacf055efda12fd5553e86d38b6?d=https%3A%2F%2Fidenticons.github.com%2Fb09a6f29ca8d6a065a488db155ff06d2.png&r=x",
      "gravatar_id": "3710daacf055efda12fd5553e86d38b6",
      "url": "https://api.github.com/users/sayurin",
      "html_url": "https://github.com/sayurin",
      "followers_url": "https://api.github.com/users/sayurin/followers",
      "following_url": "https://api.github.com/users/sayurin/following{/other_user}",
      "gists_url": "https://api.github.com/users/sayurin/gists{/gist_id}",
      "starred_url": "https://api.github.com/users/sayurin/starred{/owner}{/repo}",
      "subscriptions_url": "https://api.github.com/users/sayurin/subscriptions",
      "organizations_url": "https://api.github.com/users/sayurin/orgs",
      "repos_url": "https://api.github.com/users/sayurin/repos",
      "events_url": "https://api.github.com/users/sayurin/events{/privacy}",
      "received_events_url": "https://api.github.com/users/sayurin/received_events",
      "type": "User",
      "site_admin": false
    }
  }]"""

    /// F# gist with empty description
    let emptyDescription = """[{
  "url": "https://api.github.com/gists/9365834",
  "forks_url": "https://api.github.com/gists/9365834/forks",
  "commits_url": "https://api.github.com/gists/9365834/commits",
  "id": "9365834",
  "git_pull_url": "https://gist.github.com/9365834.git",
  "git_push_url": "https://gist.github.com/9365834.git",
  "html_url": "https://gist.github.com/9365834",
  "files": {
    "batch.fs": {
      "filename": "batch.fs",
      "type": "text/plain",
      "language": "F#",
      "raw_url": "https://gist.githubusercontent.com/mschinz/9365834/raw/76b80de0dc4bc69e1a48c51a6e0666edf5e4afa9/batch.fs",
      "size": 478
    }
  },
  "public": true,
  "created_at": "2014-03-05T11:54:42Z",
  "updated_at": "2014-03-05T11:54:43Z",
  "description": "",
  "comments": 0,
  "user": null,
  "comments_url": "https://api.github.com/gists/9365834/comments",
  "owner": {
    "login": "mschinz",
    "id": 890978,
    "avatar_url": "https://gravatar.com/avatar/da12f051730ce0461aba2502460dbfd7?d=https%3A%2F%2Fidenticons.github.com%2F8b4f8bbf34e3cfc1fbab4f8f88e7482a.png&r=x",
    "gravatar_id": "da12f051730ce0461aba2502460dbfd7",
    "url": "https://api.github.com/users/mschinz",
    "html_url": "https://github.com/mschinz",
    "followers_url": "https://api.github.com/users/mschinz/followers",
    "following_url": "https://api.github.com/users/mschinz/following{/other_user}",
    "gists_url": "https://api.github.com/users/mschinz/gists{/gist_id}",
    "starred_url": "https://api.github.com/users/mschinz/starred{/owner}{/repo}",
    "subscriptions_url": "https://api.github.com/users/mschinz/subscriptions",
    "organizations_url": "https://api.github.com/users/mschinz/orgs",
    "repos_url": "https://api.github.com/users/mschinz/repos",
    "events_url": "https://api.github.com/users/mschinz/events{/privacy}",
    "received_events_url": "https://api.github.com/users/mschinz/received_events",
    "type": "User",
    "site_admin": false
  }
}]"""

module Repositories =
    let json = """{
  "total_count": 1854,
  "items": [
    {
      "id": 1162709,
      "name": "fsharp",
      "full_name": "fsharp/fsharp",
      "owner": {
        "login": "fsharp",
        "id": 485415,
        "avatar_url": "https://gravatar.com/avatar/db0bc7081238324591d7f0d64373f13d?d=https%3A%2F%2Fidenticons.github.com%2Fc7b672773be8d873eb5a41cb4b1f6691.png&r=x",
        "gravatar_id": "db0bc7081238324591d7f0d64373f13d",
        "url": "https://api.github.com/users/fsharp",
        "html_url": "https://github.com/fsharp",
        "followers_url": "https://api.github.com/users/fsharp/followers",
        "following_url": "https://api.github.com/users/fsharp/following{/other_user}",
        "gists_url": "https://api.github.com/users/fsharp/gists{/gist_id}",
        "starred_url": "https://api.github.com/users/fsharp/starred{/owner}{/repo}",
        "subscriptions_url": "https://api.github.com/users/fsharp/subscriptions",
        "organizations_url": "https://api.github.com/users/fsharp/orgs",
        "repos_url": "https://api.github.com/users/fsharp/repos",
        "events_url": "https://api.github.com/users/fsharp/events{/privacy}",
        "received_events_url": "https://api.github.com/users/fsharp/received_events",
        "type": "Organization",
        "site_admin": false
      },
      "private": false,
      "html_url": "https://github.com/fsharp/fsharp",
      "description": "The F# compiler and core library (open edition)",
      "fork": false,
      "url": "https://api.github.com/repos/fsharp/fsharp",
      "forks_url": "https://api.github.com/repos/fsharp/fsharp/forks",
      "keys_url": "https://api.github.com/repos/fsharp/fsharp/keys{/key_id}",
      "collaborators_url": "https://api.github.com/repos/fsharp/fsharp/collaborators{/collaborator}",
      "teams_url": "https://api.github.com/repos/fsharp/fsharp/teams",
      "hooks_url": "https://api.github.com/repos/fsharp/fsharp/hooks",
      "issue_events_url": "https://api.github.com/repos/fsharp/fsharp/issues/events{/number}",
      "events_url": "https://api.github.com/repos/fsharp/fsharp/events",
      "assignees_url": "https://api.github.com/repos/fsharp/fsharp/assignees{/user}",
      "branches_url": "https://api.github.com/repos/fsharp/fsharp/branches{/branch}",
      "tags_url": "https://api.github.com/repos/fsharp/fsharp/tags",
      "blobs_url": "https://api.github.com/repos/fsharp/fsharp/git/blobs{/sha}",
      "git_tags_url": "https://api.github.com/repos/fsharp/fsharp/git/tags{/sha}",
      "git_refs_url": "https://api.github.com/repos/fsharp/fsharp/git/refs{/sha}",
      "trees_url": "https://api.github.com/repos/fsharp/fsharp/git/trees{/sha}",
      "statuses_url": "https://api.github.com/repos/fsharp/fsharp/statuses/{sha}",
      "languages_url": "https://api.github.com/repos/fsharp/fsharp/languages",
      "stargazers_url": "https://api.github.com/repos/fsharp/fsharp/stargazers",
      "contributors_url": "https://api.github.com/repos/fsharp/fsharp/contributors",
      "subscribers_url": "https://api.github.com/repos/fsharp/fsharp/subscribers",
      "subscription_url": "https://api.github.com/repos/fsharp/fsharp/subscription",
      "commits_url": "https://api.github.com/repos/fsharp/fsharp/commits{/sha}",
      "git_commits_url": "https://api.github.com/repos/fsharp/fsharp/git/commits{/sha}",
      "comments_url": "https://api.github.com/repos/fsharp/fsharp/comments{/number}",
      "issue_comment_url": "https://api.github.com/repos/fsharp/fsharp/issues/comments/{number}",
      "contents_url": "https://api.github.com/repos/fsharp/fsharp/contents/{+path}",
      "compare_url": "https://api.github.com/repos/fsharp/fsharp/compare/{base}...{head}",
      "merges_url": "https://api.github.com/repos/fsharp/fsharp/merges",
      "archive_url": "https://api.github.com/repos/fsharp/fsharp/{archive_format}{/ref}",
      "downloads_url": "https://api.github.com/repos/fsharp/fsharp/downloads",
      "issues_url": "https://api.github.com/repos/fsharp/fsharp/issues{/number}",
      "pulls_url": "https://api.github.com/repos/fsharp/fsharp/pulls{/number}",
      "milestones_url": "https://api.github.com/repos/fsharp/fsharp/milestones{/number}",
      "notifications_url": "https://api.github.com/repos/fsharp/fsharp/notifications{?since,all,participating}",
      "labels_url": "https://api.github.com/repos/fsharp/fsharp/labels{/name}",
      "releases_url": "https://api.github.com/repos/fsharp/fsharp/releases{/id}",
      "created_at": "2010-12-13T00:19:52Z",
      "updated_at": "2014-03-03T18:32:20Z",
      "pushed_at": "2014-03-03T18:32:18Z",
      "git_url": "git://github.com/fsharp/fsharp.git",
      "ssh_url": "git@github.com:fsharp/fsharp.git",
      "clone_url": "https://github.com/fsharp/fsharp.git",
      "svn_url": "https://github.com/fsharp/fsharp",
      "homepage": "http://fsharp.github.io/fsharp",
      "size": 66370,
      "stargazers_count": 494,
      "watchers_count": 494,
      "language": "F#",
      "has_issues": true,
      "has_downloads": true,
      "has_wiki": false,
      "forks_count": 126,
      "mirror_url": null,
      "open_issues_count": 37,
      "forks": 126,
      "open_issues": 37,
      "watchers": 494,
      "default_branch": "master",
      "master_branch": "master",
      "score": 1.0
    }
  ]
}"""

    let repo = { Repository.Id = 1162709
                 Name = "fsharp"
                 Description = Some "The F# compiler and core library (open edition)"
                 Owner = "fsharp"
                 Url = "https://github.com/fsharp/fsharp"
                 CreationDate = DateTime(2010, 12, 13, 0, 19, 52, DateTimeKind.Utc) }
    let activity = Repository repo

module Groups =
    let xml = """<feed xmlns="http://www.w3.org/2005/Atom">
    <id>https://groups.google.com/d/forum/fsharp-opensource</id>
    <title type="text">F# Discussions</title>
    <subtitle>Discussion group for the F# community (&lt;a href=&quot;http://fsharp.org&quot;&gt;http://fsharp.org&lt;/a&gt;). Join in!</subtitle>
    <link href="https://groups.google.com/forum/feed/fsharp-opensource/topics/atom_v1_0.xml" rel="self" title="fsharp-opensource feed"/>
    <updated/>
    <generator>Google Groups</generator>
    <entry>
        <author>
            <name>Can Erten</name>
        </author>
        <updated>2014-03-21T09:20:47Z</updated>
        <id>https://groups.google.com/d/topic/fsharp-opensource/tLgTo5hfiTo</id>
        <link href="https://groups.google.com/d/topic/fsharp-opensource/tLgTo5hfiTo"/>
        <title type="text">Xamarin Error on Signed Assembly</title>
        <summary type="html">Hi all, I developed and built my application using F# and  C# successfully using Xamarin Mac. It was all fine and finished development, until I switched to App Store compilation. The compilation goes fine, and signs all the assemblies. However when I run the application, the point where it reaches m</summary>
    </entry>
</feed>"""

    let topic = { Id = "tLgTo5hfiTo"
                  Title = "Xamarin Error on Signed Assembly"
                  Starter = "Can Erten"
                  CreationDate = DateTime(2014, 3, 21, 9, 20, 47, DateTimeKind.Utc)
                  Url = "https://groups.google.com/d/topic/fsharp-opensource/tLgTo5hfiTo" }
    let activity = GroupTopic topic
