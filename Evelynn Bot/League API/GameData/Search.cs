using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Evelynn_Bot.Struct
{
    public struct Search
    {
        [JsonProperty("dodgeData", NullValueHandling = NullValueHandling.Ignore)]
        public DodgeData DodgeData { get; set; }

        [JsonProperty("errors", NullValueHandling = NullValueHandling.Ignore)]
        public Error[] Errors { get; set; }

        [JsonProperty("estimatedQueueTime", NullValueHandling = NullValueHandling.Ignore)]
        public long? EstimatedQueueTime { get; set; }

        [JsonProperty("isCurrentlyInQueue", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsCurrentlyInQueue { get; set; }

        [JsonProperty("lobbyId", NullValueHandling = NullValueHandling.Ignore)]
        public string LobbyId { get; set; }

        [JsonProperty("lowPriorityData", NullValueHandling = NullValueHandling.Ignore)]
        public LowPriorityData LowPriorityData { get; set; }

        [JsonProperty("queueId", NullValueHandling = NullValueHandling.Ignore)]
        public long? QueueId { get; set; }

        [JsonProperty("readyCheck", NullValueHandling = NullValueHandling.Ignore)]
        public ReadyCheck ReadyCheck { get; set; }

        [JsonProperty("searchState", NullValueHandling = NullValueHandling.Ignore)]
        public string SearchState { get; set; }

        [JsonProperty("timeInQueue", NullValueHandling = NullValueHandling.Ignore)]
        public long? TimeInQueue { get; set; }
    }

    public struct DodgeData
    {
        [JsonProperty("dodgerId", NullValueHandling = NullValueHandling.Ignore)]
        public long? DodgerId { get; set; }

        [JsonProperty("state", NullValueHandling = NullValueHandling.Ignore)]
        public string State { get; set; }
    }

    public struct Error
    {
        [JsonProperty("errorType", NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorType { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty("penalizedSummonerId", NullValueHandling = NullValueHandling.Ignore)]
        public long? PenalizedSummonerId { get; set; }

        [JsonProperty("penaltyTimeRemaining", NullValueHandling = NullValueHandling.Ignore)]
        public double? PenaltyTimeRemaining { get; set; }
    }

    public struct LowPriorityData
    {
        [JsonProperty("bustedLeaverAccessToken", NullValueHandling = NullValueHandling.Ignore)]
        public string BustedLeaverAccessToken { get; set; }

        [JsonProperty("penalizedSummonerIds", NullValueHandling = NullValueHandling.Ignore)]
        public object[] PenalizedSummonerIds { get; set; }

        [JsonProperty("penaltyTime", NullValueHandling = NullValueHandling.Ignore)]
        public long? PenaltyTime { get; set; }

        [JsonProperty("penaltyTimeRemaining", NullValueHandling = NullValueHandling.Ignore)]
        public long? PenaltyTimeRemaining { get; set; }

        [JsonProperty("reason", NullValueHandling = NullValueHandling.Ignore)]
        public string Reason { get; set; }
    }

    public struct ReadyCheck
    {
        [JsonProperty("declinerIds", NullValueHandling = NullValueHandling.Ignore)]
        public object[] DeclinerIds { get; set; }

        [JsonProperty("dodgeWarning", NullValueHandling = NullValueHandling.Ignore)]
        public string DodgeWarning { get; set; }

        [JsonProperty("playerResponse", NullValueHandling = NullValueHandling.Ignore)]
        public string PlayerResponse { get; set; }

        [JsonProperty("state", NullValueHandling = NullValueHandling.Ignore)]
        public string State { get; set; }

        [JsonProperty("suppressUx", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SuppressUx { get; set; }

        [JsonProperty("timer", NullValueHandling = NullValueHandling.Ignore)]
        public long? Timer { get; set; }
    }
}
