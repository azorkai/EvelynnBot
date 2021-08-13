﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoIt;
using Evelynn_Bot.Account_Process;
using Evelynn_Bot.Constants;

namespace Evelynn_Bot.ExternalCommands
{
    public class ClientKiller
    {
        [DllImport("User32.dll")]
        public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
        [DllImport("User32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("User32.dll")]
        public static extern bool EnableWindow(IntPtr hwnd, bool enabled);

        public void KillLeagueClient(Interface itsInterface)
        {
            try
            {
                itsInterface.lcuApi.Socket.Close();
                itsInterface.lcuApi.Close();
                KillLeagueClientNormally(itsInterface);
                Thread.Sleep(3000);
            }
            catch (Exception e)
            {
                Console.WriteLine("Kill Process Hata: " + e);
            }
        }


        public void KillLeagueClientNormally(Interface itsInterface)
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    AutoItX.ProcessSetPriority("LeagueClientUx.exe", 0); // Idle/Low
                    AutoItX.ProcessSetPriority("LeagueClient.exe", 0); // Idle/Low
                    AutoItX.ProcessSetPriority("LeagueClientUxRender.exe", 0); // Idle/Low
                    AutoItX.ProcessSetPriority("LeagueClientUxRender.exe", 0); // Idle/Low
                    AutoItX.ProcessSetPriority("RiotClientServices.exe", 0); // Idle/Low
                    AutoItX.ProcessSetPriority("RiotClientUx.exe", 0); // Idle/Low
                    AutoItX.ProcessSetPriority("RiotClientUxRender.exe", 0); // Idle/Low
                    AutoItX.ProcessSetPriority("RiotClientUxRender.exe", 0); // Idle/Low
                    AutoItX.ProcessClose("LeagueClientUx.exe"); 
                    AutoItX.ProcessClose("LeagueClient.exe");
                    AutoItX.ProcessClose("LeagueClientUxRender.exe");
                    AutoItX.ProcessClose("LeagueClientUxRender.exe");
                    AutoItX.ProcessClose("RiotClientServices.exe");
                    AutoItX.ProcessClose("RiotClientUx.exe");
                    AutoItX.ProcessClose("RiotClientUxRender.exe");
                    AutoItX.ProcessClose("RiotClientUxRender.exe");
                }
                catch
                {
                    //ignored
                }
            }
        }

        [Flags]
        public enum ThreadAccess : int
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);
        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool CloseHandle(IntPtr handle);


        private void SuspendProcess(int pid)
        {
            var process = Process.GetProcessById(pid); // throws exception if process does not exist

            foreach (ProcessThread pT in process.Threads)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                if (pOpenThread == IntPtr.Zero)
                {
                    continue;
                }

                SuspendThread(pOpenThread);

                CloseHandle(pOpenThread);
            }
        }

        public void ResumeProcess(int pid)
        {
            var process = Process.GetProcessById(pid);

            if (process.ProcessName == string.Empty)
                return;

            foreach (ProcessThread pT in process.Threads)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                if (pOpenThread == IntPtr.Zero)
                {
                    continue;
                }

                var suspendCount = 0;
                do
                {
                    suspendCount = ResumeThread(pOpenThread);
                } while (suspendCount > 0);

                CloseHandle(pOpenThread);
            }
        }


    }
}
