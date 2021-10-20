using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Evelynn_Bot.Constants;

namespace Evelynn_Bot.ExternalCommands
{
    public class ProcessController
    {
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

        [DllImport("user32.dll")]
        private static extern Boolean ShowWindow(IntPtr hWnd, Int32 nCmdShow);

        [DllImport("User32.dll")]
        private static extern bool EnableWindow(IntPtr hwnd, bool enabled);

        [DllImport("User32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

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

                ShowWindow(pOpenThread, 0);

                SuspendThread(pOpenThread);

                CloseHandle(pOpenThread);
            }
        }
        private void ResumeProcess(int pid)
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

        public async Task<Task> SuspendLeagueUx(Interface itsInterface)
        {
            try
            {
                Process[] processesRender = Process.GetProcessesByName("LeagueClientUxRender");
                Process[] processesUx = Process.GetProcessesByName("LeagueClientUx");

                HideLeagueProcessSplash();
                HideLeagueProcess();

                foreach (var process in processesRender)
                {
                    SuspendProcess(process.Id);
                }

                foreach (var process in processesUx)
                {
                    SuspendProcess(process.Id);
                }
                return Task.CompletedTask;

            }
            catch (Exception e)
            {

                Console.WriteLine(e);
                return Task.CompletedTask;
            }
        }

        public async Task<Task> SuspendRiotUx(Interface itsInterface)
        {
            try
            {
                Process[] processesRender = Process.GetProcessesByName("RiotClientUxRender");
                Process[] processesUx = Process.GetProcessesByName("RiotClientUx");
                Process[] processesRch = Process.GetProcessesByName("RiotClientCrashHandler");

                //Buglu piç
                //HideRiotClientProcess();

                foreach (var process in processesRch)
                {
                    SuspendProcess(process.Id);
                }

                foreach (var process in processesRender)
                {
                    SuspendProcess(process.Id);
                }

                foreach (var process in processesUx)
                {
                    SuspendProcess(process.Id);
                }
                
                return Task.CompletedTask;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Task.CompletedTask;
            }
            
        }

        private void HideRiotClientProcess()
        {
            IntPtr pOpenThread = FindWindow("RCLIENT", "Riot Client Main");
            if (pOpenThread == IntPtr.Zero)
            {
                return;
            }
            ShowWindow(pOpenThread, 0);
        }

        private void HideLeagueProcessSplash()
        {
            IntPtr pOpenThread = FindWindow("SplashScreenClassName", "");
            if (pOpenThread == IntPtr.Zero)
            {
                return;
            }
            ShowWindow(pOpenThread, 0);
        }

        private void HideLeagueProcess()
        {
            IntPtr pOpenThread = FindWindow("RCLIENT", "League of Legends");
            if (pOpenThread == IntPtr.Zero)
            {
                return;
            }
            ShowWindow(pOpenThread, 0);
        }

    }
}
