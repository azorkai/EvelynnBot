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
        public static extern bool SetConsoleCtrlHandler(GDelegate0 gdelegate0_0, bool bool_0);

        [DllImport("Kernel32")]
        public static extern bool TerminateProcess(IntPtr intptr_2, uint uint_2);

        public delegate bool GDelegate0(GEnum0 CtrlType);

        public enum GEnum0
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6,
        }
    }
}
