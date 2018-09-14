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
        Task<GetEntriesResult> GetEntries(Account account, string id, long count, long maxPosition = -1, long minPosition = -1);

        //GET collections/list
        Task<FindListResult> FindList(Account account, long userId, long count, string cursor);
        Task<FindListResult> FindList(Account account, string screenName, long count, string cursor);
        Task<FindListResult> FindListByTweetId(Account account, long tweetId, long count, string cursor);

        //GET collections/show
        Task<Collection> GetCollection(Account account, string id);

        //POST collections/create
        Task<Collection> CreateCollection(Account account, string name, string description, string url, Collection.Order? order);

        //POST collections/update
        Task<Collection> UpdateCollection(Account account, string id, string name, string description, string url);

        //POST collections/destroy
        Task DestroyCollection(Account account, string id);

        //POST collections/entries/add
        Task AddTweetToCollection(Account account, string id, long statusId, long relativeTo, bool above = true);
        //POST collections/entries/move
        Task MoveTweetFromCollection(Account account, string id, long[] statusId, long relativeTo, bool above = true);
        //POST collections/entries/remove
        Task RemoveTweetFromCollection(Account account, string id, long statusId);

        //POST collections/entries/curate
        Task AddAllTweetToCollection(Account account, string id, long[] statusId, long relativeTo, bool above = true);
        Task RemoveAllTweetFromCollection(Account account, string id, long[] statusId, long relativeTo, bool above = true);


    }
}
