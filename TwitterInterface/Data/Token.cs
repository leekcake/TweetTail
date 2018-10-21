using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data
{
    public struct Token
    {
        public string Key, Secret;

        public Token(string key, string secret)
        {
            Key = key;
            Secret = secret;
        }
    }
}
