using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data
{
    class Friendship
    {
        public long id;

        public string name;
        public string screenName;

        public bool isFollowing;
        public bool isFollowingRequested;
        public bool isFollowedBy;
        public bool isBlocking;
        public bool isMuting;
    }
}
