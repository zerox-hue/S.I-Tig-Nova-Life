using Json;
using Life;
using System.IO;
using UnityEngine;
using UnityEngine.Playables;

namespace SITig
{
    public class Config
    {
        public string BaseTigPosition { get; set; } = Json.Vector3Converters.ConvertToJson(Vector3.zero);
        public string TextOnStartTig { get; set; }
        public static Config CreateConfig(string path)
        {
            string directoryPathHueTig = path + "/S.I - Tig";
            string directoryPathConfig = path + "/S.I - Tig/Config";
            string filePathConfig = path + "/S.I - Tig/Config/S.I - Tig.json";
            string filePathRight = path + "/S.I - Tig/right.txt";
            if (!Directory.Exists(directoryPathHueTig))
            {
                Directory.CreateDirectory(directoryPathHueTig);
            }
            if (!Directory.Exists(directoryPathConfig))
            {
                Directory.CreateDirectory(directoryPathConfig);
            }
            if (!File.Exists(filePathConfig))
            {
                Config config = new Config()
                {
                    TextOnStartTig = "Votre texte",
                    BaseTigPosition = Vector3Converters.ConvertToJson(Vector3.zero),
                };
                string Tojson = Newtonsoft.Json.JsonConvert.SerializeObject(config);
                File.WriteAllText(filePathConfig, Tojson);
            }
            if (!File.Exists(filePathRight))
            {
                File.WriteAllText(filePathRight, "All Right reserved at Spehere Interactive and .zerox-hue");
            }
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(File.ReadAllText(filePathConfig));
        }
    }
}