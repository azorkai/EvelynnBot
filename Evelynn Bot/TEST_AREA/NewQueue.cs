﻿using System;
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

        public bool GameAiBool;
        public bool _playAgain;
        public string state;

        private int _reconnectCount;
        private int BugTime;



        private event MessageHandlerDelegate<UxState> UxStateChanged;
        private event MessageHandlerDelegate<GameFlow> StateChanged;
        private event MessageHandlerDelegate<Search> OnSearchStateChanged;
        public Interface itsInterface2;
        private bool isAlreadyConnected = false;

        private readonly TaskCompletionSource<bool> _work = new TaskCompletionSource<bool>(false);
        public async Task<Task> Test(Interface itsInterface, bool isTuto)
        {
            itsInterface2 = itsInterface;
            await Connect(itsInterface, isTuto);
            return Task.CompletedTask;
        }

        public bool isDone = false;

        public void SetWorkDone()
        {
            _work.SetResult(true);
        }

        public async Task DoTutorials(Interface itsInterface)
        {
            itsInterface2.ProcessController.SuspendLeagueUx();
            await Task.Delay(5000);
            itsInterface2.lcuPlugins.CreateLobbyAsync(new LolLobbyLobbyChangeGameDto { queueId = itsInterface.queueId });
            await Task.Delay(5000);
            itsInterface2.lcuPlugins.PostMatchmakingSearch();
            await Task.Delay(5000);
            itsInterface2.lcuPlugins.AcceptReadyCheck();
            await Task.Delay(5000);
            itsInterface2.logger.Log(true, "Tutorial Game - " + itsInterface.queueId);
            await Task.Delay(10000);
            itsInterface2.gameAi.YeniAIBaslat(itsInterface2);
        }

        private async Task Connect(Interface itsInterface, bool isTuto)
        {
            if (!isTuto)
            {
                bugTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                bugTimer.Interval = 60000;
                bugTimer.Enabled = true;
                bugTimer.Stop();
            }

            isDone = false;

            EventExampleAsync();
            isAlreadyConnected = true;
            CreateLobby(itsInterface);
            itsInterface.newQueue.StartMatchmaking();
        }

        public async void CreateLobby(Interface itsInterface)
        {
            try
            {
                itsInterface2.ProcessController.SuspendLeagueUx();
                await Task.Delay(1000);
                GameAiBool = true;
                itsInterface2.lcuPlugins.CreateLobbyAsync(new LolLobbyLobbyChangeGameDto {queueId = 830});
                itsInterface2.logger.Log(true, itsInterface2.messages.SuccessCreateGame);
            }
            catch (Exception e)
            {
                itsInterface2.logger.Log(false,itsInterface2.messages.ErrorCreateGame + e);
            }
        }
        public static List<T> Shuffle<T>(List<T> list)
        {
            var rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }

        async void PickChampion()
        {
            try
            {
                itsInterface2.ProcessController.SuspendLeagueUx();
                List<long> pickableChampions = await itsInterface2.lcuPlugins.GetPickableChampions();
                pickableChampions = Shuffle<long>(pickableChampions);

                LolChampSelectChampSelectSession champSession = await itsInterface2.lcuPlugins.GetChampSelectSessionAsync();
                long localPlayerCellId = champSession.localPlayerCellId.Value;
                long value2 = champSession.actions[0][(int)localPlayerCellId].id.Value;
                long selectedChampId = -1;
                bool isLocked = false;
                itsInterface2.championDatas.ADCChampions = Shuffle<long>(itsInterface2.championDatas.ADCChampions);
                foreach (long championId in pickableChampions.Where((long cId) => itsInterface2.championDatas.ADCChampions.Contains(cId)))
                {
                    itsInterface2.lcuPlugins.SelectChampionAsync(value2, championId, localPlayerCellId);
                    int currentChampion = await itsInterface2.lcuPlugins.GetCurrentChampion();
                    if (currentChampion == 0)
                    {
                        await Task.Delay(2500);
                        continue;
                    }
                    //Console.WriteLine($"Playing {championId}");
                    selectedChampId = championId;
                    isLocked = true;
                    long summonerId = itsInterface2.summoner.summonerId;
                    var champDetails = await itsInterface2.lcuPlugins.GetChampionDetails(summonerId, currentChampion);
                    itsInterface2.player.CurrentGame_ChampName = champDetails.name;
                    itsInterface2.logger.Log(true, itsInterface2.messages.SuccessChampionPick);
                    itsInterface2.lcuPlugins.KillUXAsync();

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
                            await Task.Delay(2500);
                            continue;
                        }
                        //Console.WriteLine($"Locked {championId2}");
                        selectedChampId = championId2;
                        long summonerId = itsInterface2.summoner.summonerId;
                        var champDetails = await itsInterface2.lcuPlugins.GetChampionDetails(summonerId, currentChampion);
                        itsInterface2.player.CurrentGame_ChampName = champDetails.name;
                        itsInterface2.logger.Log(true, itsInterface2.messages.SuccessChampionPick);
                        itsInterface2.lcuPlugins.KillUXAsync();
                        break;
                    }
                }

                // Set Spells
                itsInterface2.ProcessController.SuspendLeagueUx();
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
            if (isAlreadyConnected == false)
            {
                //UxStateChanged += OnUxStateChanged;
                StateChanged += OnStateChanged;
                OnSearchStateChanged += OnSearchChanged;

                itsInterface2.lcuApi.Socket.Subscribe("/lol-lobby/v2/lobby/matchmaking/search-state", OnSearchStateChanged);
                //itsInterface2.lcuApi.Socket.Subscribe("/riotclient/ux-state/request", UxStateChanged);
                itsInterface2.lcuApi.Socket.Subscribe("/lol-gameflow/v1/session", StateChanged);

                // Wait until work is complete.
                await _work.Task;
                itsInterface2.logger.Log(true, "Done - Game State and Session");
            }
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
                    itsInterface2.ProcessController.SuspendLeagueUx();
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
            if (Process.GetProcessesByName("League of Legends").Length == 1)
            {
                itsInterface2.logger.Log(true, "League Game Found");
                if (GameAiBool)
                {
                    itsInterface2.lcuPlugins.KillUXAsync();
                    itsInterface2.dashboardHelper.UpdateLolStatus("In Game", itsInterface2);
                    bugTimer.Stop();
                    BugTime = 0;
                    GameAiBool = false;
                    _playAgain = true;
                    itsInterface2.gameAi.YeniAIBaslat(itsInterface2);
                }
            }

            if (BugTime>=8)
            {
                Console.WriteLine("Bug!Fix!");
                BugTime = 0;
                RestartEverything();
            }

            BugTime++;
        }

        private void RestartEverything()
        {
            itsInterface2.clientKiller.KillAllLeague();
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

        public async void StartMatchmaking()
        {
            if (Process.GetProcessesByName("LeagueClient").Length == 1)
            {
                itsInterface2.ProcessController.SuspendLeagueUx();
                itsInterface2.lcuPlugins.KillUXAsync();
                itsInterface2.dashboardHelper.UpdateLolStatus("In Lobby", itsInterface2);
                var searchState = await itsInterface2.lcuPlugins.GetSearchState();
                if (searchState.errors.Count <= 0)
                {
                    if (searchState.lowPriorityData.penaltyTimeRemaining.Value > 0)
                    {
                        double lpqRemaining = searchState.lowPriorityData.penaltyTimeRemaining.Value;
                        if (lpqRemaining > 0.0)
                        {
                            itsInterface2.dashboardHelper.UpdateLPQStatus("true", itsInterface2);
                            itsInterface2.logger.Log(true, $"LPQ Detected - Waiting {lpqRemaining * 1000} seconds.");
                            await Task.Delay((int)lpqRemaining * 1000);
                            return;
                        }
                    }
                    else
                    {
                        itsInterface2.dashboardHelper.UpdateLPQStatus("false", itsInterface2);
                    }
                }
                else
                {
                    double penaltyRemaining = searchState.errors[0].penaltyTimeRemaining.Value;
                    if (!(penaltyRemaining <= 0.0))
                    {
                        itsInterface2.dashboardHelper.UpdateLPQStatus("true", itsInterface2);
                        itsInterface2.logger.Log(true, $"Penalty Detected - Waiting {penaltyRemaining * 1000} seconds.");
                        await Task.Delay((int)penaltyRemaining * 1000);
                        return;
                    }
                    else
                    {
                        itsInterface2.dashboardHelper.UpdateLPQStatus("false", itsInterface2);
                    }
                }

                await Task.Delay(2500);
                GameAiBool = true;
                bugTimer.Start();
                itsInterface2.lcuPlugins.PostMatchmakingSearch();

            }
            else
            {
                RestartEverything();
            }
        }

        private async void OnStateChanged(EventType sender, GameFlow result)
        {
            try
            {
                switch (result.Phase)
                {
                    case "None":
                        state = "Main Menu";
                        itsInterface2.lcuPlugins.KillUXAsync();
                        if (Process.GetProcessesByName("LeagueClient").Length == 1)
                        {
                            itsInterface2.ProcessController.SuspendLeagueUx();
                            itsInterface2.newQueue.CreateLobby(itsInterface2);
                            StartMatchmaking();
                        }
                        else
                        {
                            RestartEverything();
                        }
                        break;
                    case "Lobby":
                        state = "Lobby";
                        StartMatchmaking();
                        break;
                    case "ChampSelect":
                        state = "Champ Select [Ignore this message if game is started!]";
                        itsInterface2.dashboardHelper.UpdateLolStatus("In Queue", itsInterface2);
                        itsInterface2.ProcessController.SuspendLeagueUx();
                        BugTime = 0;
                        await Task.Delay(1500);
                        itsInterface2.lcuPlugins.KillUXAsync();
                        itsInterface2.newQueue.PickChampion();
                        itsInterface2.lcuPlugins.KillUXAsync();
                        break;

                    case "GameStart":
                        state = "Game Started";
                        itsInterface2.lcuPlugins.KillUXAsync();
                        itsInterface2.dashboardHelper.UpdateLolStatus("In Game", itsInterface2);

                        bugTimer.Stop();
                        BugTime = 0;

                        if (GameAiBool)
                        {
                            GameAiBool = false;
                            _playAgain = true;
                            itsInterface2.gameAi.YeniAIBaslat(itsInterface2);
                        }

                        break;

                    case "ReadyCheck":
                        state = "Match Found";
                        itsInterface2.dashboardHelper.UpdateLolStatus("In Queue", itsInterface2);
                        itsInterface2.lcuPlugins.KillUXAsync();
                        itsInterface2.ProcessController.SuspendLeagueUx();
                        GameAiBool = true;
                        BugTime = 0;

                        itsInterface2.lcuPlugins.AcceptReadyCheck();
                        break;

                    case "InProgress":
                        state = "Game in Progress";
                        itsInterface2.lcuPlugins.KillUXAsync();
                        
                        break;
                    case "WaitingForStats":
                        state = "Waiting for Stats";
                        itsInterface2.lcuPlugins.KillUXAsync();
                        GameAiBool = true;
                        bugTimer.Start();
                        itsInterface2.gameAi.isGameEnd = true;
                        break;
                    case "Matchmaking":
                        state = "Matchmaking";
                        itsInterface2.lcuPlugins.KillUXAsync();
                        bugTimer.Stop();
                        BugTime = 0;
                        break;

                    case "PreEndOfGame":
                        state = "Honor Screen";
                        itsInterface2.lcuPlugins.KillUXAsync();
                        GameAiBool = true;
                        bugTimer.Stop();
                        BugTime = 0;

                        //Bunu bir kere yapması gerekiyor, iki kere "PreEndOfGame" geldiği vakit üst üste biniyor.
                        if (_playAgain)
                        {
                            _playAgain = false;
                            await itsInterface2.processManager.PlayAgain(itsInterface2);
                        }
                        break;
                    case "EndOfGame":
                        await itsInterface2.lcuPlugins.SkipStatsAsync();
                        itsInterface2.gameAi.isGameEnd = true;
                        break;
                    case "Reconnect":
                        state = "Reconnect";

                        _reconnectCount++;
                        itsInterface2.logger.Log(true, "Reconnect Count: " + _reconnectCount);

                        try
                        {
                            await itsInterface2.lcuPlugins.ReconnectGameAsync();
                        }
                        catch (Exception e)
                        {
                            itsInterface2.logger.Log(false, "Reconnect is not available.");
                        }

                        itsInterface2.logger.Log(false, "Could not load game! Reconnecting...");
                        itsInterface2.lcuPlugins.KillUXAsync();

                        await Task.Delay(25000);

                        if (_reconnectCount !>= 2)
                        {
                            GameAiBool = true;
                        }

                        if (Process.GetProcessesByName("League of Legends").Length == 1)
                        {
                            itsInterface2.logger.Log(true, "League Game Found");
                            if (GameAiBool)
                            {
                                _reconnectCount = 0;
                                itsInterface2.lcuPlugins.KillUXAsync();
                                itsInterface2.dashboardHelper.UpdateLolStatus("In Game", itsInterface2);
                                itsInterface2.logger.Log(true, "Starting Game AI - Reconnect");
                                bugTimer.Stop();
                                BugTime = 0;
                                GameAiBool = false;
                                _playAgain = true;
                                itsInterface2.gameAi.YeniAIBaslat(itsInterface2);
                            }
                        }
                        break;
                    default:
                        state = $"unknown state: {result.Phase}";
                        break;
                }
                itsInterface2.logger.Log(true, itsInterface2.messages.InfoQueueStats + " " + state);
            }
            catch (Exception e)
            {
                //Test-ReSubIfBug
                itsInterface2.logger.Log(false, "Error State Checker: " + e);
                //EventExampleAsync();
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
