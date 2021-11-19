using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bAUTH;
using Evelynn_Bot.Entities;
using Evelynn_Bot.ExternalCommands;
using Evelynn_Bot.GameAI;
using Evelynn_Bot.League_API;
using Evelynn_Bot.League_API.GameData;
using Evelynn_Bot.TEST_AREA;
using EvelynnLCU.Plugins.LoL;
using EvelynnLCU;
using Leaf.xNet;
using Wallet = Evelynn_Bot.League_API.GameData.Wallet;

namespace Evelynn_Bot.Constants
{
    public class Interface : IClass
    {
        public NewQueue newQueue = new NewQueue();
        public License license = new License();
        public UpTimer uptime = new UpTimer();
        public Logger logger = new Logger();    
        public ClientKiller clientKiller = new ClientKiller();
        public DashboardHelper dashboardHelper = new DashboardHelper();
        public Messages messages = new Messages();
        public ChampionDatas championDatas = new ChampionDatas();
        public Dashboard dashboard = new Dashboard();
        public GameAi gameAi = new GameAi();
        public LolSummonerSummoner summoner = new LolSummonerSummoner();
        public EvelynnLCU.Wallet wallet = new EvelynnLCU.Wallet();
        public TestAI testAi = new TestAI();
        public Player player = new Player();
        public JsonRead jsonRead = new JsonRead();
        public ProcessManager.ProcessManager processManager = new ProcessManager.ProcessManager();
        public MySelection mySelection = new MySelection();
        public NewLeaguePlayer newLeaguePlayer = new NewLeaguePlayer();
        public ChampionSelectInformation champSelectInfos = new ChampionSelectInformation();
        public bSecurity sec = new bSecurity();
        public bHTTP req = new bHTTP();
        public bUtils u = new bUtils();
        public ProcessController ProcessController = new ProcessController();
        public ApiVariables apiVariables = new ApiVariables();
        public HttpRequest request = new HttpRequest();
        public ImagePaths ImgPaths = new ImagePaths();
        public Matchmaking matchmaking = new Matchmaking();
        public GameflowSession gameflowSession = new GameflowSession();
        public ILeagueClient lcuApi  = LeagueClient.CreateNew();
        public Plugins lcuPlugins;
        public bool isBotStarted = false;
        public int queueId = 830;
        public bool Result(bool succes, string message)
        {
            Message = message;
            if (message != "")
            {
                logger.Log(succes, message);
            }
            return succes;
        }
        public bool Result(bool success)
        {
            Success = success;
            return success;
        }

        public bool Success { get; set; }
        public string Message { get; set; }

    }
}
