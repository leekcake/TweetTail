using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TwitterInterface.Data;

namespace TwitterLibrary.Data
{
    class LibAccount : Account
    {
        public Token token;

        public override void Save(Stream stream)
        {
            var writer = new BinaryWriter(stream);

            writer.Write(1); //Version Code
            writer.Write(id);

            writer.Write(token.key);
            writer.Write(token.secret);
        }

        public static LibAccount Load(Stream stream)
        {
            var reader = new BinaryReader(stream);

            var version = reader.ReadInt32();

            var id = reader.ReadInt64();
            var key = reader.ReadString();
            var secret = reader.ReadString();

            var result = new LibAccount();
            result.id = id;
            result.token = new Token(key, secret);

            return result;
        }
    }
}
