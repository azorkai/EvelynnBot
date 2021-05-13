using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using Newtonsoft.Json;

namespace Evelynn_Bot.ExternalCommands
{
    public class JsonRead:Config
    {

        private string JsonPath = AppDomain.CurrentDomain.BaseDirectory + "config.json";

        public string Id()
        {
            using (StreamReader r = new StreamReader(JsonPath))
            {
                string json = r.ReadToEnd();
                Config myDeserializedClass = JsonConvert.DeserializeObject<Config>(json);
                return myDeserializedClass.id;
            }
        }

        public string Password()
        {
            using (StreamReader r = new StreamReader(JsonPath))
            {
                string json = r.ReadToEnd();
                Config myDeserializedClass = JsonConvert.DeserializeObject<Config>(json);
                return myDeserializedClass.pass;
            }
        }

        public string Location()
        {
            using (StreamReader r = new StreamReader(JsonPath))
            {
                string json = r.ReadToEnd();
                Config myDeserializedClass = JsonConvert.DeserializeObject<Config>(json);
                return myDeserializedClass.location;
            }
        }
    }

    public class Config
    {
        public string id { get; set; }
        public string pass { get; set; }
        public string location { get; set; }
    }
}
