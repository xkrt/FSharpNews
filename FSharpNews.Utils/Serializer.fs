module FSharpNews.Utils.Serializer

open Newtonsoft.Json

let toJson v = JsonConvert.SerializeObject(v)
let fromJson<'a> json = JsonConvert.DeserializeObject<'a>(json)
