using System;
using System.Collections.Generic;
using System.Text;
using TwitterInterface.Data;

namespace TwitterInterface.API
{
    interface StatusAPI
    {
        //GET statuses/home_timeline
        List<Status> GetTimeline(Account account, long count = 200, long sinceId = -1, long maxId = -1);

        //GET statuses/user_timeline
        List<Status> GetUserline(Account account, long userId, long count = 200, long sinceId = -1, long maxId = -1);

        //GET statuses/retweets_of_me
        List<Status> GetAccountRetweets(Account account, long count = 200, long sinceId = -1, long maxId = -1);

        //GET statuses/mentions_timeline
        List<Status> GetMentionline(Account account, long count = 200, long sinceId = -1, long maxId = -1);

        //POST statuses/update
        Status CreateStatus(Account account, StatusUpdate update);

        //POST statuses/destroy/:id
        Status DestroyStatus(Account account, long id);

        //GET statuses/show/:id
        Status GetStatuses(Account account, long id);
        //TODO: GET statuses/oembed
        //GET statuses/lookup
        List<Status> GetStatuses(Account account, long[] ids);

        //POST statuses/retweet/:id
        Status RetweetStatus(Account account, long id);

        //POST statuses/unretweet/:id
        Status UnretweetStatus(Account account, long id);

        //GET statuses/retweets/:id
        List<Status> GetRetweetedStatus(Account account, long id, long count = 100);

        //GET statuses/retweeters/ids
        CursoredList<long> GetRetweeterIds(Account account, long id, long count = 100, long cursor = -1);

        //POST favorites/create
        Status CreateFavorite(Account account, long id);

        //POST favorites/destroy
        Status DestroyFavorite(Account account, long id);

        //GET favorites/list
        List<Status> GetFavorites(Account account, long userId, long count = 200, long sinceId = -1, long maxId = -1);
        List<Status> GetFavorites(Account account, string userScreenName, long count = 200, long sinceId = -1, long maxId = -1);
    }
}
