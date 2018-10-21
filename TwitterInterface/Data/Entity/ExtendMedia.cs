using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data.Entity
{
    public class ExtendMedia
    {
        public long ID;

        public string MediaURL;
        public string MediaURLHttps;
        public URL URL;

        public string Type;

        public VideoInformation Video;
        public Indices Indices;
    }
}
