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
    public class APIWrapper : IAPI
    {
        private IAPI origin;

        public APIWrapper(IAPI origin)
        {
            this.origin = origin;
        }

        public virtual Task AddAllTweetToCollectionAsync(Account account, string id, long[] statusId, long relativeTo, bool above = true)
        {
            return origin.AddAllTweetToCollectionAsync(account, id, statusId, relativeTo, above);
        }

        public virtual Task AddMembersToUserAsync(Account account, long[] userId, long listId)
        {
            return origin.AddMembersToUserAsync(account, userId, listId);
        }

        public virtual Task AddMembersToUserAsync(Account account, long[] userId, string slug, long ownerId)
        {
            return origin.AddMembersToUserAsync(account, userId, slug, ownerId);
        }

        public virtual Task AddMembersToUserAsync(Account account, long[] userId, string slug, string ownerScreenName)
        {
            return origin.AddMembersToUserAsync(account, userId, slug, ownerScreenName);
        }

        public virtual Task AddMembersToUserAsync(Account account, string[] screenName, long listId)
        {
            return origin.AddMembersToUserAsync(account, screenName, listId);
        }

        public virtual Task AddMembersToUserAsync(Account account, string[] screenName, string slug, long ownerId)
        {
            return origin.AddMembersToUserAsync(account, screenName, slug, ownerId);
        }

        public virtual Task AddMembersToUserAsync(Account account, string[] screenName, string slug, string ownerScreenName)
        {
            return origin.AddMembersToUserAsync(account, screenName, slug, ownerScreenName);
        }

        public virtual Task AddMemberToUserAsync(Account account, long userId, long listId)
        {
            return origin.AddMemberToUserAsync(account, userId, listId);
        }

        public virtual Task AddMemberToUserAsync(Account account, long userId, string slug, long ownerId)
        {
            return origin.AddMemberToUserAsync(account, userId, slug, ownerId);
        }

        public virtual Task AddMemberToUserAsync(Account account, long userId, string slug, string ownerScreenName)
        {
            return origin.AddMemberToUserAsync(account, userId, slug, ownerScreenName);
        }

        public virtual Task AddMemberToUserAsync(Account account, string screenName, long listId)
        {
            return origin.AddMemberToUserAsync(account, screenName, listId);
        }

        public virtual Task AddMemberToUserAsync(Account account, string screenName, string slug, long ownerId)
        {
            return origin.AddMemberToUserAsync(account, screenName, slug, ownerId);
        }

        public virtual Task AddMemberToUserAsync(Account account, string screenName, string slug, string ownerScreenName)
        {
            return origin.AddMemberToUserAsync(account, screenName, slug, ownerScreenName);
        }

        public virtual Task AddTweetToCollectionAsync(Account account, string id, long statusId, long relativeTo, bool above = true)
        {
            return origin.AddTweetToCollectionAsync(account, id, statusId, relativeTo, above);
        }

        public virtual Task<User> BlockAsync(Account account, long userId)
        {
            return origin.BlockAsync(account, userId);
        }

        public virtual Task<User> BlockAsync(Account account, string screenName)
        {
            return origin.BlockAsync(account, screenName);
        }

        public virtual Task<Collection> CreateCollectionAsync(Account account, string name, string description, string url, Collection.Order? order)
        {
            return origin.CreateCollectionAsync(account, name, description, url, order);
        }

        public virtual Task<Status> CreateFavoriteAsync(Account account, long id)
        {
            return origin.CreateFavoriteAsync(account, id);
        }

        public virtual Task<User> CreateFriendshipAsync(Account account, long userId)
        {
            return origin.CreateFriendshipAsync(account, userId);
        }

        public virtual Task<User> CreateFriendshipAsync(Account account, string screenName)
        {
            return origin.CreateFriendshipAsync(account, screenName);
        }

        public virtual Task<TwitterList> CreateListAsync(Account account, string name, string mode = "public", string description = "")
        {
            return origin.CreateListAsync(account, name, mode, description);
        }

        public virtual Task<SavedSearch> CreateSavedSearchAsync(Account account, string query)
        {
            return origin.CreateSavedSearchAsync(account, query);
        }

        public virtual Task<Status> CreateStatusAsync(Account account, StatusUpdate update)
        {
            return origin.CreateStatusAsync(account, update);
        }

        public virtual Task DestroyCollectionAsync(Account account, string id)
        {
            return origin.DestroyCollectionAsync(account, id);
        }

        public virtual Task<Status> DestroyFavoriteAsync(Account account, long id)
        {
            return origin.DestroyFavoriteAsync(account, id);
        }

        public virtual Task<User> DestroyFriendshipAsync(Account account, long userId)
        {
            return origin.DestroyFriendshipAsync(account, userId);
        }

        public virtual Task<User> DestroyFriendshipAsync(Account account, string screenName)
        {
            return origin.DestroyFriendshipAsync(account, screenName);
        }

        public virtual Task<TwitterList> DestroyListAsync(Account account, long listId)
        {
            return origin.DestroyListAsync(account, listId);
        }

        public virtual Task<TwitterList> DestroyListAsync(Account account, string slug, long ownerId)
        {
            return origin.DestroyListAsync(account, slug, ownerId);
        }

        public virtual Task<TwitterList> DestroyListAsync(Account account, string slug, string ownerScreenName)
        {
            return origin.DestroyListAsync(account, slug, ownerScreenName);
        }

        public virtual Task<SavedSearch> DestroySavedSearchAsync(Account account, long id)
        {
            return origin.DestroySavedSearchAsync(account, id);
        }

        public virtual Task<Status> DestroyStatusAsync(Account account, long id)
        {
            return origin.DestroyStatusAsync(account, id);
        }

        public virtual Task<FindListResult> FindListAsync(Account account, long userId, long count, string cursor)
        {
            return origin.FindListAsync(account, userId, count, cursor);
        }

        public virtual Task<FindListResult> FindListAsync(Account account, string screenName, long count, string cursor)
        {
            return origin.FindListAsync(account, screenName, count, cursor);
        }

        public virtual Task<FindListResult> FindListByTweetIdAsync(Account account, long tweetId, long count, string cursor)
        {
            return origin.FindListByTweetIdAsync(account, tweetId, count, cursor);
        }

        public virtual Task<Account> GetAccountFromTweetdeckCookieAsync(CookieCollection cookieData)
        {
            return origin.GetAccountFromTweetdeckCookieAsync(cookieData);
        }

        public virtual Task<List<Status>> GetAccountRetweetsAsync(Account account, long count = 200, long sinceId = -1, long maxId = -1)
        {
            return origin.GetAccountRetweetsAsync(account, count, sinceId, maxId);
        }

        public virtual Task<AccountSetting> GetAccountSettingAsync(Account account)
        {
            return origin.GetAccountSettingAsync(account);
        }

        public virtual Task<CursoredList<long>> GetBlockIdsAsync(Account account, long cursor = -1)
        {
            return origin.GetBlockIdsAsync(account, cursor);
        }

        public virtual Task<CursoredList<User>> GetBlockListAsync(Account account, long cursor = -1)
        {
            return origin.GetBlockListAsync(account, cursor);
        }

        public virtual Task<Collection> GetCollectionAsync(Account account, string id)
        {
            return origin.GetCollectionAsync(account, id);
        }

        public virtual Task<List<Account>> GetContributeesAsync(Account account)
        {
            return origin.GetContributeesAsync(account);
        }

        public virtual Task<List<Status>> GetConversationAsync(Account account, long id)
        {
            return origin.GetConversationAsync(account, id);
        }

        public virtual Task<GetEntriesResult> GetEntriesAsync(Account account, string id, long count, long maxPosition = -1, long minPosition = -1)
        {
            return origin.GetEntriesAsync(account, id, count, maxPosition, minPosition);
        }

        public virtual Task<List<Status>> GetFavoritesAsync(Account account, long userId, long count = 200, long sinceId = -1, long maxId = -1)
        {
            return origin.GetFavoritesAsync(account, userId, count, sinceId, maxId);
        }

        public virtual Task<List<Status>> GetFavoritesAsync(Account account, string userScreenName, long count = 200, long sinceId = -1, long maxId = -1)
        {
            return origin.GetFavoritesAsync(account, userScreenName, count, sinceId, maxId);
        }

        public virtual Task<CursoredList<long>> GetFollowerIdsAsync(Account account, long userId, long cursor = -1, long count = 100)
        {
            return origin.GetFollowerIdsAsync(account, userId, cursor, count);
        }

        public virtual Task<CursoredList<long>> GetFollowerIdsAsync(Account account, string screenName, long cursor = -1, long count = 100)
        {
            return origin.GetFollowerIdsAsync(account, screenName, cursor, count);
        }

        public virtual Task<CursoredList<User>> GetFollowersAsync(Account account, long userId, long cursor = -1, long count = 100)
        {
            return origin.GetFollowersAsync(account, userId, cursor, count);
        }

        public virtual Task<CursoredList<User>> GetFollowersAsync(Account account, string screenName, long cursor = -1, long count = 100)
        {
            return origin.GetFollowersAsync(account, screenName, cursor, count);
        }

        public virtual Task<CursoredList<long>> GetFriendIdsAsync(Account account, long userId, long cursor = -1, long count = 100)
        {
            return origin.GetFriendIdsAsync(account, userId, cursor, count);
        }

        public virtual Task<CursoredList<long>> GetFriendIdsAsync(Account account, string screenName, long cursor = -1, long count = 100)
        {
            return origin.GetFriendIdsAsync(account, screenName, cursor, count);
        }

        public virtual Task<CursoredList<User>> GetFriendsAsync(Account account, long userId, long cursor = -1, long count = 100)
        {
            return origin.GetFriendsAsync(account, userId, cursor, count);
        }

        public virtual Task<CursoredList<User>> GetFriendsAsync(Account account, string screenName, long cursor = -1, long count = 100)
        {
            return origin.GetFriendsAsync(account, screenName, cursor, count);
        }

        public virtual Task<List<Friendship>> GetFriendshipAsync(Account account, params long[] userId)
        {
            return origin.GetFriendshipAsync(account, userId);
        }

        public virtual Task<List<Friendship>> GetFriendshipAsync(Account account, params string[] screenName)
        {
            return origin.GetFriendshipAsync(account, screenName);
        }

        public virtual Task<TwitterList> GetListAsync(Account account, long listId)
        {
            return origin.GetListAsync(account, listId);
        }

        public virtual Task<TwitterList> GetListAsync(Account account, string slug, long ownerId)
        {
            return origin.GetListAsync(account, slug, ownerId);
        }

        public virtual Task<TwitterList> GetListAsync(Account account, string slug, string ownerScreenName)
        {
            return origin.GetListAsync(account, slug, ownerScreenName);
        }

        public virtual Task<List<Status>> GetListlineAsync(Account account, long listId, long sinceId, long maxId)
        {
            return origin.GetListlineAsync(account, listId, sinceId, maxId);
        }

        public virtual Task<List<Status>> GetListlineAsync(Account account, string slug, long ownerId, long sinceId, long maxId)
        {
            return origin.GetListlineAsync(account, slug, ownerId, sinceId, maxId);
        }

        public virtual Task<List<Status>> GetListlineAsync(Account account, string slug, string ownerScreenName, long sinceId, long maxId)
        {
            return origin.GetListlineAsync(account, slug, ownerScreenName, sinceId, maxId);
        }

        public virtual Task<List<TwitterList>> GetListsAsync(Account account, string screenName, bool reverse = false)
        {
            return origin.GetListsAsync(account, screenName, reverse);
        }

        public virtual Task<List<TwitterList>> GetListsAsync(Account account, long userId, bool reverse = false)
        {
            return origin.GetListsAsync(account, userId, reverse);
        }

        public virtual Task<ILoginToken> GetLoginTokenAsync(Token consumerToken)
        {
            return origin.GetLoginTokenAsync(consumerToken);
        }

        public virtual Task<List<Status>> GetMedialineAsync(Account account, long userId, long count = 200, long sinceId = -1, long maxId = -1)
        {
            return origin.GetMedialineAsync(account, userId, count, sinceId, maxId);
        }

        public virtual Task<CursoredList<User>> GetMemberOfListAsync(Account account, long listId, long count = 20, long cursor = -1)
        {
            return origin.GetMemberOfListAsync(account, listId, count, cursor);
        }

        public virtual Task<CursoredList<User>> GetMemberOfListAsync(Account account, string slug, long ownerId, long count = 20, long cursor = -1)
        {
            return origin.GetMemberOfListAsync(account, slug, ownerId, count, cursor);
        }

        public virtual Task<CursoredList<User>> GetMemberOfListAsync(Account account, string slug, string ownerScreenName, long count = 20, long cursor = -1)
        {
            return origin.GetMemberOfListAsync(account, slug, ownerScreenName, count, cursor);
        }

        public virtual Task<CursoredList<TwitterList>> GetMembershipsOfUserAsync(Account account, long userId, long count = 20, long cursor = -1, bool filterToOwnedLists = false)
        {
            return origin.GetMembershipsOfUserAsync(account, userId, count, cursor, filterToOwnedLists);
        }

        public virtual Task<CursoredList<TwitterList>> GetMembershipsOfUserAsync(Account account, string screenName, long count = 20, long cursor = -1, bool filterToOwnedLists = false)
        {
            return origin.GetMembershipsOfUserAsync(account, screenName, count, cursor, filterToOwnedLists);
        }

        public virtual Task<List<Status>> GetMentionlineAsync(Account account, long count = 200, long sinceId = -1, long maxId = -1)
        {
            return origin.GetMentionlineAsync(account, count, sinceId, maxId);
        }

        public virtual Task<CursoredList<long>> GetMuteIdsAsync(Account account, long cursor = -1)
        {
            return origin.GetMuteIdsAsync(account, cursor);
        }

        public virtual Task<CursoredList<User>> GetMuteUsersAsync(Account account, long cursor = -1)
        {
            return origin.GetMuteUsersAsync(account, cursor);
        }

        public virtual Task<List<long>> GetNoRetweetListOfAccountAsync(Account account)
        {
            return origin.GetNoRetweetListOfAccountAsync(account);
        }

        public virtual Task<List<Notification>> GetNotificationsAsync(Account account, int count = 40, long sinceId = -1, long maxId = -1)
        {
            return origin.GetNotificationsAsync(account, count, sinceId, maxId);
        }

        public virtual Task<CursoredList<TwitterList>> GetOwnershipsOfUserAsync(Account account, long userId, long count = 20, long cursor = -1)
        {
            return origin.GetOwnershipsOfUserAsync(account, userId, count, cursor);
        }

        public virtual Task<CursoredList<TwitterList>> GetOwnershipsOfUserAsync(Account account, string screenName, long count = 20, long cursor = -1)
        {
            return origin.GetOwnershipsOfUserAsync(account, screenName, count, cursor);
        }

        public virtual Task<CursoredList<long>> GetPendingRequestFromAccountAsync(Account account, long cursor = -1)
        {
            return origin.GetPendingRequestFromAccountAsync(account, cursor);
        }

        public virtual Task<CursoredList<long>> GetPendingRequestToAccountAsync(Account account, long cursor = -1)
        {
            return origin.GetPendingRequestToAccountAsync(account, cursor);
        }

        public virtual Task<Relationship> GetRelationshipAsync(Account account, long sourceId, long targetId)
        {
            return origin.GetRelationshipAsync(account, sourceId, targetId);
        }

        public virtual Task<Relationship> GetRelationshipAsync(Account account, string sourceScreenName, string targetScreenName)
        {
            return origin.GetRelationshipAsync(account, sourceScreenName, targetScreenName);
        }

        public virtual Task<List<Status>> GetRetweetedStatusAsync(Account account, long id, long count = 100)
        {
            return origin.GetRetweetedStatusAsync(account, id, count);
        }

        public virtual Task<CursoredList<long>> GetRetweeterIdsAsync(Account account, long id, long count = 100, long cursor = -1)
        {
            return origin.GetRetweeterIdsAsync(account, id, count, cursor);
        }

        public virtual Task<SavedSearch> GetSavedSearchByIdAsync(Account account, long id)
        {
            return origin.GetSavedSearchByIdAsync(account, id);
        }

        public virtual Task<List<SavedSearch>> GetSavedSearchesAsync(Account account)
        {
            return origin.GetSavedSearchesAsync(account);
        }

        public virtual Task<Status> GetStatusesAsync(Account account, long id)
        {
            return origin.GetStatusesAsync(account, id);
        }

        public virtual Task<List<Status>> GetStatusesAsync(Account account, long[] ids)
        {
            return origin.GetStatusesAsync(account, ids);
        }

        public virtual Task<User> GetSubScriberFromListAsync(Account account, long userId, long listId)
        {
            return origin.GetSubScriberFromListAsync(account, userId, listId);
        }

        public virtual Task<User> GetSubScriberFromListAsync(Account account, long userId, string slug, long ownerId)
        {
            return origin.GetSubScriberFromListAsync(account, userId, slug, ownerId);
        }

        public virtual Task<User> GetSubScriberFromListAsync(Account account, long userId, string slug, string ownerScreenName)
        {
            return origin.GetSubScriberFromListAsync(account, userId, slug, ownerScreenName);
        }

        public virtual Task<User> GetSubScriberFromListAsync(Account account, string screenName, long listId)
        {
            return origin.GetSubScriberFromListAsync(account, screenName, listId);
        }

        public virtual Task<User> GetSubScriberFromListAsync(Account account, string screenName, string slug, long ownerId)
        {
            return origin.GetSubScriberFromListAsync(account, screenName, slug, ownerId);
        }

        public virtual Task<User> GetSubScriberFromListAsync(Account account, string screenName, string slug, string ownerScreenName)
        {
            return origin.GetSubScriberFromListAsync(account, screenName, slug, ownerScreenName);
        }

        public virtual Task<CursoredList<User>> GetSubScribersFromListAsync(Account account, long listId, long count = 20, long cursor = -1)
        {
            return origin.GetSubScribersFromListAsync(account, listId, count, cursor);
        }

        public virtual Task<CursoredList<User>> GetSubScribersFromListAsync(Account account, string slug, long ownerId, long count = 20, long cursor = -1)
        {
            return origin.GetSubScribersFromListAsync(account, slug, ownerId, count, cursor);
        }

        public virtual Task<CursoredList<User>> GetSubScribersFromListAsync(Account account, string slug, string ownerScreenName, long count = 20, long cursor = -1)
        {
            return origin.GetSubScribersFromListAsync(account, slug, ownerScreenName, count, cursor);
        }

        public virtual Task<List<Status>> GetTimelineAsync(Account account, long count = 200, long sinceId = -1, long maxId = -1)
        {
            return origin.GetTimelineAsync(account, count, sinceId, maxId);
        }

        public virtual Task<User> GetUserAsync(Account account, long userIds)
        {
            return origin.GetUserAsync(account, userIds);
        }

        public virtual Task<User> GetUserAsync(Account account, string userScreenNames)
        {
            return origin.GetUserAsync(account, userScreenNames);
        }

        public virtual Task<User> GetUserFromListAsync(Account account, long userId, long listId)
        {
            return origin.GetUserFromListAsync(account, userId, listId);
        }

        public virtual Task<User> GetUserFromListAsync(Account account, long userId, string slug, long ownerId)
        {
            return origin.GetUserFromListAsync(account, userId, slug, ownerId);
        }

        public virtual Task<User> GetUserFromListAsync(Account account, long userId, string slug, string ownerScreenName)
        {
            return origin.GetUserFromListAsync(account, userId, slug, ownerScreenName);
        }

        public virtual Task<User> GetUserFromListAsync(Account account, string screenName, long listId)
        {
            return origin.GetUserFromListAsync(account, screenName, listId);
        }

        public virtual Task<User> GetUserFromListAsync(Account account, string screenName, string slug, long ownerId)
        {
            return origin.GetUserFromListAsync(account, screenName, slug, ownerId);
        }

        public virtual Task<User> GetUserFromListAsync(Account account, string screenName, string slug, string ownerScreenName)
        {
            return origin.GetUserFromListAsync(account, screenName, slug, ownerScreenName);
        }

        public virtual Task<List<Status>> GetUserlineAsync(Account account, long userId, long count = 200, long sinceId = -1, long maxId = -1)
        {
            return origin.GetUserlineAsync(account, userId, count, sinceId, maxId);
        }

        public virtual Task<List<User>> GetUsersAsync(Account account, long[] userIds)
        {
            return origin.GetUsersAsync(account, userIds);
        }

        public virtual Task<List<User>> GetUsersAsync(Account account, string[] userScreenNames)
        {
            return origin.GetUsersAsync(account, userScreenNames);
        }

        public virtual Task<CursoredList<TwitterList>> GetUserSubscriptionsAsync(Account account, long userId, long count = 20, long cursor = -1)
        {
            return origin.GetUserSubscriptionsAsync(account, userId, count, cursor);
        }

        public virtual Task<CursoredList<TwitterList>> GetUserSubscriptionsAsync(Account account, string screenName, long count = 20, long cursor = -1)
        {
            return origin.GetUserSubscriptionsAsync(account, screenName, count, cursor);
        }

        public Account LoadAccount(JObject data)
        {
            return origin.LoadAccount(data);
        }

        public virtual Task MoveTweetFromCollectionAsync(Account account, string id, long[] statusId, long relativeTo, bool above = true)
        {
            return origin.MoveTweetFromCollectionAsync(account, id, statusId, relativeTo, above);
        }

        public virtual Task<User> MuteAsync(Account account, long userId)
        {
            return origin.MuteAsync(account, userId);
        }

        public virtual Task<User> MuteAsync(Account account, string screenName)
        {
            return origin.MuteAsync(account, screenName);
        }

        public virtual Task RemoveAllTweetFromCollectionAsync(Account account, string id, long[] statusId, long relativeTo, bool above = true)
        {
            return origin.RemoveAllTweetFromCollectionAsync(account, id, statusId, relativeTo, above);
        }

        public virtual Task RemoveMembersToUserAsync(Account account, long[] userId, long listId)
        {
            return origin.RemoveMembersToUserAsync(account, userId, listId);
        }

        public virtual Task RemoveMembersToUserAsync(Account account, long[] userId, string slug, long ownerId)
        {
            return origin.RemoveMembersToUserAsync(account, userId, slug, ownerId);
        }

        public virtual Task RemoveMembersToUserAsync(Account account, long[] userId, string slug, string ownerScreenName)
        {
            return origin.RemoveMembersToUserAsync(account, userId, slug, ownerScreenName);
        }

        public virtual Task RemoveMembersToUserAsync(Account account, string[] screenName, long listId)
        {
            return origin.RemoveMembersToUserAsync(account, screenName, listId);
        }

        public virtual Task RemoveMembersToUserAsync(Account account, string[] screenName, string slug, long ownerId)
        {
            return origin.RemoveMembersToUserAsync(account, screenName, slug, ownerId);
        }

        public virtual Task RemoveMembersToUserAsync(Account account, string[] screenName, string slug, string ownerScreenName)
        {
            return origin.RemoveMembersToUserAsync(account, screenName, slug, ownerScreenName);
        }

        public virtual Task RemoveMemberToUserAsync(Account account, long userId, long listId)
        {
            return origin.RemoveMemberToUserAsync(account, userId, listId);
        }

        public virtual Task RemoveMemberToUserAsync(Account account, long userId, string slug, long ownerId)
        {
            return origin.RemoveMemberToUserAsync(account, userId, slug, ownerId);
        }

        public virtual Task RemoveMemberToUserAsync(Account account, long userId, string slug, string ownerScreenName)
        {
            return origin.RemoveMemberToUserAsync(account, userId, slug, ownerScreenName);
        }

        public virtual Task RemoveMemberToUserAsync(Account account, string screenName, long listId)
        {
            return origin.RemoveMemberToUserAsync(account, screenName, listId);
        }

        public virtual Task RemoveMemberToUserAsync(Account account, string screenName, string slug, long ownerId)
        {
            return origin.RemoveMemberToUserAsync(account, screenName, slug, ownerId);
        }

        public virtual Task RemoveMemberToUserAsync(Account account, string screenName, string slug, string ownerScreenName)
        {
            return origin.RemoveMemberToUserAsync(account, screenName, slug, ownerScreenName);
        }

        public virtual Task RemoveProfileBannerAsync(Account account)
        {
            return origin.RemoveProfileBannerAsync(account);
        }

        public virtual Task RemoveTweetFromCollectionAsync(Account account, string id, long statusId)
        {
            return origin.RemoveTweetFromCollectionAsync(account, id, statusId);
        }

        public virtual Task<User> ReportSpamAsync(Account account, long userId, bool performBlock = true)
        {
            return origin.ReportSpamAsync(account, userId, performBlock);
        }

        public virtual Task<User> ReportSpamAsync(Account account, string screenName, bool performBlock = true)
        {
            return origin.ReportSpamAsync(account, screenName, performBlock);
        }

        public virtual Task<Status> RetweetStatusAsync(Account account, long id)
        {
            return origin.RetweetStatusAsync(account, id);
        }

        public virtual Task<List<Status>> SearchTweetAsync(Account account, string query, bool isRecent, int count = 100, string until = null, long sinceId = -1, long maxId = -1)
        {
            return origin.SearchTweetAsync(account, query, isRecent, count, until, sinceId, maxId);
        }

        public virtual Task<List<User>> SearchUsersAsync(Account account, string query, long page, long count = 20)
        {
            return origin.SearchUsersAsync(account, query, page, count);
        }

        public virtual Task SubscribeAccountToListAsync(Account account, long listId)
        {
            return origin.SubscribeAccountToListAsync(account, listId);
        }

        public virtual Task SubscribeAccountToListAsync(Account account, string slug, long ownerId)
        {
            return origin.SubscribeAccountToListAsync(account, slug, ownerId);
        }

        public virtual Task SubscribeAccountToListAsync(Account account, string slug, string ownerScreenName)
        {
            return origin.SubscribeAccountToListAsync(account, slug, ownerScreenName);
        }

        public virtual Task<User> UnblockAsync(Account account, long userId)
        {
            return origin.UnblockAsync(account, userId);
        }

        public virtual Task<User> UnblockAsync(Account account, string screenName)
        {
            return origin.UnblockAsync(account, screenName);
        }

        public virtual Task<User> UnmuteAsync(Account account, long userId)
        {
            return origin.UnmuteAsync(account, userId);
        }

        public virtual Task<User> UnmuteAsync(Account account, string screenName)
        {
            return origin.UnmuteAsync(account, screenName);
        }

        public virtual Task<Status> UnretweetStatusAsync(Account account, long id)
        {
            return origin.UnretweetStatusAsync(account, id);
        }

        public virtual Task UnsubscribeAccountToListAsync(Account account, long listId)
        {
            return origin.UnsubscribeAccountToListAsync(account, listId);
        }

        public virtual Task UnsubscribeAccountToListAsync(Account account, string slug, long ownerId)
        {
            return origin.UnsubscribeAccountToListAsync(account, slug, ownerId);
        }

        public virtual Task UnsubscribeAccountToListAsync(Account account, string slug, string ownerScreenName)
        {
            return origin.UnsubscribeAccountToListAsync(account, slug, ownerScreenName);
        }

        public virtual Task<Collection> UpdateCollectionAsync(Account account, string id, string name, string description, string url)
        {
            return origin.UpdateCollectionAsync(account, id, name, description, url);
        }

        public virtual Task<User> UpdateFriendshipAsync(Account account, long userId, bool? enableDeviceNotifications = null, bool? enableRetweet = null)
        {
            return origin.UpdateFriendshipAsync(account, userId, enableDeviceNotifications, enableRetweet);
        }

        public virtual Task<User> UpdateFriendshipAsync(Account account, string screenName, bool? enableDeviceNotifications = null, bool? enableRetweet = null)
        {
            return origin.UpdateFriendshipAsync(account, screenName, enableDeviceNotifications, enableRetweet);
        }

        public virtual Task<TwitterList> UpdateListAsync(Account account, long listId, string name, string mode = "public", string description = "")
        {
            return origin.UpdateListAsync(account, listId, name, mode, description);
        }

        public virtual Task<TwitterList> UpdateListAsync(Account account, string slug, long ownerId, string name, string mode = "public", string description = "")
        {
            return origin.UpdateListAsync(account, slug, ownerId, name, mode, description);
        }

        public virtual Task<TwitterList> UpdateListAsync(Account account, string slug, string ownerScreenName, string name, string mode = "public", string description = "")
        {
            return origin.UpdateListAsync(account, slug, ownerScreenName, name, mode, description);
        }

        public virtual Task<User> UpdateProfileAsync(Account account, string name, string url, string location, string description, string profileLinkColor)
        {
            return origin.UpdateProfileAsync(account, name, url, location, description, profileLinkColor);
        }

        public virtual Task UpdateProfileBannerAsync(Account account, Stream image)
        {
            return origin.UpdateProfileBannerAsync(account, image);
        }

        public virtual Task<User> UpdateProfileImageAsync(Account account, Stream image)
        {
            return origin.UpdateProfileImageAsync(account, image);
        }

        public virtual Task<long> UploadMediaAsync(Account account, string fileName, Stream image)
        {
            return origin.UploadMediaAsync(account, fileName, image);
        }

        public virtual Task<User> VerifyCredentialsAsync(Account account)
        {
            return origin.VerifyCredentialsAsync(account);
        }
    }
}
