using System;
using System.Collections.Generic;
using System.Text;
using TwitterInterface.Data;

namespace TwitterInterface.API
{
    public interface TwitterListAPI
    {
        //GET lists/list
        List<TwitterList> GetLists(Account account, string screenName, bool reverse = false);
        List<TwitterList> GetLists(Account account, long userId, bool reverse = false);

        //GET lists/members
        CursoredList<User> GetMemberOfList(Account account, long listId, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);
        CursoredList<User> GetMemberOfList(Account account, string slug, long ownerId, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);
        CursoredList<User> GetMemberOfList(Account account, string slug, string ownerScreenName, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);

        //GET lists/members/show
        User GetUserFromList(Account account, long userId, long listId, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);
        User GetUserFromList(Account account, long userId, string slug, long ownerId, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);
        User GetUserFromList(Account account, long userId, string slug, string ownerScreenName, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);
        //GET lists/members/show
        User GetUserFromList(Account account, string screenName, long listId, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);
        User GetUserFromList(Account account, string screenName, string slug, long ownerId, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);
        User GetUserFromList(Account account, string screenName, string slug, string ownerScreenName, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);

        //GET lists/memberships
        CursoredList<TwitterList> GetMembershipsOfUser(Account account, long userId, long count = 20, long cursor = -1, bool filterToOwnedLists = false);
        CursoredList<TwitterList> GetMembershipsOfUser(Account account, string screenName, long count = 20, long cursor = -1, bool filterToOwnedLists = false);

        //GET lists/ownerships
        CursoredList<TwitterList> GetOwnershipsOfUser(Account account, long userId, long count = 20, long cursor = -1);
        CursoredList<TwitterList> GetOwnershipsOfUser(Account account, string screenName, long count = 20, long cursor = -1);

        //GET lists/show
        TwitterList GetList(Account account, long listId);
        TwitterList GetList(Account account, string slug, long ownerId);
        TwitterList GetList(Account account, string slug, string ownerScreenName);

        //GET lists/statuses
        List<Status> GetListline(Account account, long listId, long sinceId, long maxId, bool includeEntities = true, bool includeRts = false);
        List<Status> GetListline(Account account, string slug, long ownerId, long sinceId, long maxId, bool includeEntities = true, bool includeRts = false);
        List<Status> GetListline(Account account, string slug, string ownerScreenName, long sinceId, long maxId, bool includeEntities = true, bool includeRts = false);

        //GET lists/subscribers
        CursoredList<User> GetSubScribersFromList(Account account, long listId, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);
        CursoredList<User> GetSubScribersFromList(Account account, string slug, long ownerId, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);
        CursoredList<User> GetSubScribersFromList(Account account, string slug, string ownerScreenName, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);

        //GET lists/subscribers/show
        User GetSubScriberFromList(Account account, long userId, long listId, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);
        User GetSubScriberFromList(Account account, long userId, string slug, long ownerId, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);
        User GetSubScriberFromList(Account account, long userId, string slug, string ownerScreenName, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);
        //GET lists/subscribers/show
        User GetSubScriberFromList(Account account, string screenName, long listId, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);
        User GetSubScriberFromList(Account account, string screenName, string slug, long ownerId, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);
        User GetSubScriberFromList(Account account, string screenName, string slug, string ownerScreenName, long count = 20, long cursor = -1, bool includeEntities = true, bool skipStatus = false);

        //GET lists/subscriptions
        CursoredList<TwitterList> GetUserSubscriptions(Account account, long userId, long count = 20, long cursor = -1);
        CursoredList<TwitterList> GetUserSubscriptions(Account account, string screenName, long count = 20, long cursor = -1);

        //POST lists/create
        TwitterList CreateList(Account account, string name, string mode = "public", string description = "");

        //POST lists/update
        TwitterList UpdateList(Account account, long listId, string name, string mode = "public", string description = "");
        TwitterList UpdateList(Account account, string slug, long ownerId, string name, string mode = "public", string description = "");
        TwitterList UpdateList(Account account, string slug, string ownerScreenName, string name, string mode = "public", string description = "");

        //POST lists/destroy
        TwitterList DestroyList(Account account, long listId);
        TwitterList DestroyList(Account account, string slug, long ownerId);
        TwitterList DestroyList(Account account, string slug, string ownerScreenName);

        //POST lists/members/create
        void AddMemberToUser(Account account, long userId, long listId);
        void AddMemberToUser(Account account, long userId, string slug, long ownerId);
        void AddMemberToUser(Account account, long userId, string slug, string ownerScreenName);
        //POST lists/members/create
        void AddMemberToUser(Account account, string screenName, long listId);
        void AddMemberToUser(Account account, string screenName, string slug, long ownerId);
        void AddMemberToUser(Account account, string screenName, string slug, string ownerScreenName);

        //POST lists/members/create_all
        void AddMembersToUser(Account account, long[] userId, long listId);
        void AddMembersToUser(Account account, long[] userId, string slug, long ownerId);
        void AddMembersToUser(Account account, long[] userId, string slug, string ownerScreenName);
        //POST lists/members/create_all
        void AddMembersToUser(Account account, string[] screenName, long listId);
        void AddMembersToUser(Account account, string[] screenName, string slug, long ownerId);
        void AddMembersToUser(Account account, string[] screenName, string slug, string ownerScreenName);

        //POST lists/members/destory
        void RemoveMemberToUser(Account account, long userId, long listId);
        void RemoveMemberToUser(Account account, long userId, string slug, long ownerId);
        void RemoveMemberToUser(Account account, long userId, string slug, string ownerScreenName);
        //POST lists/members/destory
        void RemoveMemberToUser(Account account, string screenName, long listId);
        void RemoveMemberToUser(Account account, string screenName, string slug, long ownerId);
        void RemoveMemberToUser(Account account, string screenName, string slug, string ownerScreenName);

        //POST lists/members/destory_all
        void RemoveMembersToUser(Account account, long[] userId, long listId);
        void RemoveMembersToUser(Account account, long[] userId, string slug, long ownerId);
        void RemoveMembersToUser(Account account, long[] userId, string slug, string ownerScreenName);
        //POST lists/members/destory_all
        void RemoveMembersToUser(Account account, string[] screenName, long listId);
        void RemoveMembersToUser(Account account, string[] screenName, string slug, long ownerId);
        void RemoveMembersToUser(Account account, string[] screenName, string slug, string ownerScreenName);

        //POST lists/subscribers/create
        void SubscribeAccountToList(Account account, long listId);
        void SubscribeAccountToList(Account account, string slug, long ownerId);
        void SubscribeAccountToList(Account account, string slug, string ownerScreenName);

        //POST lists/subscribers/destroy
        void UnsubscribeAccountToList(Account account, long listId);
        void UnsubscribeAccountToList(Account account, string slug, long ownerId);
        void UnsubscribeAccountToList(Account account, string slug, string ownerScreenName);


    }
}
