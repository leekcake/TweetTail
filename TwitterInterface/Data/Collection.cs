using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data
{
    public class Collection
    {
        public struct CollectionTweet
        {
            public string featureContext;
            public long tweetId;
            public long tweetSortIndex;
        }

        public enum Order
        {
            AddTime, //curation_reverse_chron
            Oldest, //tweet_chron
            Newest //tweet_reverse_chron
        }

        string name;
        string type;
        string description;

        long userId;

        string collectionURL;
        string url;

        bool isPrivate;

        Order timelineOrder;
    }
}
