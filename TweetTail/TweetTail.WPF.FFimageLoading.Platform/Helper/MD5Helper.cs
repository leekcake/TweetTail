using System.Text;
using System.Runtime.InteropServices.WindowsRuntime;
using System;
using System.IO;
using System.Security.Cryptography;

namespace FFImageLoading.Helpers
{
    public class MD5Helper : IMD5Helper
    {
        private MD5 md5 = System.Security.Cryptography.MD5.Create();

        public string MD5(Stream input)
        {
            var bytes = ComputeHash(StreamToByteArray(input));
            return BitConverter.ToString(bytes)?.ToSanitizedKey();
        }

        public string MD5(string input)
        {
            var bytes = ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes)?.ToSanitizedKey();
        }

        public byte[] ComputeHash(byte[] input)
        {
            return md5.ComputeHash(input);
        }

        public static byte[] StreamToByteArray(Stream stream)
        {
            if (stream is MemoryStream)
            {
                return ((MemoryStream)stream).ToArray();
            }
            else
            {
                return ReadFully(stream);
            }
        }

        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}