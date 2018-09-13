using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data.Entity
{
    public class ExtendMedia
    {
        public long id;

        public string mediaURL;
        public string mediaURLHttps;
        public URL url;

        public string type;

        public VideoInformation video;
        public Indices indices;
    }
}
