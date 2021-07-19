

namespace Evelynn_Bot.Constants
{
    public interface IClass
    {
        bool Success { get; }
        string Message { get; }
        bool Result(bool succes, string message);
        bool Result(bool success);

    }
}