using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evelynn_Bot.Constants;
using Evelynn_Bot.Entities;
using Evelynn_Bot.ExternalCommands;

namespace Evelynn_Bot.GameAI
{
    public interface IGameAI:IDisposable
    {
        void CurrentPlayerStats(Interface itInterface);
        Point AnaPointAl(Point point_7);
        Point PointAl(bool is43);
        Bitmap BitmapAl(int screenX, int screenY, int rectangleX, int rectangleY);
        void HepsiniTarat(Bitmap bmp, List<GameAi.RGBClass.PointerClass> rgbLists);
        double Hesap1(double healthPercentage, double prevHealthPercentage, double gameTime, double double_3);
        double Hesap0(double healthPercentage, double prevHealthPercentage);
        void PosHesapla(double healthPercentage, double prevHealthPercentage, double gameTime, double double_3, double double_4, double double_5, double double_6, double double_7, double double_8);
        void Tiklat(Point point_0);
        void SolTiklat();
        void EkraniAyarla(Point point_0);
        void TusuAyarla(GameAi.GEnum8 genum8_0);
        double PointHesapla(PointF pointF_0, PointF pointF_1, PointF pointF_2);
        void KodTarat(GameAi.GEnum8 genum8_0);
        void TusBas(GameAi.GEnum8 genum8_0);
        void EndBas();
        double PointerMath(Point point_0, Point point_1);
        bool BirseyHesapla(Point point_7);
        Point MinimapHesapla(double double_5);
        Point TowerHesapla(double double_5);
        void SagTikla();
        void TotemAt();
        void ItemKullan();
        void EsyaAl(bool bool_4);
        void BaslangicEsyaAl(bool bool_4);
        void YeniAI_1(object Interface);
        void StartNewGameAI(Interface itsInterface);
        void GetInGameStats(object Interface);
        void AraliEndGonder();
        void RGBHazirla(Interface itsInterface);
        void YeniAIBaslat(Interface itsInterface);


    }
}
