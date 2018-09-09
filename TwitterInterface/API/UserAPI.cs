using System;
using System.Collections.Generic;
using System.Text;
using TwitterInterface.Data;

namespace TwitterInterface.API
{
    public interface UserAPI
    {
        //GET followers/ids
        CursoredList<long> GetFollowerIds(Account account, long userId, long cursor = -1, long count = 100);
        CursoredList<long> GetFollowerIds(Account account, string screenName, long cursor = -1, long count = 100);

        //GET followers/list
        CursoredList<User> GetFollowers(Account account, long userId, long cursor = -1, long count = 100, bool skipStatus = false, bool includeUserEntities = true);
        CursoredList<User> GetFollowers(Account account, string screenName, long cursor = -1, long count = 100, bool skipStatus = false, bool includeUserEntities = true);

        //GET friends/ids
        CursoredList<long> GetFriendIds(Account account, long userId, long cursor = -1, long count = 100);
        CursoredList<long> GetFriendIds(Account account, string screenName, long cursor = -1, long count = 100);

        //GET friends/list
        CursoredList<User> GetFriends(Account account, long userId, long cursor = -1, long count = 100, bool skipStatus = false, bool includeUserEntities = true);
        CursoredList<User> GetFriends(Account account, string screenName, long cursor = -1, long count = 100, bool skipStatus = false, bool includeUserEntities = true);

        //GET friendships/incoming
        CursoredList<long> GetPendingRequestToAccount(Account account, long cursor = -1);
        //GET friendships/outgoing
        CursoredList<long> GetPendingRequestFromAccount(Account account, long cursor = -1);

        //GET friendships/lookup
        Friendship GetFriendship(Account account, long userId);
        Friendship GetFriendship(Account account, string screenName);

        //GET friendships/no_retweets/ids
        List<long> GetNoRetweetListOfAccount(Account account);

        //GET friendships/show
        Relationship GetRelationship(Account account, long sourceId, long targetId);
        Relationship GetRelationship(Account account, string sourceScreenName, string targetScreenName);

        //GET users/lookup
        List<User> GetUsers(Account account, long[] userIds, bool includeEntities = true);
        List<User> GetUsers(Account account, string[] userScreenNames, bool includeEntities = true);

        //GET users/show
        User GetUser(Account account, long userIds, bool includeEntities = true);
        User GetUser(Account account, string userScreenNames, bool includeEntities = true);

        //GET users/search
        List<User> SearchUsers(Account account, string query, long page, long count = 20, bool includeEntities = true);

        //TODO: GET users/suggestions
        //TODO: GET users/suggestions/:slug
        //TODO: GET users/suggestions/:slug/members

        //POST friendships/create
        User CreateFriendship(Account account, long userId);
        User CreateFriendship(Account account, string screenName);

        //POST friendships/update
        User UpdateFriendship(Account account, long userId, bool? enableDeviceNotifications = null, bool? enableRetweet = null);
        User UpdateFriendship(Account account, string screenName, bool? enableDeviceNotifications = null, bool? enableRetweet = null);

        //POST friendships/destroy
        User DestroyFriendship(Account account, long userId);
        User DestroyFriendship(Account account, string screenName);

        //GET blocks/ids
        CursoredList<long> GetBlockIds(Account account, long cursor = -1);
        //GET blocks/list
        CursoredList<User> GetBlockList(Account account, long cursor = -1, bool includeUserEntities = false, bool skipStatus = true);

        //GET mutes/users/ids
        CursoredList<long> GetMuteIds(Account account, long cursor = -1);
        //GET mutes/users/list
        CursoredList<User> GetMuteIds(Account account, long cursor = -1, bool includeUserEntities = false, bool skipStatus = true);

        //POST blocks/create
        User Block(Account account, long userId, bool includeUserEntities = false, bool skipStatus = true);
        User Block(Account account, string screenName, bool includeUserEntities = false, bool skipStatus = true);

        //POST blocks/destroy
        User Unblock(Account account, long userId, bool includeUserEntities = false, bool skipStatus = true);
        User Unblock(Account account, string screenName, bool includeUserEntities = false, bool skipStatus = true);

        //POST mutes/users/create
        User Mute(Account account, long userId);
        User Mute(Account account, string screenName);

        //POST mutes/users/create
        User Unmute(Account account, long userId);
        User Unmute(Account account, string screenName);

        //POST users/report_spam
        User ReportSpam(Account account, long userId, bool performBlock = true);
        User ReportSpam(Account account, string screenName, bool performBlock = true);
    }
}
