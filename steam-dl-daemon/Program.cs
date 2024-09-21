using System.Diagnostics;
using System.Timers;
using Newtonsoft.Json;
using Timer = System.Timers.Timer;

namespace steam_dl_daemon;

class Program
{
    private static Config? config;
    private static Process p;

    private static bool steamDlRunning = false;
        
    static int Main(string[] args)
    {
        config = Config.Load("steam-dl-daemon.json");

        if(config == null)
        {
            Console.WriteLine("Failed to load config");
            return 1;
        }

        Timer timer = new Timer();
        
        if (config.Hourly)
        {
            timer.Elapsed += new ElapsedEventHandler(RunHourlySequence!);
            timer.Interval = 3600000;
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Start();
        }
        else
        {
            timer.Elapsed += new ElapsedEventHandler(CheckTime!);
            timer.Interval = 1000;
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Start();
        }
        
        while (true)
        {
            Thread.Sleep(1000);
        }
        
        return 0;
    }

    private static void RunHourlySequence(object source, ElapsedEventArgs e)
    {
        if (steamDlRunning)
            return;
        
        if (!File.Exists("GameList.json"))
        {
            FileStream fs = File.Create("GameList.json");
            fs.Close();
        }
        
        Game[]? gameList = LoadGameList("GameList.json");

        if (gameList == null || gameList.Length == 0)
            return;

        steamDlRunning = true;
        
        foreach (Game game in gameList)
        {
            Process p = new Process();
            p.StartInfo.FileName = config.SteamDLApp;
            p.StartInfo.Arguments = string.Format(
                "-u {0} -a {1} --os {2} --arch {3} --language {4} -o {5} --verify",
                game.Username,
                game.AppId,
                game.Platform,
                game.Architecture,
                game.Language,
                game.Output
            );
            
            p.Start();
            p.WaitForExit();
        }

        steamDlRunning = false;
    }

    private static void CheckTime(object source, ElapsedEventArgs e)
    {
        if (e.SignalTime.ToString("HH:mm:ss") == config.EndTime.ToString("HH:mm:ss") && steamDlRunning == true)
        {
            steamDlRunning = false;

            if (p == null)
                return;
            
            p.Kill();
            p.Close();

            Process[] processes = Process.GetProcessesByName("steam-dl");

            if (processes.Length > 0)
            {
                foreach (Process process in processes)
                {
                    process.Kill();
                    process.Close();
                }
            }
        }
        
        if (steamDlRunning)
            return;
        
        if (!File.Exists("GameList.json"))
        {
            FileStream fs = File.Create("GameList.json");
            fs.Close();
        }
        
        Game[]? gameList = LoadGameList("GameList.json");

        if (gameList == null || gameList.Length == 0)
            return;

        p = new Process();
        
        p.StartInfo.FileName = config.SteamDLApp;
        
        if (e.SignalTime.ToString("HH:mm:ss") == config.StartTime.ToString("HH:mm:ss"))
        {
            if (p == null)
                return;
            
            steamDlRunning = true;

            foreach (Game game in gameList)
            {
                p.StartInfo.Arguments = string.Format(
                    "-u {0} -a {1} --os {2} --arch {3} --language {4} -o {5} --verify",
                    game.Username,
                    game.AppId,
                    game.Platform,
                    game.Architecture,
                    game.Language,
                    game.Output
                );
                
                p.Start();
                p.WaitForExit();

                if (steamDlRunning == false)
                    break;
            }
        }
    }
    
    private static Game[]? LoadGameList(string gameList)
    {
        string json = File.ReadAllText(gameList);

        return JsonConvert.DeserializeObject<Game[]>(json);
        
    }
}