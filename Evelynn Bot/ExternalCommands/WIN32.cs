using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Evelynn_Bot.ExternalCommands
{
    public class WIN32
    {
        private const string LibraryName = "user32";
        #region winuser.h

        [DllImport(LibraryName, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport(LibraryName, ExactSpelling = true)]
        public static extern IntPtr GetTopWindow();

        [DllImport(LibraryName, ExactSpelling = true)]
        public static extern IntPtr GetNextWindow(IntPtr hwnd, uint wCmd);

        [DllImport(LibraryName, ExactSpelling = true)]
        public static extern IntPtr GetWindow(IntPtr hwnd, uint wCmd);

        [DllImport(LibraryName, ExactSpelling = true)]
        public static extern bool AllowSetForegroundWindow(uint dwProcessId);

        [DllImport(LibraryName, ExactSpelling = true)]
        public static extern bool SetForegroundWindow(IntPtr hwnd);

        [DllImport(LibraryName, CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        #endregion

        public static IntPtr intptr_0 = (IntPtr)1;
        public static IntPtr intptr_1 = (IntPtr)0;
        public static uint uint_0 = 1;
        public static uint uint_1 = 4;

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowPos(
            IntPtr intptr_2,
            int int_0,
            int int_1,
            int int_2,
            int int_3,
            int int_4,
            uint uint_2);

        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(ControlDelegate cDelegate, bool tf);

        [DllImport("Kernel32")]
        public static extern bool TerminateProcess(IntPtr ipr, uint ui);

        public delegate bool ControlDelegate(CtrlTypes CtrlType);

        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6,
        }
    }
}
