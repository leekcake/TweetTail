using System;
using System.Collections.Generic;
using System.Text;
using TwitterInterface.Data;

namespace TwitterInterface.API.Result
{
    public struct GetEntriesResult
    {
        public Collection Collection;
        public List<Status> Tweet;
        public List<Collection.CollectionTweet> CollectionTweets;

        public long MaxPosition;
        public long MinPosition;
    }
}
