using AutoIt;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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

namespace Evelynn_Bot.GameAI
{
    public class GameAi : IGameAI
    {
        [DllImport("ImageSearchDLL.dll")]
        public static extern IntPtr ImageSearch(int x, int y, int right, int bottom, [MarshalAs(UnmanagedType.LPStr)] string imagePath);

        private readonly Random _random = new Random();

        public int game_X;
        public int game_Y;
        public int X;
        public int Y;


        public string[] UseImageSearch(string imgPath, string tolerance)
        {
            try
            {
                imgPath = "*" + tolerance + " " + imgPath;
                var lol = AutoItX.WinGetPos("League of Legends (TM) Client");
                IntPtr result = ImageSearch(lol.X, lol.Y, lol.Right, lol.Bottom, imgPath);
                string res = Marshal.PtrToStringAnsi(result);
                if (res[0] == '0') return null;
                string[] data = res.Split('|');
                int x; int y;
                int.TryParse(data[1], out x);
                int.TryParse(data[2], out y);
                Dispose(true);
                return data;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        public bool ImageSearchForGameStart(string path, string tolerance, string message, Interface itsInterface)
        {
            int x;
            int y;
            try
            {
                string[] results = UseImageSearch(path, tolerance);
                if (results != null)
                {
                    Int32.TryParse(results[1], out x);
                    Int32.TryParse(results[2], out y);
                    game_X = x;
                    game_Y = y;
                    return itsInterface.Result(true, message);
                }
                return itsInterface.Result(false, "");
            }
            catch (Exception e)
            {
                return itsInterface.Result(false, "ERROR!");
            }
        }
        public bool ImageSearch(string path, string tolerance, string message, Interface itsInterface)
        {
            int x;
            int y;
            try
            {
                string[] results = UseImageSearch(path, tolerance);
                if (results != null)
                {
                    Int32.TryParse(results[1], out x);
                    Int32.TryParse(results[2], out y);
                    X = x;
                    Y = y;
                    return itsInterface.Result(true, message);
                }
                return itsInterface.Result(false, "");
            }
            catch (Exception e)
            {
                return itsInterface.Result(false, "ERROR!");
            }
        }

        public bool ImageSearchOnlyForControl(string path, string tolerance, string message, Interface itsInterface)
        {
            try
            {
                string[] results = UseImageSearch(path, tolerance);
                if (results != null)
                {
                    return itsInterface.Result(true, message);
                }
                return itsInterface.Result(false, "");
            }
            catch (Exception e)
            {
                return itsInterface.Result(false, "ERROR!");
            }
        }

        public bool RGBPixel(Color color)
        {
            var lol = AutoItX.WinGetPos("League of Legends (TM) Client");
            if (PixelSearch(lol, color.ToArgb(), 0).X <= 1 || PixelSearch(lol, color.ToArgb(), 0).Y <= 1)
            {
                return false;
            }
            else
            {
                X = PixelSearch(lol, color.ToArgb(), 0).X;
                Y = PixelSearch(lol, color.ToArgb(), 0).Y;
                return true;
            }
        }

        public bool RGBPixelOnlyForControl(Color color)
        {
            var lol = AutoItX.WinGetPos("League of Legends (TM) Client");
            if (PixelSearch(lol, color.ToArgb(), 0).X <= 1 || PixelSearch(lol, color.ToArgb(), 0).Y <= 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Point PixelSearch(Rectangle rect, int PixelColor, int Shade_Variation)
        {
            try
            {
                Color Pixel_Color = Color.FromArgb(PixelColor);
                Point Pixel_Coords = new Point(-1, -1);
                Bitmap RegionIn_Bitmap = CaptureScreenRegion(rect);
                BitmapData RegionIn_BitmapData = RegionIn_Bitmap.LockBits(new Rectangle(0, 0, RegionIn_Bitmap.Width, RegionIn_Bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                int[] Formatted_Color = new int[3] { Pixel_Color.B, Pixel_Color.G, Pixel_Color.R }; //bgr

                unsafe
                {
                    for (int y = 0; y < RegionIn_BitmapData.Height; y++)
                    {
                        byte* row = (byte*)RegionIn_BitmapData.Scan0 + (y * RegionIn_BitmapData.Stride);

                        for (int x = 0; x < RegionIn_BitmapData.Width; x++)
                        {
                            if (row[x * 3] >= (Formatted_Color[0] - Shade_Variation) & row[x * 3] <= (Formatted_Color[0] + Shade_Variation)) //blue
                            {
                                if (row[(x * 3) + 1] >= (Formatted_Color[1] - Shade_Variation) & row[(x * 3) + 1] <= (Formatted_Color[1] + Shade_Variation)) //green
                                {
                                    if (row[(x * 3) + 2] >= (Formatted_Color[2] - Shade_Variation) & row[(x * 3) + 2] <= (Formatted_Color[2] + Shade_Variation)) //red
                                    {
                                        Pixel_Coords = new Point(x + rect.X, y + rect.Y);
                                        goto end;
                                    }
                                }
                            }
                        }
                    }
                }

                end:
                Dispose(true);
                return Pixel_Coords;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Dispose(true);
                return new Point();
            }
        }
        private Bitmap CaptureScreenRegion(Rectangle rect)
        {
            try
            {
                Bitmap BMP = new Bitmap(rect.Width, rect.Height, PixelFormat.Format24bppRgb);
                Graphics GFX = Graphics.FromImage(BMP);
                GFX.CopyFromScreen(rect.X, rect.Y, 0, 0, rect.Size, CopyPixelOperation.SourceCopy);
                Dispose(true);
                return BMP;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Dispose(true);
                return null;
            }
        }


        public bool TowerCheck(Interface itsInterface)
        {
            if (RGBPixelOnlyForControl(itsInterface.ImgPaths.TowerColor))
            {
                return true;
            }
            return false;
        }

        #region TEST

        public static class Class28
        {
            public class Class29
            {
                private int int_0;

                private int int_1;

                private int int_2;

                public int Int32_0
                {
                    get
                    {
                        return int_0;
                    }
                    set
                    {
                        int_0 = value;
                    }
                }

                public int Int32_1
                {
                    get
                    {
                        return int_1;
                    }
                    set
                    {
                        int_1 = value;
                    }
                }

                public int Int32_2
                {
                    get
                    {
                        return int_2;
                    }
                    set
                    {
                        int_2 = value;
                    }
                }

                public Class29(int int_3, int int_4, int int_5)
                {
                    Int32_0 = int_3;
                    Int32_1 = int_4;
                    Int32_2 = int_5;
                }
            }

            public class Class30
            {
                private List<Class29> list_0;

                private List<Point> list_1;

                private int int_0;

                private string string_0;

                public List<Class29> List_0
                {
                    get
                    {
                        return list_0;
                    }
                    set
                    {
                        list_0 = value;
                    }
                }

                public List<Point> List_1
                {
                    get
                    {
                        return list_1;
                    }
                    set
                    {
                        list_1 = value;
                    }
                }

                public int Int32_0
                {
                    get
                    {
                        return int_0;
                    }
                    set
                    {
                        int_0 = value;
                    }
                }

                public string String_0
                {
                    get
                    {
                        return string_0;
                    }
                    set
                    {
                        string_0 = value;
                    }
                }

                public Class30(List<Class29> list_2, string string_1)
                {
                    List_0 = list_2;
                    Int32_0 = 0;
                    List_1 = new List<Point>();
                    String_0 = string_1;
                }
            }

            public class Class31
            {
                private Point point_0;

                private bool bool_0;

                public Point Point_0
                {
                    get
                    {
                        return point_0;
                    }
                    set
                    {
                        point_0 = value;
                    }
                }

                public bool Boolean_0
                {
                    get
                    {
                        return bool_0;
                    }
                    set
                    {
                        bool_0 = value;
                    }
                }

                public Class31(Point point_1)
                {
                    Point_0 = point_1;
                    Boolean_0 = false;
                }
            }

            public class Class32
            {
                private bool bool_0;

                private bool bool_1;

                private double double_0;

                private bool bool_2;

                private double double_1;

                private double double_2;

                private double double_3;

                private int int_0;

                public bool Boolean_0
                {
                    get
                    {
                        return bool_0;
                    }
                    set
                    {
                        bool_0 = value;
                    }
                }

                public bool Boolean_1
                {
                    get
                    {
                        return bool_1;
                    }
                    set
                    {
                        bool_1 = value;
                    }
                }

                public double Double_0
                {
                    get
                    {
                        return double_0;
                    }
                    set
                    {
                        double_0 = value;
                    }
                }

                public bool Boolean_2
                {
                    get
                    {
                        return bool_2;
                    }
                    set
                    {
                        bool_2 = value;
                    }
                }

                public double Double_1
                {
                    get
                    {
                        return double_1;
                    }
                    set
                    {
                        double_1 = value;
                    }
                }

                public double Double_2
                {
                    get
                    {
                        return double_2;
                    }
                    set
                    {
                        double_2 = value;
                    }
                }

                public double Double_3
                {
                    get
                    {
                        return double_3;
                    }
                    set
                    {
                        double_3 = value;
                    }
                }

                public int Int32_0
                {
                    get
                    {
                        return int_0;
                    }
                    set
                    {
                        int_0 = value;
                    }
                }
            }

            private sealed class Class33
            {
                public Random random_0;

                internal char method_0(string string_0)
                {
                    return string_0[random_0.Next(string_0.Length)];
                }
            }


            public static double smethod_7(PointF pointF_0, PointF pointF_1, PointF pointF_2)
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
                        return Math.Sqrt(num * num + num2 * num2) * -1.0;
                    }
                    return Math.Sqrt(num * num + num2 * num2);
                }
                pointF = pointF_1;
                num = pointF_0.X - pointF_1.X;
                num2 = pointF_0.Y - pointF_1.Y;
                return Math.Sqrt(num * num + num2 * num2);
            }
        }

        private List<Class28.Class30> list_0 = new List<Class28.Class30>();

        public static Bitmap BitmapAl()
        {
            Rectangle rectangle = AutoItX.WinGetPos("League of Legends (TM) Client");
            Bitmap bitmap = new Bitmap(rectangle.Width, rectangle.Height, PixelFormat.Format24bppRgb);
            Graphics.FromImage(bitmap).CopyFromScreen(rectangle.X, rectangle.Y, 0, 0, rectangle.Size, CopyPixelOperation.SourceCopy);
            return bitmap;
        }

        public static void HepsiniTarat(Bitmap bitmap_0, List<Class28.Class30> list_0)
        {
            list_0.ToList().ForEach(delegate (Class28.Class30 class30_0)
            {
                class30_0.Int32_0 = 0;
            });
            list_0.ToList().ForEach(delegate (Class28.Class30 class30_0)
            {
                class30_0.List_1 = new List<Point>();
            });
            BitmapData bitmapData = bitmap_0.LockBits(new Rectangle(0, 0, bitmap_0.Size.Width, bitmap_0.Size.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr scan = bitmapData.Scan0;
            int num = bitmapData.Stride * bitmap_0.Height;
            byte[] array = new byte[num];
            byte[] byte_2 = new byte[num / 3];
            byte[] byte_1 = new byte[num / 3];
            byte[] byte_0 = new byte[num / 3];
            Marshal.Copy(scan, array, 0, num);
            int int_ = 0;
            int stride = bitmapData.Stride;
            for (int int_2 = 0; int_2 < bitmap_0.Height; int_2++)
            {
                int int_0;
                for (int_0 = 0; int_0 < bitmap_0.Width; int_0++)
                {
                    byte_0[int_] = array[int_2 * stride + int_0 * 3];
                    byte_1[int_] = array[int_2 * stride + int_0 * 3 + 1];
                    byte_2[int_] = array[int_2 * stride + int_0 * 3 + 2];
                    list_0.ForEach(delegate (Class28.Class30 class30_0)
                    {
                        if (Math.Abs(class30_0.List_0[class30_0.Int32_0].Int32_2 - byte_0[int_]) <= 5 && Math.Abs(class30_0.List_0[class30_0.Int32_0].Int32_1 - byte_1[int_]) <= 5 && Math.Abs(class30_0.List_0[class30_0.Int32_0].Int32_0 - byte_2[int_]) <= 5)
                        {
                            class30_0.Int32_0++;
                        }
                        else
                        {
                            class30_0.Int32_0 = 0;
                        }
                        if (class30_0.Int32_0 == class30_0.List_0.Count)
                        {
                            class30_0.List_1.Add(new Point(int_0, int_2));
                            class30_0.Int32_0 = 0;
                        }
                    });
                    int num2 = int_;
                    int_ = num2 + 1;
                }
            }
            bitmap_0.UnlockBits(bitmapData);
        }

        private static double Hesap1(double double_0, double double_1, double double_2, double double_3)
        {
            return Math.Sqrt(Math.Pow(double_2 - double_0, 2.0) + Math.Pow(double_3 - double_1, 2.0));
        }
        public static double Hesap0(double double_0, double double_1)
        {
            return Math.Sqrt(Math.Pow(double_0, 2.0) + Math.Pow(double_1, 2.0));
        }

        public static void PosHesapla(double double_0, double double_1, double double_2, double double_3, double double_4, double double_5, double double_6, double double_7, double double_8)
        {
            Random random = new Random();
            double num = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            double num4 = 0.0;
            double num5 = Math.Sqrt(2.0);
            double num6 = Math.Sqrt(3.0);
            double num7 = Math.Sqrt(5.0);
            int num8 = (int)Hesap1(Math.Round(double_0), Math.Round(double_1), Math.Round(double_2), Math.Round(double_3));
            uint num9 = (uint)(Environment.TickCount + 10000);
            int num10 = 0;
            while (Environment.TickCount <= num9)
            {
                double num11 = Hesap0(double_0 - double_2, double_1 - double_3);
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
                num += double_4 * (double_2 - double_0) / num11;
                num2 += double_4 * (double_3 - double_1) / num11;
                if (Hesap0(num, num2) > num13)
                {
                    double num14 = num13 / 2.0 + (double)random.Next((int)(Math.Round(num13) / 2.0));
                    double num15 = Math.Sqrt(num * num + num2 * num2);
                    num = num / num15 * num14;
                    num2 = num2 / num15 * num14;
                }
                int num16 = (int)Math.Round(double_0);
                int num17 = (int)Math.Round(double_1);
                double_0 += num;
                double_1 += num2;
                if ((double)num16 != Math.Round(double_0) || (double)num17 != Math.Round(double_1))
                {
                    Tiklat(new Point((int)double_0, (int)double_1));
                    num10++;
                    if (num10 % 5 == 0)
                    {
                        Thread.Sleep(1);
                    }
                }
                if (!(Hesap0(double_0 - double_2, double_1 - double_3) >= 1.0))
                {
                    break;
                }
            }
            if (Math.Round(double_2) != Math.Round(double_0) || Math.Round(double_3) != Math.Round(double_1))
            {
                Tiklat(new Point((int)double_2, (int)double_3));
            }
        }

        [DllImport("user32.dll")]
        internal static extern uint SendInput(uint uint_0, [In][MarshalAs(UnmanagedType.LPArray)] GStruct1[] gstruct1_0, int int_0);

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(GEnum3 genum3_0);

        public static void Tiklat(Point point_0)
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

        public static void TiklatTus(GEnum8 genum8_0)
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

        public static void EkraniAyarla(Point point_0)
        {
            Random random = new Random();
            double double_ = random.Next(25, 35);
            double double_2 = random.Next(50, 60);
            double double_3 = random.Next(1, 2);
            double double_4 = random.Next(3, 4);
            double double_5 = 1.0;
            PosHesapla(Cursor.Position.X, Cursor.Position.Y, point_0.X, point_0.Y, double_, double_2, double_3, double_4, double_5);
        }

        public static void TusuAyarla(GEnum8 genum8_0)
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


        #endregion

        public Point AnaPointAl(Point point_7)
        {
            return new Point(point_7.X + int_0, point_7.Y + int_1);
        }
        public static Point PointAl(bool bool_0)
        {
            Size size = Screen.PrimaryScreen.Bounds.Size;
            int num = 800;
            int num2 = 600;
            if (bool_0)
            {
                num = 400;
                num2 = 300;
            }
            return new Point((size.Width != num) ? ((size.Width - num) / 2) : 0, (size.Height != num2) ? ((size.Height - num2) / 2) : 0);
        }

        private int int_0;
        private int int_1;
        #endregion


        public bool AllyMinionCheck(Interface itsInterface)
        {
            int_0 = PointAl(true).X;
            int_1 = PointAl(true).Y;
            Bitmap bitmap_;
            list_0.Add(new Class28.Class30(new List<Class28.Class29>
            {
                new Class28.Class29(8, 12, 16),
                new Class28.Class29(44, 89, 119),
                new Class28.Class29(44, 89, 119),
                new Class28.Class29(44, 89, 119),
                new Class28.Class29(44, 89, 119),
                new Class28.Class29(44, 89, 119),
                new Class28.Class29(44, 89, 119),
                new Class28.Class29(44, 89, 119),
                new Class28.Class29(44, 89, 119),
                new Class28.Class29(44, 89, 119),
                new Class28.Class29(44, 89, 119),
                new Class28.Class29(44, 89, 119),
                new Class28.Class29(44, 89, 119)
            }, "AllyMinion"));

            try
            {
                bitmap_ = BitmapAl();
                HepsiniTarat(bitmap_, list_0);
            }
            catch (Exception)
            {
                Console.WriteLine("Error while creating bitmap");
            }

            List<Point> list4 = list_0.First((Class28.Class30 class30_0) => class30_0.String_0 == "AllyMinion").List_1;
            Point AllyMinionPos = ((list4.Count == 0) ? new Point(-1, -1) : new Point(list4[0].X + 30, list4[0].Y + 30));
            Console.WriteLine(AllyMinionPos);
            if (AllyMinionPos.X <= 1 || AllyMinionPos.Y <= 1)
            {
                return false;
            }
            else
            {
                X = AllyMinionPos.X;
                Y = AllyMinionPos.Y;

                EkraniAyarla(AnaPointAl(AllyMinionPos));
                TusuAyarla(GEnum8.KEY_A);


                return true;
            }
        }

        public void SkillUp(string skill, string skill2)
        {
            AutoItX.Send("{CTRLDOWN}");
            AutoItX.Send(skill);
            AutoItX.Send("{CTRLUP}");
            AutoItX.Send(skill2);
        }

        public void HitMove(int x, int y)
        {
            AutoItX.MouseClick("RIGHT", x - 17, y + 35, 1, 1);
            AutoItX.Send("a");
            Thread.Sleep(700);
            AutoItX.MouseClick("LEFT", x - 17, y + 35, 1, 1);
            AutoItX.Send("a");
            AutoItX.Send("a");
            AutoItX.MouseClick("RIGHT", x - 17, y + 35, 1, 1);
            AutoItX.MouseClick("RIGHT", x - 17, y + 35, 1, 1);
            AutoItX.MouseClick("RIGHT", x - 17, y + 35, 1, 1);

        }

        public async Task Combo(int x, int y)
        {
            AutoItX.MouseClick("LEFT", x + 65, y + 75, 1, 0);
            await Task.Delay(200);
            AutoItX.Send("q");
            AutoItX.Send("w");
            AutoItX.Send("e");
            await Task.Delay(200);
            AutoItX.Send("r");
            await Task.Delay(1000);
            AutoItX.Send("a");
        }

        public void Heal()
        {
            AutoItX.Send("f");
        }

        public async Task GoSafeArea()
        {
            AutoItX.MouseClick("RIGHT", game_X + 30, game_Y - 20, 1, 0);
            AutoItX.MouseClick("RIGHT", game_X + 30, game_Y - 20, 1, 0);
            await Task.Delay(1500);
        }

        public async Task GoMid()
        {
            AutoItX.MouseClick("RIGHT", game_X + 42, game_Y - 30, 1, 0);
            AutoItX.MouseClick("RIGHT", game_X + 42, game_Y - 30, 1, 0);
            await Task.Delay(1900);
        }

        public void GoTop()
        {
            AutoItX.MouseClick("RIGHT", game_X + 25, game_Y - 55, 1, 0);
            AutoItX.MouseClick("RIGHT", game_X + 25, game_Y - 55, 1, 0);
        }

        public void GoBot()
        {
            AutoItX.MouseClick("RIGHT", game_X + 73, game_Y, 1, 0);
            AutoItX.MouseClick("RIGHT", game_X + 73, game_Y, 1, 0);
        }

        public async Task GoBase()
        {
            AutoItX.MouseClick("RIGHT", game_X + 31, game_Y - 19, 1, 0);
            AutoItX.MouseClick("RIGHT", game_X + 31, game_Y - 19, 1, 0);
            await Task.Delay(14000);
            AutoItX.Send("b");
            AutoItX.Send("b");
            AutoItX.Send("b");
            await Task.Delay(14000);
            AutoItX.MouseClick("RIGHT", game_X + 31, game_Y - 19, 1, 0);
            AutoItX.MouseClick("RIGHT", game_X + 31, game_Y - 19, 1, 0);
        }

        public void PickTutorialChampionAI()
        {
            AutoItX.MouseClick("RIGHT", game_X - 199, game_Y -218, 1, 0);
            AutoItX.MouseClick("RIGHT", game_X - 199, game_Y - 218, 1, 0);
            AutoItX.MouseClick("RIGHT", game_X - 199, game_Y - 218, 1, 0);
        }

        public void BuyItem()
        {
            AutoItX.Send("p");
            Thread.Sleep(2000);
            AutoItX.MouseClick("LEFT", game_X - 158, game_Y - 172, 1, 0);
            AutoItX.MouseDown();
            Thread.Sleep(500);
            AutoItX.MouseUp();
            Thread.Sleep(700);
            for (int i = 0; i < 10; i++)
            {
                AutoItX.MouseClick("LEFT", game_X - 158, game_Y - 172, 1, 0);
                Thread.Sleep(1500);
                AutoItX.MouseClick("RIGHT", game_X - 158, game_Y - 172, 1, 0);
                Thread.Sleep(500);
                AutoItX.MouseClick("RIGHT", game_X - 158, game_Y - 172, 1, 0);
                Thread.Sleep(200);
                AutoItX.MouseClick("LEFT", game_X - 158, game_Y - 172, 1, 0);
                Thread.Sleep(200);
                AutoItX.MouseClick("RIGHT", game_X - 158, game_Y - 172, 1, 0);
                Thread.Sleep(200);
                AutoItX.MouseClick("RIGHT", game_X - 158, game_Y - 172, 1, 0);
                AutoItX.MouseClick("LEFT", game_X - 13, game_Y - 48, 1, 0);
                AutoItX.MouseClick("LEFT", game_X - 13, game_Y - 48, 5, 1);
                Thread.Sleep(200);
                AutoItX.MouseClick("LEFT", game_X - 13, game_Y - 48, 5, 1);
            }
            AutoItX.Send("p");
        }

        public void RandomLaner()
        {
            int random =_random.Next(1, 4);
            if (random == 1)
            {
                GoTop();
                Console.WriteLine("Going to Top!");
            }

            if (random == 2)
            {
                GoMid();
                Console.WriteLine("Going to Mid");
            }

            if (random == 3)
            {
                GoBot();
                Console.WriteLine("Going to Bot!");
            }
        }

        public bool processExist(string win, Interface itsInterface)
        {
            int a = AutoItX.ProcessExists(win);
            return itsInterface.Result(Convert.ToBoolean(a), "");
        }

        #region TutorialAI
        public void TutorialAI_1(Interface itsInterface)
        {
            while (IsGameStarted(itsInterface) == false)
            {
                Thread.Sleep(15000);
                IsGameStarted(itsInterface);
            }

            while (processExist("League of Legends.exe", itsInterface))
            {
                while (ImageSearchForGameStart(itsInterface.ImgPaths.game_started, "2", itsInterface.messages.GameStarted, itsInterface))
                {
                    GoMid();

                    if (ImageSearch(itsInterface.ImgPaths.shop, "1", "", itsInterface))
                    {
                        AutoItX.Send("p");
                    }

                    if (ImageSearch(itsInterface.ImgPaths.enemy_health, "2", itsInterface.messages.SuccessEnemyChampion, itsInterface))
                    {
                        AutoItX.MouseClick("RIGHT", X + 65, Y + 75, 1, 0);
                        HitMove(X, Y);
                        Combo(X, Y);
                        AutoItX.Send("d");
                        AutoItX.Send("f");
                        Thread.Sleep(1500);
                    }

                    while (ImageSearch(itsInterface.ImgPaths.minions_tutorial, "3", itsInterface.messages.SuccessMinion, itsInterface))
                    {
                        HitMove(X, Y);
                        Thread.Sleep(500);

                        if (ImageSearch(itsInterface.ImgPaths.enemy_minions, "3", itsInterface.messages.SuccessEnemyChampion, itsInterface))
                        {
                            AutoItX.MouseClick("RIGHT", X + 27, Y + 20, 1, 0);
                            AutoItX.Send("q");
                            Thread.Sleep(1500);
                            AutoItX.Send("w");
                            Thread.Sleep(1500);
                            AutoItX.Send("e");
                            Thread.Sleep(1500);
                            AutoItX.Send("r");
                            AutoItX.Send("d");
                            AutoItX.Send("f");
                            Thread.Sleep(1500);
                        }

                        if (ImageSearch(itsInterface.ImgPaths.enemy_health, "2", itsInterface.messages.SuccessEnemyChampion, itsInterface))
                        {
                            AutoItX.MouseClick("RIGHT", X + 65, Y + 75, 1, 0);
                            HitMove(X, Y);
                            Combo(X, Y);
                            AutoItX.Send("d");
                            AutoItX.Send("f");
                            Thread.Sleep(1500);
                        }

                        if (ImageSearch(itsInterface.ImgPaths.tower, "2", itsInterface.messages.SuccessEnemyChampion, itsInterface))
                        {
                            AutoItX.MouseClick("RIGHT", X + 65, Y + 75, 1, 0);
                            AutoItX.MouseClick("RIGHT", X + 65, Y + 75, 1, 0);
                            AutoItX.MouseClick("RIGHT", X + 65, Y + 75, 1, 0);
                            Thread.Sleep(3000);
                        }
                        GoMid();
                    }

                    Thread.Sleep(6000);
                    PickTutorialChampionAI();
                }

            }

        }

        public void TutorialAI_2(Interface itsInterface)
        {
            while (IsGameStarted(itsInterface) == false)
            {
                Thread.Sleep(15000);
                IsGameStarted(itsInterface);
            }

            Thread.Sleep(5000);
            SkillUp("q", "j");
            Thread.Sleep(5000);
            for (int i = 0; i < 2; i++)
            {
                BuyItem();
            }

            while (processExist("League of Legends.exe", itsInterface))
            {
                while (ImageSearchForGameStart(itsInterface.ImgPaths.game_started, "2", itsInterface.messages.GameStarted, itsInterface))
                {
                    SkillUp("q", "j");
                    GoMid();
                    Thread.Sleep(4000);

                    if (ImageSearch(itsInterface.ImgPaths.enemy_health, "2", itsInterface.messages.SuccessEnemyChampion, itsInterface))
                    {
                        AutoItX.MouseClick("RIGHT", X + 65, Y + 75, 1, 0);
                        HitMove(X, Y);
                        Combo(X, Y);
                        AutoItX.Send("d");
                        AutoItX.Send("f");
                        Thread.Sleep(1500);
                    }

                    while (ImageSearch(itsInterface.ImgPaths.minions, "3", itsInterface.messages.SuccessMinion, itsInterface))
                    {
                        SkillUp("q", "j");
                        SkillUp("w", "k");
                        SkillUp("e", "m");
                        SkillUp("r", "l");
                        HitMove(X, Y);
                        Thread.Sleep(500);

                        if (ImageSearch(itsInterface.ImgPaths.enemy_minions, "3", itsInterface.messages.SuccessEnemyChampion, itsInterface))
                        {
                            AutoItX.MouseClick("RIGHT", X + 27, Y + 20, 1, 0);
                            AutoItX.Send("q");
                            Thread.Sleep(1500);
                            AutoItX.Send("w");
                            Thread.Sleep(1500);
                            AutoItX.Send("e");
                            Thread.Sleep(1500);
                            AutoItX.Send("r");
                            AutoItX.Send("d");
                            AutoItX.Send("f");
                            Thread.Sleep(1500);
                        }

                        if (ImageSearch(itsInterface.ImgPaths.enemy_health, "2", itsInterface.messages.SuccessEnemyChampion, itsInterface))
                        {
                            AutoItX.MouseClick("RIGHT", X + 65, Y + 75, 1, 0);
                            HitMove(X, Y);
                            Combo(X, Y);
                            AutoItX.Send("d");
                            AutoItX.Send("f");
                            Thread.Sleep(1500);
                        }

                        if (ImageSearch(itsInterface.ImgPaths.tower, "2", itsInterface.messages.SuccessEnemyChampion, itsInterface))
                        {
                            AutoItX.MouseClick("RIGHT", X + 65, Y + 75, 1, 0);
                            AutoItX.MouseClick("RIGHT", X + 65, Y + 75, 1, 0);
                            AutoItX.MouseClick("RIGHT", X + 65, Y + 75, 1, 0);
                            Thread.Sleep(3000);
                        }
                    }
                }
            }
            Thread.Sleep(10000);
        }

        public bool IsGameStarted(Interface itsInterface)
        {
            if (ImageSearchForGameStart(itsInterface.ImgPaths.game_started, "2", itsInterface.messages.GameStarted, itsInterface))
            {
                return true;
            }
            return false;
        }


        #endregion

        public void CurrentPlayerStats(Interface itsInterface)
        {
            var liveData = itsInterface.lcuPlugins.GetLiveGameData();
            itsInterface.player.MaxHealth = liveData.Result.activePlayer.championStats.maxHealth;
            itsInterface.player.CurrentHealth = liveData.Result.activePlayer.championStats.currentHealth;
            itsInterface.player.CurrentGold = liveData.Result.activePlayer.currentGold;
            itsInterface.player.Level = liveData.Result.activePlayer.level;
        }

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.Collect();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
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
