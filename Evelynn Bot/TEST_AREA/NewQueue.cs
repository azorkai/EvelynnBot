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

namespace Evelynn_Bot
{
    public class NewQueue
    {
        private static event MessageHandlerDelegate<UxState> UxStateChanged;
        private static event MessageHandlerDelegate<GameFlow> StateChanged;
        private static event MessageHandlerDelegate<Search> OnSearchStateChanged;
        private static Interface itsInterface2;

        private static readonly TaskCompletionSource<bool> _work = new TaskCompletionSource<bool>(false);
        public async Task<Task> Test(Interface itsInterface)
        {
            itsInterface2 = itsInterface;
            Connect();
            return Task.CompletedTask;
        }

        private static bool isDone = false;

        private async Task Connect()
        {
            isDone = false;
            EventExampleAsync();
            Console.WriteLine("Creating Lobby!");
            CreateLobby();
            //while (!isDone)
            //{
            //    //ignored
            //}
            // finished
        }

        async void CreateLobby()
        {
            try
            {
                await Task.Delay(1000);
                itsInterface2.lcuPlugins.CreateLobbyAsync(new LolLobbyLobbyChangeGameDto {queueId = 830}); 
                //Console.WriteLine(json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }



        async static void PickChampion()
        {
            try
            {
                //Get the Pickable Champions.
                LolChampSelectChampSelectAction champSelectInfos = new LolChampSelectChampSelectAction();


                int[] pickableChampions = await itsInterface2.lcuPlugins.GetPickableChampions();

                List<int> champList = pickableChampions != null ? ((IEnumerable<int>)pickableChampions).ToList<int>() : (List<int>)null;
                List<int> champList2 = new List<int>();

                int champion = 0;

                for (int i1 = 0; i1 < champList.Count; ++i1)
                {
                    for (int i2 = 0; i2 < itsInterface2.championDatas.ADCChampions.Count; ++i2)
                    {
                        if (champList.Contains(itsInterface2.championDatas.ADCChampions[i2]))
                            champList2.Add(itsInterface2.championDatas.ADCChampions[i2]);
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
                            itsInterface2.lcuPlugins.SelectChampionAsync(champSelectInfos, k);
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

                int currentChampion = await itsInterface2.lcuPlugins.GetCurrentChampion();
                if (currentChampion == 0)
                {
                    PickChampion();
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

            itsInterface2.lcuApi.Socket.Subscribe("/lol-lobby/v2/lobby/matchmaking/search-state", OnSearchStateChanged);
            itsInterface2.lcuApi.Socket.Subscribe("/riotclient/ux-state/request", UxStateChanged);
            itsInterface2.lcuApi.Socket.Subscribe("/lol-gameflow/v1/session", StateChanged);

            // Wait until work is complete.
            await _work.Task;
            Console.WriteLine("Done.");
        }

        private static async void OnSearchChanged(EventType sender, Search result)
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

                Console.ForegroundColor = ConsoleColor.Red;
                //Console.WriteLine($"Search State: {state}.");
            }
            catch
            {
                // ignored
            }
        }


        private static async void OnUxStateChanged(EventType sender, UxState result)
        {
            var state = string.Empty;

            switch (result.State)
            {
                case "ShowMain":
                    state = "show";
                    await Task.Delay(4000);
                    //api.RequestHandler.ChangeSettings(port, auth);
                    //Console.WriteLine("Port Set! " + port + " " + auth);
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

            Console.ForegroundColor = ConsoleColor.Red;
            //Console.WriteLine($"UX update: {state}.");
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
                    itsInterface2.lcuPlugins.PostMatchmakingSearch();
                    break;
                case "ChampSelect":
                    state = "champ select";
                    await Task.Delay(3800);
                    PickChampion();
                    break;
                case "GameStart":
                    state = "game started";
                    break;
                case "ReadyCheck":
                    state = "Match Found";
                    itsInterface2.lcuPlugins.AcceptReadyCheck();
                    break;
                case "InProgress":
                    state = "game";
                    await itsInterface2.processManager.GameAi(itsInterface2);
                    isDone = true;
                    break;
                case "WaitingForStats":
                    state = "waiting for stats";
                    itsInterface2.lcuPlugins.KillUXAsync();
                    break;
                case "Matchmaking":
                    state = "Matchmaking";
                    break;
                case "PreEndOfGame":
                    state = "Honor Screen";
                    itsInterface2.lcuPlugins.KillUXAsync();
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
