using System;
using System.Collections.Generic;
using System.Text;
using TwitterInterface.Data;
using TwitterInterface.API.Result;

namespace TwitterInterface.API
{
    public interface CollectionAPI
    {
        //GET collections/entries
        GetEntriesResult GetEntries(string id, long count, long maxPosition = -1, long minPosition = -1);

        //GET collections/list
        FindListResult FindList(long userId, long count, string cursor);
        FindListResult FindList(string screenName, long count, string cursor);
        FindListResult FindListByTweetId(long tweetId, long count, string cursor);

        //GET collections/show
        Collection GetCollection(string id);

        //POST collections/create
        Collection CreateCollection(string name, string description, string url, Collection.Order? order);

        //POST collections/update
        Collection UpdateCollection(string id, string name, string description, string url);

        //POST collections/destroy
        void DestroyCollection(string id);

        //POST collections/entries/add
        void AddTweetToCollection(string id, long statusId, long relativeTo, bool above = true);
        //POST collections/entries/move
        void MoveTweetFromCollection(string id, long[] statusId, long relativeTo, bool above = true);
        //POST collections/entries/remove
        void RemoveTweetFromCollection(string id, long statusId);

        //POST collections/entries/curate
        void AddAllTweetToCollection(string id, long[] statusId, long relativeTo, bool above = true);
        void RemoveAllTweetFromCollection(string id, long[] statusId, long relativeTo, bool above = true);


    }
}
