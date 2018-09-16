using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TwitterInterface.Data;

namespace TwitterInterface.API
{
    public interface UserAPI
    {
        //GET followers/ids
        Task<CursoredList<long>> GetFollowerIds(Account account, long userId, long cursor = -1, long count = 100);
        Task<CursoredList<long>> GetFollowerIds(Account account, string screenName, long cursor = -1, long count = 100);

        //GET followers/list
        Task<CursoredList<User>> GetFollowers(Account account, long userId, long cursor = -1, long count = 100);
        Task<CursoredList<User>> GetFollowers(Account account, string screenName, long cursor = -1, long count = 100);

        //GET friends/ids
        Task<CursoredList<long>> GetFriendIds(Account account, long userId, long cursor = -1, long count = 100);
        Task<CursoredList<long>> GetFriendIds(Account account, string screenName, long cursor = -1, long count = 100);

        //GET friends/list
        Task<CursoredList<User>> GetFriends(Account account, long userId, long cursor = -1, long count = 100);
        Task<CursoredList<User>> GetFriends(Account account, string screenName, long cursor = -1, long count = 100);

        //GET friendships/incoming
        Task<CursoredList<long>> GetPendingRequestToAccount(Account account, long cursor = -1);
        //GET friendships/outgoing
        Task<CursoredList<long>> GetPendingRequestFromAccount(Account account, long cursor = -1);

        //GET friendships/lookup
        Task<List<Friendship>> GetFriendship(Account account, params long[] userId);
        Task<List<Friendship>> GetFriendship(Account account, params string[] screenName);

        //GET friendships/no_retweets/ids
        Task<List<long>> GetNoRetweetListOfAccount(Account account);

        //GET friendships/show
        Task<Relationship> GetRelationship(Account account, long sourceId, long targetId);
        Task<Relationship> GetRelationship(Account account, string sourceScreenName, string targetScreenName);

        //GET users/lookup
        Task<List<User>> GetUsers(Account account, long[] userIds);
        Task<List<User>> GetUsers(Account account, string[] userScreenNames);

        //GET users/show
        Task<User> GetUser(Account account, long userIds);
        Task<User> GetUser(Account account, string userScreenNames);

        //GET users/search
        Task<List<User>> SearchUsers(Account account, string query, long page, long count = 20);

        //TODO: GET users/suggestions
        //TODO: GET users/suggestions/:slug
        //TODO: GET users/suggestions/:slug/members

        //POST friendships/create
        Task<User> CreateFriendship(Account account, long userId);
        Task<User> CreateFriendship(Account account, string screenName);

        //POST friendships/update
        Task<User> UpdateFriendship(Account account, long userId, bool? enableDeviceNotifications = null, bool? enableRetweet = null);
        Task<User> UpdateFriendship(Account account, string screenName, bool? enableDeviceNotifications = null, bool? enableRetweet = null);

        //POST friendships/destroy
        Task<User> DestroyFriendship(Account account, long userId);
        Task<User> DestroyFriendship(Account account, string screenName);

        //GET blocks/ids
        Task<CursoredList<long>> GetBlockIds(Account account, long cursor = -1);
        //GET blocks/list
        Task<CursoredList<User>> GetBlockList(Account account, long cursor = -1);

        //GET mutes/users/ids
        Task<CursoredList<long>> GetMuteIds(Account account, long cursor = -1);
        //GET mutes/users/list
        Task<CursoredList<User>> GetMuteUsers(Account account, long cursor = -1);

        //POST blocks/create
        Task<User> Block(Account account, long userId);
        Task<User> Block(Account account, string screenName);

        //POST blocks/destroy
        Task<User> Unblock(Account account, long userId);
        Task<User> Unblock(Account account, string screenName);

        //POST mutes/users/create
        Task<User> Mute(Account account, long userId);
        Task<User> Mute(Account account, string screenName);

        //POST mutes/users/create
        Task<User> Unmute(Account account, long userId);
        Task<User> Unmute(Account account, string screenName);

        //POST users/report_spam
        Task<User> ReportSpam(Account account, long userId, bool performBlock = true);
        Task<User> ReportSpam(Account account, string screenName, bool performBlock = true);
    }
}
