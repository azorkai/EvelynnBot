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

        public static void CloseDialogBoxes()
        {
            IntPtr intPtr = IntPtr.Zero;
            while ((intPtr = FindWindowEx(IntPtr.Zero, intPtr, "#32770", null)) != IntPtr.Zero)
            {
                PostMessage(intPtr, 16, IntPtr.Zero, IntPtr.Zero);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string lclassName, string windowTitle);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr PostMessage(IntPtr hwnd, int code, IntPtr param1, IntPtr param2);

        [DllImport("kernel32.dll")]
        public static extern ErrorModes SetErrorMode(ErrorModes uMode);

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


        [Flags]
        public enum ErrorModes : uint
        {
            SYSTEM_DEFAULT = 0U,
            SEM_FAILCRITICALERRORS = 1U,
            SEM_NOALIGNMENTFAULTEXCEPT = 4U,
            SEM_NOGPFAULTERRORBOX = 2U,
            SEM_NOOPENFILEERRORBOX = 32768U
        }
    }
}
