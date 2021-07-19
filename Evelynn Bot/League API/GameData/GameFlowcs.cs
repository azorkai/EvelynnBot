using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Evelynn_Bot.Struct
{
    public partial class GameFlow
    {
        [JsonProperty("gameClient", NullValueHandling = NullValueHandling.Ignore)]
        public GameClient GameClient { get; set; }

        [JsonProperty("gameData", NullValueHandling = NullValueHandling.Ignore)]
        public GameData GameData { get; set; }

        [JsonProperty("gameDodge", NullValueHandling = NullValueHandling.Ignore)]
        public GameDodge GameDodge { get; set; }

        [JsonProperty("map", NullValueHandling = NullValueHandling.Ignore)]
        public Map Map { get; set; }

        [JsonProperty("phase", NullValueHandling = NullValueHandling.Ignore)]
        public string Phase { get; set; }
    }

    public partial class GameClient
    {
        [JsonProperty("observerServerIp", NullValueHandling = NullValueHandling.Ignore)]
        public string ObserverServerIp { get; set; }

        [JsonProperty("observerServerPort", NullValueHandling = NullValueHandling.Ignore)]
        public long? ObserverServerPort { get; set; }

        [JsonProperty("running", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Running { get; set; }

        [JsonProperty("serverIp", NullValueHandling = NullValueHandling.Ignore)]
        public string ServerIp { get; set; }

        [JsonProperty("serverPort", NullValueHandling = NullValueHandling.Ignore)]
        public long? ServerPort { get; set; }

        [JsonProperty("visible", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Visible { get; set; }
    }

    public partial class GameData
    {
        [JsonProperty("gameId", NullValueHandling = NullValueHandling.Ignore)]
        public long? GameId { get; set; }

        [JsonProperty("gameName", NullValueHandling = NullValueHandling.Ignore)]
        public string GameName { get; set; }

        [JsonProperty("isCustomGame", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsCustomGame { get; set; }

        [JsonProperty("password", NullValueHandling = NullValueHandling.Ignore)]
        public string Password { get; set; }

        [JsonProperty("playerChampionSelections", NullValueHandling = NullValueHandling.Ignore)]
        public object[] PlayerChampionSelections { get; set; }

        [JsonProperty("queue", NullValueHandling = NullValueHandling.Ignore)]
        public Queue Queue { get; set; }

        [JsonProperty("spectatorsAllowed", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SpectatorsAllowed { get; set; }

        [JsonProperty("teamOne", NullValueHandling = NullValueHandling.Ignore)]
        public object[] TeamOne { get; set; }

        [JsonProperty("teamTwo", NullValueHandling = NullValueHandling.Ignore)]
        public object[] TeamTwo { get; set; }
    }

    public partial class Queue
    {
        [JsonProperty("allowablePremadeSizes", NullValueHandling = NullValueHandling.Ignore)]
        public long[] AllowablePremadeSizes { get; set; }

        [JsonProperty("areFreeChampionsAllowed", NullValueHandling = NullValueHandling.Ignore)]
        public bool? AreFreeChampionsAllowed { get; set; }

        [JsonProperty("assetMutator", NullValueHandling = NullValueHandling.Ignore)]
        public string AssetMutator { get; set; }

        [JsonProperty("category", NullValueHandling = NullValueHandling.Ignore)]
        public string Category { get; set; }

        [JsonProperty("championsRequiredToPlay", NullValueHandling = NullValueHandling.Ignore)]
        public long? ChampionsRequiredToPlay { get; set; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("detailedDescription", NullValueHandling = NullValueHandling.Ignore)]
        public string DetailedDescription { get; set; }

        [JsonProperty("gameMode", NullValueHandling = NullValueHandling.Ignore)]
        public string GameMode { get; set; }

        [JsonProperty("gameTypeConfig", NullValueHandling = NullValueHandling.Ignore)]
        public GameTypeConfig GameTypeConfig { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        [JsonProperty("isRanked", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsRanked { get; set; }

        [JsonProperty("isTeamBuilderManaged", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsTeamBuilderManaged { get; set; }

        [JsonProperty("isTeamOnly", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsTeamOnly { get; set; }

        [JsonProperty("lastToggledOffTime", NullValueHandling = NullValueHandling.Ignore)]
        public long? LastToggledOffTime { get; set; }

        [JsonProperty("lastToggledOnTime", NullValueHandling = NullValueHandling.Ignore)]
        public long? LastToggledOnTime { get; set; }

        [JsonProperty("mapId", NullValueHandling = NullValueHandling.Ignore)]
        public long? MapId { get; set; }

        [JsonProperty("maxLevel", NullValueHandling = NullValueHandling.Ignore)]
        public long? MaxLevel { get; set; }

        [JsonProperty("maxSummonerLevelForFirstWinOfTheDay", NullValueHandling = NullValueHandling.Ignore)]
        public long? MaxSummonerLevelForFirstWinOfTheDay { get; set; }

        [JsonProperty("maximumParticipantListSize", NullValueHandling = NullValueHandling.Ignore)]
        public long? MaximumParticipantListSize { get; set; }

        [JsonProperty("minLevel", NullValueHandling = NullValueHandling.Ignore)]
        public long? MinLevel { get; set; }

        [JsonProperty("minimumParticipantListSize", NullValueHandling = NullValueHandling.Ignore)]
        public long? MinimumParticipantListSize { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("numPlayersPerTeam", NullValueHandling = NullValueHandling.Ignore)]
        public long? NumPlayersPerTeam { get; set; }

        [JsonProperty("queueAvailability", NullValueHandling = NullValueHandling.Ignore)]
        public string QueueAvailability { get; set; }

        [JsonProperty("queueRewards", NullValueHandling = NullValueHandling.Ignore)]
        public QueueRewards QueueRewards { get; set; }

        [JsonProperty("removalFromGameAllowed", NullValueHandling = NullValueHandling.Ignore)]
        public bool? RemovalFromGameAllowed { get; set; }

        [JsonProperty("removalFromGameDelayMinutes", NullValueHandling = NullValueHandling.Ignore)]
        public long? RemovalFromGameDelayMinutes { get; set; }

        [JsonProperty("shortName", NullValueHandling = NullValueHandling.Ignore)]
        public string ShortName { get; set; }

        [JsonProperty("showPositionSelector", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ShowPositionSelector { get; set; }

        [JsonProperty("spectatorEnabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SpectatorEnabled { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }
    }

    public partial class GameTypeConfig
    {
        [JsonProperty("advancedLearningQuests", NullValueHandling = NullValueHandling.Ignore)]
        public bool? AdvancedLearningQuests { get; set; }

        [JsonProperty("allowTrades", NullValueHandling = NullValueHandling.Ignore)]
        public bool? AllowTrades { get; set; }

        [JsonProperty("banMode", NullValueHandling = NullValueHandling.Ignore)]
        public string BanMode { get; set; }

        [JsonProperty("banTimerDuration", NullValueHandling = NullValueHandling.Ignore)]
        public long? BanTimerDuration { get; set; }

        [JsonProperty("battleBoost", NullValueHandling = NullValueHandling.Ignore)]
        public bool? BattleBoost { get; set; }

        [JsonProperty("crossTeamChampionPool", NullValueHandling = NullValueHandling.Ignore)]
        public bool? CrossTeamChampionPool { get; set; }

        [JsonProperty("deathMatch", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DeathMatch { get; set; }

        [JsonProperty("doNotRemove", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DoNotRemove { get; set; }

        [JsonProperty("duplicatePick", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DuplicatePick { get; set; }

        [JsonProperty("exclusivePick", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ExclusivePick { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        [JsonProperty("learningQuests", NullValueHandling = NullValueHandling.Ignore)]
        public bool? LearningQuests { get; set; }

        [JsonProperty("mainPickTimerDuration", NullValueHandling = NullValueHandling.Ignore)]
        public long? MainPickTimerDuration { get; set; }

        [JsonProperty("maxAllowableBans", NullValueHandling = NullValueHandling.Ignore)]
        public long? MaxAllowableBans { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("onboardCoopBeginner", NullValueHandling = NullValueHandling.Ignore)]
        public bool? OnboardCoopBeginner { get; set; }

        [JsonProperty("pickMode", NullValueHandling = NullValueHandling.Ignore)]
        public string PickMode { get; set; }

        [JsonProperty("postPickTimerDuration", NullValueHandling = NullValueHandling.Ignore)]
        public long? PostPickTimerDuration { get; set; }

        [JsonProperty("reroll", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Reroll { get; set; }

        [JsonProperty("teamChampionPool", NullValueHandling = NullValueHandling.Ignore)]
        public bool? TeamChampionPool { get; set; }
    }

    public partial class QueueRewards
    {
        [JsonProperty("isChampionPointsEnabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsChampionPointsEnabled { get; set; }

        [JsonProperty("isIpEnabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsIpEnabled { get; set; }

        [JsonProperty("isXpEnabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsXpEnabled { get; set; }

        [JsonProperty("partySizeIpRewards", NullValueHandling = NullValueHandling.Ignore)]
        public object[] PartySizeIpRewards { get; set; }
    }

    public partial class GameDodge
    {
        [JsonProperty("dodgeIds", NullValueHandling = NullValueHandling.Ignore)]
        public object[] DodgeIds { get; set; }

        [JsonProperty("phase", NullValueHandling = NullValueHandling.Ignore)]
        public string Phase { get; set; }

        [JsonProperty("state", NullValueHandling = NullValueHandling.Ignore)]
        public string State { get; set; }
    }

    public partial class Map
    {
        [JsonProperty("assets", NullValueHandling = NullValueHandling.Ignore)]
        public Assets Assets { get; set; }

        [JsonProperty("categorizedContentBundles", NullValueHandling = NullValueHandling.Ignore)]
        public CategorizedContentBundles CategorizedContentBundles { get; set; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("gameMode", NullValueHandling = NullValueHandling.Ignore)]
        public string GameMode { get; set; }

        [JsonProperty("gameModeName", NullValueHandling = NullValueHandling.Ignore)]
        public string GameModeName { get; set; }

        [JsonProperty("gameModeShortName", NullValueHandling = NullValueHandling.Ignore)]
        public string GameModeShortName { get; set; }

        [JsonProperty("gameMutator", NullValueHandling = NullValueHandling.Ignore)]
        public string GameMutator { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        [JsonProperty("isRGM", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsRgm { get; set; }

        [JsonProperty("mapStringId", NullValueHandling = NullValueHandling.Ignore)]
        public string MapStringId { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("perPositionDisallowedSummonerSpells", NullValueHandling = NullValueHandling.Ignore)]
        public CategorizedContentBundles PerPositionDisallowedSummonerSpells { get; set; }

        [JsonProperty("perPositionRequiredSummonerSpells", NullValueHandling = NullValueHandling.Ignore)]
        public CategorizedContentBundles PerPositionRequiredSummonerSpells { get; set; }

        [JsonProperty("platformId", NullValueHandling = NullValueHandling.Ignore)]
        public string PlatformId { get; set; }

        [JsonProperty("platformName", NullValueHandling = NullValueHandling.Ignore)]
        public string PlatformName { get; set; }

        [JsonProperty("properties", NullValueHandling = NullValueHandling.Ignore)]
        public Properties Properties { get; set; }
    }

    public partial class Assets
    {
        [JsonProperty("champ-select-background-sound", NullValueHandling = NullValueHandling.Ignore)]
        public string ChampSelectBackgroundSound { get; set; }

        [JsonProperty("champ-select-flyout-background", NullValueHandling = NullValueHandling.Ignore)]
        public string ChampSelectFlyoutBackground { get; set; }

        [JsonProperty("champ-select-planning-intro", NullValueHandling = NullValueHandling.Ignore)]
        public string ChampSelectPlanningIntro { get; set; }

        [JsonProperty("game-select-icon-active", NullValueHandling = NullValueHandling.Ignore)]
        public string GameSelectIconActive { get; set; }

        [JsonProperty("game-select-icon-active-video", NullValueHandling = NullValueHandling.Ignore)]
        public string GameSelectIconActiveVideo { get; set; }

        [JsonProperty("game-select-icon-default", NullValueHandling = NullValueHandling.Ignore)]
        public string GameSelectIconDefault { get; set; }

        [JsonProperty("game-select-icon-disabled", NullValueHandling = NullValueHandling.Ignore)]
        public string GameSelectIconDisabled { get; set; }

        [JsonProperty("game-select-icon-hover", NullValueHandling = NullValueHandling.Ignore)]
        public string GameSelectIconHover { get; set; }

        [JsonProperty("game-select-icon-intro-video", NullValueHandling = NullValueHandling.Ignore)]
        public string GameSelectIconIntroVideo { get; set; }

        [JsonProperty("gameflow-background", NullValueHandling = NullValueHandling.Ignore)]
        public string GameflowBackground { get; set; }

        [JsonProperty("gameselect-button-hover-sound", NullValueHandling = NullValueHandling.Ignore)]
        public string GameselectButtonHoverSound { get; set; }

        [JsonProperty("icon-defeat", NullValueHandling = NullValueHandling.Ignore)]
        public string IconDefeat { get; set; }

        [JsonProperty("icon-defeat-video", NullValueHandling = NullValueHandling.Ignore)]
        public string IconDefeatVideo { get; set; }

        [JsonProperty("icon-empty", NullValueHandling = NullValueHandling.Ignore)]
        public string IconEmpty { get; set; }

        [JsonProperty("icon-hover", NullValueHandling = NullValueHandling.Ignore)]
        public string IconHover { get; set; }

        [JsonProperty("icon-leaver", NullValueHandling = NullValueHandling.Ignore)]
        public string IconLeaver { get; set; }

        [JsonProperty("icon-victory", NullValueHandling = NullValueHandling.Ignore)]
        public string IconVictory { get; set; }

        [JsonProperty("icon-victory-video", NullValueHandling = NullValueHandling.Ignore)]
        public string IconVictoryVideo { get; set; }

        [JsonProperty("map-north", NullValueHandling = NullValueHandling.Ignore)]
        public string MapNorth { get; set; }

        [JsonProperty("map-south", NullValueHandling = NullValueHandling.Ignore)]
        public string MapSouth { get; set; }

        [JsonProperty("music-inqueue-loop-sound", NullValueHandling = NullValueHandling.Ignore)]
        public string MusicInqueueLoopSound { get; set; }

        [JsonProperty("parties-background", NullValueHandling = NullValueHandling.Ignore)]
        public string PartiesBackground { get; set; }

        [JsonProperty("postgame-ambience-loop-sound", NullValueHandling = NullValueHandling.Ignore)]
        public string PostgameAmbienceLoopSound { get; set; }

        [JsonProperty("ready-check-background", NullValueHandling = NullValueHandling.Ignore)]
        public string ReadyCheckBackground { get; set; }

        [JsonProperty("ready-check-background-sound", NullValueHandling = NullValueHandling.Ignore)]
        public string ReadyCheckBackgroundSound { get; set; }

        [JsonProperty("sfx-ambience-pregame-loop-sound", NullValueHandling = NullValueHandling.Ignore)]
        public string SfxAmbiencePregameLoopSound { get; set; }

        [JsonProperty("social-icon-leaver", NullValueHandling = NullValueHandling.Ignore)]
        public string SocialIconLeaver { get; set; }

        [JsonProperty("social-icon-victory", NullValueHandling = NullValueHandling.Ignore)]
        public string SocialIconVictory { get; set; }
    }

    public partial class CategorizedContentBundles
    {
    }

    public partial class Properties
    {
        [JsonProperty("suppressRunesMasteriesPerks", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SuppressRunesMasteriesPerks { get; set; }
    }
}
