using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace TwitterInterface.Data
{
    /// <summary>
    /// 로그인 되어있는 유저 토큰
    /// </summary>
    public abstract class Account
    {
        public long ID;
        public User User;

        public abstract HttpRequestMessage GenerateRequest(HttpMethod method, Uri uri, KeyValuePair<string, string>[] query);

        /// <summary>
        /// It's from tweetdeck?
        /// </summary>
        public abstract bool IsTweetdeck {
            get;
        }

        /// <summary>
        /// It acting like account?
        /// 
        /// if true, it's not saved because always created on startup
        /// </summary>
        public abstract bool IsShadowcopy {
            get;
        }

        public virtual JObject Save()
        {
            var result = new JObject();

            result["id"] = ID;

            return result;
        }
    }
}
