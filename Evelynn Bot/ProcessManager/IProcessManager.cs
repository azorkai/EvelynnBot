using System.Threading.Tasks;
using Evelynn_Bot.Constants;

namespace Evelynn_Bot.ProcessManager
{
    public interface IProcessManager
    {
        Task<Task> Start(Interface itsInterface);
        Task<Task> StartAccountProcess(Interface itsInterface, bool isFromGame);
        Task<object> CheckInGame(Interface itsInterface);
        Task<Task> PlayAgain(Interface itsInterface);
    }
}