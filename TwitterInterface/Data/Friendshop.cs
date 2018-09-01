using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data
{
    class Friendshop
    {
        public long sourceId;
        public string sourceScreen;

        public long targetId;
        public string targetScreen;

        public bool isBlocked;

        public bool isFollower;
        public bool isFollowing;
    }
}
