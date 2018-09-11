using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data
{
    public struct Token
    {
        public string key, secret;

        public Token(string key, string secret)
        {
            this.key = key;
            this.secret = secret;
        }
    }
}
