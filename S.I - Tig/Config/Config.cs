using Life;
using System.IO;
using UnityEngine;
using UnityEngine.Playables;

namespace SITig
{
    public class Config
    {
        public string BaseTigPosition { get; set; } = "0/0/0";
        public string TextOnStartTig { get; set; }
        public Config(Vector3 pos,string text)
        {
            BaseTigPosition = Kit.Vector3Converter.ConvertToJson(pos);
            TextOnStartTig = text;
        }
        public static void CreateConfig(Config cfg, string path)
        {
            string directoryPathHueTig = path + "HueTig";
            string directoryPathConfig = path + "HueTig/Config";
            string filePathConfig = path + "HueTig/Config/config.json";
            string filePathRight = path + "HueTig/right.txt";
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
                File.Create(filePathConfig);
                Config config = new Config(new Vector3(0.0f,0.0f,0.0f),"Vote Texte");
                string Tojson = Newtonsoft.Json.JsonConvert.SerializeObject(config);
                File.WriteAllText(filePathConfig, Tojson);
            }
            cfg = Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(File.ReadAllText(filePathConfig));
            if (!File.Exists(filePathRight))
            {
                File.Create(filePathRight);
            }
        }
    }
}