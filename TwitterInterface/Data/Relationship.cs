using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data
{
    public class Relationship
    {
        public long SourceId;
        public string SourceScreen;

        public long TargetId;
        public string TargetScreen;

        public bool IsCanDM;

        public bool IsBlocked;
        public bool IsMuted;

        public bool IsFollower;
        public bool IsFollowing;

        public bool IsAllReplies;
        public bool IsWantRetweets;
        public bool IsMarkedSpam;
        public bool IsNotificationsEnabled;

        public bool IsBlockedBy;
    }
}
