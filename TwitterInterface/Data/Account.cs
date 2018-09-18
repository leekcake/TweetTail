using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;

namespace TwitterInterface.Data
{
    /// <summary>
    /// 로그인 되어있는 유저 토큰
    /// </summary>
    public class Account
    {
        public long id;

        public virtual JObject Save()
        {
            var result = new JObject();

            result["id"] = id;

            return result;
        }
    }
}
