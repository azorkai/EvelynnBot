using Evelynn_Bot.Constants;

namespace Evelynn_Bot.ProcessManager
{
    public interface IProcessManager
    {
        void Start(Interface itsInterface);
        void StartAccountProcess(Interface itsInterface);
        bool IsGameStarted(Interface itsInterface);
        bool CheckInGame(Interface itsInterface);
        bool winExist(string win, Interface itsInterface);
        void GameAi(Interface itsInterface);
        void GameAi2(Interface itsInterface);
        void PlayAgain(Interface itsInterface);
    }
}