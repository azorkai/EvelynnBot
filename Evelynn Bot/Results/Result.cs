using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evelynn_Bot.ExternalCommands;

namespace Evelynn_Bot.Results
{
    public class Result : IResult
    {
        public Result(bool succes, string message, int x, int y):this(succes)
        {
            Message = message;
            X = x;
            Y = y;
            Logger.Log(succes, message);
        }
        public Result(bool succes, string message) : this(succes)
        {
            Message = message;
            Logger.Log(succes, message);
        }
        public Result(bool success)
        {
            Success = success;
        }
        public bool Success { get; }
        public string Message { get; }
        public int X { get; }
        public int Y { get; }
    }
}
