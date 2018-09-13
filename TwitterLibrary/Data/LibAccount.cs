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

        public override void Save(Stream stream)
        {
            var writer = new BinaryWriter(stream);

            writer.Write(1); //Version Code
            writer.Write(id);

            writer.Write(consumer.key);
            writer.Write(consumer.secret);

            writer.Write(oauth.key);
            writer.Write(oauth.secret);
        }

        public static LibAccount Load(Stream stream)
        {
            var reader = new BinaryReader(stream);

            var version = reader.ReadInt32();

            var id = reader.ReadInt64();
            var consumerKey = reader.ReadString();
            var consumerSecret = reader.ReadString();
            var oauthKey = reader.ReadString();
            var oauthSecret = reader.ReadString();

            var result = new LibAccount();
            result.id = id;
            result.consumer = new Token(consumerKey, consumerSecret);
            result.oauth = new Token(oauthKey, oauthSecret);

            return result;
        }
    }
}
