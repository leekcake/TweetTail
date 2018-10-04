using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TwitterInterface.Data;

namespace TwitterInterface.API
{
    public interface StatusAPI
    {
        //GET statuses/home_timeline
        Task<List<Status>> GetTimeline(Account account, long count = 200, long sinceId = -1, long maxId = -1);

        //GET statuses/user_timeline
        Task<List<Status>> GetUserline(Account account, long userId, long count = 200, long sinceId = -1, long maxId = -1);

        //GET statuses/retweets_of_me
        Task<List<Status>> GetAccountRetweets(Account account, long count = 200, long sinceId = -1, long maxId = -1);

        //GET statuses/mentions_timeline
        Task<List<Status>> GetMentionline(Account account, long count = 200, long sinceId = -1, long maxId = -1);

        //POST statuses/update
        Task<Status> CreateStatus(Account account, StatusUpdate update);

        //POST statuses/destroy/:id
        Task<Status> DestroyStatus(Account account, long id);

        //GET statuses/show/:id
        Task<Status> GetStatuses(Account account, long id);
        //TODO: GET statuses/oembed
        //GET statuses/lookup
        Task<List<Status>> GetStatuses(Account account, long[] ids);

        //POST statuses/retweet/:id
        Task<Status> RetweetStatus(Account account, long id);

        //POST statuses/unretweet/:id
        Task<Status> UnretweetStatus(Account account, long id);

        //GET statuses/retweets/:id
        Task<List<Status>> GetRetweetedStatus(Account account, long id, long count = 100);

        //GET statuses/retweeters/ids
        Task<CursoredList<long>> GetRetweeterIds(Account account, long id, long count = 100, long cursor = -1);

        //POST favorites/create
        Task<Status> CreateFavorite(Account account, long id);

        //POST favorites/destroy
        Task<Status> DestroyFavorite(Account account, long id);

        //GET favorites/list
        Task<List<Status>> GetFavorites(Account account, long userId, long count = 200, long sinceId = -1, long maxId = -1);
        Task<List<Status>> GetFavorites(Account account, string userScreenName, long count = 200, long sinceId = -1, long maxId = -1);

        //GET statuses/media_timeline
        Task<List<Status>> GetMedialine(Account account, long userId, long count = 200, long sinceId = -1, long maxId = -1);

        //GET search/tweets
        Task<List<Status>> SearchTweet(Account account, string query, bool isRecent, int count = 100, string until = null, long sinceId = -1, long maxId = -1);

        //GET timeline/conversation/:id.json
        Task<List<Status>> GetConversation(Account account, long id);
    }
}
