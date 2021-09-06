using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Evelynn_Bot.Account_Process;
using Evelynn_Bot.Constants;

namespace Evelynn_Bot.ExternalCommands
{
    public class ClientKiller
    {
        public void StartLeague()
        {
            KillLeagueOfLegends();
            KillLeagueClients();
            KillRiotClient();
            Thread.Sleep(5000);
            //LaunchLeague(GetLeaguePath() + "\\..\\Riot Client\\RiotClientServices.exe"); //Start RiotClient
            LaunchLeagueFromLeagueClient($"{GetLeaguePath()}\\LeagueClient.exe"); //Start LeagueClient
        }

        public void LaunchLeagueFromLeagueClient(string path)
        {
            while (Process.GetProcessesByName("LeagueClient").Length == 0)
            {
                try
                {
                    File.Delete(GetLeaguePath() + "system2.yaml");
                    File.Copy("Config/system.yaml", GetLeaguePath() + "system2.yaml");

                    //File.Delete("D:\\Games\\League Of Legends\\Riot Games\\League of Legends\\system2.yaml");
                    //File.Copy("Config/system.yaml", "D:\\Games\\League Of Legends\\Riot Games\\League of Legends\\system2.yaml");
                }
                catch (Exception ex)
                {
                }
                using (StreamWriter text = File.CreateText("del.bat"))
                    text.WriteLine("start \"\" \"" + path + "\" --system-yaml-override=system2.yaml --headless");
                Process.Start(new ProcessStartInfo("del.bat")
                {
                    UseShellExecute = true,
                    Verb = "runas"
                });
                Thread.Sleep(5000);
            }
            File.Delete("del.bat");
        }

        public void LaunchLeagueFromRiotClient(string path)
        {
            while (Process.GetProcessesByName("RiotClientServices").Length == 0)
            {
                using (StreamWriter streamWriter = File.CreateText("del.bat"))
                {
                    streamWriter.WriteLine("start \"\" \"" + path + "\" --launch-product=league_of_legends --launch-patchline=live");
                }
                Process.Start(new ProcessStartInfo("del.bat")
                {
                    UseShellExecute = true,
                    Verb = "runas"
                });
                Thread.Sleep(5000);
            }
            File.Delete("del.bat");
        }

        public string GetLeaguePath()
        {
            DriveInfo[] drivesArray = DriveInfo.GetDrives();
            int counter = 0;
            DriveInfo driveInfo;
            while (true)
            {
                if (counter < drivesArray.Length)
                {
                    driveInfo = drivesArray[counter];
                    if (File.Exists($"{driveInfo.RootDirectory.ToString()}Riot Games\\League of Legends\\LeagueClient.exe"))
                    {
                        break;
                    }
                    counter++;
                    continue;
                }
                Thread.Sleep(5000);
                //Environment.Exit(0);
                return string.Empty;
            }
            return $"{driveInfo.RootDirectory.ToString()}Riot Games\\League of Legends\\";
        }

        public void KillRiotLockFile()
        {
            string riotLockFilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Riot Games\\Riot Client\\Config\\lockfile";
            if (File.Exists(riotLockFilePath)) { File.Delete(riotLockFilePath); }
        }

        public void KillRiotClient()
        {
            Process[] pbN = Process.GetProcessesByName("RiotClientServices");
            foreach (Process p in pbN) { p.Kill(); }
            pbN = Process.GetProcessesByName("RiotClientUx");
            foreach (Process p2 in pbN) { p2.Kill();}
            pbN = Process.GetProcessesByName("RiotClientUxRender");
            foreach (Process p3 in pbN) { p3.Kill(); }
            pbN = Process.GetProcessesByName("RiotClientCrashHandler");
            foreach (Process p4 in pbN) { p4.Kill(); }
            KillRiotLockFile();
        }

        public void KillAllLeague()
        {
            KillLeagueOfLegends();
            KillLeagueClients();
            KillRiotClient();
        }

        public void DeleteLockFile()
        {
            string lockfilePath = $"{GetLeaguePath()}lockfile";
            //string lockfilePath = $"D:\\Games\\League Of Legends\\Riot Games\\League of Legends\\lockfile";
            if (File.Exists(lockfilePath)) { File.Delete(lockfilePath); }
        }

        public void KillLeagueOfLegends()
        {
            Process[] pBN = Process.GetProcessesByName("League of Legends");
            foreach (Process p in pBN) { p.Kill(); }
        }

        public void KillLeagueClients()
        {
            Process[] pBN = Process.GetProcessesByName("LeagueClient");
            foreach (Process p in pBN) { p.Kill(); }
            pBN = Process.GetProcessesByName("LeagueClientUx");
            foreach (Process p2 in pBN) { p2.Kill(); }
            pBN = Process.GetProcessesByName("LeagueClientUxRender");
            foreach (Process p3 in pBN) { p3.Kill(); }
            pBN = Process.GetProcessesByName("LeagueCrashHandler");
            foreach (Process p4 in pBN) { p4.Kill(); }
            DeleteLockFile();
        }
    }
}