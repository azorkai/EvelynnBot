using Evelynn_Bot.Constants;

namespace Evelynn_Bot.ProcessManager
{
    public interface IProcessManager
    {
        bool Start(Interface itsInterface);
        bool StartAccountProcess(Interface itsInterface);
        bool IsGameStarted(Interface itsInterface);
        bool CheckInGame(Interface itsInterface);
        bool winExist(string win, Interface itsInterface);
        bool GameAi(Interface itsInterface);
        bool GameAi2(Interface itsInterface);
        bool PlayAgain(Interface itsInterface);
    }
}