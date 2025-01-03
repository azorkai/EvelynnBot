﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Evelynn_Bot.Account_Process;
using Microsoft.Win32;
using Evelynn_Bot.Constants;
using Microsoft.Win32;
using Newtonsoft.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Evelynn_Bot.ExternalCommands
{
    public class ClientKiller
    {

        [DllImport("User32.dll", SetLastError = true)]
        static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        public void StartLeague()
        {
            KillExplorer();
            KillLeagueOfLegends();
            KillLeagueClients();
            KillRiotClient();
            Thread.Sleep(5000);
            LaunchLeagueFromLeagueClient($"{GetLeaguePath()}\\LeagueClient.exe"); //Start LeagueClient
        }

        public void StartRiotClient()
        {
            KillExplorer();
            KillLeagueOfLegends();
            KillLeagueClients();
            KillRiotClient();
            Thread.Sleep(8000);
            LaunchLeagueFromRiotClient(GetLeaguePath() + "\\..\\Riot Client\\RiotClientServices.exe"); //Start RiotClient
        }


        #region Bypass

        public void KillExplorer()
        {
            Process[] processesByName = Process.GetProcessesByName("explorer");
            foreach (Process process in processesByName)
            {
                try {WIN32.TerminateProcess(process.Handle, 1u);}catch{/* ignroed */}
            }
            Thread.Sleep(5000);
        }

        public void KillRiotClientUx()
        {
            Process[] processesByName = Process.GetProcessesByName("RiotClientUx");
            foreach (Process process1 in processesByName) { try { WIN32.TerminateProcess(process1.Handle, 1u); } catch { /* ignored */ } }
        }

        public void KillRiotUx()
        {
            Process[] processesByName = Process.GetProcessesByName("RiotClientUx");
            foreach (Process process1 in processesByName){ try { WIN32.TerminateProcess(process1.Handle, 1u); }catch { /* ignored */ } }
            Process[] pbN2 = Process.GetProcessesByName("RiotClientUxRender");
            foreach (Process process2 in pbN2){ try { WIN32.TerminateProcess(process2.Handle, 1u); }catch { /* ignored */ } }
            Process[] pbN3 = Process.GetProcessesByName("RiotClientCrashHandler");
            foreach (Process process3 in pbN3) { try { WIN32.TerminateProcess(process3.Handle, 1u); }catch { /* ignored */ } }
            Thread.Sleep(5000);
        }

        private async Task<string> DisableAdapter(string adapter)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("netsh", "interface set interface \"" + adapter + "\" disable");
            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
            if (!process.WaitForExit(60000))
            {
                Console.WriteLine("Error happened while disabling network");
                await Task.Delay(5000);
                // Random 400,500 ms sleep atıp uygulamayı kapayıp tekrar açıyor.
                //RestartApp();
            }

            return "ok";
        }

        private async Task<string> EnableAdapter(string adapter)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("netsh", "interface set interface \"" + adapter + "\" enable");
            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
            if (!process.WaitForExit(60000))
            {
                Console.WriteLine("Error happened while enabling network");
                await Task.Delay(5000);
                // Random 400,500 ms sleep atıp uygulamayı kapayıp tekrar açıyor.
                //RestartApp();
            }
            return "ok";
        }

        private async Task<string> EnableAndDisableAdapter()
        {
            try
            {
                // Enable WMI Service
                await RunCommand("sc config winmgmt start= demand");
                await Task.Delay(3000);
                // Start WMI Service
                await RunCommand("net start winmgmt");
                await Task.Delay(3000);
                // Disable Adapter
                await RunCommand("wmic path win32_networkadapter where index=7 call disable");
                await Task.Delay(7000);
                // Enable Adapter
                await RunCommand("wmic path win32_networkadapter where index=7 call enable");
                await Task.Delay(7000);
                // Stop WMI Service
                await RunCommand("net stop winmgmt");
                await Task.Delay(3000);
            }
            catch (Exception e)
            {
                Console.WriteLine($"DISABLE ADAPTER {e}");
            }
            return "ok";
        }


        private async Task<string> RunCommand(string cmd)
        {
            using (StreamWriter text = File.CreateText("del.bat")){
                await text.WriteLineAsync(cmd);
            }
            Process.Start(new ProcessStartInfo("del.bat")
            {
                UseShellExecute = true,
                Verb = "runas"
            });
            await Task.Delay(250);
            File.Delete("del.bat");
            return "ok";
        }

        private async Task<string> GenerateAndChangeMACAddress()
        {
            Random random = new Random();
            byte[] array = new byte[3];
            random.NextBytes(array);
            array[0] = (byte)(array[0] | 2u);
            array[0] = (byte)(array[0] & 0xFEu);

            var randomMac = "00:50:56" + BitConverter.ToString(array).Replace("-", "").ToUpper();
            //var randomMac = "00:0C:29" + BitConverter.ToString(array).Replace("-", "").ToUpper();

            try
            {
                using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Class\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\0007\\", writable: true))
                {
                    if (registryKey.GetValue("DriverDesc").ToString().Trim() == "Intel(R) PRO/1000 MT Network Connection")
                    {
                        string value = randomMac.Replace(":", "");
                        registryKey.SetValue("NetworkAddress", value, RegistryValueKind.String);
                    }
                }
                await DisableAdapter("Local Area Connection");
                await Task.Delay(500);
                await EnableAdapter("Local Area Connection");
                //await EnableAndDisableAdapter();
                await Task.Delay(6000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MAC HATA: {ex}");
            }
            return "ok";
        }

        private async Task<string> DisableUserAssistLogging()
        {
            string[] subKeyNames = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\UserAssist").GetSubKeyNames();
            foreach (string text in subKeyNames)
            {
                try
                {
                    Registry.CurrentUser.DeleteSubKeyTree("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\UserAssist\\" + text + "\\Count");
                }
                catch (Exception)
                {
                }
            }
            Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\UserAssist").SetValue("NoLog", 1, RegistryValueKind.DWord);
            return "ok";
        }

        private async Task<string> ChangeDisplayParameters()
        {
            Random random = new Random();
            byte[] array = new byte[8];
            random.NextBytes(array);
            byte[] array2 = new byte[128]
            {
                0, 255, 255, 255, 255, 255, 255, 0, 54, 105,
                98, 20, 1, 0, 0, 0, 9, 28, 1, 4,
                181, 52, 30, 120, 59, 135, 229, 164, 86, 80,
                158, 38, 13, 80, 84, 175, 207, 0, 129, 128,
                97, 124, 129, 188, 149, 0, 149, 60, 179, 0,
                179, 60, 209, 252, 55, 139, 128, 24, 113, 56,
                45, 64, 88, 44, 69, 0, 9, 37, 33, 0,
                0, 30, 2, 58, 128, 24, 113, 56, 45, 64,
                88, 44, 69, 0, 9, 37, 33, 0, 0, 30,
                0, 0, 0, 253, 0, 48, 144, 150, 150, 3,
                1, 10, 32, 32, 32, 32, 32, 32, 0, 0,
                0, 252, 0, 79, 112, 116, 105, 120, 32, 71,
                50, 52, 67, 10, 32, 32, 1, 139
            };
            array2[8] = array[0];
            array2[9] = array[1];
            array2[10] = array[2];
            array2[11] = array[3];
            array2[12] = array[4];
            array2[13] = array[5];
            array2[14] = array[6];
            array2[15] = array[7];
            using (RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum\\DISPLAY\\"))
            {
                string[] subKeyNames = registryKey2.GetSubKeyNames();
                foreach (string text2 in subKeyNames)
                {
                    using RegistryKey registryKey3 = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum\\DISPLAY\\" + text2 + "\\");
                    string[] subKeyNames2 = registryKey3.GetSubKeyNames();
                    foreach (string text3 in subKeyNames2)
                    {
                        using RegistryKey registryKey4 = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum\\DISPLAY\\" + text2 + "\\" + text3 + "\\Device Parameters\\", writable: true);
                        try
                        {
                            registryKey4.DeleteValue("BAD_EDID");
                            registryKey4.DeleteSubKey("BAD_EDID");
                            registryKey4.DeleteSubKeyTree("BAD_EDID");
                        }
                        catch (Exception)
                        {
                        }
                        registryKey4.CreateSubKey("EDID");
                        registryKey4.SetValue("EDID", array2, RegistryValueKind.Binary);
                    }
                }
            }
            return "ok";
        }

        private async Task<string> DisableWMI()
        {
            // Bypass Step 3 | Restrict Windows Management Service
            using (RegistryKey registryKey5 = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Control\\WMI\\", writable: true))
            {
                registryKey5.CreateSubKey("Restrictions");
                registryKey5.SetValue("Restrictions", 1, RegistryValueKind.DWord);
            }

            // Bypass Step 4 | Disable Windows Management Service
            try
            {
                using ManagementObject managementObject = new ManagementObject(string.Format("Win32_Service.Name=\"{0}\"", "winmgmt"));
                managementObject.InvokeMethod("ChangeStartMode", new object[1] { "Disabled" });
            }
            catch (Exception)
            {
            }

            // Bypass Step 5 | Stop Windows Management Service
            try
            {
                ServiceController serviceController = new ServiceController("winmgmt");
                serviceController.Stop();
                serviceController.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 1, 0));
            }
            catch (Exception)
            {
            }

            // Bypass Step 6 | Kill WMI Provider Host Application
            Process[] processesByName = Process.GetProcessesByName("WmiPrvSE");
            for (int i = 0; i < processesByName.Length; i++)
            {
                processesByName[i].Kill();
            }
            return "ok";
        }

        private async Task<string> ChangeWinGUID()
        {
            Random random2 = new Random();
            DateTime dateTime = new DateTime(2020, 1, 1);
            TimeSpan timeSpan = new TimeSpan(0, random2.Next(0, (int)(DateTime.Now - dateTime).TotalMinutes), 0);
            DateTime dateTime2 = dateTime + timeSpan;
            DateTime dateTime3 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            double num = Math.Floor((dateTime2.ToUniversalTime() - dateTime3).TotalSeconds);
            using (RegistryKey registryKey6 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\", writable: true))
            {
                registryKey6.SetValue("InstallDate", (int)num, RegistryValueKind.DWord);
                registryKey6.SetValue("BuildGUID", Guid.NewGuid().ToString(), RegistryValueKind.String);
            }
            return "ok";
        }

        public async Task<string> ExecuteBypass()
        {

            // Change MAC Address
            await GenerateAndChangeMACAddress();
            // Bypass Step 1
            await DisableUserAssistLogging();
            // Bypass Step 2    
            await ChangeDisplayParameters();
            // Bypass Step 3,4,5,6
            await DisableWMI();
            // Bypass Step 7
            await ChangeWinGUID();
            await Task.Delay(6000);
            return "ok";
        }

        public async Task<string> StartFPSLimiter()
        {

            Process[] pBN = Process.GetProcessesByName("fpslimiter32");
            foreach (Process p in pBN) { try{ WIN32.TerminateProcess(p.Handle, 1u); } catch{ /* ignored */} }
            await Task.Delay(2000);

            while (Process.GetProcessesByName("fpslimiter32").Length == 0)
            {
                string limiterPath = GetLeaguePath();
                string fpsLimiter32 = "fpslimiter32.exe";
                using (StreamWriter streamWriter = File.CreateText("del.bat"))
                {
                    streamWriter.WriteLine("cd " + limiterPath + " && start \"\" \"" + fpsLimiter32 + "\" --minimize");
                }
                Process.Start(new ProcessStartInfo("del.bat")
                {
                    UseShellExecute = true,
                    Verb = "runas"
                });
                await Task.Delay(5000);
            }
            File.Delete("del.bat");
            return "ok";
        }

        #endregion

        public void RestartAndExit(Interface itsInterface)
        {
            var licenseBase64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(itsInterface.license)));
            var exeDir = System.Reflection.Assembly.GetExecutingAssembly().Location;
            Process eBot = new Process();
            eBot.StartInfo.FileName = exeDir;
            eBot.StartInfo.WorkingDirectory = Path.GetDirectoryName(exeDir);
            eBot.StartInfo.Arguments = licenseBase64String;
            eBot.StartInfo.Verb = "runas";
            eBot.Start();
            Environment.Exit(0);
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
                Thread.Sleep(13000);
            }
            File.Delete("del.bat");
        }

        public string GetLeaguePath()
        {
            try
            {
                string settingsPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\\Riot Games\\Metadata\\league_of_legends.live\\league_of_legends.live.product_settings.yaml";
                var deserializer = new YamlDotNet.Serialization.DeserializerBuilder().IgnoreUnmatchedProperties().Build();
                var productSettings = deserializer.Deserialize<LeagueYAML>(File.ReadAllText(settingsPath));
                return $@"{productSettings.LeaguePath}\";
            }
            catch (Exception e)
            {
                return "no_league_found";
            }
        }

        public void KillRiotLockFile()
        {
            try
            {
                string riotLockFilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Riot Games\\Riot Client\\Config\\lockfile";
                if (File.Exists(riotLockFilePath)) { File.Delete(riotLockFilePath); }

            }
            catch (Exception exception)
            {
                //ignored
            }
        }

        public async void KillRiotClient()
        {
            //await RunCommand("taskkill /f /im RiotClientServices.exe");
            //await RunCommand("taskkill /f /im RiotClientUx.exe");
            //await RunCommand("taskkill /f /im RiotClientUxRender.exe");
            //await RunCommand("taskkill /f /im RiotClientUxRender.exe");
            //await RunCommand("taskkill /f /im RiotClientCrashHandler.exe");
            Process[] pbN = Process.GetProcessesByName("RiotClientServices");
            foreach (Process p in pbN) { try { WIN32.TerminateProcess(p.Handle, 1u); }catch{/*ignored*/} }
            pbN = Process.GetProcessesByName("RiotClientUx");
            foreach (Process p2 in pbN) { try{WIN32.TerminateProcess(p2.Handle, 1u);} catch {/*ignored*/} }
            pbN = Process.GetProcessesByName("RiotClientUxRender");
            foreach (Process p3 in pbN) { try{WIN32.TerminateProcess(p3.Handle, 1u);} catch {/*ignored*/} }
            pbN = Process.GetProcessesByName("RiotClientCrashHandler");
            foreach (Process p4 in pbN) { try{WIN32.TerminateProcess(p4.Handle, 1u);} catch {/*ignored*/} }
            KillRiotLockFile();

        }

        public void KillAllLeague()
        {
                try { KillLeagueOfLegends(); }catch (Exception e) { /*ignored*/ }
                try { KillLeagueClients(); }catch (Exception e) { /*ignored*/ }
                try { KillRiotClient(); }catch (Exception e) { /*ignored*/ }
        }

        public void DeleteLockFile()
        {
            try
            {
                string lockfilePath = $"{GetLeaguePath()}lockfile";
                if (File.Exists(lockfilePath)) { File.Delete(lockfilePath); }
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        public void KillLeagueOfLegends()
        {
            Process[] pBN = Process.GetProcessesByName("League of Legends");
            foreach (Process p in pBN) { try{WIN32.TerminateProcess(p.Handle, 1u);} catch {/*ignored*/} }
        }

        public async void KillLeagueClients()
        {
            //await RunCommand("taskkill /f /im LeagueClient.exe");
            //await RunCommand("taskkill /f /im LeagueClientUx.exe");
            //await RunCommand("taskkill /f /im LeagueClientUxRender.exe");
            //await RunCommand("taskkill /f /im LeagueClientUxRender.exe");
            //await RunCommand("taskkill /f /im LeagueCrashHandler.exe");
            Process[] pBN = Process.GetProcessesByName("LeagueClient");
            foreach (Process p in pBN) { try{WIN32.TerminateProcess(p.Handle, 1u);} catch {/*ignored*/} }
            pBN = Process.GetProcessesByName("LeagueClientUx");
            foreach (Process p2 in pBN) { try{WIN32.TerminateProcess(p2.Handle, 1u);} catch {/*ignored*/} }
            pBN = Process.GetProcessesByName("LeagueClientUxRender");
            foreach (Process p3 in pBN) { try{WIN32.TerminateProcess(p3.Handle, 1u);} catch {/*ignored*/} }
            pBN = Process.GetProcessesByName("LeagueCrashHandler");
            foreach (Process p4 in pBN) { try{WIN32.TerminateProcess(p4.Handle, 1u);} catch {/*ignored*/} }
            DeleteLockFile();
        }

        public void ActivateGame()
        {
            try
            {
                int iP;
                Process currentProcess = Process.GetCurrentProcess();
                Process[] proc = Process.GetProcessesByName("League of Legends");

                for (iP = 0; iP < proc.Length; iP++)
                {
                    SwitchToThisWindow(proc[iP].MainWindowHandle, true);
                    WIN32.SetForegroundWindow(proc[iP].MainWindowHandle);

                    if (proc[iP].Id != currentProcess.Id)
                    {
                        // Explain me why the fuck there is only 0 index is foreground!
                    }

                }
            }
            catch (Exception e)
            {

            }
        }
    }

    #region League Settings YAML

    class LeagueYAML
    {
        [YamlMember(Alias = "product_install_full_path", ApplyNamingConventions = false)]
        public string LeaguePath { get; set; }
    }

    #endregion
}