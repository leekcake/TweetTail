using System;
using System.Collections.Generic;
using System.Text;
using TwitterInterface.Data;
using TwitterInterface.Data.Entity;
using Newtonsoft.Json.Linq;
using System.Globalization;
using TwitterLibrary.Container;

namespace TwitterLibrary
{
    public class TwitterDataFactory
    {
        public const string TwitterDateTemplate = "ddd MMM dd HH:mm:ss +ffff yyyy";
        public const string PollsCardDateTemplate = "yyyy-MM-dd tt h:mm:ss";

        private static DateTime parseTwitterDateTime(string date)
        {
            return DateTime.ParseExact(date, TwitterDateTemplate, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
        }

        private static DateTime parsePollsCardDateTime(string date)
        {
            return DateTime.ParseExact(date, PollsCardDateTemplate, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
        }

        private static string SafeGetString(JObject obj, string key)
        {
            if (obj.ContainsKey(key) && obj[key].ToString() != "")
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
            if (obj.ContainsKey(key) && obj[key].ToString() != "")
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
            if (obj.ContainsKey(key) && obj[key].ToString() != "")
            {
                return obj[key].ToObject<bool>();
            }
            else
            {
                return false;
            }
        }

        private static Status SafeGetStatus(JObject obj, string key, long issuer)
        {
            if (obj.ContainsKey(key) && obj[key].ToString() != "")
            {
                return parseStatus(obj[key].ToObject<JObject>(), issuer);
            }
            else
            {
                return null;
            }
        }

        public static FilterStore<Status> statusFilter = new FilterStore<Status>();
        public static FilterStore<User> userFilter = new FilterStore<User>();

        public delegate T ParseObject<T>(JObject obj);
        public delegate T ParseObjectWithIssuer<T>(JObject obj, long issuer);
        public static T[] parseArray<T>(JArray array, ParseObject<T> parse)
        {
            var result = new List<T>(array.Count);

            for (int i = 0; i < array.Count; i++)
            {
                var data = parse(array[i].ToObject<JObject>());
                if(data != null)
                {
                    result.Add(data);
                }
            }

            return result.ToArray();
        }

        public static T[] parseArray<T>(JArray array, long issuer, ParseObjectWithIssuer<T> parse)
        {
            var result = new List<T>(array.Count);

            for (int i = 0; i < array.Count; i++)
            {
                var data = parse(array[i].ToObject<JObject>(), issuer);
                if (data != null)
                {
                    result.Add(data);
                }
            }

            return result.ToArray();
        }

        public static User parseUser(JObject obj, long issuer)
        {
            var user = new User();
            user.issuer = new List<long> { issuer };

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
            user.language = SafeGetString(obj, "lang");
            //TODO: contributors_enabled
            user.profileBackgroundColor = obj["profile_background_color"].ToString();
            user.profileBackgroundImageURL = obj["profile_background_image_url"].ToString();
            user.profileHttpsBackgroundImageURL = obj["profile_background_image_url_https"].ToString();
            user.profileBackgroundTile = obj["profile_background_tile"].ToObject<bool>();
            user.profileBannerURL = SafeGetString(obj, "profile_banner_url");
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

            return userFilter.ApplyFilter(user);
        }

        public static Status parseStatus(JObject obj, long issuer)
        {
            return parseStatus(obj, issuer, parseUser(obj["user"].ToObject<JObject>(), issuer));
        }

        public static Status parseStatus(JObject obj, long issuer, User creater)
        {
            var status = new Status();

            status.issuer = new List<long> { issuer };
            status.createdAt = parseTwitterDateTime(obj["created_at"].ToString());
            status.id = obj["id"].ToObject<long>();
            status.text = SafeGetString(obj, "full_text");
            if (status.text == null)
            {
                status.text = obj["text"].ToString();
            }
            status.source = obj["source"].ToString();
            status.truncated = obj["truncated"].ToObject<bool>();
            status.replyToStatusId = SafeGetLong(obj, "in_reply_to_status_id");
            status.replyToUserId = SafeGetLong(obj, "in_reply_to_user_id");
            status.replyToScreenName = SafeGetString(obj, "in_reply_to_screen_name");
            status.creater = creater;
            //TODO: coordinates
            //TODO: place
            status.quotedStatusId = SafeGetLong(obj, "quoted_status_id");
            status.isQuote = obj["is_quote_status"].ToObject<bool>();
            status.quotedStatus = SafeGetStatus(obj, "quoted_status", issuer);
            status.retweetedStatus = SafeGetStatus(obj, "retweeted_status", issuer);
            status.retweetCount = obj["retweet_count"].ToObject<int>();
            status.favoriteCount = obj["favorite_count"].ToObject<int>();
            var entities = obj["entities"].ToObject<JObject>();

            status.hashtags = parseArray(entities["hashtags"].ToObject<JArray>(), parseHashTag);
            status.urls = parseArray(entities["urls"].ToObject<JArray>(), parseURL);
            status.userMentions = parseArray(entities["user_mentions"].ToObject<JArray>(), parseUserMention);
            status.symbols = parseArray(entities["symbols"].ToObject<JArray>(), parseSymbol);
            if (entities.ContainsKey("polls"))
            {
                status.polls = parseArray(entities["polls"].ToObject<JArray>(), parsePolls);
            }
            else if(obj.ContainsKey("card"))
            {
                var card = obj["card"];
                var cardName = card["name"].ToString();
                if(cardName.EndsWith("choice_text_only"))
                {
                    var polls = new Polls();

                    var values = card["binding_values"];

                    polls.durationMinutes = values["duration_minutes"]["string_value"].ToObject<int>();
                    polls.endDateTime = parsePollsCardDateTime(values["end_datetime_utc"]["string_value"].ToString());
                    polls.url = values["api"]["string_value"].ToString();

                    var count = 2;
                    if(cardName == "poll3choice_text_only")
                    {
                        count = 3;
                    }
                    else if(cardName == "poll4choice_text_only")
                    {
                        count = 4;
                    }
                    polls.options = new Polls.Option[count];
                    for(int i = 0; i < count; i++)
                    {
                        polls.options[i] = parseCardPollsOption(values.ToObject<JObject>(), i + 1);
                        polls.totalCount += polls.options[i].count;
                    }

                    status.polls = new Polls[] { polls };
                }
            }
            if (obj.ContainsKey("extended_entities"))
            {
                status.extendMedias = parseArray(obj["extended_entities"]["media"].ToObject<JArray>(), parseExtendMedia);
            }
            status.isFavortedByUser = SafeGetBool(obj, "favorited");
            status.isRetweetedByUser = SafeGetBool(obj, "retweeted");
            status.possiblySensitive = SafeGetBool(obj, "possibly_sensitive");

            return statusFilter.ApplyFilter(status);
        }

        public static Polls.Option parseCardPollsOption(JObject binding, int inx)
        {
            return new Polls.Option()
            {
                position = inx,
                count = binding["choice" + inx + "_count"]["string_value"].ToObject<int>(),
                name = binding["choice" + inx + "_label"]["string_value"].ToString()
            };
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
                //animated_gif doesn't have duration_millis data
                info.duration = SafeGetLong(infoObj, "duration_millis");
                var variantArray = infoObj["variants"].ToObject<JArray>();
                info.variants = parseArray(variantArray, parseVideoVariant);

                extendMedia.video = info;
            }

            return extendMedia;
        }

        public static VideoVariant parseVideoVariant(JObject obj)
        {
            var variant = new VideoVariant();
            variant.url = obj["url"].ToString();
            variant.bitrate = (int) SafeGetLong(obj, "bitrate");
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

        public static TwitterList parseTwitterList(JObject obj, long issuer)
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
            twitterList.user = parseUser(obj["user"].ToObject<JObject>(), issuer);

            return twitterList;
        }

        public static Friendship parseFriendship(JObject obj)
        {
            var friendship = new Friendship();

            friendship.id = obj["id"].ToObject<long>();
            friendship.screenName = obj["screen_name"].ToString();
            friendship.name = obj["name"].ToString();

            foreach(var connection in obj["connections"])
            {
                switch(connection.ToString())
                {
                    case "following":
                        friendship.isFollowing = true;
                        break;
                    case "following_requested":
                        friendship.isFollowingRequested = true;
                        break;
                    case "followed_by":
                        friendship.isFollowedBy = true;
                        break;
                    case "blocking":
                        friendship.isBlocking = true;
                        break;
                    case "muting":
                        friendship.isMuting = true;
                        break;
                }
            }

            return friendship;
        }

        public static Relationship parseRelationship(JObject obj)
        {
            var relationship = new Relationship();

            obj = obj["relationship"].ToObject<JObject>();

            var target = obj["target"];
            relationship.targetId = target["id"].ToObject<long>();
            relationship.targetScreen = target["screen_name"].ToString();

            var source = obj["source"];
            relationship.sourceId = source["id"].ToObject<long>();
            relationship.sourceScreen = source["screen_name"].ToString();

            relationship.isCanDM = source["can_dm"].ToObject<bool>();
            relationship.isBlocked = source["blocking"].ToObject<bool>();
            relationship.isMuted = source["muting"].ToObject<bool>();
            //TODO: all_replies
            //TODO: want_retweets
            relationship.isMarkedSpam = source["marked_spam"].ToObject<bool>();
            relationship.isFollowing = source["following"].ToObject<bool>();
            relationship.isFollower = source["followed_by"].ToObject<bool>();
            //TODO:notifications_enabled

            //TODO: Check this value Offical API Only things?
            relationship.isBlockedBy = SafeGetBool(source.ToObject<JObject>(), "blocked_by");

            return relationship;
        }

        public static Notification parseNotification(JObject obj, long issuer)
        {
            switch( obj["action"].ToString() )
            {
                case "retweet":
                    return parseNotification(obj, issuer, new Notification.Retweet(), typeof(User), typeof(Status), typeof(Status));
                case "retweeted_mention":
                    return parseNotification(obj, issuer, new Notification.RetweetedMention(), typeof(User), typeof(Status), typeof(User));
                case "retweeted_retweet":
                    return parseNotification(obj, issuer, new Notification.RetweetedRetweet(), typeof(User), typeof(Status), typeof(User));
                case "favorite":
                    return parseNotification(obj, issuer, new Notification.Favorited(), typeof(User), typeof(Status), null);
                case "favorited_mention":
                    return parseNotification(obj, issuer, new Notification.FavoritedMention(), typeof(User), typeof(Status), typeof(User));
                case "favorited_retweet":
                    return parseNotification(obj, issuer, new Notification.FavoritedRetweet(), typeof(User), typeof(Status), typeof(User));
                case "mention":
                    return parseNotification(obj, issuer, new Notification.Mention(), typeof(User), typeof(User), typeof(Status));
                case "reply":
                    return parseNotification(obj, issuer, new Notification.Reply(), typeof(User), typeof(Status), typeof(Status));
                case "follow":
                    return parseNotification(obj, issuer, new Notification.Follow(), typeof(User), typeof(User), null);
                case "quote":
                    return parseNotification(obj, issuer, new Notification.Quote(), typeof(User), typeof(Status), typeof(Status));
            }
            //TODO: Check Unknown Type?
            return null;
        }

        private static Notification parseNotification(JObject obj, long issuer, Notification notification, Type source, Type target, Type targetObject)
        {
            notification.action = obj["action"].ToString();
            notification.maxPosition = obj["max_position"].ToObject<long>();
            notification.minPosition = obj["min_position"].ToObject<long>();
            notification.createdAt = parseTwitterDateTime(obj["created_at"].ToString());

            notification.sources = parseNotification(obj["sources"].ToObject<JArray>(), source, issuer);
            notification.targets = parseNotification(obj["targets"].ToObject<JArray>(), target, issuer);
            notification.targetObjects = parseNotification(obj["target_objects"].ToObject<JArray>(), targetObject, issuer);
            return notification;          
        }

        private static object[] parseNotification(JArray array, Type type, long issuer)
        {
            if (type == null) return null;

            if(type == typeof(Status))
            {
                return parseArray(array, issuer, parseStatus);
            }
            else if(type == typeof(User))
            {
                return parseArray(array, issuer, parseUser);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private static Status GetStatusFromConversation(long request, long issuer, JObject global, Dictionary<long, Status> statusCache, Dictionary<long, User> userCache)
        {
            if(statusCache.ContainsKey(request))
            {
                return statusCache[request];
            }
            var tweet = global["tweets"][request.ToString()].ToObject<JObject>();
            var result = parseStatus(tweet, issuer, GetUserFromConversation(tweet["user_id"].ToObject<long>(), issuer, global, userCache));
            if(result.isQuote)
            {
                try
                {
                    result.quotedStatus = GetStatusFromConversation(result.quotedStatusId, issuer, global, statusCache, userCache);
                }
                catch
                {
                    //Quoted tweet may doesn't contains quote when...
                    //Quoted[0] <- Quoted[1] <- Quoted[2]
                    //If Conversation API requested for 0,
                    //GetStatusFromConversation on Quoted[1] Can't reach to Quoted[2] because Conversation API doesn't provide it (and doesn't need to deeper)
                }
            }
            statusCache[request] = result;
            return result;   
        }

        private static User GetUserFromConversation(long request, long issuer, JObject global, Dictionary<long, User> cache)
        {
            if (cache.ContainsKey(request))
            {
                return cache[request];
            }

            var result = parseUser(global["users"][request.ToString()].ToObject<JObject>(), issuer);
            cache[request] = result;
            return result;
        }

        public static List<Status> parseConversation(JObject obj, long issuer)
        {
            var result = new List<Status>();

            var global = obj["globalObjects"].ToObject<JObject>();
            var entries = obj["timeline"]["instructions"][0]["addEntries"]["entries"].ToObject<JArray>();

            var statusDict = new Dictionary<long, Status>();
            var userDict = new Dictionary<long, User>();

            foreach(var entity in entries)
            {
                var id = entity["entryId"].ToString();
                id = id.Substring(0, id.IndexOf("-"));
                switch(id)
                {
                    case "tweet":
                        result.Add(GetStatusFromConversation(entity["content"]["item"]["content"]["tweet"]["id"].ToObject<long>(), issuer, global, statusDict, userDict));
                        break;
                    case "conversationThread":
                        var components = entity["content"]["item"]["content"]["conversationThread"]["conversationComponents"].ToObject<JArray>();
                        foreach(var component in components)
                        {
                            try
                            {
                                result.Add(GetStatusFromConversation(component["conversationTweetComponent"]["tweet"]["id"].ToObject<long>(), issuer, global, statusDict, userDict));
                            }
                            catch(Exception e) //Handle for Not conversationTweetComponent object
                            {
                                System.Diagnostics.Debug.WriteLine(e.Message + " " + e.StackTrace);
                            }
                        }
                        break;
                }
            }

            return result;
        }
    }
}
