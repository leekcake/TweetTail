using System;
using System.Collections.Generic;
using System.Text;
using TwitterInterface.Data;
using TwitterInterface.API.Result;
using System.Threading.Tasks;

namespace TwitterInterface.API
{
    public interface CollectionAPI
    {
        //GET collections/entries
        Task<GetEntriesResult> GetEntries(string id, long count, long maxPosition = -1, long minPosition = -1);

        //GET collections/list
        Task<FindListResult> FindList(long userId, long count, string cursor);
        Task<FindListResult> FindList(string screenName, long count, string cursor);
        Task<FindListResult> FindListByTweetId(long tweetId, long count, string cursor);

        //GET collections/show
        Task<Collection> GetCollection(string id);

        //POST collections/create
        Task<Collection> CreateCollection(string name, string description, string url, Collection.Order? order);

        //POST collections/update
        Task<Collection> UpdateCollection(string id, string name, string description, string url);

        //POST collections/destroy
        Task DestroyCollection(string id);

        //POST collections/entries/add
        Task AddTweetToCollection(string id, long statusId, long relativeTo, bool above = true);
        //POST collections/entries/move
        Task MoveTweetFromCollection(string id, long[] statusId, long relativeTo, bool above = true);
        //POST collections/entries/remove
        Task RemoveTweetFromCollection(string id, long statusId);

        //POST collections/entries/curate
        Task AddAllTweetToCollection(string id, long[] statusId, long relativeTo, bool above = true);
        Task RemoveAllTweetFromCollection(string id, long[] statusId, long relativeTo, bool above = true);


    }
}
