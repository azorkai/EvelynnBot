using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Evelynn_Bot.Constants;
using Evelynn_Bot.League_API.GameData;
using Evelynn_Bot.Struct;
using Newtonsoft.Json;
using EvelynnLCU;
using EvelynnLCU.API_Models;
using EvelynnLCU.Utils;

namespace Evelynn_Bot
{
    public class NewQueue
    {
        public System.Timers.Timer bugTimer = new System.Timers.Timer();
        private int BugTime;
        public bool GameAiBool;


        private event MessageHandlerDelegate<UxState> UxStateChanged;
        private event MessageHandlerDelegate<GameFlow> StateChanged;
        private event MessageHandlerDelegate<Search> OnSearchStateChanged;
        public Interface itsInterface2;

        private readonly TaskCompletionSource<bool> _work = new TaskCompletionSource<bool>(false);
        public async Task<Task> Test(Interface itsInterface)
        {
            itsInterface2 = itsInterface;
            await Connect(itsInterface);
            return Task.CompletedTask;
        }

        public bool isDone = false;

        public void SetWorkDone()
        {
            _work.SetResult(true);
        }

        private async Task Connect(Interface itsInterface)
        {
            bugTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            bugTimer.Interval = 60000;
            bugTimer.Enabled = true;
            bugTimer.Stop();


            isDone = false;

            EventExampleAsync();
            CreateLobby(itsInterface);
        }

        public async void CreateLobby(Interface itsInterface)
        {
            try
            {
                await Task.Delay(1000);
                itsInterface2.lcuPlugins.CreateLobbyAsync(new LolLobbyLobbyChangeGameDto {queueId = itsInterface.queueId});
                itsInterface2.logger.Log(true, itsInterface2.messages.SuccessCreateGame);
            }
            catch (Exception e)
            {
                itsInterface2.logger.Log(false,itsInterface2.messages.ErrorCreateGame + e);
            }

        }

        async void PickChampion()
        {
            try
            {
                List<long> pickableChampions = await itsInterface2.lcuPlugins.GetPickableChampions();

                LolChampSelectChampSelectSession champSession = await itsInterface2.lcuPlugins.GetChampSelectSessionAsync();
                long localPlayerCellId = champSession.localPlayerCellId.Value;
                long value2 = champSession.actions[0][(int)localPlayerCellId].id.Value;
                long selectedChampId = -1;
                bool isLocked = false;
                foreach (long championId in pickableChampions.Where((long cId) => itsInterface2.championDatas.ADCChampions.Contains(cId)))
                {
                    itsInterface2.lcuPlugins.SelectChampionAsync(value2, championId, localPlayerCellId);
                    int currentChampion = await itsInterface2.lcuPlugins.GetCurrentChampion();
                    if (currentChampion == 0)
                    {
                        Thread.Sleep(2500);
                        continue;
                    }
                    //Console.WriteLine($"Playing {championId}");
                    selectedChampId = championId;
                    isLocked = true;
                    long summonerId = itsInterface2.summoner.summonerId;
                    var champDetails = await itsInterface2.lcuPlugins.GetChampionDetails(summonerId, currentChampion);
                    itsInterface2.player.CurrentGame_ChampName = champDetails.name;
                    itsInterface2.logger.Log(true, itsInterface2.messages.SuccessChampionPick);
                    break;
                }
                if (!isLocked)
                {
                    foreach (long championId2 in pickableChampions)
                    {
                        itsInterface2.lcuPlugins.SelectChampionAsync(value2, championId2, localPlayerCellId);
                        int currentChampion = await itsInterface2.lcuPlugins.GetCurrentChampion();
                        if (currentChampion == 0)
                        {
                            Thread.Sleep(2500);
                            continue;
                        }
                        //Console.WriteLine($"Locked {championId2}");
                        selectedChampId = championId2;
                        long summonerId = itsInterface2.summoner.summonerId;
                        var champDetails = await itsInterface2.lcuPlugins.GetChampionDetails(summonerId, currentChampion);
                        itsInterface2.player.CurrentGame_ChampName = champDetails.name;
                        itsInterface2.logger.Log(true, itsInterface2.messages.SuccessChampionPick);
                        break;
                    }
                }

                // Set Spells
                itsInterface2.lcuPlugins.SetSpellAsync((itsInterface2.summoner.summonerLevel >= 7) ? 4 : 6, 7);

            }
            catch (Exception e)
            {

            }
        }

        public async Task UxEventAsync()
        {
            UxStateChanged += OnUxStateChanged;
            
            itsInterface2.lcuApi.Socket.Subscribe("/riotclient/ux-state/request", UxStateChanged);

            // Wait until work is complete.
            await _work.Task;
            Console.WriteLine("Done.");
        }


        public async Task EventExampleAsync()
        {
            //UxStateChanged += OnUxStateChanged;
            StateChanged += OnStateChanged;
            OnSearchStateChanged += OnSearchChanged;

            itsInterface2.lcuApi.Socket.Subscribe("/lol-lobby/v2/lobby/matchmaking/search-state", OnSearchStateChanged);
            //itsInterface2.lcuApi.Socket.Subscribe("/riotclient/ux-state/request", UxStateChanged);
            itsInterface2.lcuApi.Socket.Subscribe("/lol-gameflow/v1/session", StateChanged);

            // Wait until work is complete.
            await _work.Task;
            Console.WriteLine("Done.");
        }

        private async void OnSearchChanged(EventType sender, Search result)
        {
            try
            {
                string state;
            
                switch (result.SearchState)
                {
                    case "Error":
                        state = $"{result.Errors[0].ErrorType} | Remaining Time: {result.Errors[0].PenaltyTimeRemaining}";
                        break;
                    default:
                        state = "unknown";
                        for (int i = 0; i < result.Errors.Length; i++)
                        {
                            Console.WriteLine(result.Errors[i]);
                        }
                        break;
                }

            }
            catch
            {
                // ignored
            }
        }


        private async void OnUxStateChanged(EventType sender, UxState result)
        {
            var state = string.Empty;

            switch (result.State)
            {
                case "ShowMain":
                    state = "show";
                    //await Task.Delay(4000);
                    itsInterface2.lcuPlugins.KillUXAsync();
                    break;
                case "Quit":
                    state = "quit";
                    //api.RequestHandler.ChangeSettings(port, auth);
                    //Console.WriteLine("Port Set! " + port + " " + auth);
                    break;
                case "HideAll":
                    state = "hide";
                    break;
                default:
                    state = $"unknown ux state: {result.State}";
                    break;
            }
            //itsInterface2.logger.Log(true, "UX State: " + state);
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Console.WriteLine("Bug Check!");
            BugTime++;
            if (BugTime>=8)
            {
                Console.WriteLine("Bug!Fix!");
                BugTime = 0;
                var licenseBase64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(itsInterface2.license)));
                var exeDir = System.Reflection.Assembly.GetExecutingAssembly().Location;
                Process eBot = new Process();
                eBot.StartInfo.FileName = exeDir;
                eBot.StartInfo.WorkingDirectory = Path.GetDirectoryName(exeDir);
                eBot.StartInfo.Arguments = licenseBase64String;
                eBot.StartInfo.Verb = "runas";
                eBot.Start();
                Environment.Exit(0);
            }
        }

        private async void OnStateChanged(EventType sender, GameFlow result)
        {
            try
            {
                var state = string.Empty;

                switch (result.Phase)
                {
                    case "None":
                        state = "Main Menu";
                        itsInterface2.newQueue.CreateLobby();
                        break;

                    case "Lobby":
                        state = "Lobby";
                        itsInterface2.dashboardHelper.UpdateLolStatus("In Lobby", itsInterface2);
                        var searchState = await itsInterface2.lcuPlugins.GetSearchState();

                        if (searchState.errors.Count <= 0)
                        {
                            if (searchState.lowPriorityData.penaltyTimeRemaining.Value > 0)
                            {
                                double lpqRemaining = searchState.lowPriorityData.penaltyTimeRemaining.Value;
                                if (lpqRemaining > 0.0)
                                {
                                    // TODO: PANELE GÖNDER
                                    itsInterface2.logger.Log(true, $"LPQ Detected - Waiting {lpqRemaining * 1000} seconds.");
                                    Thread.Sleep((int)lpqRemaining * 1000);
                                    return;
                                }
                            }
                        }
                        else
                        {
                            double penaltyRemaining = searchState.errors[0].penaltyTimeRemaining.Value;
                            if (!(penaltyRemaining <= 0.0))
                            {
                                // TODO: PANELE GÖNDER
                                itsInterface2.logger.Log(true, $"Penalty Detected - Waiting {penaltyRemaining * 1000} seconds.");
                                Thread.Sleep((int)penaltyRemaining * 1000);
                                return;
                            }
                        }

                        await Task.Delay(2500);

                        GameAiBool = true;
                        bugTimer.Start();

                        itsInterface2.lcuPlugins.PostMatchmakingSearch();
                        break;
                    case "ChampSelect":
                        state = "Champ Select [Ignore this message if game is started!]";
                        itsInterface2.dashboardHelper.UpdateLolStatus("In Queue", itsInterface2);
                        BugTime = 0;

                        await Task.Delay(1500);
                        itsInterface2.newQueue.PickChampion();
                        break;

                    case "GameStart":
                        state = "Game Started";
                        break;

                    case "ReadyCheck":
                        state = "Match Found";
                        itsInterface2.dashboardHelper.UpdateLolStatus("In Queue", itsInterface2);

                        BugTime = 0;

                        itsInterface2.lcuPlugins.AcceptReadyCheck();
                        break;

                    case "InProgress":
                        state = "Game in Progress";
                        itsInterface2.dashboardHelper.UpdateLolStatus("In Game", itsInterface2);

                        bugTimer.Stop();
                        BugTime = 0;

                        if (GameAiBool)
                        {
                            GameAiBool = false;
                            //await itsInterface2.processManager.GameAi(itsInterface2);
                            itsInterface2.gameAi.YeniAIBaslat(itsInterface2);
                        }

                        break;
                    case "WaitingForStats":
                        state = "Waiting for Stats";
                        itsInterface2.lcuPlugins.KillUXAsync();

                        bugTimer.Start();

                        break;
                    case "Matchmaking":
                        state = "Matchmaking";

                        bugTimer.Stop();
                        BugTime = 0;
                        break;

                    case "PreEndOfGame":
                        state = "Honor Screen";
                        bugTimer.Stop();
                        BugTime = 0;
                        await itsInterface2.processManager.PlayAgain(itsInterface2);
                        break;

                    default:
                        state = $"unknown state: {result.Phase}";
                        break;
                }
                //itsInterface2.logger.Log(true, itsInterface2.messages.InfoQueueStats + " " + state);
            }
            catch (Exception e)
            {
                //Test-ReSubIfBug
                itsInterface2.logger.Log(false, "Error State Checker: " + e);
                EventExampleAsync();
            }
        }
    }

    public struct UxState
    {
        [JsonProperty("requestId", NullValueHandling = NullValueHandling.Ignore)]
        public string RequestId { get; set; }

        [JsonProperty("state", NullValueHandling = NullValueHandling.Ignore)]
        public string State { get; set; }
    }
}
