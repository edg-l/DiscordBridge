using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBridge.Util
{
    public class ConfigFile
    {
        // Config variables here:
        public string DiscordBotToken = "TOKENHERE";
        public string MessageFormatTerrariaToDiscord = "{0}: {1}";
        public string MessageFormatDiscordToTerraria = "[Discord] {0}: {1}";
        public int[] Messagecolor = { 0, 102, 204 };
        public ulong DiscordChannelID;
        // End of config variables

        public Microsoft.Xna.Framework.Color GetColor()
        {
            return new Microsoft.Xna.Framework.Color(Messagecolor[0], Messagecolor[1], Messagecolor[2]);
        }

        public static ConfigFile Read(string path)
        {
            if (!File.Exists(path))
            {
                ConfigFile config = new ConfigFile();

                File.WriteAllText(path, JsonConvert.SerializeObject(config, Formatting.Indented));
                return config;
            }
            return JsonConvert.DeserializeObject<ConfigFile>(File.ReadAllText(path));
        }
    }
}
