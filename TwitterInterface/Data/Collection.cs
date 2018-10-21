using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data
{
    public class Collection
    {
        public struct CollectionTweet
        {
            public string FeatureContext;
            public long TweetId;
            public long TweetSortIndex;
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

        public string Name;
        public string Type;
        public string Description;

        public long UserId;

        public string CollectionURL;
        public string URL;

        public bool IsPrivate;

        public Order TimelineOrder;
    }
}
