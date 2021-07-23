using System.Threading.Tasks;
using Evelynn_Bot.Constants;

namespace Evelynn_Bot.ProcessManager
{
    public interface IProcessManager
    {
        Task<Task> Start(Interface itsInterface);
        Task<Task> StartAccountProcess(Interface itsInterface, bool isFromGame);
        bool IsGameStarted(Interface itsInterface);
        bool CheckInGame(Interface itsInterface);
        bool winExist(string win, Interface itsInterface);
        Task<Task> GameAi(Interface itsInterface, bool isFromDetect);
        Task<Task> PlayAgain(Interface itsInterface);
    }
}