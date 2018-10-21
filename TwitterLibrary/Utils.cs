using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Net.Http;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.IO;
using TwitterInterface.Data;
using TwitterLibrary.Data;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TwitterLibrary
{
    public class Utils
    {
        public static Dictionary<string, string> ParseCookiesFromJavascript(string cookie)
        {
            cookie.Replace("\"", "");
            var result = new Dictionary<string, string>();

            int inx = 0;
            while(inx != -1)
            {
                inx = ParseCookieFromJavascript(cookie, inx, result);
            }

            return result;
        }

        private static int ParseCookieFromJavascript(string cookie, int inx, Dictionary<string, string> result)
        {
            int last = -1;
            try
            {
                last = cookie.IndexOf(';', inx);
            }
            catch{ }
            var value = cookie.Substring(inx, last == -1 ? cookie.Length - inx : last - inx);
            var split = value.Split('=');

            result[split[0]] = result[split[1]];

            return last + 1;
        }

        public static void VerifyTwitterResponse(HttpResponseMessage responseMessage)
        {
            if(!responseMessage.IsSuccessStatusCode)
            {
                var json = responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                //TODO: Wrapping with Custom Exception
                try
                {
                    throw new IOException(JObject.Parse(json)["errors"][0]["message"].ToString());
                }
                catch
                {
                    throw new IOException(json);
                }
            }
        }

        public static readonly Random random = new Random();

        public static string HMACSHA1Encode(string input, byte[] key)
        {
            HMACSHA1 myhmacsha1 = new HMACSHA1(key);
            byte[] byteArray = Encoding.ASCII.GetBytes(input);
            MemoryStream stream = new MemoryStream(byteArray);
            return Convert.ToBase64String(myhmacsha1.ComputeHash(stream));
        }

        private static string CreateOAuthTimestamp()
        {

            var nowUtc = DateTime.UtcNow;
            var timeSpan = nowUtc - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var timestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();

            return timestamp;
        }

        private static string GenerateNonce()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < 8; i++)
            {
                int g = random.Next(3);
                switch (g)
                {
                    case 0:
                        // lowercase alpha
                        sb.Append((char)(random.Next(26) + 97), 1);
                        break;
                    default:
                        // numeric digits
                        sb.Append((char)(random.Next(10) + 48), 1);
                        break;
                }
            }
            return sb.ToString();
        }

        public static string ToWebString(SortedDictionary<string, string> source)
        {
            var body = new StringBuilder();

            foreach (var requestParameter in source)
            {
                body.Append(requestParameter.Key);
                body.Append("=");
                body.Append(Uri.EscapeDataString(requestParameter.Value));
                body.Append("&");
            }
            if (body.Length != 0)
            {
                //remove trailing '&'
                body.Remove(body.Length - 1, 1);
            }
            return body.ToString();
        }
        public async static Task<string> ReadStringFromRequestMessage(HttpClient client, HttpRequestMessage message)
        {
            var response = await client.SendAsync(message);
            VerifyTwitterResponse(response);

           return await response.Content.ReadAsStringAsync();
        }

        public async static Task<string> ReadStringFromTwitter(HttpClient client, HttpMethod method, Uri uri, KeyValuePair<string, string>[] query, Account account)
        {
            return await ReadStringFromRequestMessage(client, account.GenerateRequest(method, uri, query));
        }

        public async static Task<string> readStringFromTwitter(HttpClient client, HttpMethod method, Uri uri, KeyValuePair<string, string>[] query, Token consumerToken, Token? oauthToken)
        {
            return await ReadStringFromRequestMessage(client, GenerateHttpRequest(method, uri, query, consumerToken, oauthToken));
        }

        public static HttpRequestMessage GenerateHttpRequest(HttpMethod method, Uri uri, KeyValuePair<string, string>[] query, Account account)
        {
            return account.GenerateRequest(method, uri, query);
        }

        /// <summary>
        /// Generate HttpRequestMessage with authentication(OAuth 1)
        /// </summary>
        /// <param name="method"></param>
        /// <param name="uri"></param>
        /// <param name="query"></param>
        /// <param name="consumerToken"></param>
        /// <param name="oauthToken"></param>
        /// <returns></returns>
        public static HttpRequestMessage GenerateHttpRequest(HttpMethod method, Uri uri, KeyValuePair<string, string>[] query, Token consumerToken, Token? oauthToken)
        {
            HttpRequestMessage message = new HttpRequestMessage();
            message.Method = method;
            message.RequestUri = uri;

            //Oauth Parameters
            var authorization = new SortedDictionary<string, string>();

            //Other parameters
            var queryDict = new SortedDictionary<string, string>();

            //Collect oauth from query
            foreach(var pair in query)
            {
                if(pair.Key.StartsWith("oauth"))
                {
                    authorization.Add(pair.Key, pair.Value);
                }
                else
                {
                    queryDict.Add(pair.Key, pair.Value);
                }
            }

            //Generate authorization keys
            authorization.Add("oauth_consumer_key", consumerToken.Key);
            authorization.Add("oauth_nonce", GenerateNonce());
            authorization.Add("oauth_signature_method", "HMAC-SHA1");
            authorization.Add("oauth_timestamp", CreateOAuthTimestamp());
            if (oauthToken != null)
            {
                authorization.Add("oauth_token", oauthToken.Value.Key);
            }
            authorization.Add("oauth_version", "1.0");

            //Collect parameters for create signature base
            var parameters = new SortedDictionary<string, string>();
            foreach (var pair in authorization)
            {
                parameters.Add(pair.Key, pair.Value);
            }
            foreach (var pair in queryDict)
            {
                parameters.Add(pair.Key, pair.Value);
            }
            parameters.OrderBy(pair => pair.Key);

            var signatureBase = new StringBuilder();
            signatureBase.Append(method.ToString().ToUpper());
            signatureBase.Append("&");
            signatureBase.Append(Uri.EscapeDataString(uri.ToString()));
            signatureBase.Append("&");
            signatureBase.Append(Uri.EscapeDataString( ToWebString(parameters) ));

            //Generate signing key for signture
            var signatureKey = new StringBuilder();
            signatureKey.Append(Uri.EscapeDataString(consumerToken.Secret));
            signatureKey.Append("&");
            if (oauthToken != null)
            {
                signatureKey.Append(Uri.EscapeDataString(oauthToken.Value.Secret));
            }

            //Add signature to Oauth parameters
            authorization.Add("oauth_signature", HMACSHA1Encode(signatureBase.ToString(), Encoding.UTF8.GetBytes(signatureKey.ToString())));
            authorization.OrderBy(pair => pair.Key);
            queryDict.OrderBy(pair => pair.Key);

            //Register Oauth parameters to Authorization Header
            var authorizationValue = new StringBuilder();
            authorizationValue.Append("OAuth ");
            foreach(var pair in authorization)
            {
                authorizationValue.Append(pair.Key);
                authorizationValue.Append("=");
                authorizationValue.Append( Uri.EscapeDataString( pair.Value ) );
                authorizationValue.Append(", ");
            }
            authorizationValue.Remove(authorizationValue.Length - 2, 2); //Cut ', ' from built value
            message.Headers.Add("Authorization", authorizationValue.ToString());

            //Add other parameters
            if (queryDict.Count != 0)
            {
                if (method == HttpMethod.Post)
                {
                    message.Content = new StringContent(ToWebString(queryDict), Encoding.UTF8, "application/x-www-form-urlencoded");
                }
                else
                {
                    message.RequestUri = new Uri(uri.ToString() + "?" + ToWebString(queryDict));
                }
            }
            return message;
        }
    }
}
