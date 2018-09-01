using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data.Entity
{
    public class VideoVariant
    {
        public int bitrate;
        public string contentType;
        public string url;
    }

    public class VideoInformation
    {
        public Indices aspectRatio;
        public long duration;
        public VideoVariant[] variants;
    }
}
