using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TwitterInterface.Data;

namespace TwitterInterface.API
{
    public interface ITwitterListAPI
    {
        //GET lists/list
        Task<List<TwitterList>> GetListsAsync(Account account, string screenName, bool reverse = false);
        Task<List<TwitterList>> GetListsAsync(Account account, long userId, bool reverse = false);

        //GET lists/members
        Task<CursoredList<User>> GetMemberOfListAsync(Account account, long listId, long count = 20, long cursor = -1);
        Task<CursoredList<User>> GetMemberOfListAsync(Account account, string slug, long ownerId, long count = 20, long cursor = -1);
        Task<CursoredList<User>> GetMemberOfListAsync(Account account, string slug, string ownerScreenName, long count = 20, long cursor = -1);

        //GET lists/members/show
        Task<User> GetUserFromListAsync(Account account, long userId, long listId);
        Task<User> GetUserFromListAsync(Account account, long userId, string slug, long ownerId);
        Task<User> GetUserFromListAsync(Account account, long userId, string slug, string ownerScreenName);
        //GET lists/members/show
        Task<User> GetUserFromListAsync(Account account, string screenName, long listId);
        Task<User> GetUserFromListAsync(Account account, string screenName, string slug, long ownerId);
        Task<User> GetUserFromListAsync(Account account, string screenName, string slug, string ownerScreenName);

        //GET lists/memberships
        Task<CursoredList<TwitterList>> GetMembershipsOfUserAsync(Account account, long userId, long count = 20, long cursor = -1, bool filterToOwnedLists = false);
        Task<CursoredList<TwitterList>> GetMembershipsOfUserAsync(Account account, string screenName, long count = 20, long cursor = -1, bool filterToOwnedLists = false);

        //GET lists/ownerships
        Task<CursoredList<TwitterList>> GetOwnershipsOfUserAsync(Account account, long userId, long count = 20, long cursor = -1);
        Task<CursoredList<TwitterList>> GetOwnershipsOfUserAsync(Account account, string screenName, long count = 20, long cursor = -1);

        //GET lists/show
        Task<TwitterList> GetListAsync(Account account, long listId);
        Task<TwitterList> GetListAsync(Account account, string slug, long ownerId);
        Task<TwitterList> GetListAsync(Account account, string slug, string ownerScreenName);

        //GET lists/statuses
        Task<List<Status>> GetListlineAsync(Account account, long listId, long sinceId, long maxId);
        Task<List<Status>> GetListlineAsync(Account account, string slug, long ownerId, long sinceId, long maxId);
        Task<List<Status>> GetListlineAsync(Account account, string slug, string ownerScreenName, long sinceId, long maxId);

        //GET lists/subscribers
        Task<CursoredList<User>> GetSubScribersFromListAsync(Account account, long listId, long count = 20, long cursor = -1);
        Task<CursoredList<User>> GetSubScribersFromListAsync(Account account, string slug, long ownerId, long count = 20, long cursor = -1);
        Task<CursoredList<User>> GetSubScribersFromListAsync(Account account, string slug, string ownerScreenName, long count = 20, long cursor = -1);

        //GET lists/subscribers/show
        Task<User> GetSubScriberFromListAsync(Account account, long userId, long listId);
        Task<User> GetSubScriberFromListAsync(Account account, long userId, string slug, long ownerId);
        Task<User> GetSubScriberFromListAsync(Account account, long userId, string slug, string ownerScreenName);
        //GET lists/subscribers/show
        Task<User> GetSubScriberFromListAsync(Account account, string screenName, long listId);
        Task<User> GetSubScriberFromListAsync(Account account, string screenName, string slug, long ownerId);
        Task<User> GetSubScriberFromListAsync(Account account, string screenName, string slug, string ownerScreenName);

        //GET lists/subscriptions
        Task<CursoredList<TwitterList>> GetUserSubscriptionsAsync(Account account, long userId, long count = 20, long cursor = -1);
        Task<CursoredList<TwitterList>> GetUserSubscriptionsAsync(Account account, string screenName, long count = 20, long cursor = -1);

        //POST lists/create
        Task<TwitterList> CreateListAsync(Account account, string name, string mode = "public", string description = "");

        //POST lists/update
        Task<TwitterList> UpdateListAsync(Account account, long listId, string name, string mode = "public", string description = "");
        Task<TwitterList> UpdateListAsync(Account account, string slug, long ownerId, string name, string mode = "public", string description = "");
        Task<TwitterList> UpdateListAsync(Account account, string slug, string ownerScreenName, string name, string mode = "public", string description = "");

        //POST lists/destroy
        Task<TwitterList> DestroyListAsync(Account account, long listId);
        Task<TwitterList> DestroyListAsync(Account account, string slug, long ownerId);
        Task<TwitterList> DestroyListAsync(Account account, string slug, string ownerScreenName);

        //POST lists/members/create
        Task AddMemberToUserAsync(Account account, long userId, long listId);
        Task AddMemberToUserAsync(Account account, long userId, string slug, long ownerId);
        Task AddMemberToUserAsync(Account account, long userId, string slug, string ownerScreenName);
        //POST lists/members/create
        Task AddMemberToUserAsync(Account account, string screenName, long listId);
        Task AddMemberToUserAsync(Account account, string screenName, string slug, long ownerId);
        Task AddMemberToUserAsync(Account account, string screenName, string slug, string ownerScreenName);

        //POST lists/members/create_all
        Task AddMembersToUserAsync(Account account, long[] userId, long listId);
        Task AddMembersToUserAsync(Account account, long[] userId, string slug, long ownerId);
        Task AddMembersToUserAsync(Account account, long[] userId, string slug, string ownerScreenName);
        //POST lists/members/create_all
        Task AddMembersToUserAsync(Account account, string[] screenName, long listId);
        Task AddMembersToUserAsync(Account account, string[] screenName, string slug, long ownerId);
        Task AddMembersToUserAsync(Account account, string[] screenName, string slug, string ownerScreenName);

        //POST lists/members/destory
        Task RemoveMemberToUserAsync(Account account, long userId, long listId);
        Task RemoveMemberToUserAsync(Account account, long userId, string slug, long ownerId);
        Task RemoveMemberToUserAsync(Account account, long userId, string slug, string ownerScreenName);
        //POST lists/members/destory
        Task RemoveMemberToUserAsync(Account account, string screenName, long listId);
        Task RemoveMemberToUserAsync(Account account, string screenName, string slug, long ownerId);
        Task RemoveMemberToUserAsync(Account account, string screenName, string slug, string ownerScreenName);

        //POST lists/members/destory_all
        Task RemoveMembersToUserAsync(Account account, long[] userId, long listId);
        Task RemoveMembersToUserAsync(Account account, long[] userId, string slug, long ownerId);
        Task RemoveMembersToUserAsync(Account account, long[] userId, string slug, string ownerScreenName);
        //POST lists/members/destory_all
        Task RemoveMembersToUserAsync(Account account, string[] screenName, long listId);
        Task RemoveMembersToUserAsync(Account account, string[] screenName, string slug, long ownerId);
        Task RemoveMembersToUserAsync(Account account, string[] screenName, string slug, string ownerScreenName);

        //POST lists/subscribers/create
        Task SubscribeAccountToListAsync(Account account, long listId);
        Task SubscribeAccountToListAsync(Account account, string slug, long ownerId);
        Task SubscribeAccountToListAsync(Account account, string slug, string ownerScreenName);

        //POST lists/subscribers/destroy
        Task UnsubscribeAccountToListAsync(Account account, long listId);
        Task UnsubscribeAccountToListAsync(Account account, string slug, long ownerId);
        Task UnsubscribeAccountToListAsync(Account account, string slug, string ownerScreenName);
    }
}
