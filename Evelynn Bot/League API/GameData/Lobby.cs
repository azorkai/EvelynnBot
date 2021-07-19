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
    public struct Lobby
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

    public struct LobbyHelper
    {
        [JsonProperty("eligible")]
        public bool Eligible { get; set; }

        [JsonProperty("queueId")]
        public long QueueId { get; set; }

        [JsonProperty("restrictions")]
        public Restriction[] Restrictions { get; set; }
    }

    public struct Restriction
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

    public struct RestrictionArgs
    {
        [JsonProperty("teamSizeRestriction", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? TeamSizeRestriction { get; set; }
    }

    public enum RestrictionCode { PlayerBannedRestriction, QueueDisabled, TeamSizeRestriction };

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
    }
}
