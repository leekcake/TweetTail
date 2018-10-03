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
        public long id;
        public User user;

        public abstract HttpRequestMessage GenerateRequest(HttpMethod method, Uri uri, KeyValuePair<string, string>[] query);

        public virtual JObject Save()
        {
            var result = new JObject();

            result["id"] = id;

            return result;
        }
    }
}
