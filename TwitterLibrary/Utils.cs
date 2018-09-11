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

namespace TwitterLibrary
{
    class Utils
    {
        public static readonly Random random = new Random();

        public static string HMACSHA1Encode(string input, byte[] key)
        {
            HMACSHA1 myhmacsha1 = new HMACSHA1(key);
            byte[] byteArray = Encoding.ASCII.GetBytes(input);
            MemoryStream stream = new MemoryStream(byteArray);
            return myhmacsha1.ComputeHash(stream).Aggregate("", (s, e) => s + String.Format("{0:x2}", e), s => s);
        }

        public static void authorizeHttpRequest(HttpRequestMessage message, Token consumerToken, Token? oauthToken)
        {
            var authorization = new Dictionary<string, string>();

            var nonce = new byte[32];
            random.NextBytes(nonce);
            authorization.Add("oauth_consumer_key", consumerToken.key);
            authorization.Add("oauth_signature_method", "HMAC-SHA1");
            authorization.Add("oauth_nonce", Convert.ToBase64String(nonce));
            authorization.Add("oauth_timestamp", DateTime.Now.Second.ToString());
            authorization.Add("oauth_version", "1.0");

            if (oauthToken != null)
            {
                authorization.Add("oauth_token", oauthToken.Value.key);
            }

            var signatureBase = new StringBuilder();

            signatureBase.Append(message.Method.ToString().ToUpper());
            signatureBase.Append("&");
            signatureBase.Append(Uri.EscapeDataString(message.RequestUri.ToString()));

            var headers = new List<KeyValuePair<string, string>>();
            message.Headers.ToList().ForEach(x =>
            {
                headers.Add(new KeyValuePair<string, string>(x.Key, string.Join("\r\n", x.Value)));
            });
            var query = HttpUtility.ParseQueryString(message.RequestUri.Query);
            query.AllKeys.ToList().ForEach(x =>
            {
                headers.Add(new KeyValuePair<string, string>(x, query.Get(x)));
            });

            headers.Sort(
                delegate (KeyValuePair<string, string> pair1,
                KeyValuePair<string, string> pair2)
                {
                    return pair1.Value.CompareTo(pair2.Value);
                }
            );

            headers.ForEach(key =>
            {
                signatureBase.Append("&");
                signatureBase.Append(Uri.EscapeDataString(key.Key));
                signatureBase.Append("=");
                signatureBase.Append(Uri.EscapeDataString(key.Value));
            });

            var signatureKey = new StringBuilder();
            signatureKey.Append(Uri.EscapeDataString(consumerToken.secret));
            signatureKey.Append("&");
            if (oauthToken != null)
            {
                signatureKey.Append(Uri.EscapeDataString(oauthToken.Value.secret));
            }

            authorization.Add("oauth_signature", HMACSHA1Encode(signatureBase.ToString(), Encoding.UTF8.GetBytes(signatureKey.ToString())));

            var authorizationBuilder = new StringBuilder();
            authorizationBuilder.Append("OAuth");
            bool firstAuth = true;
            authorization.ToList().ForEach(pair =>
            {
                if (firstAuth)
                {
                    firstAuth = false;
                }
                else
                {
                    authorizationBuilder.Append(", ");
                }
                authorizationBuilder.Append(Uri.EscapeDataString(pair.Key));
                authorizationBuilder.Append("=");
                authorizationBuilder.Append("\"");
                authorizationBuilder.Append(Uri.EscapeDataString(pair.Value));
                authorizationBuilder.Append("\"");
            });
            message.Headers.Authorization = new AuthenticationHeaderValue("Authorization", authorizationBuilder.ToString());
        }
    }
}
