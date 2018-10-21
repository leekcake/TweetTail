using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data
{
    public class Friendship
    {
        public long ID;

        public string Name;
        public string ScreenName;

        public bool IsFollowing;
        public bool IsFollowingRequested;
        public bool IsFollowedBy;
        public bool IsBlocking;
        public bool IsMuting;
    }
}
