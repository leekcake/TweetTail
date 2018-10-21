using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TwitterInterface.Data;

namespace TwitterInterface.API
{
    public interface IUserAPI
    {
        //GET followers/ids
        Task<CursoredList<long>> GetFollowerIdsAsync(Account account, long userId, long cursor = -1, long count = 100);
        Task<CursoredList<long>> GetFollowerIdsAsync(Account account, string screenName, long cursor = -1, long count = 100);

        //GET followers/list
        Task<CursoredList<User>> GetFollowersAsync(Account account, long userId, long cursor = -1, long count = 100);
        Task<CursoredList<User>> GetFollowersAsync(Account account, string screenName, long cursor = -1, long count = 100);

        //GET friends/ids
        Task<CursoredList<long>> GetFriendIdsAsync(Account account, long userId, long cursor = -1, long count = 100);
        Task<CursoredList<long>> GetFriendIdsAsync(Account account, string screenName, long cursor = -1, long count = 100);

        //GET friends/list
        Task<CursoredList<User>> GetFriendsAsync(Account account, long userId, long cursor = -1, long count = 100);
        Task<CursoredList<User>> GetFriendsAsync(Account account, string screenName, long cursor = -1, long count = 100);

        //GET friendships/incoming
        Task<CursoredList<long>> GetPendingRequestToAccountAsync(Account account, long cursor = -1);
        //GET friendships/outgoing
        Task<CursoredList<long>> GetPendingRequestFromAccountAsync(Account account, long cursor = -1);

        //GET friendships/lookup
        Task<List<Friendship>> GetFriendshipAsync(Account account, params long[] userId);
        Task<List<Friendship>> GetFriendshipAsync(Account account, params string[] screenName);

        //GET friendships/no_retweets/ids
        Task<List<long>> GetNoRetweetListOfAccountAsync(Account account);

        //GET friendships/show
        Task<Relationship> GetRelationshipAsync(Account account, long sourceId, long targetId);
        Task<Relationship> GetRelationshipAsync(Account account, string sourceScreenName, string targetScreenName);

        //GET users/lookup
        Task<List<User>> GetUsersAsync(Account account, long[] userIds);
        Task<List<User>> GetUsersAsync(Account account, string[] userScreenNames);

        //GET users/show
        Task<User> GetUserAsync(Account account, long userIds);
        Task<User> GetUserAsync(Account account, string userScreenNames);

        //GET users/search
        Task<List<User>> SearchUsersAsync(Account account, string query, long page, long count = 20);

        //TODO: GET users/suggestions
        //TODO: GET users/suggestions/:slug
        //TODO: GET users/suggestions/:slug/members

        //POST friendships/create
        Task<User> CreateFriendshipAsync(Account account, long userId);
        Task<User> CreateFriendshipAsync(Account account, string screenName);

        //POST friendships/update
        Task<User> UpdateFriendshipAsync(Account account, long userId, bool? enableDeviceNotifications = null, bool? enableRetweet = null);
        Task<User> UpdateFriendshipAsync(Account account, string screenName, bool? enableDeviceNotifications = null, bool? enableRetweet = null);

        //POST friendships/destroy
        Task<User> DestroyFriendshipAsync(Account account, long userId);
        Task<User> DestroyFriendshipAsync(Account account, string screenName);

        //GET blocks/ids
        Task<CursoredList<long>> GetBlockIdsAsync(Account account, long cursor = -1);
        //GET blocks/list
        Task<CursoredList<User>> GetBlockListAsync(Account account, long cursor = -1);

        //GET mutes/users/ids
        Task<CursoredList<long>> GetMuteIdsAsync(Account account, long cursor = -1);
        //GET mutes/users/list
        Task<CursoredList<User>> GetMuteUsersAsync(Account account, long cursor = -1);

        //POST blocks/create
        Task<User> BlockAsync(Account account, long userId);
        Task<User> BlockAsync(Account account, string screenName);

        //POST blocks/destroy
        Task<User> UnblockAsync(Account account, long userId);
        Task<User> UnblockAsync(Account account, string screenName);

        //POST mutes/users/create
        Task<User> MuteAsync(Account account, long userId);
        Task<User> MuteAsync(Account account, string screenName);

        //POST mutes/users/create
        Task<User> UnmuteAsync(Account account, long userId);
        Task<User> UnmuteAsync(Account account, string screenName);

        //POST users/report_spam
        Task<User> ReportSpamAsync(Account account, long userId, bool performBlock = true);
        Task<User> ReportSpamAsync(Account account, string screenName, bool performBlock = true);
    }
}
