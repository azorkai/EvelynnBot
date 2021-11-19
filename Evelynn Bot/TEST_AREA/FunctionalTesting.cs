using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Evelynn_Bot.Constants;
using Evelynn_Bot.GameAI;

namespace Evelynn_Bot.TEST_AREA
{
    class FunctionalTesting
    {
        private Point point_0;

        private int scrX;
        private int scrY;

        private List<GameAi.RGBClass.PointerClass> rgbLists = new List<GameAi.RGBClass.PointerClass>();
        private List<GameAi.RGBClass.PointlerClass> pointsLists = new List<GameAi.RGBClass.PointlerClass>();

        public void TestAIx(Interface itsInterface)
        {
            Console.WriteLine("TEST AI - Activated");

            RGBHazirla(itsInterface);

            //Point point = new Point(400, 300);
            Point point = new Point(1024, 768);


            while (true)
            {
                //Point Minimap = new Point(TestAI.GetColorPosition(Color.FromArgb(255, 255, 255)).X + 20, TestAI.GetColorPosition(Color.FromArgb(255, 255, 255)).Y + 15);
                //Point EnemyMinion = new Point(TestAI.GetColorPosition(Color.FromArgb(119, 56, 54)).X + 30, TestAI.GetColorPosition(Color.FromArgb(119, 56, 54)).Y + 30);
                //Point AllyMinion = new Point(TestAI.GetColorPosition(Color.FromArgb(44, 89, 119)).X + 30, TestAI.GetColorPosition(Color.FromArgb(44, 89, 119)).Y + 30);
                //Point Enemy = new Point(TestAI.GetColorPosition(Color.FromArgb(48, 3, 0)).X + 65, TestAI.GetColorPosition(Color.FromArgb(48, 3, 0)).Y + 45);

                //Point Fountain = new Point(TestAI.GetColorPosition(Color.FromArgb(103, 103, 65)).X + 20, TestAI.GetColorPosition(Color.FromArgb(103, 103, 65)).Y + 15);
                //Point Fountain2 = new Point(TestAI.GetColorPosition(Color.FromArgb(64, 75, 52)).X + 20, TestAI.GetColorPosition(Color.FromArgb(64, 75, 52)).Y + 15);
                //Point Fountain3 = new Point(TestAI.GetColorPosition(Color.FromArgb(22, 126, 127)).X + 20, TestAI.GetColorPosition(Color.FromArgb(22, 126, 127)).Y + 15);

                //bool isInShop = TestAI.GetColorPosition(Color.FromArgb(141, 121, 83)).X > -1 || TestAI.GetColorPosition(Color.FromArgb(141, 121, 83)).Y > -1;
                //bool isInBase = Fountain.X > -1 || Minimap.X == -1;

                
            }

        }
        private double PointerMath(Point point_0, Point point_1) => Math.Sqrt(Math.Pow((double)(point_1.X - point_0.X), 2.0) + Math.Pow((double)(point_1.Y - point_0.Y), 2.0) * 1.0);

        private Bitmap BitmapAl(int screenX, int screenY, int rectangleX, int rectangleY)
        {
            Rectangle rectangle = new Rectangle(screenX, screenY, rectangleX, rectangleY);
            Bitmap bitmap = new Bitmap(rectangle.Width, rectangle.Height, PixelFormat.Format24bppRgb);
            Graphics.FromImage(bitmap).CopyFromScreen(screenX, screenY, 0, 0, rectangle.Size, CopyPixelOperation.SourceCopy);
            return bitmap;
        }

        private void HepsiniTarat(Bitmap bmp, List<GameAi.RGBClass.PointerClass> rgbLists)
        {
            rgbLists.ToList().ForEach(delegate (GameAi.RGBClass.PointerClass pClass)
            {
                pClass.tmpB = 0;
            });

            rgbLists.ToList().ForEach(delegate (GameAi.RGBClass.PointerClass pClass)
            {
                pClass.pointerList = new List<Point>();
            });

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
                    rgbLists.ForEach(delegate (GameAi.RGBClass.PointerClass pClass)
                    {
                        ////TODO: Huge CPU - Fix!
                        if (Math.Abs(pClass.rgbLists[pClass.tmpB].B - byte_0[int_]) <= 5 && Math.Abs(pClass.rgbLists[pClass.tmpB].G - byte_1[int_]) <= 5 && Math.Abs(pClass.rgbLists[pClass.tmpB].R - byte_2[int_]) <= 5)
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
            bmp.UnlockBits(bitmapData);
        }

        public void RGBHazirla(Interface itsInterface)
        {
            rgbLists.Add(new GameAi.RGBClass.PointerClass(new List<GameAi.RGBClass.SetRGB>
            {
                new GameAi.RGBClass.SetRGB(103, 103, 65),
                new GameAi.RGBClass.SetRGB(64, 75, 52),
                new GameAi.RGBClass.SetRGB(22, 126, 127)
            }, "Fountain"));
            rgbLists.Add(new GameAi.RGBClass.PointerClass(new List<GameAi.RGBClass.SetRGB>
            {
                new GameAi.RGBClass.SetRGB(141, 121, 83),
                new GameAi.RGBClass.SetRGB(141, 121, 83),
                new GameAi.RGBClass.SetRGB(141, 121, 83),
                new GameAi.RGBClass.SetRGB(141, 121, 83),
                new GameAi.RGBClass.SetRGB(141, 121, 83),
                new GameAi.RGBClass.SetRGB(141, 121, 83),
                new GameAi.RGBClass.SetRGB(141, 121, 83),
                new GameAi.RGBClass.SetRGB(141, 121, 83),
                new GameAi.RGBClass.SetRGB(141, 121, 83),
                new GameAi.RGBClass.SetRGB(141, 121, 83),
                new GameAi.RGBClass.SetRGB(141, 121, 83),
                new GameAi.RGBClass.SetRGB(141, 121, 83)
            }, "Shop"));

            rgbLists.Add(new GameAi.RGBClass.PointerClass(new List<GameAi.RGBClass.SetRGB>
            {
                new GameAi.RGBClass.SetRGB(255, 255, 255),
                new GameAi.RGBClass.SetRGB(255, 255, 255),
                new GameAi.RGBClass.SetRGB(255, 255, 255),
                new GameAi.RGBClass.SetRGB(255, 255, 255),
                new GameAi.RGBClass.SetRGB(255, 255, 255),
                new GameAi.RGBClass.SetRGB(255, 255, 255)
            }, "Minimap"));
            rgbLists.Add(new GameAi.RGBClass.PointerClass(new List<GameAi.RGBClass.SetRGB>
            {
                new GameAi.RGBClass.SetRGB(8, 8, 8),
                new GameAi.RGBClass.SetRGB(48, 3, 0),
                new GameAi.RGBClass.SetRGB(48, 3, 0)
            }, "EnemyHero"));
            rgbLists.Add(new GameAi.RGBClass.PointerClass(new List<GameAi.RGBClass.SetRGB>
            {
                new GameAi.RGBClass.SetRGB(8, 12, 16),
                new GameAi.RGBClass.SetRGB(44, 89, 119),
                new GameAi.RGBClass.SetRGB(44, 89, 119),
                new GameAi.RGBClass.SetRGB(44, 89, 119),
                new GameAi.RGBClass.SetRGB(44, 89, 119),
                new GameAi.RGBClass.SetRGB(44, 89, 119),
                new GameAi.RGBClass.SetRGB(44, 89, 119),
                new GameAi.RGBClass.SetRGB(44, 89, 119),
                new GameAi.RGBClass.SetRGB(44, 89, 119),
                new GameAi.RGBClass.SetRGB(44, 89, 119),
                new GameAi.RGBClass.SetRGB(44, 89, 119),
                new GameAi.RGBClass.SetRGB(44, 89, 119),
                new GameAi.RGBClass.SetRGB(44, 89, 119)
            }, "AllyMinion"));

            #region TODO CHANGE TUTO

            if (itsInterface.queueId == 2000) // EĞER OYUN TUTORİAL 1 İSE MİNYON RENGİ DEĞİŞİYOR
            {
                rgbLists.Add(new GameAi.RGBClass.PointerClass(new List<GameAi.RGBClass.SetRGB>
                {
                    new GameAi.RGBClass.SetRGB(9, 13, 16),
                    new GameAi.RGBClass.SetRGB(120, 33, 26),
                    new GameAi.RGBClass.SetRGB(120, 33, 26),
                    new GameAi.RGBClass.SetRGB(120, 33, 26),
                    new GameAi.RGBClass.SetRGB(120, 33, 26),
                    new GameAi.RGBClass.SetRGB(120, 33, 26)
                }, "EnemyMinion"));
                rgbLists.Add(new GameAi.RGBClass.PointerClass(new List<GameAi.RGBClass.SetRGB>
                {
                    new GameAi.RGBClass.SetRGB(25, 67, 191)
                }, "ChangeHero"));
            }
            else
            {
                rgbLists.Add(new GameAi.RGBClass.PointerClass(new List<GameAi.RGBClass.SetRGB>
                {
                    new GameAi.RGBClass.SetRGB(9, 13, 17),
                    new GameAi.RGBClass.SetRGB(119, 56, 54),
                    new GameAi.RGBClass.SetRGB(119, 56, 54),
                    new GameAi.RGBClass.SetRGB(119, 56, 54),
                    new GameAi.RGBClass.SetRGB(119, 56, 54),
                    new GameAi.RGBClass.SetRGB(119, 56, 54)
                }, "EnemyMinion"));
            }

            #endregion

            rgbLists.Add(new GameAi.RGBClass.PointerClass(new List<GameAi.RGBClass.SetRGB>
                {
                    new GameAi.RGBClass.SetRGB(9, 13, 17),
                    new GameAi.RGBClass.SetRGB(119, 56, 54),
                    new GameAi.RGBClass.SetRGB(119, 56, 54),
                    new GameAi.RGBClass.SetRGB(119, 56, 54),
                    new GameAi.RGBClass.SetRGB(119, 56, 54),
                    new GameAi.RGBClass.SetRGB(119, 56, 54)
                }, "EnemyMinion"));

            pointsLists.Add(new GameAi.RGBClass.PointlerClass(new Point(385, 262)));
            pointsLists.Add(new GameAi.RGBClass.PointlerClass(new Point(391, 255)));
            pointsLists.Add(new GameAi.RGBClass.PointlerClass(new Point(397, 250)));
            pointsLists.Add(new GameAi.RGBClass.PointlerClass(new Point(403, 241)));
            pointsLists.Add(new GameAi.RGBClass.PointlerClass(new Point(406, 243)));
        }

        ////////////////////////////
        ///

    }
}
