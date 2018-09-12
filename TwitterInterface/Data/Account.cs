using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TwitterInterface.Data
{
    /// <summary>
    /// 로그인 되어있는 유저 토큰
    /// </summary>
    public class Account
    {
        public long id;

        public virtual void Save(Stream stream)
        {
            var writer = new BinaryWriter(stream);
            writer.Write(1); //Version Code
            writer.Write(id);
        }
    }
}
