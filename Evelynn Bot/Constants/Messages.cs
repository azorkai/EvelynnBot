using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evelynn_Bot.Constants
{
    public class Messages
    {
        public string ErrorNullUsername = "Kullanıcı adı boş kalamaz!";
        public string ErrorNullPassword = "Şifre boş kalamaz!";
        public string ErrorInitialize = "Initialize hatası! League of Legends'in açık olduğundan emin olun!";
        public string ErrorCreateGame = "Oyun oluşturma hatası!";
        public string ErrorCreateGameTooManyRequest = "Bir çok denemede oyun başlatılamadı. Sorunun çözümü için her şeyi yeniden başlatıyorum.";
        public string ErrorStartQueue = "Sıra başlatma hatası!";
        public string ErrorKillLeagueProcess = "League of Legends işlem bitirme başarısız!";
        public string ErrorSpell = "Sihirdar büyüsü seçme başarısız!";
        public string ErrorChampionPick = "Şampiyon seçiminde hata meydana geldi!";
        public string ErrorStartLeague = "Bir sebepten ötürü LOL başlatılamadı. Konumu manuel olarak girmeyi deneyin.";
        public string ErrorLogin = "Giriş başarısız! Doğru kullanıcı adı ve şifrenin girildiğinden emin olun!";
        public string SuccessLogin = "Giriş başarılı!";
        public string SuccessInitialize = "Initialize başarılı!";
        public string SuccessCreateGame = "Oyun oluşturma başarılı!";
        public string SuccessStartQueue = "Sıra başlatma başarılı!";
        public string SuccessKillLeagueProcess = "League of Legends işlem bitirme başarılı!";
        public string SuccessSpell = "Sihirdar büyüsü seçme başarılı!";
        public string SuccessChampionPick = "Şampiyon seçimi başarılı!";
        public string SuccessStartLeague = "LOL Başarılı bir şekilde başlatıldı";
        public string SuccessMinion = "";
        public string SuccessEnemyMinion = "";
        public string SuccessEnemyChampion = "";
        public string GameStarted = "";
        public string GameStartedTutorial = "Tutorial oyunu başladı";
        public string AccountDoneXP = "Hesap belirtilen seviyeye ulaştı!";
        public string AccountDoneBE = "Hesap belirtilen Mavi Öz'e ulaştı!";
        public string LookingForNewAccount = "Yeni hesap aranıyor.";
        public string ErrorDisenchant = "Disenchant aşamsında bir hata meydana geldi!";
        public string WaitingForStart = "Kontrol panelinden Başlat komutunun gelmesi bekleniyor...";
    }
}
