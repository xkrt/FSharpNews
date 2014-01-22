module FSharpNews.Tests.DataPuller.TestData

open FSharpNews.Data
open FSharpNews.Utils

module SOQuestion =
    let json = """{
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

    let activity =
        StackExchangeQuestion { Id = 21007873
                                Site = Stackoverflow
                                Title = "Drawing squares on windows form"
                                UserDisplayName = "user2165793"
                                Url = "http://stackoverflow.com/questions/21007873/drawing-squares-on-windows-form"
                                CreationDate = DateTime.unixToUtcDate 1389219805 }

module ProgQuestion =
    let json = """{
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

    let activity =
        StackExchangeQuestion { Id = 218779
                                Site = Programmers
                                Title = "Is there a way to created nested computation expressions?"
                                UserDisplayName = "Pete"
                                Url = "http://programmers.stackexchange.com/questions/218779/is-there-a-way-to-created-nested-computation-expressions"
                                CreationDate = DateTime.unixToUtcDate 1384788003 }
