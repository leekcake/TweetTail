using System;
using System.Collections.Generic;
using System.Text;
using TwitterInterface.Data.Entity;

namespace TwitterInterface.Data
{
    public class Status
    {
        //이 트윗정보를 얻기 위해 사용된 계정 아이디
        public long[] issuer;

        public long id;

        public DateTime createdAt;
        public User creater;

        public string text;
        public bool truncated;

        public string source;

        public long replyToStatusId;
        public long replyToUserId;
        public string replyToScreenName;

        //TODO: coordinates
        //TODO: place

        public bool isQuote;
        public long quotedStatusId;
        public Status quotedStatus;

        public bool isRetweetedStatus {
            get {
                return retweetedStatus != null;
            }
        }
        public Status retweetedStatus;

        public int replyCount;
        public int retweetCount;
        public int favoriteCount;

        public HashTag[] hashtags;
        public URL[] urls;
        public UserMention[] userMentions;
        public ExtendMedia[] extendMedias;
        public Symbol[] symbols;
        public Polls[] polls;

        public bool isFavortedByUser;
        public bool isRetweetedByUser;
        public Status retweetByUser;

        public bool possiblySensitive;
    }
}
