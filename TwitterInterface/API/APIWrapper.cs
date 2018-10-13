using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TwitterInterface.API.Result;
using TwitterInterface.Container;
using TwitterInterface.Data;

namespace TwitterInterface.API
{
    public class APIWrapper : API
    {
        private API origin;

        public APIWrapper(API origin)
        {
            this.origin = origin;
        }

        public virtual Task AddAllTweetToCollection(Account account, string id, long[] statusId, long relativeTo, bool above = true)
        {
            return origin.AddAllTweetToCollection(account, id, statusId, relativeTo, above);
        }

        public virtual Task AddMembersToUser(Account account, long[] userId, long listId)
        {
            return origin.AddMembersToUser(account, userId, listId);
        }

        public virtual Task AddMembersToUser(Account account, long[] userId, string slug, long ownerId)
        {
            return origin.AddMembersToUser(account, userId, slug, ownerId);
        }

        public virtual Task AddMembersToUser(Account account, long[] userId, string slug, string ownerScreenName)
        {
            return origin.AddMembersToUser(account, userId, slug, ownerScreenName);
        }

        public virtual Task AddMembersToUser(Account account, string[] screenName, long listId)
        {
            return origin.AddMembersToUser(account, screenName, listId);
        }

        public virtual Task AddMembersToUser(Account account, string[] screenName, string slug, long ownerId)
        {
            return origin.AddMembersToUser(account, screenName, slug, ownerId);
        }

        public virtual Task AddMembersToUser(Account account, string[] screenName, string slug, string ownerScreenName)
        {
            return origin.AddMembersToUser(account, screenName, slug, ownerScreenName);
        }

        public virtual Task AddMemberToUser(Account account, long userId, long listId)
        {
            return origin.AddMemberToUser(account, userId, listId);
        }

        public virtual Task AddMemberToUser(Account account, long userId, string slug, long ownerId)
        {
            return origin.AddMemberToUser(account, userId, slug, ownerId);
        }

        public virtual Task AddMemberToUser(Account account, long userId, string slug, string ownerScreenName)
        {
            return origin.AddMemberToUser(account, userId, slug, ownerScreenName);
        }

        public virtual Task AddMemberToUser(Account account, string screenName, long listId)
        {
            return origin.AddMemberToUser(account, screenName, listId);
        }

        public virtual Task AddMemberToUser(Account account, string screenName, string slug, long ownerId)
        {
            return origin.AddMemberToUser(account, screenName, slug, ownerId);
        }

        public virtual Task AddMemberToUser(Account account, string screenName, string slug, string ownerScreenName)
        {
            return origin.AddMemberToUser(account, screenName, slug, ownerScreenName);
        }

        public virtual Task AddTweetToCollection(Account account, string id, long statusId, long relativeTo, bool above = true)
        {
            return origin.AddTweetToCollection(account, id, statusId, relativeTo, above);
        }

        public virtual Task<User> Block(Account account, long userId)
        {
            return origin.Block(account, userId);
        }

        public virtual Task<User> Block(Account account, string screenName)
        {
            return origin.Block(account, screenName);
        }

        public virtual Task<Collection> CreateCollection(Account account, string name, string description, string url, Collection.Order? order)
        {
            return origin.CreateCollection(account, name, description, url, order);
        }

        public virtual Task<Status> CreateFavorite(Account account, long id)
        {
            return origin.CreateFavorite(account, id);
        }

        public virtual Task<User> CreateFriendship(Account account, long userId)
        {
            return origin.CreateFriendship(account, userId);
        }

        public virtual Task<User> CreateFriendship(Account account, string screenName)
        {
            return origin.CreateFriendship(account, screenName);
        }

        public virtual Task<TwitterList> CreateList(Account account, string name, string mode = "public", string description = "")
        {
            return origin.CreateList(account, name, mode, description);
        }

        public virtual Task<SavedSearch> CreateSavedSearch(Account account, string query)
        {
            return origin.CreateSavedSearch(account, query);
        }

        public virtual Task<Status> CreateStatus(Account account, StatusUpdate update)
        {
            return origin.CreateStatus(account, update);
        }

        public virtual Task DestroyCollection(Account account, string id)
        {
            return origin.DestroyCollection(account, id);
        }

        public virtual Task<Status> DestroyFavorite(Account account, long id)
        {
            return origin.DestroyFavorite(account, id);
        }

        public virtual Task<User> DestroyFriendship(Account account, long userId)
        {
            return origin.DestroyFriendship(account, userId);
        }

        public virtual Task<User> DestroyFriendship(Account account, string screenName)
        {
            return origin.DestroyFriendship(account, screenName);
        }

        public virtual Task<TwitterList> DestroyList(Account account, long listId)
        {
            return origin.DestroyList(account, listId);
        }

        public virtual Task<TwitterList> DestroyList(Account account, string slug, long ownerId)
        {
            return origin.DestroyList(account, slug, ownerId);
        }

        public virtual Task<TwitterList> DestroyList(Account account, string slug, string ownerScreenName)
        {
            return origin.DestroyList(account, slug, ownerScreenName);
        }

        public virtual Task<SavedSearch> DestroySavedSearch(Account account, long id)
        {
            return origin.DestroySavedSearch(account, id);
        }

        public virtual Task<Status> DestroyStatus(Account account, long id)
        {
            return origin.DestroyStatus(account, id);
        }

        public virtual Task<FindListResult> FindList(Account account, long userId, long count, string cursor)
        {
            return origin.FindList(account, userId, count, cursor);
        }

        public virtual Task<FindListResult> FindList(Account account, string screenName, long count, string cursor)
        {
            return origin.FindList(account, screenName, count, cursor);
        }

        public virtual Task<FindListResult> FindListByTweetId(Account account, long tweetId, long count, string cursor)
        {
            return origin.FindListByTweetId(account, tweetId, count, cursor);
        }

        public virtual Task<Account> GetAccountFromTweetdeckCookie(CookieCollection cookieData)
        {
            return origin.GetAccountFromTweetdeckCookie(cookieData);
        }

        public virtual Task<List<Status>> GetAccountRetweets(Account account, long count = 200, long sinceId = -1, long maxId = -1)
        {
            return origin.GetAccountRetweets(account, count, sinceId, maxId);
        }

        public virtual Task<AccountSetting> GetAccountSetting(Account account)
        {
            return origin.GetAccountSetting(account);
        }

        public virtual Task<CursoredList<long>> GetBlockIds(Account account, long cursor = -1)
        {
            return origin.GetBlockIds(account, cursor);
        }

        public virtual Task<CursoredList<User>> GetBlockList(Account account, long cursor = -1)
        {
            return origin.GetBlockList(account, cursor);
        }

        public virtual Task<Collection> GetCollection(Account account, string id)
        {
            return origin.GetCollection(account, id);
        }

        public virtual Task<List<Account>> GetContributees(Account account)
        {
            return origin.GetContributees(account);
        }

        public virtual Task<List<Status>> GetConversation(Account account, long id)
        {
            return origin.GetConversation(account, id);
        }

        public virtual Task<GetEntriesResult> GetEntries(Account account, string id, long count, long maxPosition = -1, long minPosition = -1)
        {
            return origin.GetEntries(account, id, count, maxPosition, minPosition);
        }

        public virtual Task<List<Status>> GetFavorites(Account account, long userId, long count = 200, long sinceId = -1, long maxId = -1)
        {
            return origin.GetFavorites(account, userId, count, sinceId, maxId);
        }

        public virtual Task<List<Status>> GetFavorites(Account account, string userScreenName, long count = 200, long sinceId = -1, long maxId = -1)
        {
            return origin.GetFavorites(account, userScreenName, count, sinceId, maxId);
        }

        public virtual Task<CursoredList<long>> GetFollowerIds(Account account, long userId, long cursor = -1, long count = 100)
        {
            return origin.GetFollowerIds(account, userId, cursor, count);
        }

        public virtual Task<CursoredList<long>> GetFollowerIds(Account account, string screenName, long cursor = -1, long count = 100)
        {
            return origin.GetFollowerIds(account, screenName, cursor, count);
        }

        public virtual Task<CursoredList<User>> GetFollowers(Account account, long userId, long cursor = -1, long count = 100)
        {
            return origin.GetFollowers(account, userId, cursor, count);
        }

        public virtual Task<CursoredList<User>> GetFollowers(Account account, string screenName, long cursor = -1, long count = 100)
        {
            return origin.GetFollowers(account, screenName, cursor, count);
        }

        public virtual Task<CursoredList<long>> GetFriendIds(Account account, long userId, long cursor = -1, long count = 100)
        {
            return origin.GetFriendIds(account, userId, cursor, count);
        }

        public virtual Task<CursoredList<long>> GetFriendIds(Account account, string screenName, long cursor = -1, long count = 100)
        {
            return origin.GetFriendIds(account, screenName, cursor, count);
        }

        public virtual Task<CursoredList<User>> GetFriends(Account account, long userId, long cursor = -1, long count = 100)
        {
            return origin.GetFriends(account, userId, cursor, count);
        }

        public virtual Task<CursoredList<User>> GetFriends(Account account, string screenName, long cursor = -1, long count = 100)
        {
            return origin.GetFriends(account, screenName, cursor, count);
        }

        public virtual Task<List<Friendship>> GetFriendship(Account account, params long[] userId)
        {
            return origin.GetFriendship(account, userId);
        }

        public virtual Task<List<Friendship>> GetFriendship(Account account, params string[] screenName)
        {
            return origin.GetFriendship(account, screenName);
        }

        public virtual Task<TwitterList> GetList(Account account, long listId)
        {
            return origin.GetList(account, listId);
        }

        public virtual Task<TwitterList> GetList(Account account, string slug, long ownerId)
        {
            return origin.GetList(account, slug, ownerId);
        }

        public virtual Task<TwitterList> GetList(Account account, string slug, string ownerScreenName)
        {
            return origin.GetList(account, slug, ownerScreenName);
        }

        public virtual Task<List<Status>> GetListline(Account account, long listId, long sinceId, long maxId)
        {
            return origin.GetListline(account, listId, sinceId, maxId);
        }

        public virtual Task<List<Status>> GetListline(Account account, string slug, long ownerId, long sinceId, long maxId)
        {
            return origin.GetListline(account, slug, ownerId, sinceId, maxId);
        }

        public virtual Task<List<Status>> GetListline(Account account, string slug, string ownerScreenName, long sinceId, long maxId)
        {
            return origin.GetListline(account, slug, ownerScreenName, sinceId, maxId);
        }

        public virtual Task<List<TwitterList>> GetLists(Account account, string screenName, bool reverse = false)
        {
            return origin.GetLists(account, screenName, reverse);
        }

        public virtual Task<List<TwitterList>> GetLists(Account account, long userId, bool reverse = false)
        {
            return origin.GetLists(account, userId, reverse);
        }

        public virtual Task<LoginToken> GetLoginTokenAsync(Token consumerToken)
        {
            return origin.GetLoginTokenAsync(consumerToken);
        }

        public virtual Task<List<Status>> GetMedialine(Account account, long userId, long count = 200, long sinceId = -1, long maxId = -1)
        {
            return origin.GetMedialine(account, userId, count, sinceId, maxId);
        }

        public virtual Task<CursoredList<User>> GetMemberOfList(Account account, long listId, long count = 20, long cursor = -1)
        {
            return origin.GetMemberOfList(account, listId, count, cursor);
        }

        public virtual Task<CursoredList<User>> GetMemberOfList(Account account, string slug, long ownerId, long count = 20, long cursor = -1)
        {
            return origin.GetMemberOfList(account, slug, ownerId, count, cursor);
        }

        public virtual Task<CursoredList<User>> GetMemberOfList(Account account, string slug, string ownerScreenName, long count = 20, long cursor = -1)
        {
            return origin.GetMemberOfList(account, slug, ownerScreenName, count, cursor);
        }

        public virtual Task<CursoredList<TwitterList>> GetMembershipsOfUser(Account account, long userId, long count = 20, long cursor = -1, bool filterToOwnedLists = false)
        {
            return origin.GetMembershipsOfUser(account, userId, count, cursor, filterToOwnedLists);
        }

        public virtual Task<CursoredList<TwitterList>> GetMembershipsOfUser(Account account, string screenName, long count = 20, long cursor = -1, bool filterToOwnedLists = false)
        {
            return origin.GetMembershipsOfUser(account, screenName, count, cursor, filterToOwnedLists);
        }

        public virtual Task<List<Status>> GetMentionline(Account account, long count = 200, long sinceId = -1, long maxId = -1)
        {
            return origin.GetMentionline(account, count, sinceId, maxId);
        }

        public virtual Task<CursoredList<long>> GetMuteIds(Account account, long cursor = -1)
        {
            return origin.GetMuteIds(account, cursor);
        }

        public virtual Task<CursoredList<User>> GetMuteUsers(Account account, long cursor = -1)
        {
            return origin.GetMuteUsers(account, cursor);
        }

        public virtual Task<List<long>> GetNoRetweetListOfAccount(Account account)
        {
            return origin.GetNoRetweetListOfAccount(account);
        }

        public virtual Task<List<Notification>> GetNotifications(Account account, int count = 40, long sinceId = -1, long maxId = -1)
        {
            return origin.GetNotifications(account, count, sinceId, maxId);
        }

        public virtual Task<CursoredList<TwitterList>> GetOwnershipsOfUser(Account account, long userId, long count = 20, long cursor = -1)
        {
            return origin.GetOwnershipsOfUser(account, userId, count, cursor);
        }

        public virtual Task<CursoredList<TwitterList>> GetOwnershipsOfUser(Account account, string screenName, long count = 20, long cursor = -1)
        {
            return origin.GetOwnershipsOfUser(account, screenName, count, cursor);
        }

        public virtual Task<CursoredList<long>> GetPendingRequestFromAccount(Account account, long cursor = -1)
        {
            return origin.GetPendingRequestFromAccount(account, cursor);
        }

        public virtual Task<CursoredList<long>> GetPendingRequestToAccount(Account account, long cursor = -1)
        {
            return origin.GetPendingRequestToAccount(account, cursor);
        }

        public virtual Task<Relationship> GetRelationship(Account account, long sourceId, long targetId)
        {
            return origin.GetRelationship(account, sourceId, targetId);
        }

        public virtual Task<Relationship> GetRelationship(Account account, string sourceScreenName, string targetScreenName)
        {
            return origin.GetRelationship(account, sourceScreenName, targetScreenName);
        }

        public virtual Task<List<Status>> GetRetweetedStatus(Account account, long id, long count = 100)
        {
            return origin.GetRetweetedStatus(account, id, count);
        }

        public virtual Task<CursoredList<long>> GetRetweeterIds(Account account, long id, long count = 100, long cursor = -1)
        {
            return origin.GetRetweeterIds(account, id, count, cursor);
        }

        public virtual Task<SavedSearch> GetSavedSearchById(Account account, long id)
        {
            return origin.GetSavedSearchById(account, id);
        }

        public virtual Task<List<SavedSearch>> GetSavedSearches(Account account)
        {
            return origin.GetSavedSearches(account);
        }

        public virtual Task<Status> GetStatuses(Account account, long id)
        {
            return origin.GetStatuses(account, id);
        }

        public virtual Task<List<Status>> GetStatuses(Account account, long[] ids)
        {
            return origin.GetStatuses(account, ids);
        }

        public virtual Task<User> GetSubScriberFromList(Account account, long userId, long listId)
        {
            return origin.GetSubScriberFromList(account, userId, listId);
        }

        public virtual Task<User> GetSubScriberFromList(Account account, long userId, string slug, long ownerId)
        {
            return origin.GetSubScriberFromList(account, userId, slug, ownerId);
        }

        public virtual Task<User> GetSubScriberFromList(Account account, long userId, string slug, string ownerScreenName)
        {
            return origin.GetSubScriberFromList(account, userId, slug, ownerScreenName);
        }

        public virtual Task<User> GetSubScriberFromList(Account account, string screenName, long listId)
        {
            return origin.GetSubScriberFromList(account, screenName, listId);
        }

        public virtual Task<User> GetSubScriberFromList(Account account, string screenName, string slug, long ownerId)
        {
            return origin.GetSubScriberFromList(account, screenName, slug, ownerId);
        }

        public virtual Task<User> GetSubScriberFromList(Account account, string screenName, string slug, string ownerScreenName)
        {
            return origin.GetSubScriberFromList(account, screenName, slug, ownerScreenName);
        }

        public virtual Task<CursoredList<User>> GetSubScribersFromList(Account account, long listId, long count = 20, long cursor = -1)
        {
            return origin.GetSubScribersFromList(account, listId, count, cursor);
        }

        public virtual Task<CursoredList<User>> GetSubScribersFromList(Account account, string slug, long ownerId, long count = 20, long cursor = -1)
        {
            return origin.GetSubScribersFromList(account, slug, ownerId, count, cursor);
        }

        public virtual Task<CursoredList<User>> GetSubScribersFromList(Account account, string slug, string ownerScreenName, long count = 20, long cursor = -1)
        {
            return origin.GetSubScribersFromList(account, slug, ownerScreenName, count, cursor);
        }

        public virtual Task<List<Status>> GetTimeline(Account account, long count = 200, long sinceId = -1, long maxId = -1)
        {
            return origin.GetTimeline(account, count, sinceId, maxId);
        }

        public virtual Task<User> GetUser(Account account, long userIds)
        {
            return origin.GetUser(account, userIds);
        }

        public virtual Task<User> GetUser(Account account, string userScreenNames)
        {
            return origin.GetUser(account, userScreenNames);
        }

        public virtual Task<User> GetUserFromList(Account account, long userId, long listId)
        {
            return origin.GetUserFromList(account, userId, listId);
        }

        public virtual Task<User> GetUserFromList(Account account, long userId, string slug, long ownerId)
        {
            return origin.GetUserFromList(account, userId, slug, ownerId);
        }

        public virtual Task<User> GetUserFromList(Account account, long userId, string slug, string ownerScreenName)
        {
            return origin.GetUserFromList(account, userId, slug, ownerScreenName);
        }

        public virtual Task<User> GetUserFromList(Account account, string screenName, long listId)
        {
            return origin.GetUserFromList(account, screenName, listId);
        }

        public virtual Task<User> GetUserFromList(Account account, string screenName, string slug, long ownerId)
        {
            return origin.GetUserFromList(account, screenName, slug, ownerId);
        }

        public virtual Task<User> GetUserFromList(Account account, string screenName, string slug, string ownerScreenName)
        {
            return origin.GetUserFromList(account, screenName, slug, ownerScreenName);
        }

        public virtual Task<List<Status>> GetUserline(Account account, long userId, long count = 200, long sinceId = -1, long maxId = -1)
        {
            return origin.GetUserline(account, userId, count, sinceId, maxId);
        }

        public virtual Task<List<User>> GetUsers(Account account, long[] userIds)
        {
            return origin.GetUsers(account, userIds);
        }

        public virtual Task<List<User>> GetUsers(Account account, string[] userScreenNames)
        {
            return origin.GetUsers(account, userScreenNames);
        }

        public virtual Task<CursoredList<TwitterList>> GetUserSubscriptions(Account account, long userId, long count = 20, long cursor = -1)
        {
            return origin.GetUserSubscriptions(account, userId, count, cursor);
        }

        public virtual Task<CursoredList<TwitterList>> GetUserSubscriptions(Account account, string screenName, long count = 20, long cursor = -1)
        {
            return origin.GetUserSubscriptions(account, screenName, count, cursor);
        }

        public Account LoadAccount(JObject data)
        {
            return origin.LoadAccount(data);
        }

        public virtual Task MoveTweetFromCollection(Account account, string id, long[] statusId, long relativeTo, bool above = true)
        {
            return origin.MoveTweetFromCollection(account, id, statusId, relativeTo, above);
        }

        public virtual Task<User> Mute(Account account, long userId)
        {
            return origin.Mute(account, userId);
        }

        public virtual Task<User> Mute(Account account, string screenName)
        {
            return origin.Mute(account, screenName);
        }

        public virtual Task RemoveAllTweetFromCollection(Account account, string id, long[] statusId, long relativeTo, bool above = true)
        {
            return origin.RemoveAllTweetFromCollection(account, id, statusId, relativeTo, above);
        }

        public virtual Task RemoveMembersToUser(Account account, long[] userId, long listId)
        {
            return origin.RemoveMembersToUser(account, userId, listId);
        }

        public virtual Task RemoveMembersToUser(Account account, long[] userId, string slug, long ownerId)
        {
            return origin.RemoveMembersToUser(account, userId, slug, ownerId);
        }

        public virtual Task RemoveMembersToUser(Account account, long[] userId, string slug, string ownerScreenName)
        {
            return origin.RemoveMembersToUser(account, userId, slug, ownerScreenName);
        }

        public virtual Task RemoveMembersToUser(Account account, string[] screenName, long listId)
        {
            return origin.RemoveMembersToUser(account, screenName, listId);
        }

        public virtual Task RemoveMembersToUser(Account account, string[] screenName, string slug, long ownerId)
        {
            return origin.RemoveMembersToUser(account, screenName, slug, ownerId);
        }

        public virtual Task RemoveMembersToUser(Account account, string[] screenName, string slug, string ownerScreenName)
        {
            return origin.RemoveMembersToUser(account, screenName, slug, ownerScreenName);
        }

        public virtual Task RemoveMemberToUser(Account account, long userId, long listId)
        {
            return origin.RemoveMemberToUser(account, userId, listId);
        }

        public virtual Task RemoveMemberToUser(Account account, long userId, string slug, long ownerId)
        {
            return origin.RemoveMemberToUser(account, userId, slug, ownerId);
        }

        public virtual Task RemoveMemberToUser(Account account, long userId, string slug, string ownerScreenName)
        {
            return origin.RemoveMemberToUser(account, userId, slug, ownerScreenName);
        }

        public virtual Task RemoveMemberToUser(Account account, string screenName, long listId)
        {
            return origin.RemoveMemberToUser(account, screenName, listId);
        }

        public virtual Task RemoveMemberToUser(Account account, string screenName, string slug, long ownerId)
        {
            return origin.RemoveMemberToUser(account, screenName, slug, ownerId);
        }

        public virtual Task RemoveMemberToUser(Account account, string screenName, string slug, string ownerScreenName)
        {
            return origin.RemoveMemberToUser(account, screenName, slug, ownerScreenName);
        }

        public virtual Task RemoveProfileBanner(Account account)
        {
            return origin.RemoveProfileBanner(account);
        }

        public virtual Task RemoveTweetFromCollection(Account account, string id, long statusId)
        {
            return origin.RemoveTweetFromCollection(account, id, statusId);
        }

        public virtual Task<User> ReportSpam(Account account, long userId, bool performBlock = true)
        {
            return origin.ReportSpam(account, userId, performBlock);
        }

        public virtual Task<User> ReportSpam(Account account, string screenName, bool performBlock = true)
        {
            return origin.ReportSpam(account, screenName, performBlock);
        }

        public virtual Task<Status> RetweetStatus(Account account, long id)
        {
            return origin.RetweetStatus(account, id);
        }

        public virtual Task<List<Status>> SearchTweet(Account account, string query, bool isRecent, int count = 100, string until = null, long sinceId = -1, long maxId = -1)
        {
            return origin.SearchTweet(account, query, isRecent, count, until, sinceId, maxId);
        }

        public virtual Task<List<User>> SearchUsers(Account account, string query, long page, long count = 20)
        {
            return origin.SearchUsers(account, query, page, count);
        }

        public virtual Task SubscribeAccountToList(Account account, long listId)
        {
            return origin.SubscribeAccountToList(account, listId);
        }

        public virtual Task SubscribeAccountToList(Account account, string slug, long ownerId)
        {
            return origin.SubscribeAccountToList(account, slug, ownerId);
        }

        public virtual Task SubscribeAccountToList(Account account, string slug, string ownerScreenName)
        {
            return origin.SubscribeAccountToList(account, slug, ownerScreenName);
        }

        public virtual Task<User> Unblock(Account account, long userId)
        {
            return origin.Unblock(account, userId);
        }

        public virtual Task<User> Unblock(Account account, string screenName)
        {
            return origin.Unblock(account, screenName);
        }

        public virtual Task<User> Unmute(Account account, long userId)
        {
            return origin.Unmute(account, userId);
        }

        public virtual Task<User> Unmute(Account account, string screenName)
        {
            return origin.Unmute(account, screenName);
        }

        public virtual Task<Status> UnretweetStatus(Account account, long id)
        {
            return origin.UnretweetStatus(account, id);
        }

        public virtual Task UnsubscribeAccountToList(Account account, long listId)
        {
            return origin.UnsubscribeAccountToList(account, listId);
        }

        public virtual Task UnsubscribeAccountToList(Account account, string slug, long ownerId)
        {
            return origin.UnsubscribeAccountToList(account, slug, ownerId);
        }

        public virtual Task UnsubscribeAccountToList(Account account, string slug, string ownerScreenName)
        {
            return origin.UnsubscribeAccountToList(account, slug, ownerScreenName);
        }

        public virtual Task<Collection> UpdateCollection(Account account, string id, string name, string description, string url)
        {
            return origin.UpdateCollection(account, id, name, description, url);
        }

        public virtual Task<User> UpdateFriendship(Account account, long userId, bool? enableDeviceNotifications = null, bool? enableRetweet = null)
        {
            return origin.UpdateFriendship(account, userId, enableDeviceNotifications, enableRetweet);
        }

        public virtual Task<User> UpdateFriendship(Account account, string screenName, bool? enableDeviceNotifications = null, bool? enableRetweet = null)
        {
            return origin.UpdateFriendship(account, screenName, enableDeviceNotifications, enableRetweet);
        }

        public virtual Task<TwitterList> UpdateList(Account account, long listId, string name, string mode = "public", string description = "")
        {
            return origin.UpdateList(account, listId, name, mode, description);
        }

        public virtual Task<TwitterList> UpdateList(Account account, string slug, long ownerId, string name, string mode = "public", string description = "")
        {
            return origin.UpdateList(account, slug, ownerId, name, mode, description);
        }

        public virtual Task<TwitterList> UpdateList(Account account, string slug, string ownerScreenName, string name, string mode = "public", string description = "")
        {
            return origin.UpdateList(account, slug, ownerScreenName, name, mode, description);
        }

        public virtual Task<User> UpdateProfile(Account account, string name, string url, string location, string description, string profileLinkColor)
        {
            return origin.UpdateProfile(account, name, url, location, description, profileLinkColor);
        }

        public virtual Task UpdateProfileBanner(Account account, Stream image)
        {
            return origin.UpdateProfileBanner(account, image);
        }

        public virtual Task<User> UpdateProfileImage(Account account, Stream image)
        {
            return origin.UpdateProfileImage(account, image);
        }

        public virtual Task<long> uploadMedia(Account account, string fileName, Stream image)
        {
            return origin.uploadMedia(account, fileName, image);
        }

        public virtual Task<User> VerifyCredentials(Account account)
        {
            return origin.VerifyCredentials(account);
        }
    }
}
