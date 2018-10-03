using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using TwitterInterface.Data;

namespace TwitterLibrary.Data
{
    class TDAccount : Account
    {
        private string csrfToken, twitterSession, authToken, personalizationId, lang, guestId;

        public void InitwithCookie(CookieCollection cookie)
        {
            csrfToken = cookie["ct0"].Value;
            twitterSession = cookie["_twitter_sess"].Value;
            authToken = cookie["auth_token"].Value;
            personalizationId = cookie["personalization_id"].Value;
            try
            {
                lang = cookie["lang"].Value;
            } catch
            {
                lang = "ko";
            }
            guestId = cookie["guest_id"].Value;
        }

        public override JObject Save()
        {
            var result = base.Save();

            result["type"] = "TD";
            result["csrfToken"] = csrfToken;
            result["twitterSession"] = twitterSession;
            result["authToken"] = authToken;
            result["personalizationId"] = personalizationId;
            result["lang"] = lang;
            result["guestId"] = guestId;

            return result;
        }

        public static TDAccount Load(JObject data)
        {
            var result = new TDAccount();

            result.csrfToken = data["csrfToken"].ToString();
            result.twitterSession = data["twitterSession"].ToString();
            result.authToken = data["authToken"].ToString();
            result.personalizationId = data["personalizationId"].ToString();
            result.lang = data["lang"].ToString();
            result.guestId = data["guestId"].ToString();

            return result;
        }

        public override HttpRequestMessage GenerateRequest(HttpMethod method, Uri uri, KeyValuePair<string, string>[] query)
        {
            HttpRequestMessage message = new HttpRequestMessage();
            message.Method = method;
            message.RequestUri = uri;

            message.Headers.Add("Authorization", "Bearer AAAAAAAAAAAAAAAAAAAAAF7aAAAAAAAASCiRjWvh7R5wxaKkFp7MM%2BhYBqM%3DbQ0JPmjU9F6ZoMhDfI4uTNAaQuTDm2uO9x3WFVr2xBZ2nhjdP0");

            message.Headers.Add("Cookie", string.Format("ct0={0}; auth_token={1}; _twitter_sess={2}; personalization_id={3}; lang={4}; guest_id={5}; ",
                csrfToken, authToken, twitterSession, personalizationId, lang, guestId
                ));
            message.Headers.Add("X-Twitter-Auth-Type", "OAuth2Session");
            message.Headers.Add("X-Csrf-Token", csrfToken);

            //Other parameters
            var queryDict = new SortedDictionary<string, string>();

            //Collect oauth from query
            foreach (var pair in query)
            {
                if (pair.Key.StartsWith("oauth"))
                {
                    //TODO: Warning about oauth query
                }
                else
                {
                    queryDict.Add(pair.Key, pair.Value);
                }
            }

            if (queryDict.Count != 0)
            {
                if (method == HttpMethod.Post)
                {
                    message.Content = new StringContent(Utils.ToWebString(queryDict), Encoding.UTF8, "application/x-www-form-urlencoded");
                }
                else
                {
                    message.RequestUri = new Uri(uri.ToString() + "?" + Utils.ToWebString(queryDict));
                }
            }
            
            return message;
        }
    }
}
