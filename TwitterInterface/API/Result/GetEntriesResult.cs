using System;
using System.Collections.Generic;
using System.Text;
using TwitterInterface.Data;

namespace TwitterInterface.API.Result
{
    public struct GetEntriesResult
    {
        public Collection collection;
        public List<Status> tweet;
        public List<Collection.CollectionTweet> collectionTweets;

        public long maxPosition;
        public long minPosition;
    }
}
