using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TwitterInterface.Data;

namespace TwitterInterface.API
{
    public interface TwitterListAPI
    {
        //GET lists/list
        Task<List<TwitterList>> GetLists(Account account, string screenName, bool reverse = false);
        Task<List<TwitterList>> GetLists(Account account, long userId, bool reverse = false);

        //GET lists/members
        Task<CursoredList<User>> GetMemberOfList(Account account, long listId, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);
        Task<CursoredList<User>> GetMemberOfList(Account account, string slug, long ownerId, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);
        Task<CursoredList<User>> GetMemberOfList(Account account, string slug, string ownerScreenName, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);

        //GET lists/members/show
        Task<User> GetUserFromList(Account account, long userId, long listId, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);
        Task<User> GetUserFromList(Account account, long userId, string slug, long ownerId, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);
        Task<User> GetUserFromList(Account account, long userId, string slug, string ownerScreenName, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);
        //GET lists/members/show
        Task<User> GetUserFromList(Account account, string screenName, long listId, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);
        Task<User> GetUserFromList(Account account, string screenName, string slug, long ownerId, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);
        Task<User> GetUserFromList(Account account, string screenName, string slug, string ownerScreenName, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);

        //GET lists/memberships
        Task<CursoredList<TwitterList>> GetMembershipsOfUser(Account account, long userId, long count = 20, long cursor = -1, bool filterToOwnedLists = false);
        Task<CursoredList<TwitterList>> GetMembershipsOfUser(Account account, string screenName, long count = 20, long cursor = -1, bool filterToOwnedLists = false);

        //GET lists/ownerships
        Task<CursoredList<TwitterList>> GetOwnershipsOfUser(Account account, long userId, long count = 20, long cursor = -1);
        Task<CursoredList<TwitterList>> GetOwnershipsOfUser(Account account, string screenName, long count = 20, long cursor = -1);

        //GET lists/show
        Task<TwitterList> GetList(Account account, long listId);
        Task<TwitterList> GetList(Account account, string slug, long ownerId);
        Task<TwitterList> GetList(Account account, string slug, string ownerScreenName);

        //GET lists/statuses
        Task<List<Status>> GetListline(Account account, long listId, long sinceId, long maxId, bool includeEntities = true, bool includeRts = false);
        Task<List<Status>> GetListline(Account account, string slug, long ownerId, long sinceId, long maxId, bool includeEntities = true, bool includeRts = false);
        Task<List<Status>> GetListline(Account account, string slug, string ownerScreenName, long sinceId, long maxId, bool includeEntities = true, bool includeRts = false);

        //GET lists/subscribers
        Task<CursoredList<User>> GetSubScribersFromList(Account account, long listId, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);
        Task<CursoredList<User>> GetSubScribersFromList(Account account, string slug, long ownerId, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);
        Task<CursoredList<User>> GetSubScribersFromList(Account account, string slug, string ownerScreenName, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);

        //GET lists/subscribers/show
        Task<User> GetSubScriberFromList(Account account, long userId, long listId, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);
        Task<User> GetSubScriberFromList(Account account, long userId, string slug, long ownerId, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);
        Task<User> GetSubScriberFromList(Account account, long userId, string slug, string ownerScreenName, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);
        //GET lists/subscribers/show
        Task<User> GetSubScriberFromList(Account account, string screenName, long listId, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);
        Task<User> GetSubScriberFromList(Account account, string screenName, string slug, long ownerId, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);
        Task<User> GetSubScriberFromList(Account account, string screenName, string slug, string ownerScreenName, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);

        //GET lists/subscriptions
        Task<CursoredList<TwitterList>> GetUserSubscriptions(Account account, long userId, long count = 20, long cursor = -1);
        Task<CursoredList<TwitterList>> GetUserSubscriptions(Account account, string screenName, long count = 20, long cursor = -1);

        //POST lists/create
        Task<TwitterList> CreateList(Account account, string name, string mode = "public", string description = "");

        //POST lists/update
        Task<TwitterList> UpdateList(Account account, long listId, string name, string mode = "public", string description = "");
        Task<TwitterList> UpdateList(Account account, string slug, long ownerId, string name, string mode = "public", string description = "");
        Task<TwitterList> UpdateList(Account account, string slug, string ownerScreenName, string name, string mode = "public", string description = "");

        //POST lists/destroy
        Task<TwitterList> DestroyList(Account account, long listId);
        Task<TwitterList> DestroyList(Account account, string slug, long ownerId);
        Task<TwitterList> DestroyList(Account account, string slug, string ownerScreenName);

        //POST lists/members/create
        Task AddMemberToUser(Account account, long userId, long listId);
        Task AddMemberToUser(Account account, long userId, string slug, long ownerId);
        Task AddMemberToUser(Account account, long userId, string slug, string ownerScreenName);
        //POST lists/members/create
        Task AddMemberToUser(Account account, string screenName, long listId);
        Task AddMemberToUser(Account account, string screenName, string slug, long ownerId);
        Task AddMemberToUser(Account account, string screenName, string slug, string ownerScreenName);

        //POST lists/members/create_all
        Task AddMembersToUser(Account account, long[] userId, long listId);
        Task AddMembersToUser(Account account, long[] userId, string slug, long ownerId);
        Task AddMembersToUser(Account account, long[] userId, string slug, string ownerScreenName);
        //POST lists/members/create_all
        Task AddMembersToUser(Account account, string[] screenName, long listId);
        Task AddMembersToUser(Account account, string[] screenName, string slug, long ownerId);
        Task AddMembersToUser(Account account, string[] screenName, string slug, string ownerScreenName);

        //POST lists/members/destory
        Task RemoveMemberToUser(Account account, long userId, long listId);
        Task RemoveMemberToUser(Account account, long userId, string slug, long ownerId);
        Task RemoveMemberToUser(Account account, long userId, string slug, string ownerScreenName);
        //POST lists/members/destory
        Task RemoveMemberToUser(Account account, string screenName, long listId);
        Task RemoveMemberToUser(Account account, string screenName, string slug, long ownerId);
        Task RemoveMemberToUser(Account account, string screenName, string slug, string ownerScreenName);

        //POST lists/members/destory_all
        Task RemoveMembersToUser(Account account, long[] userId, long listId);
        Task RemoveMembersToUser(Account account, long[] userId, string slug, long ownerId);
        Task RemoveMembersToUser(Account account, long[] userId, string slug, string ownerScreenName);
        //POST lists/members/destory_all
        Task RemoveMembersToUser(Account account, string[] screenName, long listId);
        Task RemoveMembersToUser(Account account, string[] screenName, string slug, long ownerId);
        Task RemoveMembersToUser(Account account, string[] screenName, string slug, string ownerScreenName);

        //POST lists/subscribers/create
        Task SubscribeAccountToList(Account account, long listId);
        Task SubscribeAccountToList(Account account, string slug, long ownerId);
        Task SubscribeAccountToList(Account account, string slug, string ownerScreenName);

        //POST lists/subscribers/destroy
        Task UnsubscribeAccountToList(Account account, long listId);
        Task UnsubscribeAccountToList(Account account, string slug, long ownerId);
        Task UnsubscribeAccountToList(Account account, string slug, string ownerScreenName);
    }
}
