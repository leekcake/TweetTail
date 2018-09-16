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

namespace TwitterLibrary
{
    public class APIImpl : AccountAPI, CollectionAPI
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
            return list.ToArray();
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

        public Account LoadAccount(Stream stream)
        {
            return LibAccount.Load(stream);
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
                )
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
                    )
                );
        }

        public async Task<User> VerifyCredentials(Account account)
        {
            var response = await Get("https://api.twitter.com/1.1/account/verify_credentials.json", account, new KeyValuePair<string, string>[] {

            });

            return TwitterDataFactory.parseUser(JObject.Parse(response));
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
                result.tweet.Add(TwitterDataFactory.parseStatus(json["objects"]["tweets"][cTweet.tweetId.ToString()].ToObject<JObject>()));
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
            foreach(var timeline in results)
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
                makeQuery("name", name, "description", description, "url", url, "order", order != null ? Collection.OrderToString(order.Value) : null )));

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
    }
}
