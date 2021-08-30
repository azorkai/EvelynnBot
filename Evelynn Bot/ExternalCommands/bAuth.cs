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


        Random Random = new Random();

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

        public License VerifyLicense(string t, Interface itsInterface)
        {
            try
            {
                t = t.Replace(" ", "");
                string d = itsInterface.sec.DecryptString(t);
                string[] p = d.Split('|');
                var epc = itsInterface.u.GetEpochTime();
                if (Convert.ToInt32(p[15]) > epc)
                {
                    //Console.WriteLine(p[1]);
                    switch (p[1])
                    {
                        case "SUCCESS":
                            itsInterface.license.Status = true;
                            itsInterface.license.ID = p[2];
                            itsInterface.license.Username = p[3];
                            itsInterface.license.Password = p[4];
                            itsInterface.license.Register = p[5];
                            itsInterface.license.Expire = p[6];
                            itsInterface.license.Last = p[7];
                            itsInterface.license.Lol_username = p[8];
                            itsInterface.license.Lol_password = p[9];
                            itsInterface.license.Lol_maxLevel = Convert.ToInt32(p[10]);
                            itsInterface.license.Lol_maxBlueEssences = Convert.ToInt32(p[11]);
                            itsInterface.license.Lol_disenchant = Convert.ToBoolean(p[12] == "0" ? "false" : "true");
                            itsInterface.license.Lol_doTutorial = Convert.ToBoolean(p[13] == "0" ? "false" : "true");
                            itsInterface.license.Lol_isEmptyNick = Convert.ToBoolean(p[14] == "0" ? "false" : "true");
                            itsInterface.license.Lol_region = p[15];
                            break;
                        default: itsInterface.license.Status = false; break;
                    }
                }
                else itsInterface.license.Status = false;
                return itsInterface.license;
            }
            catch(Exception e)
            {
                //Console.WriteLine(e.Message);
                itsInterface.license.Status = false;
                return itsInterface.license;
            }
        }


        public License GetNewLoLAccount(string t, Interface itsInterface)
        {
            try
            {
                t = t.Replace(" ", "");
                string d = itsInterface.sec.DecryptString(t);
                string[] p = d.Split('|');
                switch (p[1])
                {
                    case "SUCCESS":
                        itsInterface.license.Lol_username = p[2];
                        itsInterface.license.Lol_password = p[3];
                        itsInterface.license.Lol_maxLevel = Convert.ToInt32(p[4]);
                        itsInterface.license.Lol_maxBlueEssences = Convert.ToInt32(p[5]);
                        itsInterface.license.Lol_disenchant = Convert.ToBoolean(p[6] == "0" ? "false" : "true");
                        itsInterface.license.Lol_doTutorial = Convert.ToBoolean(p[7] == "0" ? "false" : "true");
                        itsInterface.license.Lol_isEmptyNick = Convert.ToBoolean(p[8] == "0" ? "false" : "true");
                        itsInterface.license.Lol_region = p[9];
                        break;
                    case "NO_ACCOUNT":
                        itsInterface.logger.Log(false, itsInterface.messages.LookingForNewAccount);
                        itsInterface.dashboard.IsStop = true;
                        Console.WriteLine("NO_ACCOUNT Stop geldi");
                        break;
                    default: itsInterface.license.Status = false; break;
                }
                return itsInterface.license;
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
                itsInterface.license.Status = false;
                return itsInterface.license;
            }
        }

        public async Task<Interface> GetActionStatus(string t, Interface itsInterface)
        {
            try
            {
                t = t.Replace(" ", "");
                string d = itsInterface.sec.DecryptString(t);
                string[] p = d.Split('|');
                switch (p[1])
                {
                    case "SUCCESS":
                        if (Convert.ToBoolean(p[2]) == true) // Stop Status
                        {
                            itsInterface.dashboard.IsStop = true;
                        } else if (Convert.ToBoolean(p[3])) // Start Status
                        {
                            itsInterface.dashboard.IsStart = true;
                        }
                        else if (Convert.ToBoolean(p[4])) // Restart Status
                        {
                            itsInterface.dashboard.IsRestart = true;

                        }
                        break;
                    case "NOT_FOUND":
                        itsInterface.license.Status = false; // Close the bot when game is finished
                        break;
                }
                return itsInterface;
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
                return itsInterface;
            }
        }
    }

    public class bSecurity
    {
        public string EncryptString(string plainText)
        {
            try
            {
                // KEY ve IV fonksiyon icin artik gerekli degil.
                string password = "HnSdyBPo4I";
                SHA256 mySHA256 = SHA256Managed.Create();
                byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));
                byte[] iv = new byte[16] { 0x9, 0xF, 0x9, 0x0, 0xF, 0x0, 0x0, 0x0, 0x0, 0xF, 0xA, 0x0, 0x1, 0x0, 0xF, 0x0 };

                string cipherText = String.Empty;
                using (Aes encryptor = Aes.Create())
                {
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
                    cipherText = Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);
                    //Console.WriteLine(cipherText);
                }

                return cipherText;
            }
            catch (Exception e)
            {
                Console.WriteLine($"BAUTH ENCRYPT STRING HATA {e}");
                return "";
            }
        }

        public string DecryptString(string cipherText)
        {
            try
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
            catch (Exception e)
            {
                return "";
            }
        }
    }
}
