using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Evelynn_Bot.Account_Process;
using Evelynn_Bot.Constants;
using Evelynn_Bot.Entities;
using Evelynn_Bot.ExternalCommands;
using Evelynn_Bot.League_API;
using Evelynn_Bot.League_API.GameData;
using Evelynn_Bot.TEST_AREA;
using EvelynnLCU.API_Models;
using FastBitmapLib;
using Newtonsoft.Json;

namespace Evelynn_Bot.GameAI
{
    public class GameAi : IGameAI
    {
        #region Constants

        [DllImport("user32.dll")]
        internal static extern uint SendInput(uint uint_0, [In][MarshalAs(UnmanagedType.LPArray)] GStruct1[] gstruct1_0, int int_0);

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(GEnum3 genum3_0);

        private List<RGBClass.PointerClass> rgbLists = new List<RGBClass.PointerClass>();
        private List<RGBClass.PointlerClass> pointsLists = new List<RGBClass.PointlerClass>();

        public class RGBClass
        {
            public class SetRGB
            {
                public int R { get; set; }

                public int G { get; set; }

                public int B { get; set; }

                public SetRGB(int red, int green, int blue)
                {
                    R = red;
                    G = green;
                    B = blue;
                }
            }

            public class PointerClass
            {
                public List<SetRGB> rgbLists { get; set; }

                public List<Point> pointerList { get; set; }

                public int tmpB { get; set; }

                public string mode { get; set; }
                public PointerClass(List<SetRGB> rgbPList, string whichMode)
                {
                    rgbLists = rgbPList;
                    tmpB = 0;
                    pointerList = new List<Point>();
                    mode = whichMode;
                }
            }

            public class PointlerClass
            {
                public Point Point_0 { get; set; }

                public bool Boolean_0 { get; set; }

                public PointlerClass(Point point_1)
                {
                    this.Point_0 = point_1;
                    this.Boolean_0 = false;
                }
            }

        }

        #region ENUMS WIN32
        public enum GEnum3
        {
            SM_CXSCREEN,
            SM_CYSCREEN
        }

        public struct GStruct1
        {
            public uint uint_0;

            public GStruct2 gstruct2_0;

            public static int Int32_0 => Marshal.SizeOf(typeof(GStruct1));
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct GStruct2
        {
            [FieldOffset(0)]
            internal GStruct3 gstruct3_0;

            [FieldOffset(0)]
            internal GStruct4 gstruct4_0;

            [FieldOffset(0)]
            internal GStruct5 gstruct5_0;
        }

        public struct GStruct3
        {
            internal int int_0;

            internal int int_1;

            internal GEnum4 genum4_0;

            internal GEnum5 genum5_0;

            internal uint uint_0;

            internal UIntPtr uintptr_0;
        }

        [Flags]
        public enum GEnum4 : uint
        {
            Nothing = 0x0u,
            XBUTTON1 = 0x1u,
            XBUTTON2 = 0x2u
        }

        [Flags]
        public enum GEnum5 : uint
        {
            ABSOLUTE = 0x8000u,
            HWHEEL = 0x1000u,
            MOVE = 0x1u,
            MOVE_NOCOALESCE = 0x2000u,
            LEFTDOWN = 0x2u,
            LEFTUP = 0x4u,
            RIGHTDOWN = 0x8u,
            RIGHTUP = 0x10u,
            MIDDLEDOWN = 0x20u,
            MIDDLEUP = 0x40u,
            VIRTUALDESK = 0x4000u,
            WHEEL = 0x800u,
            XDOWN = 0x80u,
            XUP = 0x100u
        }
        public struct GStruct4
        {
            internal GEnum7 genum7_0;

            internal GEnum8 genum8_0;

            internal GEnum6 genum6_0;

            internal int int_0;

            internal UIntPtr uintptr_0;
        }

        [Flags]
        public enum GEnum6 : uint
        {
            EXTENDEDKEY = 0x1u,
            KEYUP = 0x2u,
            SCANCODE = 0x8u,
            UNICODE = 0x4u
        }

        public enum GEnum7 : short
        {
            LBUTTON = 1,
            RBUTTON = 2,
            CANCEL = 3,
            MBUTTON = 4,
            XBUTTON1 = 5,
            XBUTTON2 = 6,
            BACK = 8,
            TAB = 9,
            CLEAR = 12,
            RETURN = 13,
            SHIFT = 0x10,
            CONTROL = 17,
            MENU = 18,
            PAUSE = 19,
            CAPITAL = 20,
            KANA = 21,
            HANGUL = 21,
            JUNJA = 23,
            FINAL = 24,
            HANJA = 25,
            KANJI = 25,
            ESCAPE = 27,
            CONVERT = 28,
            NONCONVERT = 29,
            ACCEPT = 30,
            MODECHANGE = 0x1F,
            SPACE = 0x20,
            PRIOR = 33,
            NEXT = 34,
            END = 35,
            HOME = 36,
            LEFT = 37,
            UP = 38,
            RIGHT = 39,
            DOWN = 40,
            SELECT = 41,
            PRINT = 42,
            EXECUTE = 43,
            SNAPSHOT = 44,
            INSERT = 45,
            DELETE = 46,
            HELP = 47,
            KEY_0 = 48,
            KEY_1 = 49,
            KEY_2 = 50,
            KEY_3 = 51,
            KEY_4 = 52,
            KEY_5 = 53,
            KEY_6 = 54,
            KEY_7 = 55,
            KEY_8 = 56,
            KEY_9 = 57,
            KEY_A = 65,
            KEY_B = 66,
            KEY_C = 67,
            KEY_D = 68,
            KEY_E = 69,
            KEY_F = 70,
            KEY_G = 71,
            KEY_H = 72,
            KEY_I = 73,
            KEY_J = 74,
            KEY_K = 75,
            KEY_L = 76,
            KEY_M = 77,
            KEY_N = 78,
            KEY_O = 79,
            KEY_P = 80,
            KEY_Q = 81,
            KEY_R = 82,
            KEY_S = 83,
            KEY_T = 84,
            KEY_U = 85,
            KEY_V = 86,
            KEY_W = 87,
            KEY_X = 88,
            KEY_Y = 89,
            KEY_Z = 90,
            LWIN = 91,
            RWIN = 92,
            APPS = 93,
            SLEEP = 95,
            NUMPAD0 = 96,
            NUMPAD1 = 97,
            NUMPAD2 = 98,
            NUMPAD3 = 99,
            NUMPAD4 = 100,
            NUMPAD5 = 101,
            NUMPAD6 = 102,
            NUMPAD7 = 103,
            NUMPAD8 = 104,
            NUMPAD9 = 105,
            MULTIPLY = 106,
            ADD = 107,
            SEPARATOR = 108,
            SUBTRACT = 109,
            DECIMAL = 110,
            DIVIDE = 111,
            F1 = 112,
            F2 = 113,
            F3 = 114,
            F4 = 115,
            F5 = 116,
            F6 = 117,
            F7 = 118,
            F8 = 119,
            F9 = 120,
            F10 = 121,
            F11 = 122,
            F12 = 123,
            F13 = 124,
            F14 = 125,
            F15 = 126,
            F16 = 0x7F,
            F17 = 0x80,
            F18 = 129,
            F19 = 130,
            F20 = 131,
            F21 = 132,
            F22 = 133,
            F23 = 134,
            F24 = 135,
            NUMLOCK = 144,
            SCROLL = 145,
            LSHIFT = 160,
            RSHIFT = 161,
            LCONTROL = 162,
            RCONTROL = 163,
            LMENU = 164,
            RMENU = 165,
            BROWSER_BACK = 166,
            BROWSER_FORWARD = 167,
            BROWSER_REFRESH = 168,
            BROWSER_STOP = 169,
            BROWSER_SEARCH = 170,
            BROWSER_FAVORITES = 171,
            BROWSER_HOME = 172,
            VOLUME_MUTE = 173,
            VOLUME_DOWN = 174,
            VOLUME_UP = 175,
            MEDIA_NEXT_TRACK = 176,
            MEDIA_PREV_TRACK = 177,
            MEDIA_STOP = 178,
            MEDIA_PLAY_PAUSE = 179,
            LAUNCH_MAIL = 180,
            LAUNCH_MEDIA_SELECT = 181,
            LAUNCH_APP1 = 182,
            LAUNCH_APP2 = 183,
            OEM_1 = 186,
            OEM_PLUS = 187,
            OEM_COMMA = 188,
            OEM_MINUS = 189,
            OEM_PERIOD = 190,
            OEM_2 = 191,
            OEM_3 = 192,
            OEM_4 = 219,
            OEM_5 = 220,
            OEM_6 = 221,
            OEM_7 = 222,
            OEM_8 = 223,
            OEM_102 = 226,
            PROCESSKEY = 229,
            PACKET = 231,
            ATTN = 246,
            CRSEL = 247,
            EXSEL = 248,
            EREOF = 249,
            PLAY = 250,
            ZOOM = 251,
            NONAME = 252,
            PA1 = 253,
            OEM_CLEAR = 254
        }

        public enum GEnum8 : short
        {
            LBUTTON = 0,
            RBUTTON = 0,
            CANCEL = 70,
            MBUTTON = 0,
            XBUTTON1 = 0,
            XBUTTON2 = 0,
            BACK = 14,
            TAB = 0xF,
            CLEAR = 76,
            RETURN = 28,
            SHIFT = 42,
            CONTROL = 29,
            MENU = 56,
            PAUSE = 0,
            CAPITAL = 58,
            KANA = 0,
            HANGUL = 0,
            JUNJA = 0,
            FINAL = 0,
            HANJA = 0,
            KANJI = 0,
            ESCAPE = 1,
            CONVERT = 0,
            NONCONVERT = 0,
            ACCEPT = 0,
            MODECHANGE = 0,
            SPACE = 57,
            PRIOR = 73,
            NEXT = 81,
            END = 79,
            HOME = 71,
            LEFT = 75,
            UP = 72,
            RIGHT = 77,
            DOWN = 80,
            SELECT = 0,
            PRINT = 0,
            EXECUTE = 0,
            SNAPSHOT = 84,
            INSERT = 82,
            DELETE = 83,
            HELP = 99,
            KEY_0 = 11,
            KEY_1 = 2,
            KEY_2 = 3,
            KEY_3 = 4,
            KEY_4 = 5,
            KEY_5 = 6,
            KEY_6 = 7,
            KEY_7 = 8,
            KEY_8 = 9,
            KEY_9 = 10,
            KEY_A = 30,
            KEY_B = 48,
            KEY_C = 46,
            KEY_D = 0x20,
            KEY_E = 18,
            KEY_F = 33,
            KEY_G = 34,
            KEY_H = 35,
            KEY_I = 23,
            KEY_J = 36,
            KEY_K = 37,
            KEY_L = 38,
            KEY_M = 50,
            KEY_N = 49,
            KEY_O = 24,
            KEY_P = 25,
            KEY_Q = 0x10,
            KEY_R = 19,
            KEY_S = 0x1F,
            KEY_T = 20,
            KEY_U = 22,
            KEY_V = 47,
            KEY_W = 17,
            KEY_X = 45,
            KEY_Y = 21,
            KEY_Z = 44,
            LWIN = 91,
            RWIN = 92,
            APPS = 93,
            SLEEP = 95,
            NUMPAD0 = 82,
            NUMPAD1 = 79,
            NUMPAD2 = 80,
            NUMPAD3 = 81,
            NUMPAD4 = 75,
            NUMPAD5 = 76,
            NUMPAD6 = 77,
            NUMPAD7 = 71,
            NUMPAD8 = 72,
            NUMPAD9 = 73,
            MULTIPLY = 55,
            ADD = 78,
            SEPARATOR = 0,
            SUBTRACT = 74,
            DECIMAL = 83,
            DIVIDE = 53,
            F1 = 59,
            F2 = 60,
            F3 = 61,
            F4 = 62,
            F5 = 0x3F,
            F6 = 0x40,
            F7 = 65,
            F8 = 66,
            F9 = 67,
            F10 = 68,
            F11 = 87,
            F12 = 88,
            F13 = 100,
            F14 = 101,
            F15 = 102,
            F16 = 103,
            F17 = 104,
            F18 = 105,
            F19 = 106,
            F20 = 107,
            F21 = 108,
            F22 = 109,
            F23 = 110,
            F24 = 118,
            NUMLOCK = 69,
            SCROLL = 70,
            LSHIFT = 42,
            RSHIFT = 54,
            LCONTROL = 29,
            RCONTROL = 29,
            LMENU = 56,
            RMENU = 56,
            BROWSER_BACK = 106,
            BROWSER_FORWARD = 105,
            BROWSER_REFRESH = 103,
            BROWSER_STOP = 104,
            BROWSER_SEARCH = 101,
            BROWSER_FAVORITES = 102,
            BROWSER_HOME = 50,
            VOLUME_MUTE = 0x20,
            VOLUME_DOWN = 46,
            VOLUME_UP = 48,
            MEDIA_NEXT_TRACK = 25,
            MEDIA_PREV_TRACK = 0x10,
            MEDIA_STOP = 36,
            MEDIA_PLAY_PAUSE = 34,
            LAUNCH_MAIL = 108,
            LAUNCH_MEDIA_SELECT = 109,
            LAUNCH_APP1 = 107,
            LAUNCH_APP2 = 33,
            OEM_1 = 39,
            OEM_PLUS = 13,
            OEM_COMMA = 51,
            OEM_MINUS = 12,
            OEM_PERIOD = 52,
            OEM_2 = 53,
            OEM_3 = 41,
            OEM_4 = 26,
            OEM_5 = 43,
            OEM_6 = 27,
            OEM_7 = 40,
            OEM_8 = 0,
            OEM_102 = 86,
            PROCESSKEY = 0,
            PACKET = 0,
            ATTN = 0,
            CRSEL = 0,
            EXSEL = 0,
            EREOF = 93,
            PLAY = 0,
            ZOOM = 98,
            NONAME = 0,
            PA1 = 0,
            OEM_CLEAR = 0
        }

        public struct GStruct5
        {
            internal int int_0;

            internal short short_0;

            internal short short_1;
        }

        private int scrX;
        private int scrY;
        #endregion

        private int isItemHasBought;

        private bool isGameEnd;

        private double healthPercentage = 100.0;

        private double prevHealthPercentage = 100.0;

        private DateTime dateTime_0 = new DateTime(2000, 1, 1);

        private double gameTime = 99999.0;

        private bool canUpgradeAbility;

        private bool isTutorialAndMF;

        private Point point_0;

        private Point point_1;

        private Point point_2;

        private Point point_3;

        private Random random_0 = new Random();

        private Point point_4;

        private Point point_5;

        private Point point_6;

        private double double_3;

        private double double_4;

        private DateTime dateTime_1;

        private int summonerItemCount;

        public bool pickedTutoChamp;

        private int tuto1ChampCount;

        private int bugfixCount;

        #endregion

        public Point AnaPointAl(Point point_7)
        {
            return new Point(point_7.X + scrX, point_7.Y + scrY);
        }
        public Point PointAl(bool is43)
        {
            Size size = Screen.PrimaryScreen.Bounds.Size;
            int num = 800;
            int num2 = 600;
            if (is43)
            {
                num = 400;
                num2 = 300;
            }
            return new Point((size.Width != num) ? ((size.Width - num) / 2) : 0, (size.Height != num2) ? ((size.Height - num2) / 2) : 0);
        }
        public Bitmap BitmapAl(int screenX, int screenY, int rectangleX, int rectangleY)
        {
            Rectangle rectangle = new Rectangle(screenX, screenY, rectangleX, rectangleY);
            Bitmap bitmap = new Bitmap(rectangle.Width, rectangle.Height, PixelFormat.Format32bppArgb);
            Graphics.FromImage(bitmap).CopyFromScreen(screenX, screenY, 0, 0, rectangle.Size, CopyPixelOperation.SourceCopy);
            return bitmap;
        }
        public double abs(double a)
        {
            return (a <= 0.0D) ? 0.0D - a : a;
        }
        public double MyPow(double num, double expp)
        {
            double result = 1.0;
            int exp = (int)expp;
            while (exp > 0)
            {
                if (exp % 2 == 1)
                    result *= num;
                exp >>= 1;
                num *= num;
            }

            return result;
        }

        public double Isqrt(double num)
        {
            if (0 == num) { return 0; }
            double n = (num / 2) + 1;
            double n1 = (n + (num / n)) / 2;
            while (n1 < n)
            {
                n = n1;
                n1 = (n + (num / n)) / 2;
            }
            return n;
        }

        public void HepsiniTarat(Bitmap bmp, List<RGBClass.PointerClass> rgbLists)
        {
            rgbLists.ToList().ForEach(delegate (RGBClass.PointerClass pClass)
            {
                pClass.tmpB = 0;
            });

            rgbLists.ToList().ForEach(delegate (RGBClass.PointerClass pClass)
            {
                pClass.pointerList = new List<Point>();
            });

            //var bitmapData = new FastBitmap(bmp);
            //bitmapData.Lock();

            BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Size.Width, bmp.Size.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr scan = bitmapData.Scan0;
            int num = bitmapData.Stride * bmp.Height;
            byte[] array = new byte[num];
            byte[] byte_2 = new byte[num / 3];
            byte[] byte_1 = new byte[num / 3];
            byte[] byte_0 = new byte[num / 3];
            Marshal.Copy(scan, array, 0, num);
            int int_ = 0;
            int stride = bitmapData.Stride;


            for (int i = 0; i < bmp.Height; i++)
            {
                for (int i2 = 0; i2 < bmp.Width; i2++)
                {
                    
                    byte_0[int_] = array[i * stride + i2 * 3];
                    byte_1[int_] = array[i * stride + i2 * 3 + 1];
                    byte_2[int_] = array[i * stride + i2 * 3 + 2];
                    rgbLists.ForEach(delegate (RGBClass.PointerClass pClass)
                    {
                        ////TODO: Huge CPU - Fix!
                        if (abs(pClass.rgbLists[pClass.tmpB].B - byte_0[int_]) <= 5 && abs(pClass.rgbLists[pClass.tmpB].G - byte_1[int_]) <= 5 && abs(pClass.rgbLists[pClass.tmpB].R - byte_2[int_]) <= 5)
                        {
                            pClass.tmpB++;
                        }
                        else
                        {
                            pClass.tmpB = 0;
                        }
                        if (pClass.tmpB == pClass.rgbLists.Count)
                        {
                            pClass.pointerList.Add(new Point(i2, i));
                            pClass.tmpB = 0;
                        }
                    });
                    int num2 = int_;
                    int_ = num2 + 1;
                }
            }

            bmp.Dispose();
            Dispose(true);
        }
        public double Hesap1(double healthPercentage, double prevHealthPercentage, double gameTime, double double_3)
        {
            return Isqrt(MyPow(gameTime - healthPercentage, 2.0) + MyPow(double_3 - prevHealthPercentage, 2.0));
        }
        public double Hesap0(double healthPercentage, double prevHealthPercentage)
        {
            return Isqrt(MyPow(healthPercentage, 2.0) + MyPow(prevHealthPercentage, 2.0));
        }
        public void PosHesapla(double healthPercentage, double prevHealthPercentage, double gameTime, double double_3, double double_4, double double_5, double double_6, double double_7, double double_8)
        {
            Random random = new Random();
            double num = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            double num4 = 0.0;
            double num5 = 1.41;
            double num6 = 1.73;
            double num7 = 2.23;
            int num8 = (int)Hesap1(Math.Round(healthPercentage), Math.Round(prevHealthPercentage), Math.Round(gameTime), Math.Round(double_3));
            uint num9 = (uint)(Environment.TickCount + 10000);
            int num10 = 0;
            while (Environment.TickCount <= num9)
            {
                double num11 = Hesap0(healthPercentage - gameTime, prevHealthPercentage - double_3);
                double_5 = Math.Min(double_5, num11);
                if (num11 < 1.0)
                {
                    num11 = 1.0;
                }
                double num12 = Math.Round(Math.Round((double)num8) * 0.3) / 7.0;
                if (num12 > 25.0)
                {
                    num12 = 25.0;
                }
                if (num12 < 5.0)
                {
                    num12 = 5.0;
                }
                if ((double)random.Next(6) == 1.0)
                {
                    num12 = 2.0;
                }
                double num13 = ((!(num12 <= Math.Round(num11))) ? Math.Round(num11) : num12);
                if (num11 >= double_8)
                {
                    num3 = num3 / num6 + ((double)random.Next((int)(Math.Round(double_5) * 2.0 + 1.0)) - double_5) / num7;
                    num4 = num4 / num6 + ((double)random.Next((int)(Math.Round(double_5) * 2.0 + 1.0)) - double_5) / num7;
                }
                else
                {
                    num3 /= num5;
                    num4 /= num5;
                }
                num += num3;
                num2 += num4;
                num += double_4 * (gameTime - healthPercentage) / num11;
                num2 += double_4 * (double_3 - prevHealthPercentage) / num11;
                if (Hesap0(num, num2) > num13)
                {
                    double num14 = num13 / 2.0 + (double)random.Next((int)(Math.Round(num13) / 2.0));
                    double num15 = Isqrt(num * num + num2 * num2);
                    num = num / num15 * num14;
                    num2 = num2 / num15 * num14;
                }
                int num16 = (int)Math.Round(healthPercentage);
                int num17 = (int)Math.Round(prevHealthPercentage);
                healthPercentage += num;
                prevHealthPercentage += num2;
                if ((double)num16 != Math.Round(healthPercentage) || (double)num17 != Math.Round(prevHealthPercentage))
                {
                    Tiklat(new Point((int)healthPercentage, (int)prevHealthPercentage));
                    num10++;
                    if (num10 % 5 == 0)
                    {
                        Thread.Sleep(1);
                    }
                }
                if (!(Hesap0(healthPercentage - gameTime, prevHealthPercentage - double_3) >= 1.0))
                {
                    break;
                }
            }
            if (Math.Round(gameTime) != Math.Round(healthPercentage) || Math.Round(double_3) != Math.Round(prevHealthPercentage))
            {
                Tiklat(new Point((int)gameTime, (int)double_3));
            }
        }
        public void Tiklat(Point point_0)
        {
            GStruct1[] array = new GStruct1[1];
            GStruct1 gStruct = default(GStruct1);
            gStruct.uint_0 = 0u;
            gStruct.gstruct2_0.gstruct3_0.genum5_0 = GEnum5.ABSOLUTE | GEnum5.MOVE;
            gStruct.gstruct2_0.gstruct3_0.int_0 = point_0.X * (65535 / GetSystemMetrics(GEnum3.SM_CXSCREEN));
            gStruct.gstruct2_0.gstruct3_0.int_1 = point_0.Y * (65535 / GetSystemMetrics(GEnum3.SM_CYSCREEN));
            array[0] = gStruct;
            SendInput(1u, array, GStruct1.Int32_0);
        }
        public void SolTiklat()
        {
            GStruct1[] array = new GStruct1[1];
            GStruct1 gStruct = default(GStruct1);
            gStruct.uint_0 = 0u;
            gStruct.gstruct2_0.gstruct3_0.genum5_0 = GEnum5.LEFTDOWN;
            gStruct.gstruct2_0.gstruct3_0.genum4_0 = GEnum4.Nothing;
            gStruct.gstruct2_0.gstruct3_0.uintptr_0 = (UIntPtr)0uL;
            array[0] = gStruct;
            SendInput(1u, array, GStruct1.Int32_0);
            Thread.Sleep(250);
            gStruct.gstruct2_0.gstruct3_0.genum5_0 = GEnum5.LEFTUP;
            array[0] = gStruct;
            SendInput(1u, array, GStruct1.Int32_0);
        }
        public void EkraniAyarla(Point point_0)
        {
            Random random = new Random();
            double double_ = random.Next(25, 35);
            double gameTime = random.Next(50, 60);
            double double_3 = random.Next(1, 2);
            double double_4 = random.Next(3, 4);
            double double_5 = 1.0;
            PosHesapla(Cursor.Position.X, Cursor.Position.Y, point_0.X, point_0.Y, double_, gameTime, double_3, double_4, double_5);
        }
        public void TusuAyarla(GEnum8 genum8_0)
        {
            GStruct1[] array = new GStruct1[1];
            GStruct1 gStruct = default(GStruct1);
            gStruct.uint_0 = 1u;
            gStruct.gstruct2_0.gstruct4_0.genum8_0 = genum8_0;
            gStruct.gstruct2_0.gstruct4_0.genum6_0 = GEnum6.SCANCODE;
            array[0] = gStruct;
            SendInput(1u, array, GStruct1.Int32_0);
            Thread.Sleep(100);
            gStruct.gstruct2_0.gstruct4_0.genum6_0 = GEnum6.KEYUP | GEnum6.SCANCODE;
            array[0] = gStruct;
            SendInput(1u, array, GStruct1.Int32_0);
        }
        public double PointHesapla(PointF pointF_0, PointF pointF_1, PointF pointF_2)
        {
            float num = pointF_2.X - pointF_1.X;
            float num2 = pointF_2.Y - pointF_1.Y;
            PointF pointF;
            if (num != 0f || num2 != 0f)
            {
                float num3 = ((pointF_0.X - pointF_1.X) * num + (pointF_0.Y - pointF_1.Y) * num2) / (num * num + num2 * num2);
                if (num3 >= 0f)
                {
                    if (num3 <= 1f)
                    {
                        pointF = new PointF(pointF_1.X + num3 * num, pointF_1.Y + num3 * num2);
                        num = pointF_0.X - pointF.X;
                        num2 = pointF_0.Y - pointF.Y;
                    }
                    else
                    {
                        pointF = new PointF(pointF_2.X, pointF_2.Y);
                        num = pointF_0.X - pointF_2.X;
                        num2 = pointF_0.Y - pointF_2.Y;
                    }
                }
                else
                {
                    pointF = new PointF(pointF_1.X, pointF_1.Y);
                    num = pointF_0.X - pointF_1.X;
                    num2 = pointF_0.Y - pointF_1.Y;
                }
                if (!(num >= 0f))
                {
                    return Isqrt(num * num + num2 * num2) * -1.0;
                }
                return Isqrt(num * num + num2 * num2);
            }
            pointF = pointF_1;
            num = pointF_0.X - pointF_1.X;
            num2 = pointF_0.Y - pointF_1.Y;
            return Isqrt(num * num + num2 * num2);
        }
        public void KodTarat(GEnum8 genum8_0)
        {
            GStruct1[] array = new GStruct1[1];
            GStruct1 gStruct = default(GStruct1);
            gStruct.uint_0 = 1u;
            gStruct.gstruct2_0.gstruct4_0.genum8_0 = genum8_0;
            gStruct.gstruct2_0.gstruct4_0.genum6_0 = GEnum6.SCANCODE;
            array[0] = gStruct;
            SendInput(1u, array, GStruct1.Int32_0);
            Thread.Sleep(100);
        }
        public void TusBas(GEnum8 genum8_0)
        {
            GStruct1[] array = new GStruct1[1];
            GStruct1 gStruct = default(GStruct1);
            gStruct.uint_0 = 1u;
            gStruct.gstruct2_0.gstruct4_0.genum8_0 = genum8_0;
            gStruct.gstruct2_0.gstruct4_0.genum6_0 = GEnum6.KEYUP | GEnum6.SCANCODE;
            array[0] = gStruct;
            SendInput(1u, array, GStruct1.Int32_0);
            Thread.Sleep(100);
        }
        public void EndBas()
        {
            KodTarat(GEnum8.CONTROL);
            TusuAyarla(GEnum8.END);
            TusBas(GEnum8.CONTROL);
            Dispose(true);
        }

        public double PointerMath(Point mapPoint, Point towerPoint)
        {
            return Isqrt(MyPow((double)(towerPoint.X - mapPoint.X), 2.0) + MyPow((double)(towerPoint.Y - mapPoint.Y), 2.0) * 1.0);
        } 
        public bool BirseyHesapla(Point mapPoint)
        {
            foreach (RGBClass.PointlerClass pointList in pointsLists)
            {
                if (!pointList.Boolean_0 && !(PointerMath(mapPoint, pointList.Point_0) >= 6.0))
                {
                    return true;
                }
            }
            return false;
        }
        public Point MinimapHesapla(double double_5)
        {
            double num = (double)random_0.Next(15, 95) / 100.0;
            double num2 = (double)random_0.Next(15, 95) / 100.0;
            double num3 = (double)200 * num;
            double num4 = (double)150 * num2;
            if (double_5 <= double_4)
            {
                num3 = 0.0;
            }
            else if (double_5 >= double_3)
            {
                num4 = 0.0;
            }
            return new Point(point_4.X - (int)num3, point_4.Y + (int)num4);
        }
        public Point TowerHesapla(double double_5)
        {
            double num = (double)random_0.Next(15, 95) / 100.0;
            double num2 = (double)random_0.Next(15, 95) / 100.0;
            double num3 = (double)200 * num;
            double num4 = (double)150 * num2;
            if (double_5 > double_4)
            {
                if (double_5 >= double_3)
                {
                    num3 = 0.0;
                }
            }
            else
            {
                num4 = 0.0;
            }
            return new Point(point_4.X + (int)num3, point_4.Y - (int)num4);
        }
        public void SagTikla()
        {
            GStruct1[] array = new GStruct1[1];
            GStruct1 gStruct = default(GStruct1);
            gStruct.uint_0 = 0u;
            gStruct.gstruct2_0.gstruct3_0.genum5_0 = GEnum5.RIGHTDOWN;
            gStruct.gstruct2_0.gstruct3_0.genum4_0 = GEnum4.Nothing;
            gStruct.gstruct2_0.gstruct3_0.uintptr_0 = (UIntPtr)0uL;
            array[0] = gStruct;
            SendInput(1u, array, GStruct1.Int32_0);
            Thread.Sleep(100);
            gStruct.gstruct2_0.gstruct3_0.genum5_0 = GEnum5.RIGHTUP;
            array[0] = gStruct;
            SendInput(1u, array, GStruct1.Int32_0);
        }
        public void TotemAt()
        {
            KodTarat(GEnum8.SHIFT);
            TusuAyarla(GEnum8.KEY_4);
            TusBas(GEnum8.SHIFT);
        }
        public void ItemKullan()
        {
            KodTarat(GEnum8.SHIFT);
            TusuAyarla(GEnum8.KEY_1); // 2
            TusuAyarla(GEnum8.KEY_2); // 3
            TusuAyarla(GEnum8.KEY_3); // 4
            TusuAyarla(GEnum8.KEY_5); // 6
            TusuAyarla(GEnum8.KEY_6); // 7
            TusuAyarla(GEnum8.KEY_7); // 8
            TusBas(GEnum8.SHIFT);
        }
        public void EsyaAl(bool bool_4)
        {
            if (bool_4)
            {
                int num = random_0.Next(1, 15);
                EkraniAyarla(new Point(point_3.X + num, point_3.Y + num));
                Thread.Sleep(4000);
                SagTikla();
                Thread.Sleep(150);
                SagTikla();
                Thread.Sleep(150);
                SagTikla();
                Thread.Sleep(150);
                Thread.Sleep(3000);
                TusuAyarla(GEnum8.KEY_P);
                Thread.Sleep(8000);
                dateTime_0 = DateTime.Now;
                Dispose(true);
            }
            else
            {
                TusuAyarla(GEnum8.KEY_P);
                Dispose(true);
                Thread.Sleep(8000);
            }
        }
        public void BaslangicEsyaAl(bool bool_4)
        {
            if (bool_4)
            {
                int num = random_0.Next(1, 15);
                EkraniAyarla(new Point(point_2.X + num, point_2.Y + num));
                Thread.Sleep(4000);
                SagTikla();
                Thread.Sleep(150);
                SagTikla();
                Thread.Sleep(150);
                SagTikla();
                Thread.Sleep(150);
                EkraniAyarla(new Point(point_1.X + num, point_1.Y + num));
                Thread.Sleep(4000);
                SagTikla();
                Thread.Sleep(150);
                SagTikla();
                Thread.Sleep(150);
                SagTikla();
                Thread.Sleep(150);
                Thread.Sleep(3000);
                TusuAyarla(GEnum8.KEY_P);
                Thread.Sleep(8000);
                dateTime_0 = DateTime.Now;
                Dispose(true);
            }
            else
            {
                TusuAyarla(GEnum8.KEY_P);
                int num = random_0.Next(1, 15);
                EkraniAyarla(new Point(point_2.X + num, point_2.Y + num));
                Thread.Sleep(4000);
                SagTikla();
                Thread.Sleep(150);
                SagTikla();
                Thread.Sleep(150);
                SagTikla();
                Thread.Sleep(150);
                EkraniAyarla(new Point(point_1.X + num, point_1.Y + num));
                Thread.Sleep(4000);
                SagTikla();
                Thread.Sleep(150);
                SagTikla();
                Thread.Sleep(150);
                SagTikla();
                Thread.Sleep(150);
                Thread.Sleep(3000);
                TusuAyarla(GEnum8.KEY_P);
                Thread.Sleep(8000);
                dateTime_0 = DateTime.Now;
                Dispose(true);
            }
        }

        private bool flaggo;

        public void YeniAI_1(object Interface)
        {
            bugfixCount = 0;
            Interface itsInterface = (Interface)Interface;
            itsInterface.clientKiller.KillRiotUx();
            Point point = new Point(400, 300);
            while (!isGameEnd)
            {
                try
                {
                    if (itsInterface.queueId == 2000)
                    {
                        if (bugfixCount < 7)
                        {
                            bugfixCount++;
                            SagTikla();
                        }
                    }

                    Bitmap bitmap_;
                    try
                    {
                        bitmap_ = BitmapAl(scrX, scrY, point.X, point.Y);
                    }
                    catch (Exception)
                    {
                        itsInterface.logger.Log(false, "Error while creating bitmap");
                        Thread.Sleep(500);
                        continue;
                    }


                    HepsiniTarat(bitmap_, rgbLists);
                    List<Point> list = rgbLists.First((RGBClass.PointerClass class30_0) => class30_0.mode == "Minimap").pointerList;
                    List<Point> list2 = rgbLists.First((RGBClass.PointerClass class30_0) => class30_0.mode == "EnemyHero").pointerList;
                    List<Point> list3 = rgbLists.First((RGBClass.PointerClass class30_0) => class30_0.mode == "EnemyMinion").pointerList;
                    List<Point> list4 = rgbLists.First((RGBClass.PointerClass class30_0) => class30_0.mode == "AllyMinion").pointerList;
                    List<Point> list5 = rgbLists.First((RGBClass.PointerClass class30_0) => class30_0.mode == "Shop").pointerList;
                    List<Point> list6 = rgbLists.First((RGBClass.PointerClass class30_0) => class30_0.mode == "Fountain").pointerList;
                    bool flag = list5.Count > 9;

                    Point point2 = ((list.Count == 0) ? new Point(-1, -1) : new Point(list[0].X + 20, list[0].Y + 15));
                    Point point_ = ((list3.Count == 0) ? new Point(-1, -1) : new Point(list3[list3.Count - 1].X + 30, list3[list3.Count - 1].Y + 30));
                    Point point_2 = ((list4.Count == 0) ? new Point(-1, -1) : new Point(list4[0].X + 30, list4[0].Y + 30));
                    Point point_3 = ((list2.Count == 0) ? new Point(-1, -1) : new Point(list2[0].X + 65, list2[0].Y + 45));
                    double double_ = PointHesapla(point2, point_5, point_6);
                    double num = PointerMath(point2, point_0);
                    bool flag2 = (num < 5.0 && list6.Count > 0) || point2.X == -1; // Base'demi diye kontrol et
                    bool flag3 = BirseyHesapla(point2);

                    //////////////

                    //int count;
                    //count = 0;
                    //while (count < 7)
                    //{
                    //    var a = new Point(TestAI.GetColorPosition(Color.FromArgb(255, 255, 255), bitmap_).X, TestAI.GetColorPosition(Color.FromArgb(255, 255, 255), bitmap_).Y);
                    //    Console.WriteLine(a);
                    //}

                    //Point point2 = new Point(TestAI.GetColorPosition(Color.FromArgb(255, 255, 255), bitmap_).X, TestAI.GetColorPosition(Color.FromArgb(255, 255, 255), bitmap_).Y);
                    //Point point_ = new Point(TestAI.GetColorPosition(Color.FromArgb(255, 119, 56, 54), bitmap_).X, TestAI.GetColorPosition(Color.FromArgb(255, 119, 56, 54), bitmap_).Y);

                    //Point point_2 = new Point(TestAI.GetColorPosition(Color.FromArgb(255, 44, 89, 119), bitmap_).X, TestAI.GetColorPosition(Color.FromArgb(255, 44, 89, 119), bitmap_).Y);
                    //Point allyMinion2 = new Point(TestAI.GetColorPosition(Color.FromArgb(255, 8, 12, 16), bitmap_).X, TestAI.GetColorPosition(Color.FromArgb(255, 8, 12, 16), bitmap_).Y);

                    //Point point_3 = new Point(TestAI.GetColorPosition(Color.FromArgb(255, 48, 3, 0), bitmap_).X, TestAI.GetColorPosition(Color.FromArgb(255, 48, 3, 0), bitmap_).Y);
                    //Point enemyMinion2 = new Point(TestAI.GetColorPosition(Color.FromArgb(255, 8, 8, 8), bitmap_).X, TestAI.GetColorPosition(Color.FromArgb(255, 8, 8, 8), bitmap_).Y);

                    //Point Fountain = new Point(TestAI.GetColorPosition(Color.FromArgb(255, 103, 103, 65), bitmap_).X, TestAI.GetColorPosition(Color.FromArgb(255, 103, 103, 65), bitmap_).Y);
                    //Point Fountain2 = new Point(TestAI.GetColorPosition(Color.FromArgb(255, 64, 75, 52), bitmap_).X, TestAI.GetColorPosition(Color.FromArgb(255, 64, 75, 52), bitmap_).Y);
                    //Point Fountain3 = new Point(TestAI.GetColorPosition(Color.FromArgb(255, 22, 126, 127), bitmap_).X, TestAI.GetColorPosition(Color.FromArgb(255, 22, 126, 127), bitmap_).Y);

                    //Point Shop = new Point(TestAI.GetColorPosition(Color.FromArgb(255, 141, 121, 83), bitmap_).X, (TestAI.GetColorPosition(Color.FromArgb(255, 141, 121, 83), bitmap_).Y));

                    //if (point2.X == 0 || point2.Y == 0)
                    //{
                    //    point2.X = -1;
                    //    point2.Y = -1;
                    //}
                    //else
                    //{
                    //    point2.X += 20;
                    //    point2.Y += 15;
                    //}

                    //if (point_.X == 0 || point_.Y == 0)
                    //{
                    //    point_.X = -1;
                    //    point_.Y = -1;
                    //}
                    //else
                    //{
                    //    point_.X += 30;
                    //    point_.Y += 30;
                    //}

                    //if ((point_2.X == 0 || point_2.Y == 0) || (allyMinion2.X == 0 || allyMinion2.Y == 0))
                    //{
                    //    point_2.X = -1;
                    //    point_2.Y = -1;
                    //}
                    //if ((point_2.X != -1 || point_2.Y != -1) && (allyMinion2.X != -1 || allyMinion2.Y != -1))
                    //{
                    //    point_2.X += 30;
                    //    point_2.Y += 30;
                    //}


                    //if ((point_3.X == 0 || point_3.Y == 0) || (enemyMinion2.X == 0 || enemyMinion2.Y == 0))
                    //{
                    //    point_3.X = -1;
                    //    point_3.Y = -1;
                    //}
                    //if ((point_3.X != -1 || point_3.Y != -1) && (enemyMinion2.X != -1 || enemyMinion2.Y != -1))
                    //{
                    //    point_3.X += 65;
                    //    point_3.Y += 45;
                    //}

                    //if (Fountain.X != 0 || Fountain.Y != 0)
                    //{
                    //    Fountain.X += 20;
                    //    Fountain.Y += 15;
                    //}

                    //if (Fountain2.X != 0 || Fountain2.Y != 0)
                    //{
                    //    Fountain2.X += 20;
                    //    Fountain2.Y += 15;
                    //}

                    //if (Fountain3.X != 0 || Fountain3.Y != 0)
                    //{
                    //    Fountain3.X += 20;
                    //    Fountain3.Y += 15;
                    //}

                    ////Console.WriteLine("---------------------");

                    ////Console.WriteLine("Map: " + point2);
                    ////Console.WriteLine("EnemyMinion: " + point_);
                    ////Console.WriteLine("AllyMinion: " + point_2);
                    ////Console.WriteLine("Enemy: " + point_3);
                    ////Console.WriteLine("Fountain1: " + Fountain);
                    ////Console.WriteLine("Fountain2: " + Fountain2);
                    ////Console.WriteLine("Fountain3: " + Fountain3);

                    ////Console.WriteLine("---------------------");

                    //double double_ = PointHesapla(point2, point_5, point_6);
                    //double num = PointerMath(point2, point_0);

                    //bool flag = Shop.X != 0;

                    //if (Fountain.X != 0 || Fountain.Y != 0 || Fountain2.X != 0 || Fountain2.Y != 0 || Fountain3.X != 0 || Fountain.Y != 0)
                    //{
                    //    flaggo = true;
                    //}

                    //bool flag2 = (num < 5.0 && flaggo) || point2.X == -1;
                    //bool flag3 = BirseyHesapla(point2);

                    //Console.WriteLine(point2);
                    /////////////


                    bool debug;
                    debug = false;
                    while (debug)
                    {
                        Thread.Sleep(5000);
                    }
                    

                    if (point2.X != -1 || flag)
                    {
                        if (itsInterface.queueId != 2000 || !isTutorialAndMF) // Tutorial 1 değil ya da Miss Fortune ile 1 kill almadıysa
                        {
                            goto IL_03e7;
                        }

                        List<Point> list7 = rgbLists.First((RGBClass.PointerClass class30_0) => class30_0.mode == "ChangeHero").pointerList;
                        if (list7.Count <= 0)
                        {
                            goto IL_03e7;
                        }

                        //Tutorail 1 de sonsuza kadar şampiyon değiştiroyr ve kuleyi alması çok uzuyor. Ondan dolayı limit eklendi
                        if (tuto1ChampCount <= 10)
                        {
                            tuto1ChampCount++;
                            pickedTutoChamp = true;
                            Thread.Sleep(1500);

                            EkraniAyarla(AnaPointAl(list7[0]));
                            SagTikla();
                            Thread.Sleep(2000);
                        }
                    }
                    else
                    {
                        Thread.Sleep(5000);
                    }
                    goto end_IL_0035;
                IL_051e:
                    if (healthPercentage < prevHealthPercentage && prevHealthPercentage - healthPercentage > 2.0)
                    {
                        EkraniAyarla(MinimapHesapla(double_));
                        SagTikla();
                        ItemKullan();
                        Thread.Sleep(1000);
                    }
                    if (canUpgradeAbility)
                    {
                        switch (itsInterface.player.Level)
                        {
                            case 1:
                                TusuAyarla(GEnum8.KEY_J);
                                break;
                            case 2:
                                TusuAyarla(GEnum8.KEY_K);
                                break;
                            case 3:
                                TusuAyarla(GEnum8.KEY_M);
                                break;
                            case 4:
                                TusuAyarla(GEnum8.KEY_J);
                                break;
                            case 5:
                                TusuAyarla(GEnum8.KEY_J);
                                TusuAyarla(GEnum8.KEY_K);
                                break;
                            case 6:
                                TusuAyarla(GEnum8.KEY_L);
                                break;
                            case 7:
                                TusuAyarla(GEnum8.KEY_J);
                                break;
                            case 8:
                                TusuAyarla(GEnum8.KEY_K);
                                break;
                            case 9:
                                TusuAyarla(GEnum8.KEY_J);
                                break;
                            case 10:
                                TusuAyarla(GEnum8.KEY_K);
                                break;
                            case 11:
                                TusuAyarla(GEnum8.KEY_L);
                                break;
                            case 12:
                                TusuAyarla(GEnum8.KEY_K);
                                break;
                            case 13:
                                TusuAyarla(GEnum8.KEY_K);
                                break;
                            case 14:
                                TusuAyarla(GEnum8.KEY_M);
                                break;
                            case 15:
                                TusuAyarla(GEnum8.KEY_M);
                                break;
                            case 16:
                                TusuAyarla(GEnum8.KEY_L);
                                break;
                            case 17:
                                TusuAyarla(GEnum8.KEY_M);
                                break;
                            case 18:
                                TusuAyarla(GEnum8.KEY_M);
                                break;
                            default:
                                TusuAyarla(GEnum8.KEY_J);
                                TusuAyarla(GEnum8.KEY_K);
                                TusuAyarla(GEnum8.KEY_M);
                                TusuAyarla(GEnum8.KEY_L);
                                break;
                        }
                    }
                    if (healthPercentage >= 25.0)
                    {
                        //point_2.X != -1 && list4.Count >= 2
                        if (point_2.X != -1 && list4.Count >= 2) // En az 2 veya daha fazla AllyMinion varsa
                        {
                            if (!flag3)
                            {
                                if (point_3.X == -1) // EnemyHero yoksa
                                {
                                    
                                    if (point_.X == -1) // EnemyMinion yoksa
                                    {
                                        itsInterface.clientKiller.ActivateGame();
                                        EkraniAyarla(TowerHesapla(double_));
                                        TusuAyarla(GEnum8.KEY_A);
                                    }
                                    else
                                    {
                                        itsInterface.clientKiller.ActivateGame();
                                        EkraniAyarla(AnaPointAl(point_));
                                        TusuAyarla(GEnum8.KEY_A);
                                        isItemHasBought = 0;
                                    }
                                }
                                else
                                {
                                    EkraniAyarla(AnaPointAl(point_3));
                                    switch (random_0.Next(1, 5))
                                    {
                                        case 3:
                                            TusuAyarla(GEnum8.KEY_E);
                                            break;
                                        case 4:
                                            TusuAyarla(GEnum8.KEY_R);
                                            break;
                                        case 2:
                                            TusuAyarla(GEnum8.KEY_W);
                                            break;
                                        case 1:
                                            TusuAyarla(GEnum8.KEY_Q);
                                            break;
                                    }
                                    TusuAyarla(GEnum8.KEY_A);
                                }
                            }
                            else if (point_3.X == -1) // EnemyHero yoksa
                            {
                                EkraniAyarla(AnaPointAl(point_2));
                                TusuAyarla(GEnum8.KEY_A);
                            }
                            else // Hiçbir şey yoksa
                            {
                                EkraniAyarla(MinimapHesapla(double_));
                                SagTikla();
                            }
                        }
                        else if (flag3)
                        {
                            EkraniAyarla(MinimapHesapla(double_));
                            SagTikla();
                        }
                        else if (num >= 15.0)
                        {
                            if (point_3.X != -1)
                            {
                                EkraniAyarla(AnaPointAl(point_3));
                                switch (random_0.Next(1, 5))
                                {
                                    case 1:
                                        TusuAyarla(GEnum8.KEY_Q);
                                        break;
                                    case 4:
                                        TusuAyarla(GEnum8.KEY_R);
                                        break;
                                    case 3:
                                        TusuAyarla(GEnum8.KEY_E);
                                        break;
                                    case 2:
                                        TusuAyarla(GEnum8.KEY_W);
                                        break;
                                }
                                TusuAyarla(GEnum8.KEY_A);

                            }
                            else if (point_.X != -1)
                            {
                                EkraniAyarla(AnaPointAl(point_));
                                TusuAyarla(GEnum8.KEY_A);
                            }
                            else
                            {
                                //Burası
                                EkraniAyarla(TowerHesapla(double_));
                                TusuAyarla(GEnum8.KEY_A);
                            }
                        }
                        else
                        {
                            double num2 = (double)random_0.Next(50, 95) / 100.0;
                            double num3 = (double)random_0.Next(50, 95) / 100.0;
                            double num4 = (double)200 * num2;
                            double num5 = (double)150 * num3;
                            EkraniAyarla(new Point(point_4.X + (int)num4, point_4.Y - (int)num5));
                            SagTikla();
                        }
                    }
                    else // Can 35den düşük TODO: Bizim algoritmayı koy
                    {
                        TusuAyarla(GEnum8.KEY_F);
                        //if (spell1ID != 7)
                        //{
                        //    if (spell2ID == 7)
                        //    {
                        //        TusuAyarla(GEnum8.KEY_F);
                        //    }
                        //}
                        //else
                        //{
                        //    TusuAyarla(GEnum8.KEY_D);
                        //}
                        TotemAt();
                        if (itsInterface.queueId == 2000) // Eğer Tutorial ise
                        {
                            EkraniAyarla(MinimapHesapla(double_));
                            SagTikla();
                        }
                        else if (point_.X == -1 && point_3.X == -1)
                        {
                            isItemHasBought = 0;
                            TusuAyarla(GEnum8.KEY_B);
                            Thread.Sleep(2000);
                        }
                        else
                        {
                            EkraniAyarla(MinimapHesapla(double_));
                            SagTikla();
                            TusuAyarla(GEnum8.KEY_D);

                            //if (spell1ID == 4 || spell1ID == 6)
                            //{
                            //    TusuAyarla(GEnum8.KEY_D);
                            //}
                            //else if (spell2ID == 4 || spell2ID == 6)
                            //{
                            //    TusuAyarla(GEnum8.KEY_F);
                            //}
                        }
                    }
                    goto IL_0964;
                IL_03e7:
                    if (flag2 && itsInterface.queueId != 2000) // Eğer Basedeysek ve tutorial'da değilsek //&& isItemHasBought < 2
                    {
                        if (summonerItemCount != 0) // Şu anki şampiyonun eşyası varsa
                        {
                            if (DateTime.Now.Subtract(dateTime_0).TotalMinutes > 3.0)
                            {
                                EsyaAl(flag);
                                Thread.Sleep(random_0.Next(1000, 1300));
                                //isItemHasBought++;
                                continue;
                            }
                            if (flag)
                            {
                                TusuAyarla(GEnum8.KEY_P);
                                Thread.Sleep(3000);
                                continue;
                            }
                            if (!(healthPercentage < 100.0))
                            {
                                goto IL_051e;
                            }
                            Thread.Sleep(random_0.Next(1000, 1300));
                        }
                        else
                        {
                            BaslangicEsyaAl(flag); // TODO: BUNLARI REFACTOR ET! TEK KODA AL
                            Thread.Sleep(random_0.Next(1000, 1300));
                            //isItemHasBought++;
                        }
                    }
                    else //Basede Değilsek
                    {
                        if (!(!flag2 && flag))
                        {
                            goto IL_051e; //Combo Yap
                        }
                        TusuAyarla(GEnum8.KEY_P);
                        Thread.Sleep(3000);
                    }
                    Dispose(true);
                    end_IL_0035:;
                }

                catch (Exception ex2)
                {
                    Console.WriteLine($"HATA: {ex2}");
                    goto IL_0964;
                }
                Dispose(true);
                continue;
            IL_0964:
                Thread.Sleep(random_0.Next(400, 800));
                Dispose(true);
            }

            Dispose(true);
            itsInterface.logger.Log(true, "Game is Done!");
            Thread.Sleep(3000);
            itsInterface.ProcessController.SuspendLeagueUx(itsInterface);
        }
        public void StartNewGameAI(Interface itsInterface)
        {
            try
            {
                isGameEnd = false;
                List<Thread> list = new List<Thread>();
                Thread thread = new Thread(YeniAI_1); // Oyun AI
                thread.Start(itsInterface);
                list.Add(thread);
                Thread thread2 = new Thread(GetInGameStats);
                thread2.Start(itsInterface);
                list.Add(thread2);
                Thread thread3 = new Thread(AraliEndGonder);
                thread3.Start();
                list.Add(thread3);

                foreach (Thread item in list)
                {
                    item.Join();
                }

                Dispose(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"HATA: {ex}");
            }
        }
        public void GetInGameStats(object Interface)
        {
            Interface itsInterface = (Interface)Interface;

            while (!isGameEnd)
            {
                try
                {
                    CurrentPlayerStats(itsInterface);

                    string tutoChampName = "Miss Fortune";
                    string cGame_championName = itsInterface.player.CurrentGame_ChampName;
                    
                    if (cGame_championName == null)
                    {
                        cGame_championName = tutoChampName;
                    }

                    try
                    {
                        var inGameData = itsInterface.player.Data;
                        canUpgradeAbility = itsInterface.player.Level_Q + itsInterface.player.Level_W + itsInterface.player.Level_E + itsInterface.player.Level_R != itsInterface.player.Level;
                        gameTime = inGameData.GameData.GameTime.Value;

                        if (Process.GetProcessesByName("League of Legends").Length == 1)
                        {
                            isGameEnd = false;
                        }
                        else
                        {
                            isGameEnd = inGameData.Events.EventsEvents.Count((Event details) => details.EventName == "GameEnd") > 0 || gameTime > 3600.0;
                        }

                        if (itsInterface.queueId != 2000)
                        {
                            summonerItemCount = inGameData.AllPlayers.FirstOrDefault((AllPlayer liveData) =>
                            {
                                return liveData.ChampionName == cGame_championName;

                            }).Items.Length;
                        }

                        prevHealthPercentage = healthPercentage;
                        healthPercentage = itsInterface.player.CurrentHealth * 100.0 / itsInterface.player.MaxHealth;

                        if (!pickedTutoChamp && itsInterface.queueId == 2000)
                        {
                            isTutorialAndMF = inGameData.AllPlayers
                                .FirstOrDefault((AllPlayer liveData) =>
                                {

                                    return liveData.ChampionName == cGame_championName;


                                }).Scores
                                .Kills == 1 && inGameData.AllPlayers

                                .FirstOrDefault((AllPlayer liveData) =>
                                {

                                    return liveData.ChampionName == cGame_championName;

                                })
                                .ChampionName == "Miss Fortune";
                        }

                        foreach (Event item in inGameData.Events.EventsEvents)
                        {
                            if (item.EventName == "Turret_T2_C_05_A")
                            {
                                pointsLists[0].Boolean_0 = true;
                            }
                            else if (item.EventName != "Turret_T2_C_04_A")
                            {
                                if (item.EventName == "Turret_T2_C_03_A")
                                {
                                    pointsLists[2].Boolean_0 = true;
                                }
                                else if (item.EventName == "Turret_T2_C_02_A")
                                {
                                    pointsLists[3].Boolean_0 = true;
                                }
                                else if (item.EventName == "Turret_T2_C_01_A")
                                {
                                    pointsLists[4].Boolean_0 = true;
                                }
                            }
                            else
                            {
                                pointsLists[1].Boolean_0 = true;
                            }
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
                catch (Exception ec)
                {
                    //Console.WriteLine(ec);
                    isGameEnd = isGameEnd || Process.GetProcessesByName("League of Legends").Length == 0;
                }

                //Burada bota restart atılmasının sebebi: Leageue process (oyun) açık kalıyor, arkada queue oluşturuluyor ve iki oyun üst üste binior (buglu) ve normal.
                //Restart atmadan ise League process (oyun) kapanamıyor.
                if (DateTime.Now.Subtract(dateTime_1).TotalMinutes > 90.0)
                {
                    itsInterface.logger.Log(false,"Overtime playing");
                    Restart(itsInterface);
                }

                Dispose(true);
                Thread.Sleep(random_0.Next(400, 800));
            }
            Dispose(true);
        }
        public void AraliEndGonder()
        {
            while (!isGameEnd)
            {
                EndBas();
                Thread.Sleep(15000);
            }
        }
        public void RGBHazirla(Interface itsInterface)
        {
            rgbLists.Add(new RGBClass.PointerClass(new List<RGBClass.SetRGB>
            {
                new RGBClass.SetRGB(103, 103, 65),
                new RGBClass.SetRGB(64, 75, 52),
                new RGBClass.SetRGB(22, 126, 127)
            }, "Fountain"));
            rgbLists.Add(new RGBClass.PointerClass(new List<RGBClass.SetRGB>
            {
                new RGBClass.SetRGB(141, 121, 83),
                new RGBClass.SetRGB(141, 121, 83),
                new RGBClass.SetRGB(141, 121, 83),
                new RGBClass.SetRGB(141, 121, 83),
                new RGBClass.SetRGB(141, 121, 83),
                new RGBClass.SetRGB(141, 121, 83),
                new RGBClass.SetRGB(141, 121, 83),
                new RGBClass.SetRGB(141, 121, 83),
                new RGBClass.SetRGB(141, 121, 83),
                new RGBClass.SetRGB(141, 121, 83),
                new RGBClass.SetRGB(141, 121, 83),
                new RGBClass.SetRGB(141, 121, 83)
            }, "Shop"));

            rgbLists.Add(new RGBClass.PointerClass(new List<RGBClass.SetRGB>
            {
                new RGBClass.SetRGB(255, 255, 255),
                new RGBClass.SetRGB(255, 255, 255),
                new RGBClass.SetRGB(255, 255, 255),
                new RGBClass.SetRGB(255, 255, 255),
                new RGBClass.SetRGB(255, 255, 255),
                new RGBClass.SetRGB(255, 255, 255)
            }, "Minimap"));
            rgbLists.Add(new RGBClass.PointerClass(new List<RGBClass.SetRGB>
            {
                new RGBClass.SetRGB(8, 8, 8),
                new RGBClass.SetRGB(48, 3, 0),
                new RGBClass.SetRGB(48, 3, 0)
            }, "EnemyHero"));
            rgbLists.Add(new RGBClass.PointerClass(new List<RGBClass.SetRGB>
            {
                new RGBClass.SetRGB(8, 12, 16),
                new RGBClass.SetRGB(44, 89, 119),
                new RGBClass.SetRGB(44, 89, 119),
                new RGBClass.SetRGB(44, 89, 119),
                new RGBClass.SetRGB(44, 89, 119),
                new RGBClass.SetRGB(44, 89, 119),
                new RGBClass.SetRGB(44, 89, 119),
                new RGBClass.SetRGB(44, 89, 119),
                new RGBClass.SetRGB(44, 89, 119),
                new RGBClass.SetRGB(44, 89, 119),
                new RGBClass.SetRGB(44, 89, 119),
                new RGBClass.SetRGB(44, 89, 119),
                new RGBClass.SetRGB(44, 89, 119)
            }, "AllyMinion"));

            #region TODO CHANGE TUTO

            if (itsInterface.queueId == 2000) // EĞER OYUN TUTORİAL 1 İSE MİNYON RENGİ DEĞİŞİYOR
            {
                rgbLists.Add(new RGBClass.PointerClass(new List<RGBClass.SetRGB>
                {
                    new RGBClass.SetRGB(9, 13, 16),
                    new RGBClass.SetRGB(120, 33, 26),
                    new RGBClass.SetRGB(120, 33, 26),
                    new RGBClass.SetRGB(120, 33, 26),
                    new RGBClass.SetRGB(120, 33, 26),
                    new RGBClass.SetRGB(120, 33, 26)
                }, "EnemyMinion"));
                rgbLists.Add(new RGBClass.PointerClass(new List<RGBClass.SetRGB>
                {
                    new RGBClass.SetRGB(25, 67, 191)
                }, "ChangeHero"));
            }
            else
            {
                rgbLists.Add(new RGBClass.PointerClass(new List<RGBClass.SetRGB>
                {
                    new RGBClass.SetRGB(9, 13, 17),
                    new RGBClass.SetRGB(119, 56, 54),
                    new RGBClass.SetRGB(119, 56, 54),
                    new RGBClass.SetRGB(119, 56, 54),
                    new RGBClass.SetRGB(119, 56, 54),
                    new RGBClass.SetRGB(119, 56, 54)
                }, "EnemyMinion"));
            }

            #endregion

            rgbLists.Add(new RGBClass.PointerClass(new List<RGBClass.SetRGB>
                {
                    new RGBClass.SetRGB(9, 13, 17),
                    new RGBClass.SetRGB(119, 56, 54),
                    new RGBClass.SetRGB(119, 56, 54),
                    new RGBClass.SetRGB(119, 56, 54),
                    new RGBClass.SetRGB(119, 56, 54),
                    new RGBClass.SetRGB(119, 56, 54)
                }, "EnemyMinion"));

            pointsLists.Add(new RGBClass.PointlerClass(new Point(385, 262)));
            pointsLists.Add(new RGBClass.PointlerClass(new Point(391, 255)));
            pointsLists.Add(new RGBClass.PointlerClass(new Point(397, 250)));
            pointsLists.Add(new RGBClass.PointlerClass(new Point(403, 241)));
            pointsLists.Add(new RGBClass.PointlerClass(new Point(406, 243)));
        }
        private void Restart(Interface itsInterface)
        {
            itsInterface.clientKiller.KillAllLeague();
            itsInterface.clientKiller.RestartAndExit(itsInterface);
        }
        public void YeniAIBaslat(Interface itsInterface)
        {
            try
            {
                itsInterface.logger.Log(true, "Starting NEW AI");

                Thread.Sleep(75000);

                if (Process.GetProcessesByName("League of Legends").Length == 1)
                {
                    itsInterface.logger.Log(true, "League Game has Found");
                    itsInterface.newQueue._playAgain = true;
                    isGameEnd = false;
                    dateTime_1 = DateTime.Now;

                    scrX = PointAl(true).X;
                    scrY = PointAl(true).Y;

                    point_5 = new Point(352, 294);
                    point_6 = new Point(405, 241);
                    point_4 = new Point(scrX + 200, scrY + 150);
                    double_3 = 2.0;
                    double_4 = -2.0;
                    point_0 = new Point(351, 301);
                    point_1 = AnaPointAl(new Point(70, 95));
                    point_2 = AnaPointAl(new Point(15, 47));
                    point_3 = AnaPointAl(new Point(38, 180));

                    isGameEnd = false;
                    itsInterface.logger.Log(true, itsInterface.messages.GameStarted);
                    itsInterface.clientKiller.ActivateGame();
                    EkraniAyarla(point_4);
                    SolTiklat();
                    Thread.Sleep(5000);
                    EndBas();
                    RGBHazirla(itsInterface);
                    StartNewGameAI(itsInterface);
                }
                else
                {
                    //TODO: Recursive kaldır.
                    //Recursive çok var. eğer sistem çalışıyorsa optimize edilcek.
                    itsInterface.logger.Log(false, "League Game is not found.");
                    itsInterface.newQueue.GameAiBool = true;

                    //Eğer socketten gelen bilgi oyunun başladığına işaret etmiyorsa ama lol yine de açıksa AI başlat.
                    if (itsInterface.newQueue.state != "Game in Progress")
                    {
                        if (itsInterface.newQueue.state != "Game Started")
                        {
                            if (Process.GetProcessesByName("League of Legends").Length == 1)
                            {
                                itsInterface.logger.Log(true, "API shows that game is not started but League Game is available.");
                                YeniAIBaslat(itsInterface);
                            }
                        }
                    }

                    //Eğer socketten gelen bilgi oyunun başladığı yönünde ise biraz daha delay ekle.
                    if (itsInterface.newQueue.state == "Game in Progress" || itsInterface.newQueue.state == "Game Started")
                    {

                        if (Process.GetProcessesByName("League of Legends").Length == 0)
                        {
                            itsInterface.logger.Log(true, "Waiting for The League - GameProgress/GameStarted");

                            Thread.Sleep(65000);

                            if (Process.GetProcessesByName("League of Legends").Length == 1)
                            {
                                YeniAIBaslat(itsInterface);
                            }
                            else
                            {
                                itsInterface.logger.Log(false, "Waited too much, restarting...");
                                Restart(itsInterface);
                            }
                        }

                        //TODO: Belirli sayıda buraya gelindiyse yeniden başlat.
                        else
                        {
                            YeniAIBaslat(itsInterface);
                        }
                    }

                    //Eğer socketten gelen bilgi "Reconnect" ise biraz daha delay ekle
                    if (itsInterface.newQueue.state == "Reconnect")
                    {
                        itsInterface.logger.Log(true, "Reconnect State has found");

                        if (Process.GetProcessesByName("League of Legends").Length == 0)
                        {
                            itsInterface.logger.Log(true, "Waiting for The League - Reconnect");

                            Thread.Sleep(65000);

                            if (Process.GetProcessesByName("League of Legends").Length == 1)
                            {
                                YeniAIBaslat(itsInterface);
                            }
                            else
                            {
                                itsInterface.logger.Log(false, "Waited too much, restarting...");
                                Restart(itsInterface);
                            }
                        }
                        else
                        {
                            YeniAIBaslat(itsInterface);
                        }
                    }

                    if (itsInterface.queueId == 2000 || itsInterface.queueId == 2010 || itsInterface.queueId == 2020)
                    {
                        if (Process.GetProcessesByName("League of Legends").Length == 0)
                        {
                            itsInterface.logger.Log(true, "Waiting for The League - Tutorial: " + itsInterface.queueId);

                            Thread.Sleep(65000);

                            if (Process.GetProcessesByName("League of Legends").Length == 1)
                            {
                                YeniAIBaslat(itsInterface);
                            }
                            else
                            {
                                itsInterface.logger.Log(false, "Waited too much, restarting...");
                                Restart(itsInterface);
                            }
                        }
                        else
                        {
                            YeniAIBaslat(itsInterface);
                        }
                    }
                }


            }
            catch (Exception e)
            {
                Console.WriteLine($"YENI AI HATA: {e}");
                throw;
            }
        }
        public void CurrentPlayerStats(Interface itsInterface)
        {
            try
            {
                var liveData = itsInterface.lcuPlugins.GetLiveGameData();
                itsInterface.player.MaxHealth = liveData.Result.ActivePlayer.ChampionStats.MaxHealth.Value;
                itsInterface.player.CurrentHealth = liveData.Result.ActivePlayer.ChampionStats.CurrentHealth.Value;
                itsInterface.player.CurrentGold = liveData.Result.ActivePlayer.CurrentGold.Value;
                itsInterface.player.Level = Convert.ToInt32(liveData.Result.ActivePlayer.Level.Value);
                itsInterface.player.Level_Q = liveData.Result.ActivePlayer.Abilities.Q.AbilityLevel.Value;
                itsInterface.player.Level_W = liveData.Result.ActivePlayer.Abilities.W.AbilityLevel.Value;
                itsInterface.player.Level_E = liveData.Result.ActivePlayer.Abilities.E.AbilityLevel.Value;
                itsInterface.player.Level_R = liveData.Result.ActivePlayer.Abilities.R.AbilityLevel.Value;
                itsInterface.player.Data = liveData.Result;
                //Console.WriteLine(itsInterface.player.Level);
            }
            catch (Exception e)
            {
                //ignored
            }
        }

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.Collect();
            }
        }

        ~GameAi()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            //GC.SuppressFinalize(this);
        }
        #endregion

    }
}
