using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Evelynn_Bot.League_API.GameData
{
    public class Lobby
    {
        public string gameMode
        {
            get
            {
                return this.string_0;
            }
            set
            {
                this.string_0 = value;
            }
        }

        public int queueId
        {
            get
            {
                return this.int_0;
            }
            set
            {
                this.int_0 = value;
            }
        }

        private string string_0;

        private int int_0;
    }

    public partial class LobbyHelper
    {
        [JsonProperty("eligible")]
        public bool Eligible { get; set; }

        [JsonProperty("queueId")]
        public long QueueId { get; set; }

        [JsonProperty("restrictions")]
        public Restriction[] Restrictions { get; set; }
    }

    public partial class Restriction
    {
        [JsonProperty("expiredTimestamp")]
        public long ExpiredTimestamp { get; set; }

        [JsonProperty("restrictionArgs")]
        public RestrictionArgs RestrictionArgs { get; set; }

        [JsonProperty("restrictionCode")]
        public RestrictionCode RestrictionCode { get; set; }

        [JsonProperty("summonerIds")]
        public long[] SummonerIds { get; set; }

        [JsonProperty("summonerIdsString")]
        public string SummonerIdsString { get; set; }
    }

    public partial class RestrictionArgs
    {
        [JsonProperty("teamSizeRestriction", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? TeamSizeRestriction { get; set; }
    }

    public enum RestrictionCode { PlayerBannedRestriction, QueueDisabled, TeamSizeRestriction };

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                RestrictionCodeConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }

    internal class RestrictionCodeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(RestrictionCode) || t == typeof(RestrictionCode?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "PlayerBannedRestriction":
                    return RestrictionCode.PlayerBannedRestriction;
                case "QueueDisabled":
                    return RestrictionCode.QueueDisabled;
                case "TeamSizeRestriction":
                    return RestrictionCode.TeamSizeRestriction;
            }
            throw new Exception("Cannot unmarshal type RestrictionCode");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (RestrictionCode)untypedValue;
            switch (value)
            {
                case RestrictionCode.PlayerBannedRestriction:
                    serializer.Serialize(writer, "PlayerBannedRestriction");
                    return;
                case RestrictionCode.QueueDisabled:
                    serializer.Serialize(writer, "QueueDisabled");
                    return;
                case RestrictionCode.TeamSizeRestriction:
                    serializer.Serialize(writer, "TeamSizeRestriction");
                    return;
            }
            throw new Exception("Cannot marshal type RestrictionCode");
        }

        public static readonly RestrictionCodeConverter Singleton = new RestrictionCodeConverter();
    }
}
