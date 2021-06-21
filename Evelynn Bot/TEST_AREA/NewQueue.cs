using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Evelynn_Bot.Constants;
using Evelynn_Bot.League_API.GameData;
using Evelynn_Bot.Struct;
using Newtonsoft.Json;
using LCU.NET;
using LCU.NET.API_Models;
using LCU.NET.Utils;
using LCU.NET.Plugins;
using Lobby = LCU.NET.Plugins.LoL.Lobby;

namespace Evelynn_Bot
{
    public class NewQueue
    {
        private static ILeagueClient api;
        private static Lobby lobby;
        private static RiotClient riotClient;
        private static LCU.NET.Plugins.LoL.Matchmaking matchmaking;
        private static LCU.NET.Plugins.LoL.ChampSelect champSelect;
        private static event MessageHandlerDelegate<UxState> UxStateChanged;
        private static event MessageHandlerDelegate<GameFlow> StateChanged;
        private static event MessageHandlerDelegate<Search> OnSearchStateChanged;


        private static readonly TaskCompletionSource<bool> _work = new TaskCompletionSource<bool>(false);
        private static Interface itsInterface = new Interface();

        public static bool Test()
        {
            Connect();
            return true;
        }

        private async static Task Connect()
        {
            Console.WriteLine("[TEST]: Connecting...");
            api = LeagueClient.CreateNew();
            api.BeginTryInit(InitializeMethod.Lockfile);
            Console.WriteLine("[TEST]: Connected!");
            lobby = new Lobby(api);
            riotClient = new RiotClient(api);
            matchmaking = new LCU.NET.Plugins.LoL.Matchmaking(api);
            champSelect = new LCU.NET.Plugins.LoL.ChampSelect(api);
            EventExampleAsync();
            Console.WriteLine("Creating Lobby!");
            CreateLobby();
        }

        async static void CreateLobby()
        {
            try
            {
                await Task.Delay(1000);
                await lobby.PostLobbyAsync(new LolLobbyLobbyChangeGameDto {queueId = 830}); 
                //Console.WriteLine(json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        async static void StartQueue()
        {
            try
            {
                await lobby.PostMatchmakingSearch();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        async static void AcceptQueue()
        {
            try
            {
                await matchmaking.PostReadyCheckAccept();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        async static void PickChampion(Interface itsInterface)
        {
            try
            {
                //Get the Pickable Champions.
                LolChampSelectChampSelectAction champSelectInfos = new LolChampSelectChampSelectAction();

           
                int[] pickableChampions = await champSelect.GetPickableChampions();

                List<int> champList = pickableChampions != null ? ((IEnumerable<int>)pickableChampions).ToList<int>() : (List<int>)null;
                List<int> champList2 = new List<int>();

                int champion = 0;

                for (int i1 = 0; i1 < champList.Count; ++i1)
                {
                    for (int i2 = 0; i2 < itsInterface.championDatas.ADCChampions.Count; ++i2)
                    {
                        if (champList.Contains(itsInterface.championDatas.ADCChampions[i2]))
                            champList2.Add(itsInterface.championDatas.ADCChampions[i2]);
                    }
                }
                if (champList2.Count > 0)
                {
                    int index = new Random().Next(0, champList2.Count);
                    champion = champList2[index];
                }

                champSelectInfos.actorCellId = 0;
                champSelectInfos.championId = champion;
                champSelectInfos.completed = true;
                champSelectInfos.id = 0;
                champSelectInfos.type = "pick";

                for (int k = 0; k < 6; k++)
                {
                    for (int l = 0; l < 6; l++)
                    {
                        try
                        {
                            champSelectInfos.actorCellId = k;
                            champSelectInfos.id = l;
                            await champSelect.PatchActionById(champSelectInfos, k);
                            goto IL_KIRMANOKTASI;
                        }
                        catch
                        {
                            goto IL_KIRMANOKTASI;
                        }
                        break;
                    IL_KIRMANOKTASI:;
                    }
                }

                int currentChampion = await champSelect.GetCurrentChampion();
                if (currentChampion == 0)
                {
                    PickChampion(itsInterface);
                }


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        public static async Task EventExampleAsync()
        {
            // Register game flow event.
            UxStateChanged += OnUxStateChanged;
            StateChanged += OnStateChanged;
            OnSearchStateChanged += OnSearchChanged;

            api.Socket.Subscribe("/lol-lobby/v2/lobby/matchmaking/search-state", OnSearchStateChanged);
            api.Socket.Subscribe("/riotclient/ux-state/request", UxStateChanged);
            api.Socket.Subscribe("/lol-gameflow/v1/session", StateChanged);

            // Wait until work is complete.
            await _work.Task;
            Console.WriteLine("Done.");
        }

        private static async void OnSearchChanged(EventType sender, Search result)
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

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Search State: {state}.");
        }


        private static async void OnUxStateChanged(EventType sender, UxState result)
        {
            var state = string.Empty;

            switch (result.State)
            {
                case "ShowMain":
                    state = "show";
                    //api.RequestHandler.ChangeSettings(port, auth);
                    //Console.WriteLine("Port Set! " + port + " " + auth);
                    riotClient.KillUXAsync();
                    Console.WriteLine("UxRender Killed");
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

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"UX update: {state}.");
        }

        private static async void OnStateChanged(EventType sender, GameFlow result)
        {
            var state = string.Empty;

            switch (result.Phase)
            {
                case "None":
                    state = "main menu";
                    break;
                case "Lobby":
                    state = "lobby";
                    await Task.Delay(2500);
                    StartQueue();
                    break;
                case "ChampSelect":
                    state = "champ select";
                    await Task.Delay(3800);
                    riotClient.KillUXAsync();
                    PickChampion(itsInterface);
                    break;
                case "GameStart":
                    state = "game started";
                    break;
                case "ReadyCheck":
                    state = "Match Found";
                    await Task.Delay(2000);
                    riotClient.KillUXAsync();
                    AcceptQueue();
                    break;
                case "InProgress":
                    state = "game";
                    //GameAI loop! :D ez pz
                    break;
                case "WaitingForStats":
                    state = "waiting for stats";
                    break;
                case "Matchmaking":
                    state = "Matchmaking";
                    break;
                default:
                    state = $"unknown state: {result.Phase}";
                    break;
            }

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"Status update: Entered {state}.");
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
