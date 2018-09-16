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

        public static string OrderToString(Order order)
        {
            switch (order)
            {
                case Order.AddTime:
                    return "curation_reverse_chron";
                case Order.Newest:
                    return "tweet_reverse_chron";
                case Order.Oldest:
                    return "tweet_reverse_chron";
            }
            return "";
        }

        public string name;
        public string type;
        public string description;

        public long userId;

        public string collectionURL;
        public string url;

        public bool isPrivate;

        public Order timelineOrder;
    }
}
