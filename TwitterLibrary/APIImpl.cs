using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TwitterInterface.API;
using TwitterInterface.Container;
using TwitterInterface.Data;
using TwitterLibrary.Data;
using TwitterLibrary.Container;
using Newtonsoft.Json.Linq;
using TwitterInterface.API.Result;
using System.Net.Http.Headers;

namespace TwitterLibrary
{
    public class APIImpl : API
    {
        private static KeyValuePair<string, string>[] makeQuery(params string[] query)
        {
            var list = new List<KeyValuePair<string, string>>();
            for (int i = 0; i < query.Length; i += 2)
            {
                if (query[i + 1] != null)
                {
                    list.Add(new KeyValuePair<string, string>(query[i], query[i + 1]));
                }
            }
            list.Add(new KeyValuePair<string, string>("tweet_mode", "extended"));
            return list.ToArray();
        }

        private static CursoredList<T> makeCursoredList<T>(JObject array)
        {
            return new CursoredList<T>(array["previous_cursor"].ToObject<long>(), array["next_cursor"].ToObject<long>());
        }

        private static string streamToBase64(Stream stream)
        {
            var reader = new BinaryReader(stream);
            var data = reader.ReadBytes((int)stream.Length);
            reader.Close();

            var base64 = Convert.ToBase64String(data);
            data = null;

            return base64;
        }

        internal async Task<string> Get(string uri, Account account, KeyValuePair<string, string>[] query)
        {
            return await Utils.readStringFromTwitter(httpClient, HttpMethod.Get, new Uri(uri), query, account as LibAccount);
        }

        internal async Task<string> Get(string uri, Token consumer, Token? oauth, KeyValuePair<string, string>[] query)
        {
            return await Utils.readStringFromTwitter(httpClient, HttpMethod.Get, new Uri(uri), query, consumer, oauth);
        }

        internal async Task<string> Post(string uri, Account account, KeyValuePair<string, string>[] query)
        {
            return await Utils.readStringFromTwitter(httpClient, HttpMethod.Post, new Uri(uri), query, account as LibAccount);
        }

        internal async Task<string> Post(string uri, Token consumer, Token? oauth, KeyValuePair<string, string>[] query)
        {
            return await Utils.readStringFromTwitter(httpClient, HttpMethod.Post, new Uri(uri), query, consumer, oauth);
        }

        internal readonly HttpClient httpClient = new HttpClient();

        public async Task<SavedSearch> CreateSavedSearch(Account account, string query)
        {
            var response = await Post("https://api.twitter.com/1.1/saved_searches/create.json", account,
                makeQuery("query", query));

            return TwitterDataFactory.parseSavedSearch(JObject.Parse(response));
        }

        public async Task<SavedSearch> DestroySavedSearch(Account account, long id)
        {
            var response = await Post("https://api.twitter.com/1.1/saved_searches/destroy/" + id + ".json", account, new KeyValuePair<string, string>[]
            {

            });

            return TwitterDataFactory.parseSavedSearch(JObject.Parse(response));
        }

        public async Task<AccountSetting> GetAccountSetting(Account account)
        {
            var response = await Get("https://api.twitter.com/1.1/account/settings.json", account, new KeyValuePair<string, string>[]
            {

            });

            var obj = JObject.Parse(response);
            var result = new AccountSetting();

            result.isAlwaysUseHttps = obj["always_use_https"].ToObject<bool>();
            result.isDiscoverableByEmail = obj["discoverable_by_email"].ToObject<bool>();
            result.isGeoEnabled = obj["geo_enabled"].ToObject<bool>();
            result.language = obj["language"].ToString();
            result.isProtected = obj["protected"].ToObject<bool>();
            result.screenName = obj["screen_name"].ToString();
            result.showAllInlineMedia = obj["show_all_inline_media"].ToObject<bool>();
            result.sleepTime = new AccountSetting.SleepTime();
            var sleepTime = obj["sleep_time"].ToObject<JObject>();
            result.sleepTime.isEnabled = sleepTime["enabled"].ToObject<bool>();
            //TODO: startTime / endTime
            //TODO: timeZone
            result.trendLocation = new AccountSetting.TrendLocation();
            var trendLocation = obj["trend_location"].ToObject<JObject>();
            result.trendLocation.country = trendLocation["country"].ToString();
            result.trendLocation.countryCode = trendLocation["countryCode"].ToString();
            result.trendLocation.name = trendLocation["name"].ToString();
            result.trendLocation.parentId = trendLocation["parentid"].ToObject<long>();
            result.trendLocation.placeTypeCode = trendLocation["placeType"]["code"].ToObject<long>();
            result.trendLocation.placeTypeName = trendLocation["placeType"]["name"].ToString();
            result.trendLocation.url = trendLocation["url"].ToString();
            result.trendLocation.woeid = trendLocation["woeid"].ToObject<long>();

            result.useCookiePersonalization = obj["use_cookie_personalization"].ToObject<bool>();
            result.allowContributorRequest = obj["allow_contributor_request"].ToString();

            return result;
        }

        public async Task<LoginToken> GetLoginTokenAsync(Token consumerToken)
        {
            var response = await Post("https://api.twitter.com/oauth/request_token", consumerToken, null, new KeyValuePair<string, string>[] {

            });
            var data = HttpUtility.ParseQueryString(response);

            var token = new Token();
            token.key = data["oauth_token"];
            token.secret = data["oauth_token_secret"];

            return new LoginTokenImpl(this, consumerToken, token);
        }

        public async Task<SavedSearch> GetSavedSearchById(Account account, long id)
        {
            return TwitterDataFactory.parseSavedSearch(
                JObject.Parse(
                    await Get("https://api.twitter.com/1.1/saved_searches/show/" + id + ".json", account, new KeyValuePair<string, string>[]
                    {

                    })
                ));
        }

        public async Task<List<SavedSearch>> GetSavedSearches(Account account)
        {
            return TwitterDataFactory.parseArray(
                JArray.Parse(
                    await Get("https://api.twitter.com/1.1/saved_searches/list.json", account, new KeyValuePair<string, string>[]
                    {

                    })
                ), TwitterDataFactory.parseSavedSearch).ToList();
        }

        public Account LoadAccount(JObject data)
        {
            return LibAccount.Load(data);
        }

        public async Task RemoveProfileBanner(Account account)
        {
            await Post("https://api.twitter.com/1.1/account/remove_profile_banner.json", account, new KeyValuePair<string, string>[]
            {

            });
        }

        public async Task<User> UpdateProfile(Account account, string name, string url, string location, string description, string profileLinkColor)
        {
            return TwitterDataFactory.parseUser(
                JObject.Parse(
                        await Post("https://api.twitter.com/1.1/account/update_profile.json", account,
                        makeQuery("name", name, "url", url, "location", location, "description", description, "profile_link_color", profileLinkColor)
                    )
                ), account.id
                );
        }

        public async Task UpdateProfileBanner(Account account, Stream image)
        {
            await Post("https://api.twitter.com/1.1/account/update_profile_banner.json", account,
                makeQuery("banner", streamToBase64(image)));
        }

        public async Task<User> UpdateProfileImage(Account account, Stream image)
        {
            return TwitterDataFactory.parseUser(
                JObject.Parse(
                        await Post("https://api.twitter.com/1.1/account/update_profile_image.json", account,
                            makeQuery("image", streamToBase64(image)))
                    ), account.id
                );
        }

        public async Task<User> VerifyCredentials(Account account)
        {
            var response = await Get("https://api.twitter.com/1.1/account/verify_credentials.json", account, new KeyValuePair<string, string>[] {

            });

            return TwitterDataFactory.parseUser(JObject.Parse(response), account.id);
        }

        public async Task<GetEntriesResult> GetEntries(Account account, string id, long count, long maxPosition = -1, long minPosition = -1)
        {
            var response = await Get("https://api.twitter.com/1.1/collections/entries.json", account,
                makeQuery("id", id, "count", count.ToString(),
                "max_position", maxPosition != -1 ? maxPosition.ToString() : null,
                "min_position", minPosition != -1 ? minPosition.ToString() : null));

            var json = JObject.Parse(response);

            var result = new GetEntriesResult();

            var position = json["response"]["position"];
            result.minPosition = position["min_position"].ToObject<long>();
            result.maxPosition = position["max_position"].ToObject<long>();

            result.collectionTweets = TwitterDataFactory.parseArray(json["response"]["timeline"].ToObject<JArray>(), TwitterDataFactory.parseCollectionTweet).ToList();
            result.collection = TwitterDataFactory.parseCollection(json["objects"]["timelines"][
                json["response"]["timeline-id"].ToString()
                ].ToObject<JObject>());

            result.tweet = new List<Status>();
            foreach (var cTweet in result.collectionTweets)
            {
                result.tweet.Add(TwitterDataFactory.parseStatus(json["objects"]["tweets"][cTweet.tweetId.ToString()].ToObject<JObject>(), account.id));
            }

            return result;
        }

        private FindListResult FindList(string response)
        {
            var json = JObject.Parse(response);
            var result = new FindListResult();

            result.nextCursor = json["response"]["cursors"]["next_cursor"].ToString();
            result.collections = new List<Collection>();

            var results = json["response"]["result"].ToObject<JArray>();
            foreach (var timeline in results)
            {
                result.collections.Add(TwitterDataFactory.parseCollection(json["objects"]["timelines"][timeline["timeline-id"].ToString()].ToObject<JObject>()));
            }

            return result;
        }

        public async Task<FindListResult> FindList(Account account, long userId, long count, string cursor)
        {
            return FindList(await Get("https://api.twitter.com/1.1/collections/list.json", account,
                makeQuery("user_id", userId.ToString(), "count", count != -1 ? count.ToString() : null, "cursor", cursor)
                ));
        }

        public async Task<FindListResult> FindList(Account account, string screenName, long count, string cursor)
        {
            return FindList(await Get("https://api.twitter.com/1.1/collections/list.json", account,
                makeQuery("screen_name", screenName, "count", count != -1 ? count.ToString() : null, "cursor", cursor)
                ));
        }

        public async Task<FindListResult> FindListByTweetId(Account account, long tweetId, long count, string cursor)
        {
            return FindList(await Get("https://api.twitter.com/1.1/collections/list.json", account,
                makeQuery("tweet_id", tweetId.ToString(), "count", count != -1 ? count.ToString() : null, "cursor", cursor)
                ));
        }

        public async Task<Collection> GetCollection(Account account, string id)
        {
            var json = JObject.Parse(await Get("https://api.twitter.com/1.1/collections/show.json", account,
                makeQuery("id", id)));

            return TwitterDataFactory.parseCollection(json["objects"]["timelines"][json["response"]["timeline_id"].ToString()].ToObject<JObject>());
        }

        public async Task<Collection> CreateCollection(Account account, string name, string description, string url, Collection.Order? order)
        {
            var json = JObject.Parse(await Post("https://api.twitter.com/1.1/collections/create.json", account,
                makeQuery("name", name, "description", description, "url", url, "order", order != null ? Collection.OrderToString(order.Value) : null)));

            return TwitterDataFactory.parseCollection(json["objects"]["timelines"][json["response"]["timeline_id"].ToString()].ToObject<JObject>());
        }

        public async Task<Collection> UpdateCollection(Account account, string id, string name, string description, string url)
        {
            var json = JObject.Parse(await Post("https://api.twitter.com/1.1/collections/update.json", account,
                makeQuery("id", id, "name", name, "description", description, "url", url)));
            //TODO: I think UpdateColllection's result collection data is incomplete?
            return TwitterDataFactory.parseCollection(json["objects"]["timelines"][json["response"]["timeline_id"].ToString()].ToObject<JObject>());
        }

        public async Task DestroyCollection(Account account, string id)
        {
            //TODO: Error handleing
            await Post("https://api.twitter.com/1.1/collections/destroy.json", account, makeQuery("id", id));
        }

        public async Task AddTweetToCollection(Account account, string id, long statusId, long relativeTo, bool above = true)
        {
            //TODO: Error handleing
            await Post("https://api.twitter.com/1.1/collections/entries/add.json", account,
                makeQuery("id", id, "tweet_id", statusId.ToString(), "relative_to", relativeTo != -1 ? relativeTo.ToString() : null,
                "above", above == false ? above.ToString() : null));
        }

        public async Task MoveTweetFromCollection(Account account, string id, long[] statusId, long relativeTo, bool above = true)
        {
            //TODO: Error handleing
            await Post("https://api.twitter.com/1.1/collections/entries/move.json", account,
                makeQuery("id", id, "tweet_id", statusId.ToString(), "relative_to", relativeTo != -1 ? relativeTo.ToString() : null,
                "above", above == false ? above.ToString() : null));
        }

        public async Task RemoveTweetFromCollection(Account account, string id, long statusId)
        {
            //TODO: Error handleing
            await Post("https://api.twitter.com/1.1/collections/entries/remove.json", account,
                makeQuery("id", id, "tweet_id", statusId.ToString()));
        }

        private JObject makeCurateJson(string op, string id, long[] statusId)
        {
            var json = new JObject();
            json["id"] = id;
            var changes = new JArray();

            foreach (var sid in statusId)
            {
                var operationJson = new JObject();
                operationJson["op"] = op;
                operationJson["tweet_id"] = sid;
                changes.Add(operationJson);
            }

            json["changes"] = changes;

            return json;
        }

        private async Task handleCurate(HttpRequestMessage message, JObject operation)
        {
            message.Content = new StringContent(operation.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(message);
        }

        public async Task AddAllTweetToCollection(Account account, string id, long[] statusId, long relativeTo, bool above = true)
        {
            //TODO: Error handleing
            await handleCurate(Utils.generateHttpRequest(HttpMethod.Post, new Uri("https://api.twitter.com/1.1/collections/entries/curate.json"), new KeyValuePair<string, string>[] { }
            , account as LibAccount), makeCurateJson("add", id, statusId));
        }

        public async Task RemoveAllTweetFromCollection(Account account, string id, long[] statusId, long relativeTo, bool above = true)
        {
            //TODO: Error handleing
            await handleCurate(Utils.generateHttpRequest(HttpMethod.Post, new Uri("https://api.twitter.com/1.1/collections/entries/curate.json"), new KeyValuePair<string, string>[] { }
            , account as LibAccount), makeCurateJson("remove", id, statusId));
        }

        public async Task<long> uploadMedia(Account account, string fileName, Stream image)
        {
            var message = Utils.generateHttpRequest(HttpMethod.Post, new Uri("https://upload.twitter.com/1.1/media/upload.json"), new KeyValuePair<string, string>[] { }
            , account as LibAccount);

            byte[] imgdata = new byte[image.Length];
            image.Read(imgdata, 0, (int)image.Length);
            var imageContent = new ByteArrayContent(imgdata);

            imageContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");

            var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(imageContent, "media");

            message.Content = multipartContent;

            var response = await httpClient.SendAsync(message);

            Utils.VerifyTwitterResponse(response);

            var result = JObject.Parse(await response.Content.ReadAsStringAsync());

            return result["media_id"].ToObject<long>();
        }

        public async Task<List<Status>> GetTimeline(Account account, long count = 200, long sinceId = -1, long maxId = -1)
        {
            var response = JArray.Parse(
                await Get("https://api.twitter.com/1.1/statuses/home_timeline.json", account,
                makeQuery("count", count.ToString(), "since_id", sinceId != -1 ? sinceId.ToString() : null, "max_id", maxId != -1 ? maxId.ToString() : null)
                )
                );

            return TwitterDataFactory.parseArray(response, account.id, TwitterDataFactory.parseStatus).ToList();
        }

        public async Task<List<Status>> GetUserline(Account account, long userId, long count = 200, long sinceId = -1, long maxId = -1)
        {
            var response = JArray.Parse(
                await Get("https://api.twitter.com/1.1/statuses/user_timeline.json", account,
                makeQuery("user_id", userId.ToString(), "count", count.ToString(), "since_id", sinceId != -1 ? sinceId.ToString() : null, "max_id", maxId != -1 ? maxId.ToString() : null)
                )
                );

            return TwitterDataFactory.parseArray(response, account.id, TwitterDataFactory.parseStatus).ToList();
        }

        public async Task<List<Status>> GetAccountRetweets(Account account, long count = 200, long sinceId = -1, long maxId = -1)
        {
            var response = JArray.Parse(
                await Get("https://api.twitter.com/1.1/statuses/retweets_of_me.json", account,
                makeQuery("count", count.ToString(), "since_id", sinceId != -1 ? sinceId.ToString() : null, "max_id", maxId != -1 ? maxId.ToString() : null)
                )
                );

            return TwitterDataFactory.parseArray(response, account.id, TwitterDataFactory.parseStatus).ToList();
        }

        public async Task<List<Status>> GetMentionline(Account account, long count = 200, long sinceId = -1, long maxId = -1)
        {
            var response = JArray.Parse(
                await Get("https://api.twitter.com/1.1/statuses/mentions_timeline.json", account,
                makeQuery("count", count.ToString(), "since_id", sinceId != -1 ? sinceId.ToString() : null, "max_id", maxId != -1 ? maxId.ToString() : null)
                )
                );

            return TwitterDataFactory.parseArray(response, account.id, TwitterDataFactory.parseStatus).ToList();
        }

        public async Task<Status> CreateStatus(Account account, StatusUpdate update)
        {
            return TwitterDataFactory.parseStatus(JObject.Parse(
                await Post("https://api.twitter.com/1.1/statuses/update.json", account,
                makeQuery("status", update.text,
                "in_reply_to_status_id", update.inReplyToStatusId != -1 ? update.inReplyToStatusId.ToString() : null,
                "auto_populate_reply_metadata", update.autoPopulateReplyMetadata ? "true" : null,
                "exclude_reply_user_ids", update.excludeReplyUserIds != null ? string.Join(",", update.excludeReplyUserIds) : null,
                "attachment_url", update.attachmentURL,
                "media_ids", update.mediaIDs != null ? string.Join(",", update.mediaIDs) : null,
                "possibly_sensitive", update.possiblySensitive ? "true" : null
                ))), account.id);
        }

        public async Task<Status> DestroyStatus(Account account, long id)
        {
            return TwitterDataFactory.parseStatus(JObject.Parse(
                await Post("https://api.twitter.com/1.1/statuses/destroy/" + id + ".json", account, makeQuery())
                ), account.id);
        }

        public async Task<Status> GetStatuses(Account account, long id)
        {
            return TwitterDataFactory.parseStatus(JObject.Parse(
                await Get("https://api.twitter.com/1.1/statuses/show.json", account,
                makeQuery("id", id.ToString())
                )), account.id);
        }

        public async Task<List<Status>> GetStatuses(Account account, long[] ids)
        {
            return TwitterDataFactory.parseArray(JArray.Parse(
                await Get("https://api.twitter.com/1.1/statuses/lookup.json", account,
                makeQuery("id", string.Join(",", ids))
                )), account.id, TwitterDataFactory.parseStatus).ToList();
        }

        public async Task<Status> RetweetStatus(Account account, long id)
        {
            return TwitterDataFactory.parseStatus(JObject.Parse(
                await Post("https://api.twitter.com/1.1/statuses/retweet/" + id.ToString() + ".json", account, makeQuery())
                ), account.id);
        }

        public async Task<Status> UnretweetStatus(Account account, long id)
        {
            return TwitterDataFactory.parseStatus(JObject.Parse(
                await Post("https://api.twitter.com/1.1/statuses/unretweet/" + id.ToString() + ".json", account, makeQuery())
                ), account.id);
        }

        public async Task<List<Status>> GetRetweetedStatus(Account account, long id, long count = 100)
        {
            return TwitterDataFactory.parseArray(JArray.Parse(
                await Get("https://api.twitter.com/1.1/statuses/retweets/" + id + ".json", account, makeQuery("count", count.ToString()))
                ), account.id, TwitterDataFactory.parseStatus).ToList();
        }

        public async Task<CursoredList<long>> GetRetweeterIds(Account account, long id, long count = 100, long cursor = -1)
        {
            var response = JObject.Parse(await Get("https://api.twitter.com/1.1/statuses/retweets/" + id + ".json", account,
                makeQuery("count", count.ToString(), "cursor", cursor != -1 ? cursor.ToString() : null)));

            var result = makeCursoredList<long>(response);
            foreach (var _id in response["ids"].ToObject<JArray>())
            {
                result.Add(_id.ToObject<long>());
            }

            return result;
        }

        public async Task<Status> CreateFavorite(Account account, long id)
        {
            return TwitterDataFactory.parseStatus(JObject.Parse(
                    await Post("https://api.twitter.com/1.1/favorites/create.json", account, makeQuery("id", id.ToString()))
                ), account.id);
        }

        public async Task<Status> DestroyFavorite(Account account, long id)
        {
            return TwitterDataFactory.parseStatus(JObject.Parse(
                    await Post("https://api.twitter.com/1.1/favorites/destroy.json", account, makeQuery("id", id.ToString()))
                ), account.id);
        }

        public async Task<List<Status>> GetFavorites(Account account, long userId, long count = 200, long sinceId = -1, long maxId = -1)
        {
            var response = JArray.Parse(
                await Get("https://api.twitter.com/1.1/favorites/list.json", account,
                makeQuery("user_id", userId.ToString(),
                "count", count.ToString(),
                "since_id", sinceId != -1 ? sinceId.ToString() : null,
                "max_id", maxId != -1 ? maxId.ToString() : null)
                ));

            return TwitterDataFactory.parseArray(response, account.id, TwitterDataFactory.parseStatus).ToList();
        }

        public async Task<List<Status>> GetFavorites(Account account, string userScreenName, long count = 200, long sinceId = -1, long maxId = -1)
        {
            var response = JArray.Parse(
                await Get("https://api.twitter.com/1.1/favorites/list.json", account,
                makeQuery("screen_name", userScreenName,
                "count", count.ToString(),
                "since_id", sinceId != -1 ? sinceId.ToString() : null,
                "max_id", maxId != -1 ? maxId.ToString() : null)
                ));

            return TwitterDataFactory.parseArray(response, account.id, TwitterDataFactory.parseStatus).ToList();
        }

        public async Task<List<TwitterList>> GetLists(Account account, string screenName, bool reverse = false)
        {
            return TwitterDataFactory.parseArray(JArray.Parse(
                await Get("https://api.twitter.com/1.1/lists/list.json", account,
                makeQuery("screen_name", screenName, "reverse", reverse.ToString()))), account.id,
                TwitterDataFactory.parseTwitterList).ToList();
        }

        public async Task<List<TwitterList>> GetLists(Account account, long userId, bool reverse = false)
        {
            return TwitterDataFactory.parseArray(JArray.Parse(
                await Get("https://api.twitter.com/1.1/lists/list.json", account,
                makeQuery("user_id", userId.ToString(), "reverse", reverse.ToString()))), account.id,
                TwitterDataFactory.parseTwitterList).ToList();
        }

        public async Task<CursoredList<User>> GetMemberOfList(Account account, long listId, long count = 20, long cursor = -1)
        {
            var response = JObject.Parse(await Get("https://api.twitter.com/1.1/lists/members.json", account,
                makeQuery("list_id", listId.ToString(),
                "count", count != 20 ? count.ToString() : null,
                "cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = makeCursoredList<User>(response);

            result.AddRange(TwitterDataFactory.parseArray(response["users"].ToObject<JArray>(), account.id,
                TwitterDataFactory.parseUser));

            return result;
        }

        public async Task<CursoredList<User>> GetMemberOfList(Account account, string slug, long ownerId, long count = 20, long cursor = -1)
        {
            var response = JObject.Parse(await Get("https://api.twitter.com/1.1/lists/members.json", account,
                makeQuery("slug", slug, "owner_id", ownerId.ToString(),
                "count", count != 20 ? count.ToString() : null,
                "cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = makeCursoredList<User>(response);

            result.AddRange(TwitterDataFactory.parseArray(response["users"].ToObject<JArray>(), account.id,
                TwitterDataFactory.parseUser));

            return result;
        }

        public async Task<CursoredList<User>> GetMemberOfList(Account account, string slug, string ownerScreenName, long count = 20, long cursor = -1)
        {
            var response = JObject.Parse(await Get("https://api.twitter.com/1.1/lists/members.json", account,
                makeQuery("slug", slug, "owner_screen_name", ownerScreenName,
                "count", count != 20 ? count.ToString() : null,
                "cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = makeCursoredList<User>(response);

            result.AddRange(TwitterDataFactory.parseArray(response["users"].ToObject<JArray>(), account.id,
                TwitterDataFactory.parseUser));

            return result;
        }

        public async Task<User> GetUserFromList(Account account, long userId, long listId)
        {
            return TwitterDataFactory.parseUser(JObject.Parse(
                await Get("https://api.twitter.com/1.1/lists/members/show.json", account,
                makeQuery("user_id", userId.ToString(), "list_id", listId.ToString())
                )), account.id);
        }

        public async Task<User> GetUserFromList(Account account, long userId, string slug, long ownerId)
        {
            return TwitterDataFactory.parseUser(JObject.Parse(
                await Get("https://api.twitter.com/1.1/lists/members/show.json", account,
                makeQuery("user_id", userId.ToString(), "slug", slug, "owner_id", ownerId.ToString())
                )), account.id);
        }

        public async Task<User> GetUserFromList(Account account, long userId, string slug, string ownerScreenName)
        {
            return TwitterDataFactory.parseUser(JObject.Parse(
                await Get("https://api.twitter.com/1.1/lists/members/show.json", account,
                makeQuery("user_id", userId.ToString(), "slug", slug, "owner_screen_name", ownerScreenName)
                )), account.id);
        }

        public async Task<User> GetUserFromList(Account account, string screenName, long listId)
        {
            return TwitterDataFactory.parseUser(JObject.Parse(
                await Get("https://api.twitter.com/1.1/lists/members/show.json", account,
                makeQuery("screen_name", screenName, "list_id", listId.ToString())
                )), account.id);
        }

        public async Task<User> GetUserFromList(Account account, string screenName, string slug, long ownerId)
        {
            return TwitterDataFactory.parseUser(JObject.Parse(
                await Get("https://api.twitter.com/1.1/lists/members/show.json", account,
                makeQuery("screen_name", screenName, "slug", slug, "owner_id", ownerId.ToString())
                )), account.id);
        }

        public async Task<User> GetUserFromList(Account account, string screenName, string slug, string ownerScreenName)
        {
            return TwitterDataFactory.parseUser(JObject.Parse(
                await Get("https://api.twitter.com/1.1/lists/members/show.json", account,
                makeQuery("screen_name", screenName, "slug", slug, "owner_screen_name", ownerScreenName)
                )), account.id);
        }

        public async Task<CursoredList<TwitterList>> GetMembershipsOfUser(Account account, long userId, long count = 20, long cursor = -1, bool filterToOwnedLists = false)
        {
            var response = JObject.Parse(await Get("https://api.twitter.com/1.1/lists/memberships.json", account,
                makeQuery("user_id", userId.ToString(),
                "count", count != 20 ? count.ToString() : null,
                "cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = makeCursoredList<TwitterList>(response);

            result.AddRange(TwitterDataFactory.parseArray(response["lists"].ToObject<JArray>(), account.id,
                TwitterDataFactory.parseTwitterList));

            return result;
        }

        public async Task<CursoredList<TwitterList>> GetMembershipsOfUser(Account account, string screenName, long count = 20, long cursor = -1, bool filterToOwnedLists = false)
        {
            var response = JObject.Parse(await Get("https://api.twitter.com/1.1/lists/memberships.json", account,
                makeQuery("screen_name", screenName,
                "count", count != 20 ? count.ToString() : null,
                "cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = makeCursoredList<TwitterList>(response);

            result.AddRange(TwitterDataFactory.parseArray(response["lists"].ToObject<JArray>(), account.id,
                TwitterDataFactory.parseTwitterList));

            return result;
        }

        public async Task<CursoredList<TwitterList>> GetOwnershipsOfUser(Account account, long userId, long count = 20, long cursor = -1)
        {
            var response = JObject.Parse(await Get("https://api.twitter.com/1.1/lists/ownerships.json", account,
                makeQuery("user_id", userId.ToString(),
                "count", count != 20 ? count.ToString() : null,
                "cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = makeCursoredList<TwitterList>(response);

            result.AddRange(TwitterDataFactory.parseArray(response["lists"].ToObject<JArray>(), account.id,
                TwitterDataFactory.parseTwitterList));

            return result;
        }

        public async Task<CursoredList<TwitterList>> GetOwnershipsOfUser(Account account, string screenName, long count = 20, long cursor = -1)
        {
            var response = JObject.Parse(await Get("https://api.twitter.com/1.1/lists/ownerships.json", account,
                makeQuery("screen_name", screenName,
                "count", count != 20 ? count.ToString() : null,
                "cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = makeCursoredList<TwitterList>(response);

            result.AddRange(TwitterDataFactory.parseArray(response["lists"].ToObject<JArray>(), account.id,
                TwitterDataFactory.parseTwitterList));

            return result;
        }

        public async Task<TwitterList> GetList(Account account, long listId)
        {
            return TwitterDataFactory.parseTwitterList(JObject.Parse(
                await Get("https://api.twitter.com/1.1/lists/show.json", account,
                makeQuery("list_id", listId.ToString())
                )), account.id);
        }

        public async Task<TwitterList> GetList(Account account, string slug, long ownerId)
        {
            return TwitterDataFactory.parseTwitterList(JObject.Parse(
                await Get("https://api.twitter.com/1.1/lists/show.json", account,
                makeQuery("slug", slug, "owner_id", ownerId.ToString())
                )), account.id);
        }

        public async Task<TwitterList> GetList(Account account, string slug, string ownerScreenName)
        {
            return TwitterDataFactory.parseTwitterList(JObject.Parse(
                await Get("https://api.twitter.com/1.1/lists/show.json", account,
                makeQuery("slug", slug, "owner_screen_name", ownerScreenName)
                )), account.id);
        }

        public async Task<List<Status>> GetListline(Account account, long listId, long sinceId, long maxId)
        {
            return TwitterDataFactory.parseArray(JArray.Parse(
                await Get("https://api.twitter.com/1.1/lists/statuses.json", account,
                makeQuery("list_id", listId.ToString(),
                "since_id", sinceId.ToString(), "max_id", maxId.ToString()
                ))), account.id,
                TwitterDataFactory.parseStatus).ToList();
        }

        public async Task<List<Status>> GetListline(Account account, string slug, long ownerId, long sinceId, long maxId)
        {
            return TwitterDataFactory.parseArray(JArray.Parse(
                await Get("https://api.twitter.com/1.1/lists/statuses.json", account,
                makeQuery("slug", slug, "owner_id", ownerId.ToString(),
                "since_id", sinceId.ToString(), "max_id", maxId.ToString()
                ))), account.id,
                TwitterDataFactory.parseStatus).ToList();
        }

        public async Task<List<Status>> GetListline(Account account, string slug, string ownerScreenName, long sinceId, long maxId)
        {
            return TwitterDataFactory.parseArray(JArray.Parse(
                await Get("https://api.twitter.com/1.1/lists/statuses.json", account,
                makeQuery("slug", slug, "owner_screen_name", ownerScreenName,
                "since_id", sinceId.ToString(), "max_id", maxId.ToString()
                ))), account.id,
                TwitterDataFactory.parseStatus).ToList();
        }

        public async Task<CursoredList<User>> GetSubScribersFromList(Account account, long listId, long count = 20, long cursor = -1)
        {
            var response = JObject.Parse(
                await Get("https://api.twitter.com/1.1/lists/subscribers.json", account,
                makeQuery("list_id", listId.ToString(),
                        "count", count != 20 ? count.ToString() : null,
                        "cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = makeCursoredList<User>(response);

            result.AddRange(TwitterDataFactory.parseArray(response["users"].ToObject<JArray>(), account.id, TwitterDataFactory.parseUser));

            return result;
        }

        public async Task<CursoredList<User>> GetSubScribersFromList(Account account, string slug, long ownerId, long count = 20, long cursor = -1)
        {
            var response = JObject.Parse(
                await Get("https://api.twitter.com/1.1/lists/subscribers.json", account,
                makeQuery("slug", slug, "owner_id", ownerId.ToString(),
                        "count", count != 20 ? count.ToString() : null,
                        "cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = makeCursoredList<User>(response);

            result.AddRange(TwitterDataFactory.parseArray(response["users"].ToObject<JArray>(), account.id, TwitterDataFactory.parseUser));

            return result;
        }

        public async Task<CursoredList<User>> GetSubScribersFromList(Account account, string slug, string ownerScreenName, long count = 20, long cursor = -1)
        {
            var response = JObject.Parse(
                await Get("https://api.twitter.com/1.1/lists/subscribers.json", account,
                makeQuery("slug", slug, "owner_screen_name", ownerScreenName,
                        "count", count != 20 ? count.ToString() : null,
                        "cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = makeCursoredList<User>(response);

            result.AddRange(TwitterDataFactory.parseArray(response["users"].ToObject<JArray>(), account.id, TwitterDataFactory.parseUser));

            return result;
        }

        public async Task<User> GetSubScriberFromList(Account account, long userId, long listId)
        {
            return TwitterDataFactory.parseUser(
                JObject.Parse(
                    await Get("https://api.twitter.com/1.1/lists/subscribers/show.json", account,
                    makeQuery("user_id", userId.ToString(),
                    "list_id", listId.ToString())
                )), account.id);
        }

        public async Task<User> GetSubScriberFromList(Account account, long userId, string slug, long ownerId)
        {
            return TwitterDataFactory.parseUser(
                JObject.Parse(
                    await Get("https://api.twitter.com/1.1/lists/subscribers/show.json", account,
                    makeQuery("user_id", userId.ToString(),
                    "slug", slug, "owner_id", ownerId.ToString())
                )), account.id);
        }

        public async Task<User> GetSubScriberFromList(Account account, long userId, string slug, string ownerScreenName)
        {
            return TwitterDataFactory.parseUser(
                JObject.Parse(
                    await Get("https://api.twitter.com/1.1/lists/subscribers/show.json", account,
                    makeQuery("user_id", userId.ToString(),
                    "slug", slug, "owner_screen_name", ownerScreenName)
                )), account.id);
        }

        public async Task<User> GetSubScriberFromList(Account account, string screenName, long listId)
        {
            return TwitterDataFactory.parseUser(
                JObject.Parse(
                    await Get("https://api.twitter.com/1.1/lists/subscribers/show.json", account,
                    makeQuery("screen_name", screenName,
                    "list_id", listId.ToString())
                )), account.id);
        }

        public async Task<User> GetSubScriberFromList(Account account, string screenName, string slug, long ownerId)
        {
            return TwitterDataFactory.parseUser(
                JObject.Parse(
                    await Get("https://api.twitter.com/1.1/lists/subscribers/show.json", account,
                    makeQuery("screen_name", screenName,
                    "slug", slug, "owner_id", ownerId.ToString())
                )), account.id);
        }

        public async Task<User> GetSubScriberFromList(Account account, string screenName, string slug, string ownerScreenName)
        {
            return TwitterDataFactory.parseUser(
                JObject.Parse(
                    await Get("https://api.twitter.com/1.1/lists/subscribers/show.json", account,
                    makeQuery("screen_name", screenName,
                    "slug", slug, "owner_screen_name", ownerScreenName)
                )), account.id);
        }

        public async Task<CursoredList<TwitterList>> GetUserSubscriptions(Account account, long userId, long count = 20, long cursor = -1)
        {
            var response = JObject.Parse(await Get("https://api.twitter.com/1.1/lists/subscriptions.json", account,
                makeQuery("user_id", userId.ToString(),
                "count", count != 20 ? count.ToString() : null,
                "cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = makeCursoredList<TwitterList>(response);

            result.AddRange(TwitterDataFactory.parseArray(response["lists"].ToObject<JArray>(), account.id,
                TwitterDataFactory.parseTwitterList));

            return result;
        }

        public async Task<CursoredList<TwitterList>> GetUserSubscriptions(Account account, string screenName, long count = 20, long cursor = -1)
        {
            var response = JObject.Parse(await Get("https://api.twitter.com/1.1/lists/subscriptions.json", account,
                makeQuery("screen_name", screenName,
                "count", count != 20 ? count.ToString() : null,
                "cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = makeCursoredList<TwitterList>(response);

            result.AddRange(TwitterDataFactory.parseArray(response["lists"].ToObject<JArray>(), account.id,
                TwitterDataFactory.parseTwitterList));

            return result;
        }

        public async Task<TwitterList> CreateList(Account account, string name, string mode = "public", string description = "")
        {
            return TwitterDataFactory.parseTwitterList(JObject.Parse(
                await Post("https://api.twitter.com/1.1/lists/create.json", account,
                makeQuery("name", name, "mode", mode, "description", description)
                )), account.id);
        }

        public async Task<TwitterList> UpdateList(Account account, long listId, string name, string mode = "public", string description = "")
        {
            return TwitterDataFactory.parseTwitterList(JObject.Parse(
                await Post("https://api.twitter.com/1.1/lists/update.json", account,
                makeQuery("list_id", listId.ToString(),
                    "name", name, "mode", mode, "description", description)
                )), account.id);
        }

        public async Task<TwitterList> UpdateList(Account account, string slug, long ownerId, string name, string mode = "public", string description = "")
        {
            return TwitterDataFactory.parseTwitterList(JObject.Parse(
                await Post("https://api.twitter.com/1.1/lists/update.json", account,
                makeQuery("slug", slug, "owner_id", ownerId.ToString(),
                    "name", name, "mode", mode, "description", description)
                )), account.id);
        }

        public async Task<TwitterList> UpdateList(Account account, string slug, string ownerScreenName, string name, string mode = "public", string description = "")
        {
            return TwitterDataFactory.parseTwitterList(JObject.Parse(
                await Post("https://api.twitter.com/1.1/lists/update.json", account,
                makeQuery("slug", slug, "owner_screen_name", ownerScreenName,
                    "name", name, "mode", mode, "description", description)
                )), account.id);
        }

        public async Task<TwitterList> DestroyList(Account account, long listId)
        {
            return TwitterDataFactory.parseTwitterList(JObject.Parse(
                await Post("https://api.twitter.com/1.1/lists/destroy.json", account,
                makeQuery("list_id", listId.ToString())
                )), account.id);
        }

        public async Task<TwitterList> DestroyList(Account account, string slug, long ownerId)
        {
            return TwitterDataFactory.parseTwitterList(JObject.Parse(
                await Post("https://api.twitter.com/1.1/lists/destroy.json", account,
                makeQuery("slug", slug, "owner_id", ownerId.ToString())
                )), account.id);
        }

        public async Task<TwitterList> DestroyList(Account account, string slug, string ownerScreenName)
        {
            return TwitterDataFactory.parseTwitterList(JObject.Parse(
                await Post("https://api.twitter.com/1.1/lists/destroy.json", account,
                makeQuery("slug", slug, "owner_screen_name", ownerScreenName)
                )), account.id);
        }

        public async Task AddMemberToUser(Account account, long userId, long listId)
        {
            await Post("https://api.twitter.com/1.1/lists/members/create.json", account,
                makeQuery("user_id", userId.ToString(),
                "list_id", listId.ToString()));
        }

        public async Task AddMemberToUser(Account account, long userId, string slug, long ownerId)
        {
            await Post("https://api.twitter.com/1.1/lists/members/create.json", account,
                makeQuery("user_id", userId.ToString(),
                "slug", slug, "owner_id", ownerId.ToString()));
        }

        public async Task AddMemberToUser(Account account, long userId, string slug, string ownerScreenName)
        {
            await Post("https://api.twitter.com/1.1/lists/members/create.json", account,
                makeQuery("user_id", userId.ToString(),
                "slug", slug, "owner_screen_name", ownerScreenName));
        }

        public async Task AddMemberToUser(Account account, string screenName, long listId)
        {
            await Post("https://api.twitter.com/1.1/lists/members/create.json", account,
                makeQuery("screen_name", screenName,
                "list_id", listId.ToString()));
        }

        public async Task AddMemberToUser(Account account, string screenName, string slug, long ownerId)
        {
            await Post("https://api.twitter.com/1.1/lists/members/create.json", account,
                makeQuery("screen_name", screenName,
                "slug", slug, "owner_id", ownerId.ToString()));
        }

        public async Task AddMemberToUser(Account account, string screenName, string slug, string ownerScreenName)
        {
            await Post("https://api.twitter.com/1.1/lists/members/create.json", account,
               makeQuery("screen_name", screenName,
               "slug", slug, "owner_screen_name", ownerScreenName));
        }

        public async Task AddMembersToUser(Account account, long[] userId, long listId)
        {
            await Post("https://api.twitter.com/1.1/lists/members/create_all.json", account,
                makeQuery("user_id", string.Join(",", userId),
                "list_id", listId.ToString()));
        }

        public async Task AddMembersToUser(Account account, long[] userId, string slug, long ownerId)
        {
            await Post("https://api.twitter.com/1.1/lists/members/create_all.json", account,
                makeQuery("user_id", string.Join(",", userId),
                "slug", slug, "owner_id", ownerId.ToString()));
        }

        public async Task AddMembersToUser(Account account, long[] userId, string slug, string ownerScreenName)
        {
            await Post("https://api.twitter.com/1.1/lists/members/create_all.json", account,
                makeQuery("user_id", string.Join(",", userId),
                "slug", slug, "owner_screen_name", ownerScreenName));
        }

        public async Task AddMembersToUser(Account account, string[] screenName, long listId)
        {
            await Post("https://api.twitter.com/1.1/lists/members/create_all.json", account,
                makeQuery("screen_name", string.Join(",", screenName),
                "list_id", listId.ToString()));
        }

        public async Task AddMembersToUser(Account account, string[] screenName, string slug, long ownerId)
        {
            await Post("https://api.twitter.com/1.1/lists/members/create_all.json", account,
                makeQuery("screen_name", string.Join(",", screenName),
                "slug", slug, "owner_id", ownerId.ToString()));
        }

        public async Task AddMembersToUser(Account account, string[] screenName, string slug, string ownerScreenName)
        {
            await Post("https://api.twitter.com/1.1/lists/members/create_all.json", account,
                makeQuery("screen_name", string.Join(",", screenName),
                "slug", slug, "owner_screen_name", ownerScreenName));
        }

        public async Task RemoveMemberToUser(Account account, long userId, long listId)
        {
            await Post("https://api.twitter.com/1.1/lists/members/destroy.json", account,
                makeQuery("user_id", userId.ToString(),
                "list_id", listId.ToString()));
        }

        public async Task RemoveMemberToUser(Account account, long userId, string slug, long ownerId)
        {
            await Post("https://api.twitter.com/1.1/lists/members/destroy.json", account,
                makeQuery("user_id", userId.ToString(),
                "slug", slug, "owner_id", ownerId.ToString()));
        }

        public async Task RemoveMemberToUser(Account account, long userId, string slug, string ownerScreenName)
        {
            await Post("https://api.twitter.com/1.1/lists/members/destroy.json", account,
                makeQuery("user_id", userId.ToString(),
                "slug", slug, "owner_screen_name", ownerScreenName));
        }

        public async Task RemoveMemberToUser(Account account, string screenName, long listId)
        {
            await Post("https://api.twitter.com/1.1/lists/members/destroy.json", account,
                makeQuery("screen_name", screenName,
                "list_id", listId.ToString()));
        }

        public async Task RemoveMemberToUser(Account account, string screenName, string slug, long ownerId)
        {
            await Post("https://api.twitter.com/1.1/lists/members/destroy.json", account,
                makeQuery("screen_name", screenName,
                "slug", slug, "owner_id", ownerId.ToString()));
        }

        public async Task RemoveMemberToUser(Account account, string screenName, string slug, string ownerScreenName)
        {
            await Post("https://api.twitter.com/1.1/lists/members/destroy.json", account,
               makeQuery("screen_name", screenName,
               "slug", slug, "owner_screen_name", ownerScreenName));
        }

        public async Task RemoveMembersToUser(Account account, long[] userId, long listId)
        {
            await Post("https://api.twitter.com/1.1/lists/members/create_all.json", account,
                makeQuery("user_id", string.Join(",", userId),
                "list_id", listId.ToString()));
        }

        public async Task RemoveMembersToUser(Account account, long[] userId, string slug, long ownerId)
        {
            await Post("https://api.twitter.com/1.1/lists/members/create_all.json", account,
                makeQuery("user_id", string.Join(",", userId),
                "slug", slug, "owner_id", ownerId.ToString()));
        }

        public async Task RemoveMembersToUser(Account account, long[] userId, string slug, string ownerScreenName)
        {
            await Post("https://api.twitter.com/1.1/lists/members/create_all.json", account,
                makeQuery("user_id", string.Join(",", userId),
                "slug", slug, "owner_screen_name", ownerScreenName));
        }

        public async Task RemoveMembersToUser(Account account, string[] screenName, long listId)
        {
            await Post("https://api.twitter.com/1.1/lists/members/create_all.json", account,
                makeQuery("screen_name", string.Join(",", screenName),
                "list_id", listId.ToString()));
        }

        public async Task RemoveMembersToUser(Account account, string[] screenName, string slug, long ownerId)
        {
            await Post("https://api.twitter.com/1.1/lists/members/create_all.json", account,
                makeQuery("screen_name", string.Join(",", screenName),
                "slug", slug, "owner_id", ownerId.ToString()));
        }

        public async Task RemoveMembersToUser(Account account, string[] screenName, string slug, string ownerScreenName)
        {
            await Post("https://api.twitter.com/1.1/lists/members/create_all.json", account,
                makeQuery("screen_name", string.Join(",", screenName),
                "slug", slug, "owner_screen_name", ownerScreenName));
        }

        public async Task SubscribeAccountToList(Account account, long listId)
        {
            await Post("https://api.twitter.com/1.1/lists/subscribers/create.json", account,
                makeQuery("list_id", listId.ToString()));
        }

        public async Task SubscribeAccountToList(Account account, string slug, long ownerId)
        {
            await Post("https://api.twitter.com/1.1/lists/subscribers/create.json", account,
                makeQuery("slug", slug, "owner_id", ownerId.ToString()));
        }

        public async Task SubscribeAccountToList(Account account, string slug, string ownerScreenName)
        {
            await Post("https://api.twitter.com/1.1/lists/subscribers/create.json", account,
                makeQuery("slug", slug, "owner_screen_name", ownerScreenName));
        }

        public async Task UnsubscribeAccountToList(Account account, long listId)
        {
            await Post("https://api.twitter.com/1.1/lists/subscribers/destroy.json", account,
                makeQuery("list_id", listId.ToString()));
        }

        public async Task UnsubscribeAccountToList(Account account, string slug, long ownerId)
        {
            await Post("https://api.twitter.com/1.1/lists/subscribers/destroy.json", account,
                makeQuery("slug", slug, "owner_id", ownerId.ToString()));
        }

        public async Task UnsubscribeAccountToList(Account account, string slug, string ownerScreenName)
        {
            await Post("https://api.twitter.com/1.1/lists/subscribers/destroy.json", account,
                makeQuery("slug", slug, "owner_screen_name", ownerScreenName));
        }

        public async Task<CursoredList<long>> GetFollowerIds(Account account, long userId, long cursor = -1, long count = 100)
        {
            var response = JObject.Parse(
                await Get("https://api.twitter.com/1.1/followers/ids.json", account,
                makeQuery("user_id", userId.ToString(),
                "cursor", cursor != -1 ? cursor.ToString() : null,
                "count", count.ToString()
                )));

            var result = makeCursoredList<long>(response);

            foreach (var id in response["ids"].ToObject<JArray>())
            {
                result.Add(id.ToObject<long>());
            }

            return result;
        }

        public async Task<CursoredList<long>> GetFollowerIds(Account account, string screenName, long cursor = -1, long count = 100)
        {
            var response = JObject.Parse(
                await Get("https://api.twitter.com/1.1/followers/ids.json", account,
                makeQuery("screen_name", screenName,
                "cursor", cursor != -1 ? cursor.ToString() : null,
                "count", count.ToString()
                )));

            var result = makeCursoredList<long>(response);

            foreach (var id in response["ids"].ToObject<JArray>())
            {
                result.Add(id.ToObject<long>());
            }

            return result;
        }

        public async Task<CursoredList<User>> GetFollowers(Account account, long userId, long cursor = -1, long count = 100)
        {
            var response = JObject.Parse(
                await Get("https://api.twitter.com/1.1/followers/list.json", account,
                makeQuery("user_id", userId.ToString(),
                "cursor", cursor != -1 ? cursor.ToString() : null,
                "count", count.ToString()
                )));

            var result = makeCursoredList<User>(response);

            result.AddRange(TwitterDataFactory.parseArray(response["users"].ToObject<JArray>(), account.id, TwitterDataFactory.parseUser));

            return result;
        }

        public async Task<CursoredList<User>> GetFollowers(Account account, string screenName, long cursor = -1, long count = 100)
        {
            var response = JObject.Parse(
                await Get("https://api.twitter.com/1.1/followers/list.json", account,
                makeQuery("screen_name", screenName,
                "cursor", cursor != -1 ? cursor.ToString() : null,
                "count", count.ToString()
                )));

            var result = makeCursoredList<User>(response);

            result.AddRange(TwitterDataFactory.parseArray(response["users"].ToObject<JArray>(), account.id, TwitterDataFactory.parseUser));

            return result;
        }

        public async Task<CursoredList<long>> GetFriendIds(Account account, long userId, long cursor = -1, long count = 100)
        {
            var response = JObject.Parse(
                await Get("https://api.twitter.com/1.1/friends/ids.json", account,
                makeQuery("user_id", userId.ToString(),
                "cursor", cursor != -1 ? cursor.ToString() : null,
                "count", count.ToString()
                )));

            var result = makeCursoredList<long>(response);

            foreach (var id in response["ids"].ToObject<JArray>())
            {
                result.Add(id.ToObject<long>());
            }

            return result;
        }

        public async Task<CursoredList<long>> GetFriendIds(Account account, string screenName, long cursor = -1, long count = 100)
        {
            var response = JObject.Parse(
                await Get("https://api.twitter.com/1.1/friends/ids.json", account,
                makeQuery("screen_name", screenName,
                "cursor", cursor != -1 ? cursor.ToString() : null,
                "count", count.ToString()
                )));

            var result = makeCursoredList<long>(response);

            foreach (var id in response["ids"].ToObject<JArray>())
            {
                result.Add(id.ToObject<long>());
            }

            return result;
        }

        public async Task<CursoredList<User>> GetFriends(Account account, long userId, long cursor = -1, long count = 100)
        {
            var response = JObject.Parse(
                await Get("https://api.twitter.com/1.1/friends/list.json", account,
                makeQuery("user_id", userId.ToString(),
                "cursor", cursor != -1 ? cursor.ToString() : null,
                "count", count.ToString()
                )));

            var result = makeCursoredList<User>(response);

            result.AddRange(TwitterDataFactory.parseArray(response["users"].ToObject<JArray>(), account.id, TwitterDataFactory.parseUser));

            return result;
        }

        public async Task<CursoredList<User>> GetFriends(Account account, string screenName, long cursor = -1, long count = 100)
        {
            var response = JObject.Parse(
                await Get("https://api.twitter.com/1.1/friends/list.json", account,
                makeQuery("screen_name", screenName,
                "cursor", cursor != -1 ? cursor.ToString() : null,
                "count", count.ToString()
                )));

            var result = makeCursoredList<User>(response);

            result.AddRange(TwitterDataFactory.parseArray(response["users"].ToObject<JArray>(), account.id, TwitterDataFactory.parseUser));

            return result;
        }

        public async Task<CursoredList<long>> GetPendingRequestToAccount(Account account, long cursor = -1)
        {
            var response = JObject.Parse(
                await Get("https://api.twitter.com/1.1/friendships/incoming.json", account,
                makeQuery("cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = makeCursoredList<long>(response);

            foreach (var id in response["ids"].ToObject<JArray>())
            {
                result.Add(id.ToObject<long>());
            }

            return result;
        }

        public async Task<CursoredList<long>> GetPendingRequestFromAccount(Account account, long cursor = -1)
        {
            var response = JObject.Parse(
                await Get("https://api.twitter.com/1.1/friendships/outgoing.json", account,
                makeQuery("cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = makeCursoredList<long>(response);

            foreach (var id in response["ids"].ToObject<JArray>())
            {
                result.Add(id.ToObject<long>());
            }

            return result;
        }

        public async Task<List<Friendship>> GetFriendship(Account account, params long[] userId)
        {
            return TwitterDataFactory.parseArray(JArray.Parse(
                await Get("https://api.twitter.com/1.1/friendships/lookup.json", account,
                makeQuery("user_id", string.Join(",", userId))
                ))
                , TwitterDataFactory.parseFriendship).ToList();
        }

        public async Task<List<Friendship>> GetFriendship(Account account, params string[] screenName)
        {
            return TwitterDataFactory.parseArray(JArray.Parse(
                await Get("https://api.twitter.com/1.1/friendships/lookup.json", account,
                makeQuery("screen_name", string.Join(",", screenName))
                ))
                , TwitterDataFactory.parseFriendship).ToList();
        }

        public async Task<List<long>> GetNoRetweetListOfAccount(Account account)
        {
            var response = JArray.Parse(
                await Get("https://api.twitter.com/1.1/friendships/no_retweets/ids.json", account,
                makeQuery()
                ));

            var result = new List<long>();

            foreach (var id in response)
            {
                result.Add(id.ToObject<long>());
            }

            return result;
        }

        public async Task<Relationship> GetRelationship(Account account, long sourceId, long targetId)
        {
            return TwitterDataFactory.parseRelationship(JObject.Parse(
                await Get("https://api.twitter.com/1.1/friendships/show.json", account,
                makeQuery("source_id", sourceId.ToString(), "target_id", targetId.ToString())
                )));
        }

        public async Task<Relationship> GetRelationship(Account account, string sourceScreenName, string targetScreenName)
        {
            return TwitterDataFactory.parseRelationship(JObject.Parse(
                await Get("https://api.twitter.com/1.1/friendships/show.json", account,
                makeQuery("source_screen_name", sourceScreenName, "target_screen_name", targetScreenName)
                )));
        }

        public async Task<List<User>> GetUsers(Account account, long[] userIds)
        {
            return TwitterDataFactory.parseArray(JArray.Parse(
                    await Get("https://api.twitter.com/1.1/users/lookup.json", account,
                    makeQuery("user_id", string.Join(",", userIds))
                )), account.id, TwitterDataFactory.parseUser).ToList();
        }

        public async Task<List<User>> GetUsers(Account account, string[] userScreenNames)
        {
            return TwitterDataFactory.parseArray(JArray.Parse(
                    await Get("https://api.twitter.com/1.1/users/lookup.json", account,
                    makeQuery("screen_name", string.Join(",", userScreenNames))
                )), account.id, TwitterDataFactory.parseUser).ToList();
        }

        public async Task<User> GetUser(Account account, long userIds)
        {
            return TwitterDataFactory.parseUser(JObject.Parse(
                await Get("https://api.twitter.com/1.1/users/show.json", account,
                makeQuery("user_id", userIds.ToString())
                )), account.id);
        }

        public async Task<User> GetUser(Account account, string userScreenNames)
        {
            return TwitterDataFactory.parseUser(JObject.Parse(
                await Get("https://api.twitter.com/1.1/users/show.json", account,
                makeQuery("screen_name", userScreenNames)
                )), account.id);
        }

        public async Task<List<User>> SearchUsers(Account account, string query, long page, long count = 20)
        {
            return TwitterDataFactory.parseArray(JArray.Parse(
                    await Get("https://api.twitter.com/1.1/users/search.json", account,
                    makeQuery("q", query, "page", page.ToString(), "count", count.ToString())
                )), account.id, TwitterDataFactory.parseUser).ToList();
        }

        public async Task<User> CreateFriendship(Account account, long userId)
        {
            return TwitterDataFactory.parseUser(
                JObject.Parse(
                    await Post("https://api.twitter.com/1.1/friendships/create.json", account, 
                    makeQuery("user_id", userId.ToString())
                )), account.id);
        }

        public async Task<User> CreateFriendship(Account account, string screenName)
        {
            return TwitterDataFactory.parseUser(
                JObject.Parse(
                    await Post("https://api.twitter.com/1.1/friendships/create.json", account,
                    makeQuery("screen_name", screenName)
                )), account.id);
        }

        public async Task<User> UpdateFriendship(Account account, long userId, bool? enableDeviceNotifications = null, bool? enableRetweet = null)
        {
            return TwitterDataFactory.parseUser(
                JObject.Parse(
                    await Post("https://api.twitter.com/1.1/friendships/update.json", account,
                    makeQuery("user_id", userId.ToString(),
                    "device", enableDeviceNotifications != null ? enableDeviceNotifications.Value.ToString() : null,
                    "retweets", enableRetweet != null ? enableRetweet.Value.ToString() : null)
                )), account.id);
        }

        public async Task<User> UpdateFriendship(Account account, string screenName, bool? enableDeviceNotifications = null, bool? enableRetweet = null)
        {
            return TwitterDataFactory.parseUser(
                JObject.Parse(
                    await Post("https://api.twitter.com/1.1/friendships/update.json", account,
                    makeQuery("screen_name", screenName,
                    "device", enableDeviceNotifications != null ? enableDeviceNotifications.Value.ToString() : null,
                    "retweets", enableRetweet != null ? enableRetweet.Value.ToString() : null)
                )), account.id);
        }

        public async Task<User> DestroyFriendship(Account account, long userId)
        {
            return TwitterDataFactory.parseUser(
                JObject.Parse(
                    await Post("https://api.twitter.com/1.1/friendships/destroy.json", account,
                    makeQuery("user_id", userId.ToString())
                )), account.id);
        }

        public async Task<User> DestroyFriendship(Account account, string screenName)
        {
            return TwitterDataFactory.parseUser(
                JObject.Parse(
                    await Post("https://api.twitter.com/1.1/friendships/destroy.json", account,
                    makeQuery("screen_name", screenName)
                )), account.id);
        }

        public async Task<CursoredList<long>> GetBlockIds(Account account, long cursor = -1)
        {
            var response = JObject.Parse(
                await Get("https://api.twitter.com/1.1/blocks/ids.json", account,
                makeQuery("cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = makeCursoredList<long>(response);

            foreach (var id in response["ids"].ToObject<JArray>())
            {
                result.Add(id.ToObject<long>());
            }

            return result;
        }

        public async Task<CursoredList<User>> GetBlockList(Account account, long cursor = -1)
        {
            var response = JObject.Parse(
                await Get("https://api.twitter.com/1.1/blocks/list.json", account,
                makeQuery("cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = makeCursoredList<User>(response);

            result.AddRange(TwitterDataFactory.parseArray(response["users"].ToObject<JArray>(), account.id, TwitterDataFactory.parseUser));

            return result;
        }

        public async Task<CursoredList<long>> GetMuteIds(Account account, long cursor = -1)
        {
            var response = JObject.Parse(
                await Get("https://api.twitter.com/1.1/mutes/ids.json", account,
                makeQuery("cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = makeCursoredList<long>(response);

            foreach (var id in response["ids"].ToObject<JArray>())
            {
                result.Add(id.ToObject<long>());
            }

            return result;
        }

        public async Task<CursoredList<User>> GetMuteUsers(Account account, long cursor = -1)
        {
            var response = JObject.Parse(
                await Get("https://api.twitter.com/1.1/mutes/list.json", account,
                makeQuery("cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = makeCursoredList<User>(response);

            result.AddRange(TwitterDataFactory.parseArray(response["users"].ToObject<JArray>(), account.id, TwitterDataFactory.parseUser));

            return result;
        }

        public async Task<User> Block(Account account, long userId)
        {
            return TwitterDataFactory.parseUser(
                JObject.Parse(
                    await Post("https://api.twitter.com/1.1/blocks/create.json", account,
                    makeQuery("user_id", userId.ToString())
                )), account.id);
        }

        public async Task<User> Block(Account account, string screenName)
        {
            return TwitterDataFactory.parseUser(
                JObject.Parse(
                    await Post("https://api.twitter.com/1.1/blocks/create.json", account,
                    makeQuery("screen_name", screenName)
                )), account.id);
        }

        public async Task<User> Unblock(Account account, long userId)
        {
            return TwitterDataFactory.parseUser(
                JObject.Parse(
                    await Post("https://api.twitter.com/1.1/blocks/destroy.json", account,
                    makeQuery("user_id", userId.ToString())
                )), account.id);
        }

        public async Task<User> Unblock(Account account, string screenName)
        {
            return TwitterDataFactory.parseUser(
                JObject.Parse(
                    await Post("https://api.twitter.com/1.1/blocks/destroy.json", account,
                    makeQuery("screen_name", screenName)
                )), account.id);
        }

        public async Task<User> Mute(Account account, long userId)
        {
            return TwitterDataFactory.parseUser(
                JObject.Parse(
                    await Post("https://api.twitter.com/1.1/mutes/users/create.json", account,
                    makeQuery("user_id", userId.ToString())
                )), account.id);
        }

        public async Task<User> Mute(Account account, string screenName)
        {
            return TwitterDataFactory.parseUser(
                JObject.Parse(
                    await Post("https://api.twitter.com/1.1/mutes/users/create.json", account,
                    makeQuery("screen_name", screenName)
                )), account.id);
        }

        public async Task<User> Unmute(Account account, long userId)
        {
            return TwitterDataFactory.parseUser(
                JObject.Parse(
                    await Post("https://api.twitter.com/1.1/mutes/users/destroy.json", account,
                    makeQuery("user_id", userId.ToString())
                )), account.id);
        }

        public async Task<User> Unmute(Account account, string screenName)
        {
            return TwitterDataFactory.parseUser(
                JObject.Parse(
                    await Post("https://api.twitter.com/1.1/mutes/users/destroy.json", account,
                    makeQuery("screen_name", screenName)
                )), account.id);
        }

        public async Task<User> ReportSpam(Account account, long userId, bool performBlock = true)
        {
            return TwitterDataFactory.parseUser(
                JObject.Parse(
                    await Post("https://api.twitter.com/1.1/users/report_spam.json", account,
                    makeQuery("user_id", userId.ToString(),
                    "performBlock", performBlock == false ? "false" : null)
                )), account.id);
        }

        public async Task<User> ReportSpam(Account account, string screenName, bool performBlock = true)
        {
            return TwitterDataFactory.parseUser(
                JObject.Parse(
                    await Post("https://api.twitter.com/1.1/users/report_spam.json", account,
                    makeQuery("screen_name", screenName,
                    "performBlock", performBlock == false ? "false" : null)
                )), account.id);
        }
    }
}
