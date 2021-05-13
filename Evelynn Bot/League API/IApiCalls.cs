using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evelynn_Bot.League_API.GameData;

namespace Evelynn_Bot.League_API
{
    public interface IApiCalls:IDisposable
    {
        void PatchObject<T>(T obj, string endpoint, string Auth, int Port);
        T GetObject<T>(string endpoint, string Auth, int Port);
        void PutObject<T>(T obj, string endpoint, string Auth, int Port);
        bool PostObject<T>(T obj, string endpoint, string Auth, int Port);
        string GetObject(string endpoint, string Auth, int Port);
        object PostObjectJSON<T>(T obj, string endpoint, string Auth, int Port);
        LiveGameData GetLiveGameData();

    }
}
