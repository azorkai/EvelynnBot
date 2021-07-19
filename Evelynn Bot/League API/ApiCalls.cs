using Evelynn_Bot.League_API.GameData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Evelynn_Bot.Constants;

namespace Evelynn_Bot.League_API
{
    public class ApiCalls : ApiVariables, IApiCalls
    {
        private int Port;
        private string Auth;

        public Encoding HttpRequestEncoding = Encoding.UTF8;
        #region DLL

        [DllImport("User32.dll")]
        private static extern int SetForegroundWindow(IntPtr intptr_0);

        int SW_SHOW = 5;
        private bool disposedValue;

        [DllImport("User32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("User32.dll")]
        public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("User32.dll")]
        public static extern bool EnableWindow(IntPtr hwnd, bool enabled);

        [DllImport("User32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr intptr_0, IntPtr intptr_1, int int_0, int int_1, int int_2, int int_3, uint uint_0);

        #endregion

        #region LCU API

        bool sertifica_metot(object object_0, X509Certificate x509Certificate_0, X509Chain x509Chain_0, SslPolicyErrors sslPolicyErrors_0)
        {
            return true;
        }

        public string requestWeb(string string_0, string string_1, string string_2, string string_3, string string_4)
        {
            string result;
            try
            {
                WebClient webClient = new WebClient
                {
                    Credentials = new NetworkCredential("riot", string_3)
                };
                webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
                string text = null;
                string address = "https://127.0.0.1:" + string_4 + string_0;
                string text2 = string_2.ToLower();
                string text3 = text2;
                if (text3 != null)
                {
                    if (!(text3 == "get"))
                    {
                        if (!(text3 == "post"))
                        {
                            if (!(text3 == "put"))
                            {
                                result = text;
                            }
                            try
                            {
                                text = webClient.UploadString(address, "PUT", string_1);
                                result = text;
                            }
                            catch
                            {
                                result = text;
                            }
                        }
                        try
                        {
                            text = webClient.UploadString(address, string_1);
                            result = text;
                        }
                        catch (Exception)
                        {
                            result = text;
                        }
                    }
                    try
                    {
                        text = webClient.DownloadString(address);
                    }
                    catch (Exception)
                    {
                    }
                }
                result = text;
            }
            catch
            {
                result = string.Empty;
            }
            return result;
        }

        public void PatchObject<T>(T obj, string endpoint, string Auth, int Port)
        {
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://127.0.0.1:" + Port + endpoint);
                httpWebRequest.Headers.Add("Authorization", "Basic " + Auth);
                httpWebRequest.Accept = "application/json";
                HttpWebRequest httpWebRequest2 = httpWebRequest;
                httpWebRequest2.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(httpWebRequest2.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(sertifica_metot));
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "PATCH";
                string value = JsonConvert.SerializeObject(obj);
                using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(value);
                }
                WebResponse response = httpWebRequest.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream);
                streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
                response.Close();
            }
            catch
            {
               Dispose(true);
            }
        }

        public T GetObject<T>(string endpoint, string Auth, int Port)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            T result;
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://127.0.0.1:" + Port + endpoint);
                httpWebRequest.Headers.Add("Authorization", "Basic " + Auth);
                httpWebRequest.Accept = "application/json";
                HttpWebRequest httpWebRequest2 = httpWebRequest;
                httpWebRequest2.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(httpWebRequest2.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(sertifica_metot));
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";
                WebResponse response = httpWebRequest.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream);
                string text2 = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
                response.Close();
                result = JsonConvert.DeserializeObject<T>(text2);
            }
            catch
            {
                result = default(T);
            }
            return result;
        }

        public void PutObject<T>(T obj, string endpoint, string Auth, int Port)
        {
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://127.0.0.1:" + Port + endpoint);
                httpWebRequest.Headers.Add("Authorization", "Basic " + Auth);
                httpWebRequest.Accept = "application/json";
                HttpWebRequest httpWebRequest2 = httpWebRequest;
                httpWebRequest2.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(httpWebRequest2.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(sertifica_metot));
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "PUT";
                string value = JsonConvert.SerializeObject(obj);
                using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(value);
                }
                WebResponse response = httpWebRequest.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream);
                streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
                response.Close();
            }
            catch
            {
                Dispose(true);

            }
        }

        public bool PostObject<T>(T obj, string endpoint, string Auth, int Port)
        {
            bool result;
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://127.0.0.1:" + Port + endpoint);
                httpWebRequest.Headers.Add("Authorization", "Basic " + Auth);
                httpWebRequest.Accept = "application/json";
                HttpWebRequest httpWebRequest2 = httpWebRequest;
                httpWebRequest2.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(httpWebRequest2.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(sertifica_metot));
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";
                string value = JsonConvert.SerializeObject(obj);
                using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(value);
                }
                WebResponse response = httpWebRequest.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream);
                var rsp = streamReader.ReadToEnd();
                //Console.WriteLine(rsp);
                streamReader.Close();
                responseStream.Close();
                response.Close();
                result = true;
            }
            catch
            {
                Dispose(true);
                result = false;
            }
            return result;
        }

        public string GetObject(string endpoint, string Auth, int Port)
        {
            string result;
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://127.0.0.1:" + Port + endpoint);
                httpWebRequest.Headers.Add("Authorization", "Basic " + Auth);
                httpWebRequest.Accept = "application/json";
                HttpWebRequest httpWebRequest2 = httpWebRequest;
                httpWebRequest2.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(httpWebRequest2.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(sertifica_metot));
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";
                WebResponse response = httpWebRequest.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream);
                string text2 = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
                response.Close();
                result = text2;
            }
            catch
            {
                Dispose(true);
                result = null;
            }
            return result;
        }

        public object PostObjectJSON<T>(T obj, string endpoint, string Auth, int Port)
        {
            object result = null;
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://127.0.0.1:" + Port + endpoint);
                httpWebRequest.Headers.Add("Authorization", "Basic " + Auth);
                httpWebRequest.Accept = "application/json";
                HttpWebRequest httpWebRequest2 = httpWebRequest;
                //httpWebRequest2.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(httpWebRequest2.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(sertifica_metot));
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";
                string value = JsonConvert.SerializeObject(obj);
                using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(value);
                }
                WebResponse response = httpWebRequest.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream);
                var rsp = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
                response.Close();
                result = JsonConvert.DeserializeObject(rsp);
            }
            catch
            {
                Dispose(true);
                result = null;
            }
            return result;
        }

        public T[] GetObjectArray<T>(string endpoint, string Auth, int Port)
        {
            T[] result;
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://127.0.0.1:" + Port + endpoint);
                httpWebRequest.Headers.Add("Authorization", "Basic " + Auth);
                httpWebRequest.Accept = "application/json";
                HttpWebRequest httpWebRequest2 = httpWebRequest;
                httpWebRequest2.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(httpWebRequest2.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(sertifica_metot));
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";
                WebResponse response = httpWebRequest.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream);
                string text2 = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
                response.Close();
                result = JsonConvert.DeserializeObject<T[]>(text2);
            }
            catch
            {
                Dispose(true);
                result = null;
            }
            return result;
        }

        public LiveGameData GetLiveGameData()
        {
            LiveGameData result;
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://127.0.0.1:" + 2999 + "/liveclientdata/allgamedata");
                httpWebRequest.Accept = "application/json";
                HttpWebRequest httpWebRequest2 = httpWebRequest;
                httpWebRequest2.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(httpWebRequest2.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(sertifica_metot));
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";
                WebResponse response = httpWebRequest.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream);
                string text = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
                response.Close();
                result = JsonConvert.DeserializeObject<LiveGameData>(text);
            }
            catch
            {
                Dispose(true);
                result = new LiveGameData();
            }
            return result;
        }

        #endregion

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
        ~ApiCalls()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
