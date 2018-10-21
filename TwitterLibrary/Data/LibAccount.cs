using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using TwitterInterface.Data;

namespace TwitterLibrary.Data
{
    class LibAccount : Account
    {
        public Token Consumer, Oauth;

        public override bool IsTweetdeck => false;

        public override bool IsShadowcopy => false;

        public override JObject Save()
        {
            var result = base.Save();
            result["type"] = "Lib";
            result["consumer_key"] = Consumer.Key;
            result["consumer_secret"] = Consumer.Secret;

            result["oauth_key"] = Oauth.Key;
            result["oauth_secret"] = Oauth.Secret;

            return result;
        }

        public static LibAccount Load(JObject data)
        {
            var result = new LibAccount();
            result.ID = data["id"].ToObject<long>();
            result.Consumer = new Token(data["consumer_key"].ToString(), data["consumer_secret"].ToString());
            result.Oauth = new Token(data["oauth_key"].ToString(), data["oauth_secret"].ToString());

            return result;
        }

        public override HttpRequestMessage GenerateRequest(HttpMethod method, Uri uri, KeyValuePair<string, string>[] query)
        {
            return Utils.GenerateHttpRequest(method, uri, query, Consumer, Oauth);
        }
    }
}
