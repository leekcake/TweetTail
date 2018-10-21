using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TwitterInterface.Data;

namespace TwitterInterface.API
{
    public interface IStatusAPI
    {
        //GET statuses/home_timeline
        Task<List<Status>> GetTimelineAsync(Account account, long count = 200, long sinceId = -1, long maxId = -1);

        //GET statuses/user_timeline
        Task<List<Status>> GetUserlineAsync(Account account, long userId, long count = 200, long sinceId = -1, long maxId = -1);

        //GET statuses/retweets_of_me
        Task<List<Status>> GetAccountRetweetsAsync(Account account, long count = 200, long sinceId = -1, long maxId = -1);

        //GET statuses/mentions_timeline
        Task<List<Status>> GetMentionlineAsync(Account account, long count = 200, long sinceId = -1, long maxId = -1);

        //POST statuses/update
        Task<Status> CreateStatusAsync(Account account, StatusUpdate update);

        //POST statuses/destroy/:id
        Task<Status> DestroyStatusAsync(Account account, long id);

        //GET statuses/show/:id
        Task<Status> GetStatusesAsync(Account account, long id);
        //TODO: GET statuses/oembed
        //GET statuses/lookup
        Task<List<Status>> GetStatusesAsync(Account account, long[] ids);

        //POST statuses/retweet/:id
        Task<Status> RetweetStatusAsync(Account account, long id);

        //POST statuses/unretweet/:id
        Task<Status> UnretweetStatusAsync(Account account, long id);

        //GET statuses/retweets/:id
        Task<List<Status>> GetRetweetedStatusAsync(Account account, long id, long count = 100);

        //GET statuses/retweeters/ids
        Task<CursoredList<long>> GetRetweeterIdsAsync(Account account, long id, long count = 100, long cursor = -1);

        //POST favorites/create
        Task<Status> CreateFavoriteAsync(Account account, long id);

        //POST favorites/destroy
        Task<Status> DestroyFavoriteAsync(Account account, long id);

        //GET favorites/list
        Task<List<Status>> GetFavoritesAsync(Account account, long userId, long count = 200, long sinceId = -1, long maxId = -1);
        Task<List<Status>> GetFavoritesAsync(Account account, string userScreenName, long count = 200, long sinceId = -1, long maxId = -1);

        //GET statuses/media_timeline
        Task<List<Status>> GetMedialineAsync(Account account, long userId, long count = 200, long sinceId = -1, long maxId = -1);

        //GET search/tweets
        Task<List<Status>> SearchTweetAsync(Account account, string query, bool isRecent, int count = 100, string until = null, long sinceId = -1, long maxId = -1);

        //GET timeline/conversation/:id.json
        Task<List<Status>> GetConversationAsync(Account account, long id);
    }
}
