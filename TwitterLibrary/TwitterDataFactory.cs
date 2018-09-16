using System;
using System.Collections.Generic;
using System.Text;
using TwitterInterface.Data;
using TwitterInterface.Data.Entity;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace TwitterLibrary
{
    class TwitterDataFactory
    {
        public const string TwitterDateTemplate = "ddd MMM dd HH:mm:ss +ffff yyyy";

        private static DateTime parseTwitterDateTime(string date)
        {
            return DateTime.ParseExact(date, TwitterDateTemplate, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
        }

        private static string SafeGetString(JObject obj, string key)
        {
            if (obj.ContainsKey(key))
            {
                return obj[key].ToString();
            }
            else
            {
                return null;
            }
        }

        private static long SafeGetLong(JObject obj, string key)
        {
            if (obj.ContainsKey(key))
            {
                return obj[key].ToObject<long>();
            }
            else
            {
                return -1;
            }
        }

        private static bool SafeGetBool(JObject obj, string key)
        {
            if (obj.ContainsKey(key))
            {
                return obj[key].ToObject<bool>();
            }
            else
            {
                return false;
            }
        }

        private static Status SafeGetStatus(JObject obj, string key)
        {
            if (obj.ContainsKey(key))
            {
                return parseStatus(obj["key"].ToObject<JObject>());
            }
            else
            {
                return null;
            }
        }

        public delegate T ParseObject<T>(JObject obj);
        public static T[] parseArray<T>(JArray array, ParseObject<T> parse)
        {
            var result = new T[array.Count];

            for (int i = 0; i < array.Count; i++)
            {
                result[i] = parse(array[i].ToObject<JObject>());
            }

            return result;
        }

        public static User parseUser(JObject obj)
        {
            var user = new User();

            user.id = obj["id"].ToObject<long>();
            user.nickName = obj["name"].ToString();
            user.screenName = obj["screen_name"].ToString();
            user.location = SafeGetString(obj, "location");
            user.url = SafeGetString(obj, "url");
            user.description = SafeGetString(obj, "description");
            //TOOD: derived
            user.isProtected = obj["protected"].ToObject<bool>();
            user.isVerified = obj["verified"].ToObject<bool>();
            user.followerCount = obj["followers_count"].ToObject<long>();
            user.followingCount = obj["friends_count"].ToObject<long>();
            user.listedCount = obj["listed_count"].ToObject<long>();
            user.favouritesCount = obj["favourites_count"].ToObject<long>();
            user.statusesCount = obj["statuses_count"].ToObject<long>();
            user.createdAt = parseTwitterDateTime(obj["created_at"].ToString());
            user.geoEnabled = obj["geo_enabled"].ToObject<bool>();
            user.language = obj["lang"].ToString();
            //TODO: contributors_enabled
            user.profileBackgroundColor = obj["profile_background_color"].ToString();
            user.profileBackgroundImageURL = obj["profile_background_image_url"].ToString();
            user.profileHttpsBackgroundImageURL = obj["profile_background_image_url_https"].ToString();
            user.profileBackgroundTile = obj["profile_background_tile"].ToObject<bool>();
            user.profileBannerURL = obj["profile_banner_url"].ToString();
            user.profileImageURL = obj["profile_image_url"].ToString();
            user.profileHttpsImageURL = obj["profile_image_url_https"].ToString();
            //TODO: profile_link_color
            //TODO: profile_sidebar_border_color
            //TODO: profile_sidebar_fill_color
            //TODO: profile_text_color
            //TODO: profile_use_background_image
            //TODO: default_profile
            //TODO: default_profile_image
            //TODO: withheld_in_countries
            //TODO: withheld_scope

            return user;
        }

        public static Status parseStatus(JObject obj)
        {
            var status = new Status();

            status.createdAt = parseTwitterDateTime(obj["created_at"].ToString());
            status.id = obj["id"].ToObject<long>();
            status.text = obj["text"].ToString();
            status.source = obj["source"].ToString();
            status.truncated = obj["truncated"].ToObject<bool>();
            status.replyToStatusId = SafeGetLong(obj, "in_reply_to_status_id");
            status.replyToUserId = SafeGetLong(obj, "in_reply_to_user_id");
            status.replyToScreenName = SafeGetString(obj, "in_reply_to_screen_name");
            status.creater = parseUser(obj["user"].ToObject<JObject>());
            //TODO: coordinates
            //TODO: place
            status.quotedStatusId = SafeGetLong(obj, "quoted_status_id");
            status.isQuote = obj["is_quote_status"].ToObject<bool>();
            status.quotedStatus = SafeGetStatus(obj, "quoted_status");
            status.retweetedStatus = SafeGetStatus(obj, "retweeted_status");
            status.retweetCount = obj["retweet_count"].ToObject<int>();
            status.favoriteCount = obj["favorite_count"].ToObject<int>();
            var entities = obj["entities"].ToObject<JObject>();

            status.hashtags = parseArray(entities["hashtags"].ToObject<JArray>(), parseHashTag);
            status.urls = parseArray(entities["urls"].ToObject<JArray>(), parseURL);
            status.userMentions = parseArray(entities["user_mentions"].ToObject<JArray>(), parseUserMention);
            status.symbols = parseArray(entities["symbols"].ToObject<JArray>(), parseSymbol);
            status.polls = parseArray(entities["polls"].ToObject<JArray>(), parsePolls);
            status.extendMedias = parseArray(obj["extented_entities"]["media"].ToObject<JArray>(), parseExtendMedia);

            status.isFavortedByUser = SafeGetBool(obj, "favorited");
            status.isRetweetedByUser = obj["retweeted"].ToObject<bool>();
            status.possiblySensitive = SafeGetBool(obj, "possibly_sensitive");

            return status;
        }

        public static Indices parseIndices(JArray obj)
        {
            var indices = new Indices();

            indices.start = obj[0].ToObject<int>();
            indices.end = obj[1].ToObject<int>();

            return indices;
        }

        public static HashTag parseHashTag(JObject obj)
        {
            var hashtag = new HashTag();
            hashtag.indices = parseIndices(obj["indices"].ToObject<JArray>());
            hashtag.text = obj["text"].ToString();

            return hashtag;
        }

        public static URL parseURL(JObject obj)
        {
            var url = new URL();
            url.indices = parseIndices(obj["indices"].ToObject<JArray>());
            url.url = obj["url"].ToString();
            url.displayURL = obj["display_url"].ToString();
            url.expandedURL = obj["expanded_url"].ToString();

            return url;
        }

        public static UserMention parseUserMention(JObject obj)
        {
            var userMention = new UserMention();

            userMention.indices = parseIndices(obj["indices"].ToObject<JArray>());
            userMention.name = obj["name"].ToString();
            userMention.screenNane = obj["screen_name"].ToString();
            userMention.id = obj["id"].ToObject<long>();

            return userMention;
        }

        public static Symbol parseSymbol(JObject obj)
        {
            var symbol = new Symbol();
            symbol.indices = parseIndices(obj["indices"].ToObject<JArray>());
            symbol.text = obj["text"].ToString();

            return symbol;
        }

        public static Polls parsePolls(JObject obj)
        {
            var polls = new Polls();

            polls.endDateTime = parseTwitterDateTime(obj["end_datetime"].ToString());
            polls.durationMinutes = obj["duration_minutes"].ToObject<int>();

            var options = obj["options"].ToObject<JArray>();
            polls.options = parseArray(options, parsePollsOption);

            return polls;
        }

        public static Polls.Option parsePollsOption(JObject obj)
        {
            var option = new Polls.Option();

            option.position = obj["position"].ToObject<int>();
            option.name = obj["text"].ToString();

            return option;
        }

        public static ExtendMedia parseExtendMedia(JObject obj)
        {
            var extendMedia = new ExtendMedia();

            extendMedia.id = obj["id"].ToObject<long>();
            extendMedia.indices = parseIndices(obj["indices"].ToObject<JArray>());
            extendMedia.mediaURL = obj["media_url"].ToString();
            extendMedia.mediaURLHttps = obj["media_url_https"].ToString();
            extendMedia.url = parseURL(obj);
            extendMedia.type = obj["type"].ToString();
            if (obj.ContainsKey("video_info"))
            {
                var infoObj = obj["video_info"].ToObject<JObject>();
                var info = new VideoInformation();
                info.aspectRatio = parseIndices( infoObj["aspect_ratio"].ToObject<JArray>() );
                info.duration = infoObj["duration_millis"].ToObject<int>();

                var variantArray = obj["variants"].ToObject<JArray>();
                info.variants = parseArray(variantArray, parseVideoVariant);
            }

            return extendMedia;
        }

        public static VideoVariant parseVideoVariant(JObject obj)
        {
            var variant = new VideoVariant();
            variant.url = obj["url"].ToString();
            variant.bitrate = obj["bitrate"].ToObject<int>();
            variant.contentType = obj["content_type"].ToString();

            return variant;
        }

        public static SavedSearch parseSavedSearch(JObject obj)
        {
            var savedSearch = new SavedSearch();

            savedSearch.createdAt = parseTwitterDateTime(obj["created_at"].ToString());
            savedSearch.id = obj["id"].ToObject<long>();
            savedSearch.name = obj["name"].ToString();
            savedSearch.query = obj["query"].ToString();

            return savedSearch;
        }

        public static Collection parseCollection(JObject obj)
        {
            var collection = new Collection();

            collection.type = obj["collection_type"].ToString();
            collection.url = obj["collection_url"].ToString();
            collection.description = obj["description"].ToString();
            collection.name = obj["name"].ToString();
            var order = obj["timeline_order"].ToString();
            if (order == "curation_reverse_chron")
            {
                collection.timelineOrder = Collection.Order.AddTime;
            }
            else if (order == "tweet_chron")
            {
                collection.timelineOrder = Collection.Order.Oldest;
            }
            else
            {
                collection.timelineOrder = Collection.Order.Newest;
            }
            collection.userId = obj["user_id"].ToObject<long>();
            collection.isPrivate = obj["visibility"].ToString() == "private";
            
            return collection;
        }

        public static Collection.CollectionTweet parseCollectionTweet(JObject obj)
        {
            var collectionTweet = new Collection.CollectionTweet();
            collectionTweet.featureContext = obj["feature_context"].ToString();
            var tweet = obj["tweet"];
            collectionTweet.tweetId = tweet["id"].ToObject<long>();
            collectionTweet.tweetSortIndex = tweet["sort_index"].ToObject<long>();

            return collectionTweet;
        }

        public static TwitterList parseTwitterList(JObject obj)
        {
            var twitterList = new TwitterList();

            twitterList.slug = obj["slug"].ToString();
            twitterList.name = obj["name"].ToString();
            twitterList.createdAt = parseTwitterDateTime(obj["created_at"].ToString());
            twitterList.url = obj["url"].ToString();
            twitterList.subscriberCount = obj["subscriber_count"].ToObject<long>();
            twitterList.memberCount = obj["member_count"].ToObject<long>();
            twitterList.mode = obj["mode"].ToString();
            twitterList.id = obj["id"].ToObject<long>();
            twitterList.fullName = obj["full_name"].ToString();
            twitterList.description = obj["description"].ToString();
            twitterList.user = parseUser(obj["user"].ToObject<JObject>());

            return twitterList;
        }
    }
}
