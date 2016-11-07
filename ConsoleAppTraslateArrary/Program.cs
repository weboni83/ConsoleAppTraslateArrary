using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml.Linq;

namespace ConsoleAppTraslateArrary
{
    class Program
    {
        
        static void Main(string[] args)
        {

            //Get Client Id and Client Secret from https://datamarket.azure.com/developer/applications/
            string clientId = "";
            string clientSecret = "";

            //GetLanguagesForTranslate(clientId, clientSecret);

            TranslateArrayMethod(clientId, clientSecret);

            //TranslateMethod(clientId, clientSecret);

        }
        #region 번역 Arrary
        private static void TranslateMethod(string clientId, string clientSecret)
        {
            AdmAccessToken admToken;
            string headerValue;
            //Get Client Id and Client Secret from https://datamarket.azure.com/developer/applications/
            //Refer obtaining AccessToken (http://msdn.microsoft.com/en-us/library/hh454950.aspx) 
            AdmAuthentication admAuth = new AdmAuthentication(clientId, clientSecret);
            try
            {
                admToken = admAuth.GetAccessToken();
                // Create a header with the access_token property of the returned token
                headerValue = "Bearer " + admToken.access_token;
                TranslateMethod(headerValue);
            }
            catch (WebException e)
            {
                ProcessWebException(e);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
            }
        }

        private static void TranslateMethod(string authToken)
        {
            string text = "Use pixels to express measurements for padding and margins.";
            string from = "en";
            string to = "ko";
            string uri = "http://api.microsofttranslator.com/v2/Http.svc/Translate?text=" + System.Web.HttpUtility.UrlEncode(text) + "&from=" + from + "&to=" + to;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.Headers.Add("Authorization", authToken);
            WebResponse response = null;
            try
            {
                response = httpWebRequest.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    System.Runtime.Serialization.DataContractSerializer dcs = new System.Runtime.Serialization.DataContractSerializer(Type.GetType("System.String"));
                    string translation = (string)dcs.ReadObject(stream);
                    Console.WriteLine("Translation for source text '{0}' from {1} to {2} is", text, "en", "de");
                    Console.WriteLine(translation);
                }
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
            }
        }

        private static void TranslateArrayMethod(string clientId, string clientSecret)
        {
            AdmAccessToken admToken;
            //Get Client Id and Client Secret from https://datamarket.azure.com/developer/applications/
            //AdmAuthentication admAuth = new AdmAuthentication("clientId", "clientSecret");
            AdmAuthentication admAuth = new AdmAuthentication(clientId, clientSecret);
            string headerValue;

            try
            {
                admToken = admAuth.GetAccessToken();
                // Create a header with the access_token property of the returned token
                headerValue = "Bearer " + admToken.access_token;
                TranslateArrayMethod(headerValue);
            }
            catch (WebException e)
            {
                ProcessWebException(e);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
            }
        }

        private static void TranslateArrayMethod(string authToken)
        {
            //번역 목록
            List<string> translateList = new List<string>();
            translateList.Add("거래처마스터 관리");
            translateList.Add("가격리스트");
            translateList.Add("기종정보");
            translateList.Add("분류등록");
            translateList.Add("그룹등록");
            translateList.Add("옵션유형");
            translateList.Add("옵코션드");
            translateList.Add("품목정보");


            string texts = string.Empty;

            foreach (string item in translateList)
            {
                texts += string.Format("<string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\">{0}</string>", item);
            }

            if (string.IsNullOrEmpty(texts))
                return;


            string from = "ko";
            string to = "en";
            //string[] translateArraySourceTexts = { "The answer lies in machine translation.", "the best machine translation technology cannot always provide translations tailored to a site or users like a human ", "Simply copy and paste a code snippet anywhere " };
            string uri = "http://api.microsofttranslator.com/v2/Http.svc/TranslateArray";
            string body = "<TranslateArrayRequest>" +
                             "<AppId />" +
                             "<From>{0}</From>" +
                             "<Options>" +
                                " <Category xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\" />" +
                                 "<ContentType xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\">{1}</ContentType>" +
                                 "<ReservedFlags xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\" />" +
                                 "<State xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\" />" +
                                 "<Uri xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\" />" +
                                 "<User xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\" />" +
                             "</Options>" +
                             "<Texts>" +
                                "{2}" +
                             "</Texts>" +
                             "<To>{3}</To>" +
                          "</TranslateArrayRequest>";
            string reqBody = string.Format(body, from, "text/plain", texts, to);
            // create the request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Headers.Add("Authorization", authToken);
            request.ContentType = "text/xml";
            request.Method = "POST";

            using (System.IO.Stream stream = request.GetRequestStream())
            {
                byte[] arrBytes = System.Text.Encoding.UTF8.GetBytes(reqBody);
                stream.Write(arrBytes, 0, arrBytes.Length);
            }

            // Get the response
            WebResponse response = null;
            try
            {
                response = request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader rdr = new StreamReader(stream, System.Text.Encoding.UTF8))
                    {
                        // Deserialize the response
                        string strResponse = rdr.ReadToEnd();
                        Console.WriteLine("Result of translate array method is:");
                        XDocument doc = XDocument.Parse(@strResponse);
                        XNamespace ns = "http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2";
                        int soureceTextCounter = 0;
                        foreach (XElement xe in doc.Descendants(ns + "TranslateArrayResponse"))
                        {

                            foreach (var node in xe.Elements(ns + "TranslatedText"))
                            {
                                
                                Console.WriteLine("\n\nSource text: {0}\nTranslated Text: {1}", translateList[soureceTextCounter], node.Value);
                            }
                            soureceTextCounter++;
                        }
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
            }

        }

        private static void ProcessWebException(WebException e)
        {
            Console.WriteLine("{0}", e.ToString());
            // Obtain detailed error information
            string strResponse = string.Empty;
            using (HttpWebResponse response = (HttpWebResponse)e.Response)
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(responseStream, System.Text.Encoding.ASCII))
                    {
                        strResponse = sr.ReadToEnd();
                    }
                }
            }
            Console.WriteLine("Http status code={0}, error message={1}", e.Status, strResponse);
        }

        #endregion

        #region 번역 가능한 언어

        private static void GetLanguagesForTranslate(string clientId, string clientSecret)
        {
            AdmAccessToken admToken;
            //Get Client Id and Client Secret from https://datamarket.azure.com/developer/applications/
            //AdmAuthentication admAuth = new AdmAuthentication("clientId", "clientSecret");
            AdmAuthentication admAuth = new AdmAuthentication(clientId, clientSecret);
            string headerValue;

            try
            {
                admToken = admAuth.GetAccessToken();
                DateTime tokenReceived = DateTime.Now;
                // Create a header with the access_token property of the returned token
                headerValue = "Bearer " + admToken.access_token;
                GetLanguagesForTranslate(headerValue);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
            }
        }

        private static void GetLanguagesForTranslate(string authToken)
        {

            string uri = "http://api.microsofttranslator.com/v2/Http.svc/GetLanguagesForTranslate";
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.Headers.Add("Authorization", authToken);
            WebResponse response = null;
            try
            {
                response = httpWebRequest.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {

                    System.Runtime.Serialization.DataContractSerializer dcs = new System.Runtime.Serialization.DataContractSerializer(typeof(List<string>));

                    List<string> languagesForTranslate = (List<string>)dcs.ReadObject(stream);
                    Console.WriteLine("The languages available for translation are: ");
                    languagesForTranslate.ForEach(a => Console.WriteLine(a));
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey(true);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
            }
        }

        #endregion


        [DataContract]
        public class AdmAccessToken
        {
            [DataMember]
            public string access_token { get; set; }
            [DataMember]
            public string token_type { get; set; }
            [DataMember]
            public string expires_in { get; set; }
            [DataMember]
            public string scope { get; set; }
        }

        public class AdmAuthentication
        {
            public static readonly string DatamarketAccessUri = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";
            private string clientId;
            private string clientSecret;
            private string request;
            private AdmAccessToken token;
            private System.Threading.Timer accessTokenRenewer;
            //Access token expires every 10 minutes. Renew it every 9 minutes only. 
            private const int RefreshTokenDuration = 9;

            public AdmAuthentication(string clientId, string clientSecret)
            {
                this.clientId = clientId;
                this.clientSecret = clientSecret;
                //If clientid or client secret has special characters, encode before sending request 
                this.request = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com", HttpUtility.UrlEncode(clientId), HttpUtility.UrlEncode(clientSecret));
                this.token = HttpPost(DatamarketAccessUri, this.request);
                //renew the token every specified minutes 
                accessTokenRenewer = new System.Threading.Timer(new TimerCallback(OnTokenExpiredCallback), this, TimeSpan.FromMinutes(RefreshTokenDuration), TimeSpan.FromMilliseconds(-1));
            }

            public AdmAccessToken GetAccessToken()
            {
                return this.token;
            }

            private void RenewAccessToken()
            {
                AdmAccessToken newAccessToken = HttpPost(DatamarketAccessUri, this.request);
                //swap the new token with old one 
                //Note: the swap is thread unsafe 
                this.token = newAccessToken;
                Console.WriteLine(string.Format("Renewed token for user: {0} is: {1}", this.clientId, this.token.access_token));
            }

            private void OnTokenExpiredCallback(object stateInfo)
            {
                try
                {
                    RenewAccessToken();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Failed renewing access token. Details: {0}", ex.Message));
                }
                finally
                {
                    try
                    {
                        accessTokenRenewer.Change(TimeSpan.FromMinutes(RefreshTokenDuration), TimeSpan.FromMilliseconds(-1));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(string.Format("Failed to reschedule the timer to renew access token. Details: {0}", ex.Message));
                    }
                }
            }

            private AdmAccessToken HttpPost(string DatamarketAccessUri, string requestDetails)
            {
                //Prepare OAuth request 
                WebRequest webRequest = WebRequest.Create(DatamarketAccessUri);
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.Method = "POST";
                byte[] bytes = Encoding.ASCII.GetBytes(requestDetails);
                webRequest.ContentLength = bytes.Length;
                using (Stream outputStream = webRequest.GetRequestStream())
                {
                    outputStream.Write(bytes, 0, bytes.Length);
                }
                using (WebResponse webResponse = webRequest.GetResponse())
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AdmAccessToken));
                    //Get deserialized object from JSON stream 
                    AdmAccessToken token = (AdmAccessToken)serializer.ReadObject(webResponse.GetResponseStream());
                    return token;
                }
            }
        }
    }
}