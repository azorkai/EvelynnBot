using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evelynn_Bot.Constants
{
    public class Messages
    {
        public string InfoQueueStats = "Sıra durumu:";
        public string InfoStartingAgain = "Bir sonraki oyuna geçiliyor.";
        public string ErrorNullUsername = "Kullanıcı adı boş kalamaz!";
        public string ErrorNullPassword = "Şifre boş kalamaz!";
        public string ErrorInitialize = "Initialize hatası! League of Legends'in açık olduğundan emin olun!";
        public string ErrorCreateGame = "Oyun oluşturma hatası!";
        public string ErrorCreateGameTooManyRequest = "Bir çok denemede oyun başlatılamadı. Sorunun çözümü için her şey yeniden başlatılıyor.";
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
        public string GameFound = "Oyun bulundu!";
        public string AccountDoneXP = "Hesap belirtilen seviyeye ulaştı!";
        public string AccountDoneBE = "Hesap belirtilen Mavi Öz'e ulaştı!";
        public string LookingForNewAccount = "Yeni hesap aranıyor.";
        public string ErrorDisenchant = "Disenchant aşamsında bir hata meydana geldi!";
        public string WaitingForStart = "Kontrol panelinden Başlat komutunun gelmesi bekleniyor...";

        public void SetLanguage()
        {
            //İngilizce
            if (Language.language == "en")
            {
                InfoQueueStats = "Queue Status:";
                InfoStartingAgain = "Starting the next game";
                ErrorNullUsername = "Username must be filled!";
                ErrorNullPassword = "Password must be filled!";
                ErrorInitialize = "Error on Initialize attempt! Make sure League of Legends is running!";
                ErrorCreateGame = "Error on Create Game attempt";
                ErrorCreateGameTooManyRequest = "The game failed to start after many attempts. Restarting everything to fix the problem.";
                ErrorStartQueue = "Error on Start Queue attempt";
                ErrorKillLeagueProcess = "Error on Kill League Process attempt";
                ErrorSpell = "Error on Summoner Spell Select attempt";
                ErrorChampionPick = "Error on Champion Pick attempt";
                ErrorStartLeague = "Couldn't start LOL for some reason. Try entering the location manually.";
                ErrorLogin = "Login failed! Make sure the correct username and password are entered!";
                ErrorDisenchant = "Error on Disenchant attempt.";
                SuccessLogin = "Login is successful!";
                SuccessInitialize = "Initialize is successful!";
                SuccessCreateGame = "Creating Game is successful!";
                SuccessStartQueue = "Starting Queue is successful!";
                SuccessKillLeagueProcess = "Killing League Process is successful!!";
                SuccessSpell = "Selecting Summoner Spell is successful!";
                SuccessChampionPick = "Champion Pick is successful!";
                SuccessStartLeague = "League is started successfully!";
                SuccessMinion = "";
                SuccessEnemyMinion = "";
                SuccessEnemyChampion = "";
                GameStarted = "";
                GameStartedTutorial = "Tutorial game has started!";
                GameFound = "Game Found!";
                AccountDoneXP = "The account has reached the specified level!";
                AccountDoneBE = "The account has reached the specified Blue Essence!";
                LookingForNewAccount = "Looking for new account...";
                WaitingForStart = "Waiting for the 'Start' command from the dashboard...";
            }

            //İspanyolca
            if (Language.language == "es")
            {

            }

            //Brezilya Portekizcesi
            if (Language.language == "br")
            {

            }

            //Fransızca
            if (Language.language == "fr")
            {
                InfoQueueStats = "Statut de la file d'attente:";
                InfoStartingAgain = "Commencer le prochain jeu";
                ErrorNullUsername = "Le nom d'utilisateur doit être renseigné!";
                ErrorNullPassword = "Le mot de passe doit être renseigné !";
                ErrorInitialize = "Erreur lors de la tentative d'initialisation ! Assurez-vous que League of Legends fonctionne!";
                ErrorCreateGame = "Erreur lors de la tentative de création de jeu";
                ErrorCreateGameTooManyRequest = "Le jeu n'a pas pu démarrer après plusieurs tentatives. Tout redémarrer pour résoudre le problème.";
                ErrorStartQueue = "Erreur lors de la tentative de démarrage de la file d'attente";
                ErrorKillLeagueProcess = "Erreur lors de la tentative de processus de Kill League";
                ErrorSpell = "Erreur lors de la tentative de sélection de sort de l'invocateur";
                ErrorChampionPick = "Erreur lors de la tentative de sélection de champion";
                ErrorStartLeague = "Impossible de démarrer LOL pour une raison quelconque. Essayez d'entrer l'emplacement manuellement.";
                ErrorLogin = "Échec de la connexion! Assurez-vous que le nom d'utilisateur et le mot de passe corrects sont entrés!";
                ErrorDisenchant = "Erreur lors de la tentative de désenchantement.";
                SuccessLogin = "La connexion est réussie!";
                SuccessInitialize = "L'initialisation a réussi!";
                SuccessCreateGame = "La création de jeu est réussie !";
                SuccessStartQueue = "Le démarrage de la file d'attente a réussi!";
                SuccessKillLeagueProcess = "Le processus de Killing League est réussi!";
                SuccessSpell = "La sélection du sort d'invocateur est réussie !";
                SuccessChampionPick = "Le choix des champions est réussi!";
                SuccessStartLeague = "La ligue a démarré avec succès!";
                SuccessMinion = "";
                SuccessEnemyMinion = "";
                SuccessEnemyChampion = "";
                GameStarted = "";
                GameStartedTutorial = "Le jeu tutoriel a commencé!";
                GameFound = "Jeu trouvé!";
                AccountDoneXP = "Le compte a atteint le niveau spécifié!";
                AccountDoneBE = "Le compte a atteint l'essence bleue spécifiée!";
                LookingForNewAccount = "A la recherche d'un nouveau compte...";
                WaitingForStart = "En attente de la commande 'Démarrer' du tableau de bord...";
            }
        }
    }
}
