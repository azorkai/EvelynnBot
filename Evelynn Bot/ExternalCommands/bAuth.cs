using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Evelynn_Bot;
using Evelynn_Bot.Constants;
using Evelynn_Bot.Entities;
using Evelynn_Bot.ExternalCommands;

namespace bAUTH
{
    public class bUtils
    {
        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        public Random Random = new Random();
        public string GetRandomString(int length)
        {
            StringBuilder randomName = new StringBuilder(length);
            for (int i = 0; i < length; i++)
                randomName.Append(Alphabet[Random.Next(Alphabet.Length)]);

            return randomName.ToString();
        }

        public int GetEpochTime()
        {
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return (int)t.TotalSeconds-15;
        }
    }

    public enum Method
    {
        POST,
        GET
    }

    public class bHTTP
    {

        public Dashboard dashboardActions = new Dashboard();

        public string CreateRequest(string u, string[] p, string[] v, Method m)
        {
            string r = String.Empty;
            switch (m)
            {
                case Method.POST:
                    using (var client = new System.Net.WebClient())
                    {
                        client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        var values = new NameValueCollection();
                        for (int j = 0; j < p.Length; j++)
                        {
                            values.Add(p[j], v[j]);
                        }
                        var response = client.UploadValues(u, values);
                        r = Encoding.Default.GetString(response);
                    }
                    break;

                case Method.GET:
                    string b = String.Empty;
                    for (int k = 0; k < p.Length; k++)
                    {
                        b += p[k] + "=" + v[k] + "&";
                    }
                    using (var client = new System.Net.WebClient())
                    {
                        r = client.DownloadString(u + "?" + b);
                    }
                    break;
            }
#if DEBUG
            //Console.WriteLine("RESULT: " + r);
#endif
            return r;
        }

        public License VerifyLicense(string t)
        {
            License l = new License();
            try
            {
                bSecurity sec = new bSecurity();
                bUtils u = new bUtils();
                t = t.Replace(" ", "");
                string d = sec.DecryptString(t);
                string[] p = d.Split('|');
                var epc = u.GetEpochTime();
                Console.WriteLine("Your Unix: " + epc);
                Console.WriteLine("Server Unix: " + p[15]);
                if (Convert.ToInt32(p[15]) > epc)
                {
                    //Console.WriteLine(p[1]);
                    switch (p[1])
                    {
                        case "SUCCESS":
                            l.Status = true;
                            l.ID = p[2];
                            l.Username = p[3];
                            l.Password = p[4];
                            l.Register = p[5];
                            l.Expire = p[6];
                            l.Last = p[7];
                            l.Lol_username = p[8];
                            l.Lol_password = p[9];
                            l.Lol_maxLevel = Convert.ToInt32(p[10]);
                            l.Lol_maxBlueEssences = Convert.ToInt32(p[11]);
                            l.Lol_disenchant =  Convert.ToBoolean(p[12] == "0" ? "false" : "true");
                            l.Lol_doTutorial = Convert.ToBoolean(p[13] == "0" ? "false" : "true");
                            l.Lol_isEmptyNick = Convert.ToBoolean(p[14] == "0" ? "false" : "true");
                            break;
                        default: l.Status = false; break;
                    }
                }
                else l.Status = false;
                return l;
            }
            catch(Exception e)
            {
                //Console.WriteLine(e.Message);
                l.Status = false;
                return l;
            }
        }

        public License GetNewLoLAccount(string t, License l)
        {
            try
            {
                bSecurity sec = new bSecurity();
                bUtils u = new bUtils();
                t = t.Replace(" ", "");
                string d = sec.DecryptString(t);
                string[] p = d.Split('|');
                switch (p[1])
                {
                    case "SUCCESS":
                        l.Lol_username = p[2];
                        l.Lol_password = p[3];
                        l.Lol_maxLevel = Convert.ToInt32(p[4]);
                        l.Lol_maxBlueEssences = Convert.ToInt32(p[5]);
                        l.Lol_disenchant = Convert.ToBoolean(p[6] == "0" ? "false" : "true");
                        l.Lol_doTutorial = Convert.ToBoolean(p[7] == "0" ? "false" : "true");
                        l.Lol_isEmptyNick = Convert.ToBoolean(p[8] == "0" ? "false" : "true");
                        break;
                    case "NO_ACCOUNT":
                        Logger.Log(false, Messages.LookingForNewAccount);
                        //TODO: BURADA GLOBAL OLAN ISSTOPPED VARIABLE TRUE YAP
                        break;
                    default: l.Status = false; break;
                }
                return l;
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
                l.Status = false;
                return l;
            }
        }

        public async Task<Dashboard> GetActionStatus(string t)
        {
            try
            {
                bSecurity sec = new bSecurity();
                bUtils u = new bUtils();
                t = t.Replace(" ", "");
                string d = sec.DecryptString(t);
                string[] p = d.Split('|');
                switch (p[1])
                {
                    case "SUCCESS":
                        if (Convert.ToBoolean(p[2])) // Stop Status
                        {
                            dashboardActions.IsStop = true;
                        } else if (Convert.ToBoolean(p[3])) // Start Status
                        {
                            dashboardActions.IsStart = true;
                        }
                        else if (Convert.ToBoolean(p[4])) // Restart Status
                        {
                            dashboardActions.IsRestart = true;

                        }
                        break;
                    case "NOT_FOUND":
                        DashboardHelper.license.Status = false; // Close the bot when game is finished
                        break;
                }
                return dashboardActions;
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
                return dashboardActions;
            }
        }

        public bool VerifyToken(string t)
        {
            bool r = false;
            try
            {
                bSecurity sec = new bSecurity();
                bUtils u = new bUtils();
                t = t.Replace(" ", "");
                string d = sec.DecryptString(t);
                string[] p = d.Split('|');
                if (Convert.ToInt32(p[2]) > u.GetEpochTime())
                {
                    switch (p[1])
                    {
                        case "SUCCESS": r = true; break;
                        default: r = false; break;
                    }
                }
                else r = false;
                return r;
            }
            catch
            {
                r = false;
                return r;
            }
        }

        public int GetOnlineClients(string t)
        {
            int r = 0;
            try
            {
                bSecurity sec = new bSecurity();
                bUtils u = new bUtils();
                t = t.Replace(" ", "");
                string d = sec.DecryptString(t);
                string[] p = d.Split('|');
                if (Convert.ToInt32(p[2]) > u.GetEpochTime())
                {
                    switch (p[1])
                    {
                        case "SUCCESS": r = Convert.ToInt32(p[3]); break;
                        default: r = 0; break;
                    }
                }
                else r = 0;
                return r;
            }
            catch
            {
                r = 0;
                return r;
            }
            return r;
        }
    }

    public class bSecurity
    {
        public string EncryptString(string plainText)
        {
            // KEY ve IV fonksiyon icin artik gerekli degil.
            string password = "HnSdyBPo4I";
            SHA256 mySHA256 = SHA256Managed.Create();
            byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));
            byte[] iv = new byte[16] { 0x9, 0xF, 0x9, 0x0, 0xF, 0x0, 0x0, 0x0, 0x0, 0xF, 0xA, 0x0, 0x1, 0x0, 0xF, 0x0 };

            Aes encryptor = Aes.Create();
            encryptor.Mode = CipherMode.CBC;
            encryptor.Key = key;
            encryptor.IV = iv;
            MemoryStream memoryStream = new MemoryStream();
            ICryptoTransform aesEncryptor = encryptor.CreateEncryptor();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write);
            byte[] plainBytes = Encoding.ASCII.GetBytes(plainText);
            cryptoStream.Write(plainBytes, 0, plainBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            string cipherText = Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);
            //Console.WriteLine(cipherText);
            return cipherText;
        }

        public string DecryptString(string cipherText)
        {
            // KEY ve IV fonksiyon icin artik gerekli degil.
            string password = "HnSdyBPo4I";
            SHA256 mySHA256 = SHA256Managed.Create();
            byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));
            byte[] iv = new byte[16] { 0x9, 0xF, 0x9, 0x0, 0xF, 0x0, 0x0, 0x0, 0x0, 0xF, 0xA, 0x0, 0x1, 0x0, 0xF, 0x0 };

            Aes encryptor = Aes.Create();
            encryptor.Mode = CipherMode.CBC;
            encryptor.Key = key.Take(32).ToArray();
            encryptor.IV = iv;
            MemoryStream memoryStream = new MemoryStream();
            ICryptoTransform aesDecryptor = encryptor.CreateDecryptor();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write);
            string plainText = String.Empty;
            try
            {
                    byte[] cipherBytes = Convert.FromBase64String(cipherText);
                cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);
                cryptoStream.FlushFinalBlock();
                byte[] plainBytes = memoryStream.ToArray();
                plainText = Encoding.ASCII.GetString(plainBytes, 0, plainBytes.Length);
            }
            finally
            {
                memoryStream.Close();
                cryptoStream.Close();
            }
            return plainText;
        }
    }
}
