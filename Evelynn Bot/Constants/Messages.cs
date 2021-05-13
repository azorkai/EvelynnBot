using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evelynn_Bot.Constants
{
    public static class Messages
    {
        public static string ErrorNullUsername = "Kullanıcı adı boş kalamaz!";
        public static string ErrorNullPassword = "Şifre boş kalamaz!";
        public static string ErrorInitialize = "Initialize hatası! League of Legends'in açık olduğundan emin olun!";
        public static string ErrorCreateGame = "Oyun oluşturma hatası!";
        public static string ErrorCreateGameTooManyRequest = "Bir çok denemede oyun başlatılamadı. Sorunun çözümü için her şeyi yeniden başlatıyorum.";
        public static string ErrorStartQueue = "Sıra başlatma hatası!";
        public static string ErrorKillLeagueProcess = "League of Legends işlem bitirme başarısız!";
        public static string ErrorSpell = "Sihirdar büyüsü seçme başarısız!";
        public static string ErrorChampionPick = "Şampiyon seçiminde hata meydana geldi!";
        public static string ErrorStartLeague = "Bir sebepten ötürü LOL başlatılamadı. Konumu manuel olarak girmeyi deneyin.";
        public static string ErrorLogin = "Giriş başarısız! Doğru kullanıcı adı ve şifrenin girildiğinden emin olun!";
        public static string SuccessLogin = "Giriş başarılı!";
        public static string SuccessInitialize = "Initialize başarılı!";
        public static string SuccessCreateGame = "Oyun oluşturma başarılı!";
        public static string SuccessStartQueue = "Sıra başlatma başarılı!";
        public static string SuccessKillLeagueProcess = "League of Legends işlem bitirme başarılı!";
        public static string SuccessSpell = "Sihirdar büyüsü seçme başarılı!";
        public static string SuccessChampionPick = "Şampiyon seçimi başarılı!";
        public static string SuccessStartLeague = "LOL Başarılı bir şekilde başlatıldı";
        public static string SuccessMinion = "Minyon bulundu!";
        public static string SuccessEnemyMinion = "Düşman minyon bulundu!";
        public static string SuccessEnemyChampion = "Düşman şampiyon bulundu!";
        public static string GameStarted = "Oyun başladı!";
        public static string GameStartedTutorial = "Tutorial oyunu başladı";
        public static string AccountDoneXP = "Hesap belirtilen seviyeye ulaştı!";
        public static string AccountDoneBE = "Hesap belirtilen Mavi Öz'e ulaştı!";
        public static string LookingForNewAccount = "Yeni hesap aranıyor.";
        public static string ErrorDisenchant = "Disenchant aşamsında bir hata meydana geldi!";
        public static string WaitingForStart = "Kontrol panelinden Başlat komutunun gelmesi bekleniyor...";
    }
}
