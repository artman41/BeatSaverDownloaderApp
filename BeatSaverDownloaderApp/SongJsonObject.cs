// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using QuickType;
//
//    var songJsonObject = SongJsonObject.FromJson(jsonString);

namespace BeatSaverDownloader
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class SongJsonObject
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("beatname")]
        public string Beatname { get; set; }

        [JsonProperty("ownerid")]
        public string Ownerid { get; set; }

        [JsonProperty("downloads")]
        public string Downloads { get; set; }

        [JsonProperty("upvotes")]
        public string Upvotes { get; set; }

        [JsonProperty("plays")]
        public string Plays { get; set; }

        [JsonProperty("beattext")]
        public string Beattext { get; set; }

        [JsonProperty("uploadtime")]
        public string Uploadtime { get; set; }

        [JsonProperty("songName")]
        public string SongName { get; set; }

        [JsonProperty("songSubName")]
        public string SongSubName { get; set; }

        [JsonProperty("authorName")]
        public string AuthorName { get; set; }

        [JsonProperty("beatsPerMinute")]
        public string BeatsPerMinute { get; set; }

        [JsonProperty("difficultyLevels")]
        public DifficultyLevel[] DifficultyLevels { get; set; }

        [JsonProperty("img")]
        public string Img { get; set; }
    }

    public partial class DifficultyLevel
    {
        [JsonProperty("difficulty")]
        public string Difficulty { get; set; }

        [JsonProperty("difficultyRank")]
        public long DifficultyRank { get; set; }

        [JsonProperty("audioPath")]
        public string AudioPath { get; set; }

        [JsonProperty("jsonPath")]
        public string JsonPath { get; set; }

        [JsonProperty("offset")]
        public long Offset { get; set; }

        [JsonProperty("oldOffset")]
        public long OldOffset { get; set; }

        [JsonProperty("stats")]
        public Stats Stats { get; set; }
    }

    public partial class Stats
    {
        [JsonProperty("time")]
        public double Time { get; set; }

        //[JsonProperty("slashstat")]
        //public Dictionary<string, long> Slashstat { get; set; }

        [JsonProperty("events")]
        public long Events { get; set; }

        [JsonProperty("notes")]
        public long Notes { get; set; }

        [JsonProperty("obstacles")]
        public long Obstacles { get; set; }
    }

    public partial class SongJsonObject
    {
        public static SongJsonObject FromJson(string json) => JsonConvert.DeserializeObject<SongJsonObject>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this SongJsonObject self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
