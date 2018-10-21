using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data.Entity
{
    public class VideoVariant
    {
        public int Bitrate;
        public string ContentType;
        public string URL;
    }

    public class VideoInformation
    {
        public Indices AspectRatio;
        public long Duration;
        public VideoVariant[] Variants;
    }
}
