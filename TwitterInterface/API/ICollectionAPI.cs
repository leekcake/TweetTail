using System;
using System.Collections.Generic;
using System.Text;
using TwitterInterface.Data;
using TwitterInterface.API.Result;
using System.Threading.Tasks;

namespace TwitterInterface.API
{
    public interface ICollectionAPI
    {
        //GET collections/entries
        Task<GetEntriesResult> GetEntriesAsync(Account account, string id, long count, long maxPosition = -1, long minPosition = -1);

        //GET collections/list
        Task<FindListResult> FindListAsync(Account account, long userId, long count, string cursor);
        Task<FindListResult> FindListAsync(Account account, string screenName, long count, string cursor);
        Task<FindListResult> FindListByTweetIdAsync(Account account, long tweetId, long count, string cursor);

        //GET collections/show
        Task<Collection> GetCollectionAsync(Account account, string id);

        //POST collections/create
        Task<Collection> CreateCollectionAsync(Account account, string name, string description, string url, Collection.Order? order);

        //POST collections/update
        Task<Collection> UpdateCollectionAsync(Account account, string id, string name, string description, string url);

        //POST collections/destroy
        Task DestroyCollectionAsync(Account account, string id);

        //POST collections/entries/add
        Task AddTweetToCollectionAsync(Account account, string id, long statusId, long relativeTo, bool above = true);
        //POST collections/entries/move
        Task MoveTweetFromCollectionAsync(Account account, string id, long[] statusId, long relativeTo, bool above = true);
        //POST collections/entries/remove
        Task RemoveTweetFromCollectionAsync(Account account, string id, long statusId);

        //POST collections/entries/curate
        Task AddAllTweetToCollectionAsync(Account account, string id, long[] statusId, long relativeTo, bool above = true);
        Task RemoveAllTweetFromCollectionAsync(Account account, string id, long[] statusId, long relativeTo, bool above = true);


    }
}
