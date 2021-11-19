using FastBitmapLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Evelynn_Bot.TEST_AREA
{
    public class TestAI
    {

        public static Point GetColorPosition(Color color, Bitmap bitmap)
        {

            int num = color.ToArgb();
            var CurrentCapture = new FastBitmap(bitmap);

            using (FastBitmap currentCapture = CurrentCapture)
            {
                currentCapture.Lock();
                for (int i = 0; i < currentCapture.Height; i++)
                {
                    for (int j = 0; j < currentCapture.Width; j++)
                    {
                        if (num == currentCapture.GetPixelInt(j, i))
                        {
                            return new Point(j,i);
                        }
                    }

                }
            }

            return Point.Empty;
        }

        public static Bitmap GetCapture()
        {
            return CaptureApplication("League of Legends");
        }

        public static Point windowPosition;

        public static Bitmap CaptureApplication(string procName)
        {
            Process process = Process.GetProcessesByName(procName)[0];
            BringWindowToFront(procName);
            User32.Rect rect = default(User32.Rect);
            User32.GetWindowRect(process.MainWindowHandle, ref rect);
            windowPosition = new Point(rect.left, rect.top);
            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(rect.left, rect.top, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
            graphics.Dispose();
            return bitmap;
        }

        public static bool BringWindowToFront(string processName)
        {
            Process process = Process.GetProcessesByName(processName).FirstOrDefault<Process>();
            bool result;
            if (process == null || process.HasExited)
            {
                result = false;
            }
            else
            {
                IntPtr mainWindowHandle = process.MainWindowHandle;
                ShowWindow(mainWindowHandle, 9);
                SetForegroundWindow(mainWindowHandle);
                result = true;
            }
            return result;
        }

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hwnd);

        public class User32
        {
            // Token: 0x06002018 RID: 8216
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref User32.Rect rect);

            // Token: 0x02000466 RID: 1126
            public struct Rect
            {
                // Token: 0x04001418 RID: 5144
                public int left;

                // Token: 0x04001419 RID: 5145
                public int top;

                // Token: 0x0400141A RID: 5146
                public int right;

                // Token: 0x0400141B RID: 5147
                public int bottom;
            }
        }
    }
}
