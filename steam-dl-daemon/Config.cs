using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace steam_dl_daemon;

public class Config
{
    private Config()
    {
    }

    private static JsonSerializerSettings _settings = new JsonSerializerSettings {
        DateFormatString = "HH:mm:ss"
    };
    
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool Hourly { get; set; }
    public string SteamDLApp { get; set; }
    
    public static Config? Load(string configFile)
    {
        if (!File.Exists(configFile))
            CreateConfigFile(configFile);
        

        string configJson = File.ReadAllText(configFile);
        
        return JsonConvert.DeserializeObject<Config>(configJson, _settings);
    }

    private static void CreateConfigFile(string configFile)
    {
        FileStream fs = File.Create(configFile);

        Config c = new Config();

        c.StartTime = new DateTime(0);
        c.EndTime = new DateTime(0);
        c.Hourly = true;
        c.SteamDLApp = "steam-dl";

        string json = JsonConvert.SerializeObject(c, _settings);
        fs.Write(Encoding.UTF8.GetBytes(json));
        fs.Flush();
        fs.Close();
    }
}