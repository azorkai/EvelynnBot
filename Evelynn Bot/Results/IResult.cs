using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evelynn_Bot.Results
{
    public interface IResult
    {
        bool Success { get; }
        string Message { get; }
        int X { get; }
        int Y { get; }
    }
}
