using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data
{
    class Relationship
    {
        public long sourceId;
        public string sourceScreen;

        public long targetId;
        public string targetScreen;

        public bool isCanDM;

        public bool isBlocked;
        public bool isMuted;

        public bool isFollower;
        public bool isFollowing;

        public bool isAllReplies;
        public bool isWantRetweets;
        public bool isMarkedSpam;
        public bool isNotificationsEnabled;
    }
}
