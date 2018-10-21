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
using System.Net;

namespace TwitterLibrary
{
    public class API : IAPI
    {
        private static List<KeyValuePair<string, string>> MakeQuery(params string[] query)
        {
            var list = new List<KeyValuePair<string, string>>();
            for (int i = 0; i < query.Length; i += 2)
            {
                if (query[i + 1] != null)
                {
                    list.Add(new KeyValuePair<string, string>(query[i], query[i + 1]));
                }
            }
            return list;
        }

        private static CursoredList<T> MakeCursoredList<T>(JObject array)
        {
            return new CursoredList<T>(array["previous_cursor"].ToObject<long>(), array["next_cursor"].ToObject<long>());
        }

        private static string StreamToBase64(Stream stream)
        {
            var reader = new BinaryReader(stream);
            var data = reader.ReadBytes((int)stream.Length);
            reader.Close();

            var base64 = Convert.ToBase64String(data);
            data = null;

            return base64;
        }

        internal void AttachImageToRequest(HttpRequestMessage message, Stream image, string name)
        {
            byte[] imgdata = new byte[image.Length];
            image.Read(imgdata, 0, (int)image.Length);
            var imageContent = new ByteArrayContent(imgdata);

            imageContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");

            var multipartContent = new MultipartFormDataContent
            {
                { imageContent, name }
            };

            message.Content = multipartContent;
        }

        internal async Task<string> GetAsync(string uri, Account account, List<KeyValuePair<string, string>> query)
        {
            if(uri.Contains("1.1/statuses") || uri.Contains("2/timeline"))
            {
                query.Add(new KeyValuePair<string, string>("tweet_mode", "extended"));
                query.Add(new KeyValuePair<string, string>("include_cards", "1"));
                query.Add(new KeyValuePair<string, string>("cards_platform", "Web-13"));
            }
            return await Utils.ReadStringFromTwitter(httpClient, HttpMethod.Get, new Uri(uri), query.ToArray(), account);
        }

        internal async Task<string> GetAsync(string uri, Token consumer, Token? oauth, List<KeyValuePair<string, string>> query)
        {
            return await Utils.ReadStringFromTwitter(httpClient, HttpMethod.Get, new Uri(uri), query.ToArray(), consumer, oauth);
        }

        internal async Task<string> PostAsync(string uri, Account account, List<KeyValuePair<string, string>> query)
        {
            if (uri.Contains("1.1/statuses") || uri.Contains("2/timeline"))
            {
                query.Add(new KeyValuePair<string, string>("tweet_mode", "extended"));
                query.Add(new KeyValuePair<string, string>("include_cards", "1"));
                query.Add(new KeyValuePair<string, string>("cards_platform", "Web-13"));
            }
            return await Utils.ReadStringFromTwitter(httpClient, HttpMethod.Post, new Uri(uri), query.ToArray(), account);
        }

        internal async Task<string> PostAsync(string uri, Token consumer, Token? oauth, List<KeyValuePair<string, string>> query)
        {
            return await Utils.ReadStringFromTwitter(httpClient, HttpMethod.Post, new Uri(uri), query.ToArray(), consumer, oauth);
        }

        internal readonly HttpClient httpClient;

        public API()
        {
            HttpClientHandler handler = new HttpClientHandler
            {
                UseCookies = false
            };

            httpClient = new HttpClient(handler);
        }

        public async Task<SavedSearch> CreateSavedSearchAsync(Account account, string query)
        {
            var response = await PostAsync("https://api.twitter.com/1.1/saved_searches/create.json", account,
                MakeQuery("query", query));

            return TwitterDataFactory.ParseSavedSearch(JObject.Parse(response));
        }

        public async Task<SavedSearch> DestroySavedSearchAsync(Account account, long id)
        {
            var response = await PostAsync("https://api.twitter.com/1.1/saved_searches/destroy/" + id + ".json", account, MakeQuery());

            return TwitterDataFactory.ParseSavedSearch(JObject.Parse(response));
        }

        public async Task<AccountSetting> GetAccountSettingAsync(Account account)
        {
            var response = await GetAsync("https://api.twitter.com/1.1/account/settings.json", account, MakeQuery());

            var obj = JObject.Parse(response);
            var result = new AccountSetting
            {
                IsAlwaysUseHttps = obj["always_use_https"].ToObject<bool>(),
                IsDiscoverableByEmail = obj["discoverable_by_email"].ToObject<bool>(),
                IsGeoEnabled = obj["geo_enabled"].ToObject<bool>(),
                Language = obj["language"].ToString(),
                IsProtected = obj["protected"].ToObject<bool>(),
                ScreenName = obj["screen_name"].ToString(),
                ShowAllInlineMedia = obj["show_all_inline_media"].ToObject<bool>(),
                SleepTime = new AccountSetting.SleepTimeData()
            };
            var sleepTime = obj["sleep_time"].ToObject<JObject>();
            result.SleepTime.IsEnabled = sleepTime["enabled"].ToObject<bool>();
            //TODO: startTime / endTime
            //TODO: timeZone
            var trendLocation = obj["trend_location"].ToObject<JObject>();
            result.TrendLocation = new AccountSetting.TrendLocationData
            {
                Country = trendLocation["country"].ToString(),
                CountryCode = trendLocation["countryCode"].ToString(),
                Name = trendLocation["name"].ToString(),
                ParentId = trendLocation["parentid"].ToObject<long>(),
                PlaceTypeCode = trendLocation["placeType"]["code"].ToObject<long>(),
                PlaceTypeName = trendLocation["placeType"]["name"].ToString(),
                URL = trendLocation["url"].ToString(),
                Woeid = trendLocation["woeid"].ToObject<long>()
            };

            result.UseCookiePersonalization = obj["use_cookie_personalization"].ToObject<bool>();
            result.AllowContributorRequest = obj["allow_contributor_request"].ToString();

            return result;
        }

        public async Task<ILoginToken> GetLoginTokenAsync(Token consumerToken)
        {
            var response = await PostAsync("https://api.twitter.com/oauth/request_token", consumerToken, null, MakeQuery());
            var data = HttpUtility.ParseQueryString(response);

            var token = new Token
            {
                Key = data["oauth_token"],
                Secret = data["oauth_token_secret"]
            };

            return new LoginToken(this, consumerToken, token);
        }

        public async Task<Account> GetAccountFromTweetdeckCookieAsync(CookieCollection cookieData)
        {
            var account = new TDAccount();
            account.InitwithCookie(cookieData);
            account.User = await VerifyCredentialsAsync(account);
            account.ID = account.User.ID;

            return account;
        }

        public async Task<SavedSearch> GetSavedSearchByIdAsync(Account account, long id)
        {
            return TwitterDataFactory.ParseSavedSearch(
                JObject.Parse(
                    await GetAsync("https://api.twitter.com/1.1/saved_searches/show/" + id + ".json", account, MakeQuery())
                ));
        }

        public async Task<List<SavedSearch>> GetSavedSearchesAsync(Account account)
        {
            return TwitterDataFactory.ParseArray(
                JArray.Parse(
                    await GetAsync("https://api.twitter.com/1.1/saved_searches/list.json", account, MakeQuery())
                ), TwitterDataFactory.ParseSavedSearch).ToList();
        }

        public Account LoadAccount(JObject data)
        {
            try
            {
                if (data["type"].ToString() == "TD")
                {
                    return TDAccount.Load(data);
                }
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message + " " + e.StackTrace);
            }
            return LibAccount.Load(data);
        }

        public async Task RemoveProfileBannerAsync(Account account)
        {
            await PostAsync("https://api.twitter.com/1.1/account/remove_profile_banner.json", account, MakeQuery());
        }

        public async Task<User> UpdateProfileAsync(Account account, string name, string url, string location, string description, string profileLinkColor)
        {
            return TwitterDataFactory.ParseUser(
                JObject.Parse(
                        await PostAsync("https://api.twitter.com/1.1/account/update_profile.json", account,
                        MakeQuery("name", name, "url", url, "location", location, "description", description, "profile_link_color", profileLinkColor)
                    )
                ), account.ID
                );
        }

        public async Task UpdateProfileBannerAsync(Account account, Stream image)
        {
            var message = account.GenerateRequest(HttpMethod.Post, new Uri("https://api.twitter.com/1.1/account/update_profile_banner.json"), new KeyValuePair<string, string>[] { });
            AttachImageToRequest(message, image, "banner");
            
            var response = await httpClient.SendAsync(message);

            Utils.VerifyTwitterResponse(response);
        }

        public async Task<User> UpdateProfileImageAsync(Account account, Stream image)
        {
            var message = account.GenerateRequest(HttpMethod.Post, new Uri("https://api.twitter.com/1.1/account/update_profile_image.json"), new KeyValuePair<string, string>[] { });
            AttachImageToRequest(message, image, "image");

            var response = await httpClient.SendAsync(message);

            Utils.VerifyTwitterResponse(response);

            return TwitterDataFactory.ParseUser(
                JObject.Parse( await response.Content.ReadAsStringAsync() ), account.ID
                );
        }

        public async Task<User> VerifyCredentialsAsync(Account account)
        {
            var response = await GetAsync("https://api.twitter.com/1.1/account/verify_credentials.json", account, MakeQuery());

            return TwitterDataFactory.ParseUser(JObject.Parse(response), account.ID);
        }

        public async Task<GetEntriesResult> GetEntriesAsync(Account account, string id, long count, long maxPosition = -1, long minPosition = -1)
        {
            var response = await GetAsync("https://api.twitter.com/1.1/collections/entries.json", account,
                MakeQuery("id", id, "count", count.ToString(),
                "max_position", maxPosition != -1 ? maxPosition.ToString() : null,
                "min_position", minPosition != -1 ? minPosition.ToString() : null));

            var json = JObject.Parse(response);

            var result = new GetEntriesResult();

            var position = json["response"]["position"];
            result.MinPosition = position["min_position"].ToObject<long>();
            result.MaxPosition = position["max_position"].ToObject<long>();

            result.CollectionTweets = TwitterDataFactory.ParseArray(json["response"]["timeline"].ToObject<JArray>(), TwitterDataFactory.ParseCollectionTweet).ToList();
            result.Collection = TwitterDataFactory.ParseCollection(json["objects"]["timelines"][
                json["response"]["timeline-id"].ToString()
                ].ToObject<JObject>());

            result.Tweet = new List<Status>();
            foreach (var cTweet in result.CollectionTweets)
            {
                result.Tweet.Add(TwitterDataFactory.ParseStatus(json["objects"]["tweets"][cTweet.TweetId.ToString()].ToObject<JObject>(), account.ID));
            }

            return result;
        }

        private FindListResult FindList(string response)
        {
            var json = JObject.Parse(response);
            var result = new FindListResult
            {
                NextCursor = json["response"]["cursors"]["next_cursor"].ToString(),
                Collections = new List<Collection>()
            };

            var results = json["response"]["result"].ToObject<JArray>();
            foreach (var timeline in results)
            {
                result.Collections.Add(TwitterDataFactory.ParseCollection(json["objects"]["timelines"][timeline["timeline-id"].ToString()].ToObject<JObject>()));
            }

            return result;
        }

        public async Task<FindListResult> FindListAsync(Account account, long userId, long count, string cursor)
        {
            return FindList(await GetAsync("https://api.twitter.com/1.1/collections/list.json", account,
                MakeQuery("user_id", userId.ToString(), "count", count != -1 ? count.ToString() : null, "cursor", cursor)
                ));
        }

        public async Task<FindListResult> FindListAsync(Account account, string screenName, long count, string cursor)
        {
            return FindList(await GetAsync("https://api.twitter.com/1.1/collections/list.json", account,
                MakeQuery("screen_name", screenName, "count", count != -1 ? count.ToString() : null, "cursor", cursor)
                ));
        }

        public async Task<FindListResult> FindListByTweetIdAsync(Account account, long tweetId, long count, string cursor)
        {
            return FindList(await GetAsync("https://api.twitter.com/1.1/collections/list.json", account,
                MakeQuery("tweet_id", tweetId.ToString(), "count", count != -1 ? count.ToString() : null, "cursor", cursor)
                ));
        }

        public async Task<Collection> GetCollectionAsync(Account account, string id)
        {
            var json = JObject.Parse(await GetAsync("https://api.twitter.com/1.1/collections/show.json", account,
                MakeQuery("id", id)));

            return TwitterDataFactory.ParseCollection(json["objects"]["timelines"][json["response"]["timeline_id"].ToString()].ToObject<JObject>());
        }

        public async Task<Collection> CreateCollectionAsync(Account account, string name, string description, string url, Collection.Order? order)
        {
            var json = JObject.Parse(await PostAsync("https://api.twitter.com/1.1/collections/create.json", account,
                MakeQuery("name", name, "description", description, "url", url, "order", order != null ? Collection.OrderToString(order.Value) : null)));

            return TwitterDataFactory.ParseCollection(json["objects"]["timelines"][json["response"]["timeline_id"].ToString()].ToObject<JObject>());
        }

        public async Task<Collection> UpdateCollectionAsync(Account account, string id, string name, string description, string url)
        {
            var json = JObject.Parse(await PostAsync("https://api.twitter.com/1.1/collections/update.json", account,
                MakeQuery("id", id, "name", name, "description", description, "url", url)));
            //TODO: I think UpdateColllection's result collection data is incomplete?
            return TwitterDataFactory.ParseCollection(json["objects"]["timelines"][json["response"]["timeline_id"].ToString()].ToObject<JObject>());
        }

        public async Task DestroyCollectionAsync(Account account, string id)
        {
            //TODO: Error handleing
            await PostAsync("https://api.twitter.com/1.1/collections/destroy.json", account, MakeQuery("id", id));
        }

        public async Task AddTweetToCollectionAsync(Account account, string id, long statusId, long relativeTo, bool above = true)
        {
            //TODO: Error handleing
            await PostAsync("https://api.twitter.com/1.1/collections/entries/add.json", account,
                MakeQuery("id", id, "tweet_id", statusId.ToString(), "relative_to", relativeTo != -1 ? relativeTo.ToString() : null,
                "above", above == false ? above.ToString() : null));
        }

        public async Task MoveTweetFromCollectionAsync(Account account, string id, long[] statusId, long relativeTo, bool above = true)
        {
            //TODO: Error handleing
            await PostAsync("https://api.twitter.com/1.1/collections/entries/move.json", account,
                MakeQuery("id", id, "tweet_id", statusId.ToString(), "relative_to", relativeTo != -1 ? relativeTo.ToString() : null,
                "above", above == false ? above.ToString() : null));
        }

        public async Task RemoveTweetFromCollectionAsync(Account account, string id, long statusId)
        {
            //TODO: Error handleing
            await PostAsync("https://api.twitter.com/1.1/collections/entries/remove.json", account,
                MakeQuery("id", id, "tweet_id", statusId.ToString()));
        }

        private JObject MakeCurateJson(string op, string id, long[] statusId)
        {
            var json = new JObject
            {
                ["id"] = id
            };
            var changes = new JArray();

            foreach (var sid in statusId)
            {
                var operationJson = new JObject
                {
                    ["op"] = op,
                    ["tweet_id"] = sid
                };
                changes.Add(operationJson);
            }

            json["changes"] = changes;

            return json;
        }

        private async Task HandleCurate(HttpRequestMessage message, JObject operation)
        {
            message.Content = new StringContent(operation.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(message);
        }

        public async Task AddAllTweetToCollectionAsync(Account account, string id, long[] statusId, long relativeTo, bool above = true)
        {
            //TODO: Error handleing
            await HandleCurate(Utils.GenerateHttpRequest(HttpMethod.Post, new Uri("https://api.twitter.com/1.1/collections/entries/curate.json"), new KeyValuePair<string, string>[] { }
            , account), MakeCurateJson("add", id, statusId));
        }

        public async Task RemoveAllTweetFromCollectionAsync(Account account, string id, long[] statusId, long relativeTo, bool above = true)
        {
            //TODO: Error handleing
            await HandleCurate(Utils.GenerateHttpRequest(HttpMethod.Post, new Uri("https://api.twitter.com/1.1/collections/entries/curate.json"), new KeyValuePair<string, string>[] { }
            , account), MakeCurateJson("remove", id, statusId));
        }

        public async Task<long> UploadMediaAsync(Account account, string fileName, Stream image)
        {
            var message = Utils.GenerateHttpRequest(HttpMethod.Post, new Uri("https://upload.twitter.com/1.1/media/upload.json"), new KeyValuePair<string, string>[] { }
            , account);

            AttachImageToRequest(message, image, "media");

            var response = await httpClient.SendAsync(message);

            Utils.VerifyTwitterResponse(response);

            var result = JObject.Parse(await response.Content.ReadAsStringAsync());

            return result["media_id"].ToObject<long>();
        }

        public async Task<List<Status>> GetTimelineAsync(Account account, long count = 200, long sinceId = -1, long maxId = -1)
        {
            var response = JArray.Parse(
                await GetAsync("https://api.twitter.com/1.1/statuses/home_timeline.json", account,
                MakeQuery("count", count.ToString(), "since_id", sinceId != -1 ? sinceId.ToString() : null, "max_id", maxId != -1 ? maxId.ToString() : null)
                )
                );

            return TwitterDataFactory.ParseArray(response, account.ID, TwitterDataFactory.ParseStatus).ToList();
        }

        public async Task<List<Status>> GetUserlineAsync(Account account, long userId, long count = 200, long sinceId = -1, long maxId = -1)
        {
            var response = JArray.Parse(
                await GetAsync("https://api.twitter.com/1.1/statuses/user_timeline.json", account,
                MakeQuery("user_id", userId.ToString(), "count", count.ToString(), "since_id", sinceId != -1 ? sinceId.ToString() : null, "max_id", maxId != -1 ? maxId.ToString() : null)
                )
                );

            return TwitterDataFactory.ParseArray(response, account.ID, TwitterDataFactory.ParseStatus).ToList();
        }

        public async Task<List<Status>> GetAccountRetweetsAsync(Account account, long count = 200, long sinceId = -1, long maxId = -1)
        {
            var response = JArray.Parse(
                await GetAsync("https://api.twitter.com/1.1/statuses/retweets_of_me.json", account,
                MakeQuery("count", count.ToString(), "since_id", sinceId != -1 ? sinceId.ToString() : null, "max_id", maxId != -1 ? maxId.ToString() : null)
                )
                );

            return TwitterDataFactory.ParseArray(response, account.ID, TwitterDataFactory.ParseStatus).ToList();
        }

        public async Task<List<Status>> GetMentionlineAsync(Account account, long count = 200, long sinceId = -1, long maxId = -1)
        {
            var response = JArray.Parse(
                await GetAsync("https://api.twitter.com/1.1/statuses/mentions_timeline.json", account,
                MakeQuery("count", count.ToString(), "since_id", sinceId != -1 ? sinceId.ToString() : null, "max_id", maxId != -1 ? maxId.ToString() : null)
                )
                );

            return TwitterDataFactory.ParseArray(response, account.ID, TwitterDataFactory.ParseStatus).ToList();
        }

        public async Task<Status> CreateStatusAsync(Account account, StatusUpdate update)
        {
            return TwitterDataFactory.ParseStatus(JObject.Parse(
                await PostAsync("https://api.twitter.com/1.1/statuses/update.json", account,
                MakeQuery("status", update.Text,
                "in_reply_to_status_id", update.InReplyToStatusId != -1 ? update.InReplyToStatusId.ToString() : null,
                "auto_populate_reply_metadata", update.AutoPopulateReplyMetadata ? "true" : null,
                "exclude_reply_user_ids", update.ExcludeReplyUserIds != null ? string.Join(",", update.ExcludeReplyUserIds) : null,
                "attachment_url", update.AttachmentURL,
                "media_ids", update.MediaIDs != null ? string.Join(",", update.MediaIDs) : null,
                "possibly_sensitive", update.PossiblySensitive ? "true" : null
                ))), account.ID);
        }

        public async Task<Status> DestroyStatusAsync(Account account, long id)
        {
            return TwitterDataFactory.ParseStatus(JObject.Parse(
                await PostAsync("https://api.twitter.com/1.1/statuses/destroy/" + id + ".json", account, MakeQuery())
                ), account.ID);
        }

        public async Task<Status> GetStatusesAsync(Account account, long id)
        {
            return TwitterDataFactory.ParseStatus(JObject.Parse(
                await GetAsync("https://api.twitter.com/1.1/statuses/show.json", account,
                MakeQuery("id", id.ToString())
                )), account.ID);
        }

        public async Task<List<Status>> GetStatusesAsync(Account account, long[] ids)
        {
            return TwitterDataFactory.ParseArray(JArray.Parse(
                await GetAsync("https://api.twitter.com/1.1/statuses/lookup.json", account,
                MakeQuery("id", string.Join(",", ids))
                )), account.ID, TwitterDataFactory.ParseStatus).ToList();
        }

        public async Task<Status> RetweetStatusAsync(Account account, long id)
        {
            return TwitterDataFactory.ParseStatus(JObject.Parse(
                await PostAsync("https://api.twitter.com/1.1/statuses/retweet/" + id.ToString() + ".json", account, MakeQuery())
                ), account.ID);
        }

        public async Task<Status> UnretweetStatusAsync(Account account, long id)
        {
            return TwitterDataFactory.ParseStatus(JObject.Parse(
                await PostAsync("https://api.twitter.com/1.1/statuses/unretweet/" + id.ToString() + ".json", account, MakeQuery())
                ), account.ID);
        }

        public async Task<List<Status>> GetRetweetedStatusAsync(Account account, long id, long count = 100)
        {
            return TwitterDataFactory.ParseArray(JArray.Parse(
                await GetAsync("https://api.twitter.com/1.1/statuses/retweets/" + id + ".json", account, MakeQuery("count", count.ToString()))
                ), account.ID, TwitterDataFactory.ParseStatus).ToList();
        }

        public async Task<CursoredList<long>> GetRetweeterIdsAsync(Account account, long id, long count = 100, long cursor = -1)
        {
            var response = JObject.Parse(await GetAsync("https://api.twitter.com/1.1/statuses/retweets/" + id + ".json", account,
                MakeQuery("count", count.ToString(), "cursor", cursor != -1 ? cursor.ToString() : null)));

            var result = MakeCursoredList<long>(response);
            foreach (var _id in response["ids"].ToObject<JArray>())
            {
                result.Add(_id.ToObject<long>());
            }

            return result;
        }

        public async Task<Status> CreateFavoriteAsync(Account account, long id)
        {
            return TwitterDataFactory.ParseStatus(JObject.Parse(
                    await PostAsync("https://api.twitter.com/1.1/favorites/create.json", account, MakeQuery("id", id.ToString()))
                ), account.ID);
        }

        public async Task<Status> DestroyFavoriteAsync(Account account, long id)
        {
            return TwitterDataFactory.ParseStatus(JObject.Parse(
                    await PostAsync("https://api.twitter.com/1.1/favorites/destroy.json", account, MakeQuery("id", id.ToString()))
                ), account.ID);
        }

        public async Task<List<Status>> GetFavoritesAsync(Account account, long userId, long count = 200, long sinceId = -1, long maxId = -1)
        {
            var response = JArray.Parse(
                await GetAsync("https://api.twitter.com/1.1/favorites/list.json", account,
                MakeQuery("user_id", userId.ToString(),
                "count", count.ToString(),
                "since_id", sinceId != -1 ? sinceId.ToString() : null,
                "max_id", maxId != -1 ? maxId.ToString() : null)
                ));

            return TwitterDataFactory.ParseArray(response, account.ID, TwitterDataFactory.ParseStatus).ToList();
        }

        public async Task<List<Status>> GetFavoritesAsync(Account account, string userScreenName, long count = 200, long sinceId = -1, long maxId = -1)
        {
            var response = JArray.Parse(
                await GetAsync("https://api.twitter.com/1.1/favorites/list.json", account,
                MakeQuery("screen_name", userScreenName,
                "count", count.ToString(),
                "since_id", sinceId != -1 ? sinceId.ToString() : null,
                "max_id", maxId != -1 ? maxId.ToString() : null)
                ));

            return TwitterDataFactory.ParseArray(response, account.ID, TwitterDataFactory.ParseStatus).ToList();
        }

        public async Task<List<TwitterList>> GetListsAsync(Account account, string screenName, bool reverse = false)
        {
            return TwitterDataFactory.ParseArray(JArray.Parse(
                await GetAsync("https://api.twitter.com/1.1/lists/list.json", account,
                MakeQuery("screen_name", screenName, "reverse", reverse.ToString()))), account.ID,
                TwitterDataFactory.ParseTwitterList).ToList();
        }

        public async Task<List<TwitterList>> GetListsAsync(Account account, long userId, bool reverse = false)
        {
            return TwitterDataFactory.ParseArray(JArray.Parse(
                await GetAsync("https://api.twitter.com/1.1/lists/list.json", account,
                MakeQuery("user_id", userId.ToString(), "reverse", reverse.ToString()))), account.ID,
                TwitterDataFactory.ParseTwitterList).ToList();
        }

        public async Task<CursoredList<User>> GetMemberOfListAsync(Account account, long listId, long count = 20, long cursor = -1)
        {
            var response = JObject.Parse(await GetAsync("https://api.twitter.com/1.1/lists/members.json", account,
                MakeQuery("list_id", listId.ToString(),
                "count", count != 20 ? count.ToString() : null,
                "cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = MakeCursoredList<User>(response);

            result.AddRange(TwitterDataFactory.ParseArray(response["users"].ToObject<JArray>(), account.ID,
                TwitterDataFactory.ParseUser));

            return result;
        }

        public async Task<CursoredList<User>> GetMemberOfListAsync(Account account, string slug, long ownerId, long count = 20, long cursor = -1)
        {
            var response = JObject.Parse(await GetAsync("https://api.twitter.com/1.1/lists/members.json", account,
                MakeQuery("slug", slug, "owner_id", ownerId.ToString(),
                "count", count != 20 ? count.ToString() : null,
                "cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = MakeCursoredList<User>(response);

            result.AddRange(TwitterDataFactory.ParseArray(response["users"].ToObject<JArray>(), account.ID,
                TwitterDataFactory.ParseUser));

            return result;
        }

        public async Task<CursoredList<User>> GetMemberOfListAsync(Account account, string slug, string ownerScreenName, long count = 20, long cursor = -1)
        {
            var response = JObject.Parse(await GetAsync("https://api.twitter.com/1.1/lists/members.json", account,
                MakeQuery("slug", slug, "owner_screen_name", ownerScreenName,
                "count", count != 20 ? count.ToString() : null,
                "cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = MakeCursoredList<User>(response);

            result.AddRange(TwitterDataFactory.ParseArray(response["users"].ToObject<JArray>(), account.ID,
                TwitterDataFactory.ParseUser));

            return result;
        }

        public async Task<User> GetUserFromListAsync(Account account, long userId, long listId)
        {
            return TwitterDataFactory.ParseUser(JObject.Parse(
                await GetAsync("https://api.twitter.com/1.1/lists/members/show.json", account,
                MakeQuery("user_id", userId.ToString(), "list_id", listId.ToString())
                )), account.ID);
        }

        public async Task<User> GetUserFromListAsync(Account account, long userId, string slug, long ownerId)
        {
            return TwitterDataFactory.ParseUser(JObject.Parse(
                await GetAsync("https://api.twitter.com/1.1/lists/members/show.json", account,
                MakeQuery("user_id", userId.ToString(), "slug", slug, "owner_id", ownerId.ToString())
                )), account.ID);
        }

        public async Task<User> GetUserFromListAsync(Account account, long userId, string slug, string ownerScreenName)
        {
            return TwitterDataFactory.ParseUser(JObject.Parse(
                await GetAsync("https://api.twitter.com/1.1/lists/members/show.json", account,
                MakeQuery("user_id", userId.ToString(), "slug", slug, "owner_screen_name", ownerScreenName)
                )), account.ID);
        }

        public async Task<User> GetUserFromListAsync(Account account, string screenName, long listId)
        {
            return TwitterDataFactory.ParseUser(JObject.Parse(
                await GetAsync("https://api.twitter.com/1.1/lists/members/show.json", account,
                MakeQuery("screen_name", screenName, "list_id", listId.ToString())
                )), account.ID);
        }

        public async Task<User> GetUserFromListAsync(Account account, string screenName, string slug, long ownerId)
        {
            return TwitterDataFactory.ParseUser(JObject.Parse(
                await GetAsync("https://api.twitter.com/1.1/lists/members/show.json", account,
                MakeQuery("screen_name", screenName, "slug", slug, "owner_id", ownerId.ToString())
                )), account.ID);
        }

        public async Task<User> GetUserFromListAsync(Account account, string screenName, string slug, string ownerScreenName)
        {
            return TwitterDataFactory.ParseUser(JObject.Parse(
                await GetAsync("https://api.twitter.com/1.1/lists/members/show.json", account,
                MakeQuery("screen_name", screenName, "slug", slug, "owner_screen_name", ownerScreenName)
                )), account.ID);
        }

        public async Task<CursoredList<TwitterList>> GetMembershipsOfUserAsync(Account account, long userId, long count = 20, long cursor = -1, bool filterToOwnedLists = false)
        {
            var response = JObject.Parse(await GetAsync("https://api.twitter.com/1.1/lists/memberships.json", account,
                MakeQuery("user_id", userId.ToString(),
                "count", count != 20 ? count.ToString() : null,
                "cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = MakeCursoredList<TwitterList>(response);

            result.AddRange(TwitterDataFactory.ParseArray(response["lists"].ToObject<JArray>(), account.ID,
                TwitterDataFactory.ParseTwitterList));

            return result;
        }

        public async Task<CursoredList<TwitterList>> GetMembershipsOfUserAsync(Account account, string screenName, long count = 20, long cursor = -1, bool filterToOwnedLists = false)
        {
            var response = JObject.Parse(await GetAsync("https://api.twitter.com/1.1/lists/memberships.json", account,
                MakeQuery("screen_name", screenName,
                "count", count != 20 ? count.ToString() : null,
                "cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = MakeCursoredList<TwitterList>(response);

            result.AddRange(TwitterDataFactory.ParseArray(response["lists"].ToObject<JArray>(), account.ID,
                TwitterDataFactory.ParseTwitterList));

            return result;
        }

        public async Task<CursoredList<TwitterList>> GetOwnershipsOfUserAsync(Account account, long userId, long count = 20, long cursor = -1)
        {
            var response = JObject.Parse(await GetAsync("https://api.twitter.com/1.1/lists/ownerships.json", account,
                MakeQuery("user_id", userId.ToString(),
                "count", count != 20 ? count.ToString() : null,
                "cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = MakeCursoredList<TwitterList>(response);

            result.AddRange(TwitterDataFactory.ParseArray(response["lists"].ToObject<JArray>(), account.ID,
                TwitterDataFactory.ParseTwitterList));

            return result;
        }

        public async Task<CursoredList<TwitterList>> GetOwnershipsOfUserAsync(Account account, string screenName, long count = 20, long cursor = -1)
        {
            var response = JObject.Parse(await GetAsync("https://api.twitter.com/1.1/lists/ownerships.json", account,
                MakeQuery("screen_name", screenName,
                "count", count != 20 ? count.ToString() : null,
                "cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = MakeCursoredList<TwitterList>(response);

            result.AddRange(TwitterDataFactory.ParseArray(response["lists"].ToObject<JArray>(), account.ID,
                TwitterDataFactory.ParseTwitterList));

            return result;
        }

        public async Task<TwitterList> GetListAsync(Account account, long listId)
        {
            return TwitterDataFactory.ParseTwitterList(JObject.Parse(
                await GetAsync("https://api.twitter.com/1.1/lists/show.json", account,
                MakeQuery("list_id", listId.ToString())
                )), account.ID);
        }

        public async Task<TwitterList> GetListAsync(Account account, string slug, long ownerId)
        {
            return TwitterDataFactory.ParseTwitterList(JObject.Parse(
                await GetAsync("https://api.twitter.com/1.1/lists/show.json", account,
                MakeQuery("slug", slug, "owner_id", ownerId.ToString())
                )), account.ID);
        }

        public async Task<TwitterList> GetListAsync(Account account, string slug, string ownerScreenName)
        {
            return TwitterDataFactory.ParseTwitterList(JObject.Parse(
                await GetAsync("https://api.twitter.com/1.1/lists/show.json", account,
                MakeQuery("slug", slug, "owner_screen_name", ownerScreenName)
                )), account.ID);
        }

        public async Task<List<Status>> GetListlineAsync(Account account, long listId, long sinceId, long maxId)
        {
            return TwitterDataFactory.ParseArray(JArray.Parse(
                await GetAsync("https://api.twitter.com/1.1/lists/statuses.json", account,
                MakeQuery("list_id", listId.ToString(),
                "since_id", sinceId.ToString(), "max_id", maxId.ToString()
                ))), account.ID,
                TwitterDataFactory.ParseStatus).ToList();
        }

        public async Task<List<Status>> GetListlineAsync(Account account, string slug, long ownerId, long sinceId, long maxId)
        {
            return TwitterDataFactory.ParseArray(JArray.Parse(
                await GetAsync("https://api.twitter.com/1.1/lists/statuses.json", account,
                MakeQuery("slug", slug, "owner_id", ownerId.ToString(),
                "since_id", sinceId.ToString(), "max_id", maxId.ToString()
                ))), account.ID,
                TwitterDataFactory.ParseStatus).ToList();
        }

        public async Task<List<Status>> GetListlineAsync(Account account, string slug, string ownerScreenName, long sinceId, long maxId)
        {
            return TwitterDataFactory.ParseArray(JArray.Parse(
                await GetAsync("https://api.twitter.com/1.1/lists/statuses.json", account,
                MakeQuery("slug", slug, "owner_screen_name", ownerScreenName,
                "since_id", sinceId.ToString(), "max_id", maxId.ToString()
                ))), account.ID,
                TwitterDataFactory.ParseStatus).ToList();
        }

        public async Task<CursoredList<User>> GetSubScribersFromListAsync(Account account, long listId, long count = 20, long cursor = -1)
        {
            var response = JObject.Parse(
                await GetAsync("https://api.twitter.com/1.1/lists/subscribers.json", account,
                MakeQuery("list_id", listId.ToString(),
                        "count", count != 20 ? count.ToString() : null,
                        "cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = MakeCursoredList<User>(response);

            result.AddRange(TwitterDataFactory.ParseArray(response["users"].ToObject<JArray>(), account.ID, TwitterDataFactory.ParseUser));

            return result;
        }

        public async Task<CursoredList<User>> GetSubScribersFromListAsync(Account account, string slug, long ownerId, long count = 20, long cursor = -1)
        {
            var response = JObject.Parse(
                await GetAsync("https://api.twitter.com/1.1/lists/subscribers.json", account,
                MakeQuery("slug", slug, "owner_id", ownerId.ToString(),
                        "count", count != 20 ? count.ToString() : null,
                        "cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = MakeCursoredList<User>(response);

            result.AddRange(TwitterDataFactory.ParseArray(response["users"].ToObject<JArray>(), account.ID, TwitterDataFactory.ParseUser));

            return result;
        }

        public async Task<CursoredList<User>> GetSubScribersFromListAsync(Account account, string slug, string ownerScreenName, long count = 20, long cursor = -1)
        {
            var response = JObject.Parse(
                await GetAsync("https://api.twitter.com/1.1/lists/subscribers.json", account,
                MakeQuery("slug", slug, "owner_screen_name", ownerScreenName,
                        "count", count != 20 ? count.ToString() : null,
                        "cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = MakeCursoredList<User>(response);

            result.AddRange(TwitterDataFactory.ParseArray(response["users"].ToObject<JArray>(), account.ID, TwitterDataFactory.ParseUser));

            return result;
        }

        public async Task<User> GetSubScriberFromListAsync(Account account, long userId, long listId)
        {
            return TwitterDataFactory.ParseUser(
                JObject.Parse(
                    await GetAsync("https://api.twitter.com/1.1/lists/subscribers/show.json", account,
                    MakeQuery("user_id", userId.ToString(),
                    "list_id", listId.ToString())
                )), account.ID);
        }

        public async Task<User> GetSubScriberFromListAsync(Account account, long userId, string slug, long ownerId)
        {
            return TwitterDataFactory.ParseUser(
                JObject.Parse(
                    await GetAsync("https://api.twitter.com/1.1/lists/subscribers/show.json", account,
                    MakeQuery("user_id", userId.ToString(),
                    "slug", slug, "owner_id", ownerId.ToString())
                )), account.ID);
        }

        public async Task<User> GetSubScriberFromListAsync(Account account, long userId, string slug, string ownerScreenName)
        {
            return TwitterDataFactory.ParseUser(
                JObject.Parse(
                    await GetAsync("https://api.twitter.com/1.1/lists/subscribers/show.json", account,
                    MakeQuery("user_id", userId.ToString(),
                    "slug", slug, "owner_screen_name", ownerScreenName)
                )), account.ID);
        }

        public async Task<User> GetSubScriberFromListAsync(Account account, string screenName, long listId)
        {
            return TwitterDataFactory.ParseUser(
                JObject.Parse(
                    await GetAsync("https://api.twitter.com/1.1/lists/subscribers/show.json", account,
                    MakeQuery("screen_name", screenName,
                    "list_id", listId.ToString())
                )), account.ID);
        }

        public async Task<User> GetSubScriberFromListAsync(Account account, string screenName, string slug, long ownerId)
        {
            return TwitterDataFactory.ParseUser(
                JObject.Parse(
                    await GetAsync("https://api.twitter.com/1.1/lists/subscribers/show.json", account,
                    MakeQuery("screen_name", screenName,
                    "slug", slug, "owner_id", ownerId.ToString())
                )), account.ID);
        }

        public async Task<User> GetSubScriberFromListAsync(Account account, string screenName, string slug, string ownerScreenName)
        {
            return TwitterDataFactory.ParseUser(
                JObject.Parse(
                    await GetAsync("https://api.twitter.com/1.1/lists/subscribers/show.json", account,
                    MakeQuery("screen_name", screenName,
                    "slug", slug, "owner_screen_name", ownerScreenName)
                )), account.ID);
        }

        public async Task<CursoredList<TwitterList>> GetUserSubscriptionsAsync(Account account, long userId, long count = 20, long cursor = -1)
        {
            var response = JObject.Parse(await GetAsync("https://api.twitter.com/1.1/lists/subscriptions.json", account,
                MakeQuery("user_id", userId.ToString(),
                "count", count != 20 ? count.ToString() : null,
                "cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = MakeCursoredList<TwitterList>(response);

            result.AddRange(TwitterDataFactory.ParseArray(response["lists"].ToObject<JArray>(), account.ID,
                TwitterDataFactory.ParseTwitterList));

            return result;
        }

        public async Task<CursoredList<TwitterList>> GetUserSubscriptionsAsync(Account account, string screenName, long count = 20, long cursor = -1)
        {
            var response = JObject.Parse(await GetAsync("https://api.twitter.com/1.1/lists/subscriptions.json", account,
                MakeQuery("screen_name", screenName,
                "count", count != 20 ? count.ToString() : null,
                "cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = MakeCursoredList<TwitterList>(response);

            result.AddRange(TwitterDataFactory.ParseArray(response["lists"].ToObject<JArray>(), account.ID,
                TwitterDataFactory.ParseTwitterList));

            return result;
        }

        public async Task<TwitterList> CreateListAsync(Account account, string name, string mode = "public", string description = "")
        {
            return TwitterDataFactory.ParseTwitterList(JObject.Parse(
                await PostAsync("https://api.twitter.com/1.1/lists/create.json", account,
                MakeQuery("name", name, "mode", mode, "description", description)
                )), account.ID);
        }

        public async Task<TwitterList> UpdateListAsync(Account account, long listId, string name, string mode = "public", string description = "")
        {
            return TwitterDataFactory.ParseTwitterList(JObject.Parse(
                await PostAsync("https://api.twitter.com/1.1/lists/update.json", account,
                MakeQuery("list_id", listId.ToString(),
                    "name", name, "mode", mode, "description", description)
                )), account.ID);
        }

        public async Task<TwitterList> UpdateListAsync(Account account, string slug, long ownerId, string name, string mode = "public", string description = "")
        {
            return TwitterDataFactory.ParseTwitterList(JObject.Parse(
                await PostAsync("https://api.twitter.com/1.1/lists/update.json", account,
                MakeQuery("slug", slug, "owner_id", ownerId.ToString(),
                    "name", name, "mode", mode, "description", description)
                )), account.ID);
        }

        public async Task<TwitterList> UpdateListAsync(Account account, string slug, string ownerScreenName, string name, string mode = "public", string description = "")
        {
            return TwitterDataFactory.ParseTwitterList(JObject.Parse(
                await PostAsync("https://api.twitter.com/1.1/lists/update.json", account,
                MakeQuery("slug", slug, "owner_screen_name", ownerScreenName,
                    "name", name, "mode", mode, "description", description)
                )), account.ID);
        }

        public async Task<TwitterList> DestroyListAsync(Account account, long listId)
        {
            return TwitterDataFactory.ParseTwitterList(JObject.Parse(
                await PostAsync("https://api.twitter.com/1.1/lists/destroy.json", account,
                MakeQuery("list_id", listId.ToString())
                )), account.ID);
        }

        public async Task<TwitterList> DestroyListAsync(Account account, string slug, long ownerId)
        {
            return TwitterDataFactory.ParseTwitterList(JObject.Parse(
                await PostAsync("https://api.twitter.com/1.1/lists/destroy.json", account,
                MakeQuery("slug", slug, "owner_id", ownerId.ToString())
                )), account.ID);
        }

        public async Task<TwitterList> DestroyListAsync(Account account, string slug, string ownerScreenName)
        {
            return TwitterDataFactory.ParseTwitterList(JObject.Parse(
                await PostAsync("https://api.twitter.com/1.1/lists/destroy.json", account,
                MakeQuery("slug", slug, "owner_screen_name", ownerScreenName)
                )), account.ID);
        }

        public async Task AddMemberToUserAsync(Account account, long userId, long listId)
        {
            await PostAsync("https://api.twitter.com/1.1/lists/members/create.json", account,
                MakeQuery("user_id", userId.ToString(),
                "list_id", listId.ToString()));
        }

        public async Task AddMemberToUserAsync(Account account, long userId, string slug, long ownerId)
        {
            await PostAsync("https://api.twitter.com/1.1/lists/members/create.json", account,
                MakeQuery("user_id", userId.ToString(),
                "slug", slug, "owner_id", ownerId.ToString()));
        }

        public async Task AddMemberToUserAsync(Account account, long userId, string slug, string ownerScreenName)
        {
            await PostAsync("https://api.twitter.com/1.1/lists/members/create.json", account,
                MakeQuery("user_id", userId.ToString(),
                "slug", slug, "owner_screen_name", ownerScreenName));
        }

        public async Task AddMemberToUserAsync(Account account, string screenName, long listId)
        {
            await PostAsync("https://api.twitter.com/1.1/lists/members/create.json", account,
                MakeQuery("screen_name", screenName,
                "list_id", listId.ToString()));
        }

        public async Task AddMemberToUserAsync(Account account, string screenName, string slug, long ownerId)
        {
            await PostAsync("https://api.twitter.com/1.1/lists/members/create.json", account,
                MakeQuery("screen_name", screenName,
                "slug", slug, "owner_id", ownerId.ToString()));
        }

        public async Task AddMemberToUserAsync(Account account, string screenName, string slug, string ownerScreenName)
        {
            await PostAsync("https://api.twitter.com/1.1/lists/members/create.json", account,
               MakeQuery("screen_name", screenName,
               "slug", slug, "owner_screen_name", ownerScreenName));
        }

        public async Task AddMembersToUserAsync(Account account, long[] userId, long listId)
        {
            await PostAsync("https://api.twitter.com/1.1/lists/members/create_all.json", account,
                MakeQuery("user_id", string.Join(",", userId),
                "list_id", listId.ToString()));
        }

        public async Task AddMembersToUserAsync(Account account, long[] userId, string slug, long ownerId)
        {
            await PostAsync("https://api.twitter.com/1.1/lists/members/create_all.json", account,
                MakeQuery("user_id", string.Join(",", userId),
                "slug", slug, "owner_id", ownerId.ToString()));
        }

        public async Task AddMembersToUserAsync(Account account, long[] userId, string slug, string ownerScreenName)
        {
            await PostAsync("https://api.twitter.com/1.1/lists/members/create_all.json", account,
                MakeQuery("user_id", string.Join(",", userId),
                "slug", slug, "owner_screen_name", ownerScreenName));
        }

        public async Task AddMembersToUserAsync(Account account, string[] screenName, long listId)
        {
            await PostAsync("https://api.twitter.com/1.1/lists/members/create_all.json", account,
                MakeQuery("screen_name", string.Join(",", screenName),
                "list_id", listId.ToString()));
        }

        public async Task AddMembersToUserAsync(Account account, string[] screenName, string slug, long ownerId)
        {
            await PostAsync("https://api.twitter.com/1.1/lists/members/create_all.json", account,
                MakeQuery("screen_name", string.Join(",", screenName),
                "slug", slug, "owner_id", ownerId.ToString()));
        }

        public async Task AddMembersToUserAsync(Account account, string[] screenName, string slug, string ownerScreenName)
        {
            await PostAsync("https://api.twitter.com/1.1/lists/members/create_all.json", account,
                MakeQuery("screen_name", string.Join(",", screenName),
                "slug", slug, "owner_screen_name", ownerScreenName));
        }

        public async Task RemoveMemberToUserAsync(Account account, long userId, long listId)
        {
            await PostAsync("https://api.twitter.com/1.1/lists/members/destroy.json", account,
                MakeQuery("user_id", userId.ToString(),
                "list_id", listId.ToString()));
        }

        public async Task RemoveMemberToUserAsync(Account account, long userId, string slug, long ownerId)
        {
            await PostAsync("https://api.twitter.com/1.1/lists/members/destroy.json", account,
                MakeQuery("user_id", userId.ToString(),
                "slug", slug, "owner_id", ownerId.ToString()));
        }

        public async Task RemoveMemberToUserAsync(Account account, long userId, string slug, string ownerScreenName)
        {
            await PostAsync("https://api.twitter.com/1.1/lists/members/destroy.json", account,
                MakeQuery("user_id", userId.ToString(),
                "slug", slug, "owner_screen_name", ownerScreenName));
        }

        public async Task RemoveMemberToUserAsync(Account account, string screenName, long listId)
        {
            await PostAsync("https://api.twitter.com/1.1/lists/members/destroy.json", account,
                MakeQuery("screen_name", screenName,
                "list_id", listId.ToString()));
        }

        public async Task RemoveMemberToUserAsync(Account account, string screenName, string slug, long ownerId)
        {
            await PostAsync("https://api.twitter.com/1.1/lists/members/destroy.json", account,
                MakeQuery("screen_name", screenName,
                "slug", slug, "owner_id", ownerId.ToString()));
        }

        public async Task RemoveMemberToUserAsync(Account account, string screenName, string slug, string ownerScreenName)
        {
            await PostAsync("https://api.twitter.com/1.1/lists/members/destroy.json", account,
               MakeQuery("screen_name", screenName,
               "slug", slug, "owner_screen_name", ownerScreenName));
        }

        public async Task RemoveMembersToUserAsync(Account account, long[] userId, long listId)
        {
            await PostAsync("https://api.twitter.com/1.1/lists/members/create_all.json", account,
                MakeQuery("user_id", string.Join(",", userId),
                "list_id", listId.ToString()));
        }

        public async Task RemoveMembersToUserAsync(Account account, long[] userId, string slug, long ownerId)
        {
            await PostAsync("https://api.twitter.com/1.1/lists/members/create_all.json", account,
                MakeQuery("user_id", string.Join(",", userId),
                "slug", slug, "owner_id", ownerId.ToString()));
        }

        public async Task RemoveMembersToUserAsync(Account account, long[] userId, string slug, string ownerScreenName)
        {
            await PostAsync("https://api.twitter.com/1.1/lists/members/create_all.json", account,
                MakeQuery("user_id", string.Join(",", userId),
                "slug", slug, "owner_screen_name", ownerScreenName));
        }

        public async Task RemoveMembersToUserAsync(Account account, string[] screenName, long listId)
        {
            await PostAsync("https://api.twitter.com/1.1/lists/members/create_all.json", account,
                MakeQuery("screen_name", string.Join(",", screenName),
                "list_id", listId.ToString()));
        }

        public async Task RemoveMembersToUserAsync(Account account, string[] screenName, string slug, long ownerId)
        {
            await PostAsync("https://api.twitter.com/1.1/lists/members/create_all.json", account,
                MakeQuery("screen_name", string.Join(",", screenName),
                "slug", slug, "owner_id", ownerId.ToString()));
        }

        public async Task RemoveMembersToUserAsync(Account account, string[] screenName, string slug, string ownerScreenName)
        {
            await PostAsync("https://api.twitter.com/1.1/lists/members/create_all.json", account,
                MakeQuery("screen_name", string.Join(",", screenName),
                "slug", slug, "owner_screen_name", ownerScreenName));
        }

        public async Task SubscribeAccountToListAsync(Account account, long listId)
        {
            await PostAsync("https://api.twitter.com/1.1/lists/subscribers/create.json", account,
                MakeQuery("list_id", listId.ToString()));
        }

        public async Task SubscribeAccountToListAsync(Account account, string slug, long ownerId)
        {
            await PostAsync("https://api.twitter.com/1.1/lists/subscribers/create.json", account,
                MakeQuery("slug", slug, "owner_id", ownerId.ToString()));
        }

        public async Task SubscribeAccountToListAsync(Account account, string slug, string ownerScreenName)
        {
            await PostAsync("https://api.twitter.com/1.1/lists/subscribers/create.json", account,
                MakeQuery("slug", slug, "owner_screen_name", ownerScreenName));
        }

        public async Task UnsubscribeAccountToListAsync(Account account, long listId)
        {
            await PostAsync("https://api.twitter.com/1.1/lists/subscribers/destroy.json", account,
                MakeQuery("list_id", listId.ToString()));
        }

        public async Task UnsubscribeAccountToListAsync(Account account, string slug, long ownerId)
        {
            await PostAsync("https://api.twitter.com/1.1/lists/subscribers/destroy.json", account,
                MakeQuery("slug", slug, "owner_id", ownerId.ToString()));
        }

        public async Task UnsubscribeAccountToListAsync(Account account, string slug, string ownerScreenName)
        {
            await PostAsync("https://api.twitter.com/1.1/lists/subscribers/destroy.json", account,
                MakeQuery("slug", slug, "owner_screen_name", ownerScreenName));
        }

        public async Task<CursoredList<long>> GetFollowerIdsAsync(Account account, long userId, long cursor = -1, long count = 100)
        {
            var response = JObject.Parse(
                await GetAsync("https://api.twitter.com/1.1/followers/ids.json", account,
                MakeQuery("user_id", userId.ToString(),
                "cursor", cursor != -1 ? cursor.ToString() : null,
                "count", count.ToString()
                )));

            var result = MakeCursoredList<long>(response);

            foreach (var id in response["ids"].ToObject<JArray>())
            {
                result.Add(id.ToObject<long>());
            }

            return result;
        }

        public async Task<CursoredList<long>> GetFollowerIdsAsync(Account account, string screenName, long cursor = -1, long count = 100)
        {
            var response = JObject.Parse(
                await GetAsync("https://api.twitter.com/1.1/followers/ids.json", account,
                MakeQuery("screen_name", screenName,
                "cursor", cursor != -1 ? cursor.ToString() : null,
                "count", count.ToString()
                )));

            var result = MakeCursoredList<long>(response);

            foreach (var id in response["ids"].ToObject<JArray>())
            {
                result.Add(id.ToObject<long>());
            }

            return result;
        }

        public async Task<CursoredList<User>> GetFollowersAsync(Account account, long userId, long cursor = -1, long count = 100)
        {
            var response = JObject.Parse(
                await GetAsync("https://api.twitter.com/1.1/followers/list.json", account,
                MakeQuery("user_id", userId.ToString(),
                "cursor", cursor != -1 ? cursor.ToString() : null,
                "count", count.ToString()
                )));

            var result = MakeCursoredList<User>(response);

            result.AddRange(TwitterDataFactory.ParseArray(response["users"].ToObject<JArray>(), account.ID, TwitterDataFactory.ParseUser));

            return result;
        }

        public async Task<CursoredList<User>> GetFollowersAsync(Account account, string screenName, long cursor = -1, long count = 100)
        {
            var response = JObject.Parse(
                await GetAsync("https://api.twitter.com/1.1/followers/list.json", account,
                MakeQuery("screen_name", screenName,
                "cursor", cursor != -1 ? cursor.ToString() : null,
                "count", count.ToString()
                )));

            var result = MakeCursoredList<User>(response);

            result.AddRange(TwitterDataFactory.ParseArray(response["users"].ToObject<JArray>(), account.ID, TwitterDataFactory.ParseUser));

            return result;
        }

        public async Task<CursoredList<long>> GetFriendIdsAsync(Account account, long userId, long cursor = -1, long count = 100)
        {
            var response = JObject.Parse(
                await GetAsync("https://api.twitter.com/1.1/friends/ids.json", account,
                MakeQuery("user_id", userId.ToString(),
                "cursor", cursor != -1 ? cursor.ToString() : null,
                "count", count.ToString()
                )));

            var result = MakeCursoredList<long>(response);

            foreach (var id in response["ids"].ToObject<JArray>())
            {
                result.Add(id.ToObject<long>());
            }

            return result;
        }

        public async Task<CursoredList<long>> GetFriendIdsAsync(Account account, string screenName, long cursor = -1, long count = 100)
        {
            var response = JObject.Parse(
                await GetAsync("https://api.twitter.com/1.1/friends/ids.json", account,
                MakeQuery("screen_name", screenName,
                "cursor", cursor != -1 ? cursor.ToString() : null,
                "count", count.ToString()
                )));

            var result = MakeCursoredList<long>(response);

            foreach (var id in response["ids"].ToObject<JArray>())
            {
                result.Add(id.ToObject<long>());
            }

            return result;
        }

        public async Task<CursoredList<User>> GetFriendsAsync(Account account, long userId, long cursor = -1, long count = 100)
        {
            var response = JObject.Parse(
                await GetAsync("https://api.twitter.com/1.1/friends/list.json", account,
                MakeQuery("user_id", userId.ToString(),
                "cursor", cursor != -1 ? cursor.ToString() : null,
                "count", count.ToString()
                )));

            var result = MakeCursoredList<User>(response);

            result.AddRange(TwitterDataFactory.ParseArray(response["users"].ToObject<JArray>(), account.ID, TwitterDataFactory.ParseUser));

            return result;
        }

        public async Task<CursoredList<User>> GetFriendsAsync(Account account, string screenName, long cursor = -1, long count = 100)
        {
            var response = JObject.Parse(
                await GetAsync("https://api.twitter.com/1.1/friends/list.json", account,
                MakeQuery("screen_name", screenName,
                "cursor", cursor != -1 ? cursor.ToString() : null,
                "count", count.ToString()
                )));

            var result = MakeCursoredList<User>(response);

            result.AddRange(TwitterDataFactory.ParseArray(response["users"].ToObject<JArray>(), account.ID, TwitterDataFactory.ParseUser));

            return result;
        }

        public async Task<CursoredList<long>> GetPendingRequestToAccountAsync(Account account, long cursor = -1)
        {
            var response = JObject.Parse(
                await GetAsync("https://api.twitter.com/1.1/friendships/incoming.json", account,
                MakeQuery("cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = MakeCursoredList<long>(response);

            foreach (var id in response["ids"].ToObject<JArray>())
            {
                result.Add(id.ToObject<long>());
            }

            return result;
        }

        public async Task<CursoredList<long>> GetPendingRequestFromAccountAsync(Account account, long cursor = -1)
        {
            var response = JObject.Parse(
                await GetAsync("https://api.twitter.com/1.1/friendships/outgoing.json", account,
                MakeQuery("cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = MakeCursoredList<long>(response);

            foreach (var id in response["ids"].ToObject<JArray>())
            {
                result.Add(id.ToObject<long>());
            }

            return result;
        }

        public async Task<List<Friendship>> GetFriendshipAsync(Account account, params long[] userId)
        {
            return TwitterDataFactory.ParseArray(JArray.Parse(
                await GetAsync("https://api.twitter.com/1.1/friendships/lookup.json", account,
                MakeQuery("user_id", string.Join(",", userId))
                ))
                , TwitterDataFactory.ParseFriendship).ToList();
        }

        public async Task<List<Friendship>> GetFriendshipAsync(Account account, params string[] screenName)
        {
            return TwitterDataFactory.ParseArray(JArray.Parse(
                await GetAsync("https://api.twitter.com/1.1/friendships/lookup.json", account,
                MakeQuery("screen_name", string.Join(",", screenName))
                ))
                , TwitterDataFactory.ParseFriendship).ToList();
        }

        public async Task<List<long>> GetNoRetweetListOfAccountAsync(Account account)
        {
            var response = JArray.Parse(
                await GetAsync("https://api.twitter.com/1.1/friendships/no_retweets/ids.json", account,
                MakeQuery()
                ));

            var result = new List<long>();

            foreach (var id in response)
            {
                result.Add(id.ToObject<long>());
            }

            return result;
        }

        public async Task<Relationship> GetRelationshipAsync(Account account, long sourceId, long targetId)
        {
            return TwitterDataFactory.ParseRelationship(JObject.Parse(
                await GetAsync("https://api.twitter.com/1.1/friendships/show.json", account,
                MakeQuery("source_id", sourceId.ToString(), "target_id", targetId.ToString())
                )));
        }

        public async Task<Relationship> GetRelationshipAsync(Account account, string sourceScreenName, string targetScreenName)
        {
            return TwitterDataFactory.ParseRelationship(JObject.Parse(
                await GetAsync("https://api.twitter.com/1.1/friendships/show.json", account,
                MakeQuery("source_screen_name", sourceScreenName, "target_screen_name", targetScreenName)
                )));
        }

        public async Task<List<User>> GetUsersAsync(Account account, long[] userIds)
        {
            return TwitterDataFactory.ParseArray(JArray.Parse(
                    await GetAsync("https://api.twitter.com/1.1/users/lookup.json", account,
                    MakeQuery("user_id", string.Join(",", userIds))
                )), account.ID, TwitterDataFactory.ParseUser).ToList();
        }

        public async Task<List<User>> GetUsersAsync(Account account, string[] userScreenNames)
        {
            return TwitterDataFactory.ParseArray(JArray.Parse(
                    await GetAsync("https://api.twitter.com/1.1/users/lookup.json", account,
                    MakeQuery("screen_name", string.Join(",", userScreenNames))
                )), account.ID, TwitterDataFactory.ParseUser).ToList();
        }

        public async Task<User> GetUserAsync(Account account, long userIds)
        {
            return TwitterDataFactory.ParseUser(JObject.Parse(
                await GetAsync("https://api.twitter.com/1.1/users/show.json", account,
                MakeQuery("user_id", userIds.ToString())
                )), account.ID);
        }

        public async Task<User> GetUserAsync(Account account, string userScreenNames)
        {
            return TwitterDataFactory.ParseUser(JObject.Parse(
                await GetAsync("https://api.twitter.com/1.1/users/show.json", account,
                MakeQuery("screen_name", userScreenNames)
                )), account.ID);
        }

        public async Task<List<User>> SearchUsersAsync(Account account, string query, long page, long count = 20)
        {
            return TwitterDataFactory.ParseArray(JArray.Parse(
                    await GetAsync("https://api.twitter.com/1.1/users/search.json", account,
                    MakeQuery("q", query, "page", page.ToString(), "count", count.ToString())
                )), account.ID, TwitterDataFactory.ParseUser).ToList();
        }

        public async Task<User> CreateFriendshipAsync(Account account, long userId)
        {
            return TwitterDataFactory.ParseUser(
                JObject.Parse(
                    await PostAsync("https://api.twitter.com/1.1/friendships/create.json", account, 
                    MakeQuery("user_id", userId.ToString())
                )), account.ID);
        }

        public async Task<User> CreateFriendshipAsync(Account account, string screenName)
        {
            return TwitterDataFactory.ParseUser(
                JObject.Parse(
                    await PostAsync("https://api.twitter.com/1.1/friendships/create.json", account,
                    MakeQuery("screen_name", screenName)
                )), account.ID);
        }

        public async Task<User> UpdateFriendshipAsync(Account account, long userId, bool? enableDeviceNotifications = null, bool? enableRetweet = null)
        {
            return TwitterDataFactory.ParseUser(
                JObject.Parse(
                    await PostAsync("https://api.twitter.com/1.1/friendships/update.json", account,
                    MakeQuery("user_id", userId.ToString(),
                    "device", enableDeviceNotifications?.ToString(),
                    "retweets", enableRetweet?.ToString())
                )), account.ID);
        }

        public async Task<User> UpdateFriendshipAsync(Account account, string screenName, bool? enableDeviceNotifications = null, bool? enableRetweet = null)
        {
            return TwitterDataFactory.ParseUser(
                JObject.Parse(
                    await PostAsync("https://api.twitter.com/1.1/friendships/update.json", account,
                    MakeQuery("screen_name", screenName,
                    "device", enableDeviceNotifications?.ToString(),
                    "retweets", enableRetweet?.ToString())
                )), account.ID);
        }

        public async Task<User> DestroyFriendshipAsync(Account account, long userId)
        {
            return TwitterDataFactory.ParseUser(
                JObject.Parse(
                    await PostAsync("https://api.twitter.com/1.1/friendships/destroy.json", account,
                    MakeQuery("user_id", userId.ToString())
                )), account.ID);
        }

        public async Task<User> DestroyFriendshipAsync(Account account, string screenName)
        {
            return TwitterDataFactory.ParseUser(
                JObject.Parse(
                    await PostAsync("https://api.twitter.com/1.1/friendships/destroy.json", account,
                    MakeQuery("screen_name", screenName)
                )), account.ID);
        }

        public async Task<CursoredList<long>> GetBlockIdsAsync(Account account, long cursor = -1)
        {
            var response = JObject.Parse(
                await GetAsync("https://api.twitter.com/1.1/blocks/ids.json", account,
                MakeQuery("cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = MakeCursoredList<long>(response);

            foreach (var id in response["ids"].ToObject<JArray>())
            {
                result.Add(id.ToObject<long>());
            }

            return result;
        }

        public async Task<CursoredList<User>> GetBlockListAsync(Account account, long cursor = -1)
        {
            var response = JObject.Parse(
                await GetAsync("https://api.twitter.com/1.1/blocks/list.json", account,
                MakeQuery("cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = MakeCursoredList<User>(response);

            result.AddRange(TwitterDataFactory.ParseArray(response["users"].ToObject<JArray>(), account.ID, TwitterDataFactory.ParseUser));

            return result;
        }

        public async Task<CursoredList<long>> GetMuteIdsAsync(Account account, long cursor = -1)
        {
            var response = JObject.Parse(
                await GetAsync("https://api.twitter.com/1.1/mutes/ids.json", account,
                MakeQuery("cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = MakeCursoredList<long>(response);

            foreach (var id in response["ids"].ToObject<JArray>())
            {
                result.Add(id.ToObject<long>());
            }

            return result;
        }

        public async Task<CursoredList<User>> GetMuteUsersAsync(Account account, long cursor = -1)
        {
            var response = JObject.Parse(
                await GetAsync("https://api.twitter.com/1.1/mutes/list.json", account,
                MakeQuery("cursor", cursor != -1 ? cursor.ToString() : null
                )));

            var result = MakeCursoredList<User>(response);

            result.AddRange(TwitterDataFactory.ParseArray(response["users"].ToObject<JArray>(), account.ID, TwitterDataFactory.ParseUser));

            return result;
        }

        public async Task<User> BlockAsync(Account account, long userId)
        {
            return TwitterDataFactory.ParseUser(
                JObject.Parse(
                    await PostAsync("https://api.twitter.com/1.1/blocks/create.json", account,
                    MakeQuery("user_id", userId.ToString())
                )), account.ID);
        }

        public async Task<User> BlockAsync(Account account, string screenName)
        {
            return TwitterDataFactory.ParseUser(
                JObject.Parse(
                    await PostAsync("https://api.twitter.com/1.1/blocks/create.json", account,
                    MakeQuery("screen_name", screenName)
                )), account.ID);
        }

        public async Task<User> UnblockAsync(Account account, long userId)
        {
            return TwitterDataFactory.ParseUser(
                JObject.Parse(
                    await PostAsync("https://api.twitter.com/1.1/blocks/destroy.json", account,
                    MakeQuery("user_id", userId.ToString())
                )), account.ID);
        }

        public async Task<User> UnblockAsync(Account account, string screenName)
        {
            return TwitterDataFactory.ParseUser(
                JObject.Parse(
                    await PostAsync("https://api.twitter.com/1.1/blocks/destroy.json", account,
                    MakeQuery("screen_name", screenName)
                )), account.ID);
        }

        public async Task<User> MuteAsync(Account account, long userId)
        {
            return TwitterDataFactory.ParseUser(
                JObject.Parse(
                    await PostAsync("https://api.twitter.com/1.1/mutes/users/create.json", account,
                    MakeQuery("user_id", userId.ToString())
                )), account.ID);
        }

        public async Task<User> MuteAsync(Account account, string screenName)
        {
            return TwitterDataFactory.ParseUser(
                JObject.Parse(
                    await PostAsync("https://api.twitter.com/1.1/mutes/users/create.json", account,
                    MakeQuery("screen_name", screenName)
                )), account.ID);
        }

        public async Task<User> UnmuteAsync(Account account, long userId)
        {
            return TwitterDataFactory.ParseUser(
                JObject.Parse(
                    await PostAsync("https://api.twitter.com/1.1/mutes/users/destroy.json", account,
                    MakeQuery("user_id", userId.ToString())
                )), account.ID);
        }

        public async Task<User> UnmuteAsync(Account account, string screenName)
        {
            return TwitterDataFactory.ParseUser(
                JObject.Parse(
                    await PostAsync("https://api.twitter.com/1.1/mutes/users/destroy.json", account,
                    MakeQuery("screen_name", screenName)
                )), account.ID);
        }

        public async Task<User> ReportSpamAsync(Account account, long userId, bool performBlock = true)
        {
            return TwitterDataFactory.ParseUser(
                JObject.Parse(
                    await PostAsync("https://api.twitter.com/1.1/users/report_spam.json", account,
                    MakeQuery("user_id", userId.ToString(),
                    "performBlock", performBlock == false ? "false" : null)
                )), account.ID);
        }

        public async Task<User> ReportSpamAsync(Account account, string screenName, bool performBlock = true)
        {
            return TwitterDataFactory.ParseUser(
                JObject.Parse(
                    await PostAsync("https://api.twitter.com/1.1/users/report_spam.json", account,
                    MakeQuery("screen_name", screenName,
                    "performBlock", performBlock == false ? "false" : null)
                )), account.ID);
        }

        public async Task<List<Notification>> GetNotificationsAsync(Account account, int count = 40, long sinceId = -1, long maxId = -1)
        {
            return TwitterDataFactory.ParseArray(
                JArray.Parse(
                    await GetAsync("https://api.twitter.com/1.1/activity/about_me.json", account, 
                    MakeQuery("count", count.ToString(),
                            "since_id", sinceId != -1 ? sinceId.ToString() : null, "max_id", maxId != -1 ? maxId.ToString() : null,
                            "include_user_entities", "1",
                            "model_version", "7",
                            "send_error_codes", "1",
                            "skip_aggregation", "true")
                    )
                    ),
                account.ID, TwitterDataFactory.ParseNotification).ToList();
        }

        public async Task<List<Status>> GetMedialineAsync(Account account, long userId, long count = 200, long sinceId = -1, long maxId = -1)
        {
            return TwitterDataFactory.ParseArray(
                JArray.Parse(
                    await GetAsync("https://api.twitter.com/1.1/statuses/media_timeline.json", account,
                    MakeQuery(
                        "user_id", userId.ToString(),
                        "count", count.ToString(),
                        "since_id", sinceId != -1 ? sinceId.ToString() : null, "max_id", maxId != -1 ? maxId.ToString() : null
                    )
                )), account.ID, TwitterDataFactory.ParseStatus).ToList();
        }

        public async Task<List<Status>> SearchTweetAsync(Account account, string query, bool isRecent, int count = 100, string until = null, long sinceId = -1, long maxId = -1)
        {
            var result = JObject.Parse(
                await GetAsync("https://api.twitter.com/1.1/search/tweets.json", account,
                MakeQuery("q", query, "result_type", isRecent ? "recent" : "popular", "count", count.ToString(),
                "until", until,
                "since_id", sinceId != -1 ? sinceId.ToString() : null, "max_id", maxId != -1 ? maxId.ToString() : null)
                ));

            return TwitterDataFactory.ParseArray(result["statuses"].ToObject<JArray>(), account.ID, TwitterDataFactory.ParseStatus).ToList(); 
        }

        private static User ParseContributeesUser(JObject obj, long issuer)
        {
            return TwitterDataFactory.ParseUser(obj["user"].ToObject<JObject>(), issuer);
        }

        public async Task<List<Account>> GetContributeesAsync(Account account)
        {
            if(!account.IsTweetdeck)
            {
                throw new InvalidOperationException("GetContributees must use tweetdeck account?");
            }

            var users = TwitterDataFactory.ParseArray(
                JArray.Parse(
                    await GetAsync("https://api.twitter.com/1.1/users/contributees.json", account, MakeQuery())
            ), account.ID, ParseContributeesUser);

            var result = new List<Account>();

            var td = account as TDAccount;

            foreach(var user in users)
            {
                var shadow = td.MakeShadowCopy(user.ID);
                shadow.User = user;
                result.Add(shadow);            
            }

            return result;
        }

        public async Task<List<Status>> GetConversationAsync(Account account, long id)
        {
            return TwitterDataFactory.ParseConversation( 
                JObject.Parse(await GetAsync("https://api.twitter.com/2/timeline/conversation/" + id + ".json", account, MakeQuery()))
                , account.ID);
        }
    }
}
