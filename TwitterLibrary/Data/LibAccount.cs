using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TwitterInterface.Data;

namespace TwitterLibrary.Data
{
    class LibAccount : Account
    {
        public Token consumer, oauth;

        public override JObject Save()
        {
            var result = base.Save();
            result["consumer_key"] = consumer.key;
            result["consumer_secret"] = consumer.secret;

            result["oauth_key"] = oauth.key;
            result["oauth_secret"] = oauth.secret;

            return result;
        }

        public static LibAccount Load(JObject data)
        {
            var result = new LibAccount();
            result.id = data["id"].ToObject<long>();
            result.consumer = new Token(data["consumer_key"].ToString(), data["consumer_secret"].ToString());
            result.oauth = new Token(data["oauth_key"].ToString(), data["oauth_secret"].ToString());

            return result;
        }
    }
}
