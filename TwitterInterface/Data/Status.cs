using System;
using System.Collections.Generic;
using System.Text;
using TwitterInterface.Data.Entity;

namespace TwitterInterface.Data
{
    public class Status : BasicEntitiesGroup
    {
        //이 트윗정보를 얻기 위해 사용된 계정 아이디
        public List<long> Issuer;

        public long ID;

        public DateTime CreatedAt;
        public User Creater;

        public string Text;
        public bool Truncated;

        public string Source;

        public long ReplyToStatusId;
        public long ReplyToUserId;
        public string ReplyToScreenName;

        //TODO: coordinates
        //TODO: place

        public bool IsQuote;
        public long QuotedStatusId;
        public Status QuotedStatus;

        public bool IsRetweetedStatus {
            get {
                return RetweetedStatus != null;
            }
        }
        public Status RetweetedStatus;

        public int ReplyCount;
        public int RetweetCount;
        public int FavoriteCount;
        
        public ExtendMedia[] ExtendMedias;
        public Polls[] Polls;

        public bool IsFavortedByUser;
        public bool IsRetweetedByUser;
        public Status RetweetByUser;

        public bool PossiblySensitive;
    }
}
