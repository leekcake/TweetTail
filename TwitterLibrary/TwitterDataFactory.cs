using System;
using System.Collections.Generic;
using System.Text;
using TwitterInterface.Data;
using TwitterInterface.Data.Entity;
using Newtonsoft.Json.Linq;
using System.Globalization;
using TwitterInterface.Container;

namespace TwitterLibrary
{
    public class TwitterDataFactory
    {
        public const string TwitterDateTemplate = "ddd MMM dd HH:mm:ss +ffff yyyy";
        public const string PollsCardDateTemplate = "yyyy-MM-dd tt h:mm:ss";

        private DateTime ParseTwitterDateTime(string date)
        {
            return DateTime.ParseExact(date, TwitterDateTemplate, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
        }

        private DateTime ParsePollsCardDateTime(string date)
        {
            try
            {
                return DateTime.ParseExact(date, PollsCardDateTemplate, CultureInfo.CurrentCulture, DateTimeStyles.AdjustToUniversal);
            }
            catch
            {
                return DateTime.ParseExact(date, PollsCardDateTemplate, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            }
        }

        private string SafeGetString(JObject obj, string key)
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

        private long SafeGetLong(JObject obj, string key)
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

        private bool SafeGetBool(JObject obj, string key)
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

        private Status SafeGetStatus(JObject obj, string key, long issuer)
        {
            if (obj.ContainsKey(key) && obj[key].ToString() != "")
            {
                return ParseStatus(obj[key].ToObject<JObject>(), issuer);
            }
            else
            {
                return null;
            }
        }

        public FilterStore<Status> StatusFilter = new FilterStore<Status>();
        public FilterStore<User> UserFilter = new FilterStore<User>();

        public delegate T ParseObject<T>(JObject obj);
        public delegate T ParseObjectWithIssuer<T>(JObject obj, long issuer);
        public T[] ParseArray<T>(JArray array, ParseObject<T> parse)
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

        public T[] ParseArray<T>(JArray array, long issuer, ParseObjectWithIssuer<T> parse)
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

        public User ParseUser(JObject obj, long issuer)
        {
            return ParseUser(obj, issuer, true);
        }

        public User ParseUser(JObject obj, long issuer, bool useFilter)
        {
            var user = new User
            {
                Issuer = new List<long> { issuer },

                ID = obj["id"].ToObject<long>(),
                NickName = obj["name"].ToString(),
                ScreenName = obj["screen_name"].ToString(),
                Location = SafeGetString(obj, "location"),
                URL = SafeGetString(obj, "url"),
                Description = SafeGetString(obj, "description"),

                //TOOD: derived
                IsProtected = obj["protected"].ToObject<bool>(),
                IsVerified = obj["verified"].ToObject<bool>(),
                FollowerCount = obj["followers_count"].ToObject<long>(),
                FollowingCount = obj["friends_count"].ToObject<long>(),
                ListedCount = obj["listed_count"].ToObject<long>(),
                FavouritesCount = obj["favourites_count"].ToObject<long>(),
                StatusesCount = obj["statuses_count"].ToObject<long>(),
                CreatedAt = ParseTwitterDateTime(obj["created_at"].ToString()),
                GeoEnabled = obj["geo_enabled"].ToObject<bool>(),
                Language = SafeGetString(obj, "lang"),
                //TODO: contributors_enabled
                ProfileBackgroundColor = obj["profile_background_color"].ToString(),
                ProfileBackgroundImageURL = obj["profile_background_image_url"].ToString(),
                ProfileHttpsBackgroundImageURL = obj["profile_background_image_url_https"].ToString(),
                ProfileBackgroundTile = obj["profile_background_tile"].ToObject<bool>(),
                ProfileBannerURL = SafeGetString(obj, "profile_banner_url"),
                ProfileImageURL = obj["profile_image_url"].ToString(),
                ProfileHttpsImageURL = obj["profile_image_url_https"].ToString()
            };
            //TODO: profile_link_color
            //TODO: profile_sidebar_border_color
            //TODO: profile_sidebar_fill_color
            //TODO: profile_text_color
            //TODO: profile_use_background_image
            //TODO: default_profile
            //TODO: default_profile_image
            //TODO: withheld_in_countries
            //TODO: withheld_scope

            try
            {
                user.descriptionEntities = new BasicEntitiesGroup();

                var entities = obj["entities"].ToObject<JObject>();

                ParseBasicEntitesGroup(user.descriptionEntities, entities["description"].ToObject<JObject>());

                var urls = entities["url"]?["urls"];
                if(urls != null)
                    user.URLEntities = ParseArray(urls.ToObject<JArray>(), ParseURL);
            }
            catch
            {

            }

            if(!useFilter)
            {
                return user;
            }

            return UserFilter.ApplyFilter(user);
        }

        public Status ParseStatus(JObject obj, long issuer)
        {
            return ParseStatus(obj, issuer, ParseUser(obj["user"].ToObject<JObject>(), issuer, false));
        }

        private void ParseBasicEntitesGroup(BasicEntitiesGroup into, JObject entities)
        {
            if(entities.ContainsKey("hashtags"))
                into.Hashtags = ParseArray(entities["hashtags"].ToObject<JArray>(), ParseHashTag);

            if (entities.ContainsKey("urls"))
                into.URLs = ParseArray(entities["urls"].ToObject<JArray>(), ParseURL);

            if (entities.ContainsKey("user_mentions"))
                into.UserMentions = ParseArray(entities["user_mentions"].ToObject<JArray>(), ParseUserMention);

            if (entities.ContainsKey("symbols"))
                into.Symbols = ParseArray(entities["symbols"].ToObject<JArray>(), ParseSymbol);
        }

        public Status ParseStatus(JObject obj, long issuer, User creater)
        {
            var status = new Status
            {
                Issuer = new List<long> { issuer },
                CreatedAt = ParseTwitterDateTime(obj["created_at"].ToString()),
                ID = obj["id"].ToObject<long>(),
                Text = SafeGetString(obj, "full_text"),
                Source = obj["source"].ToString(),
                Truncated = obj["truncated"].ToObject<bool>(),
                ReplyToStatusId = SafeGetLong(obj, "in_reply_to_status_id"),
                ReplyToUserId = SafeGetLong(obj, "in_reply_to_user_id"),
                ReplyToScreenName = SafeGetString(obj, "in_reply_to_screen_name"),
                Creater = creater,
                //TODO: coordinates
                //TODO: place
                QuotedStatusId = SafeGetLong(obj, "quoted_status_id"),
                IsQuote = obj["is_quote_status"].ToObject<bool>(),
                QuotedStatus = SafeGetStatus(obj, "quoted_status", issuer),
                RetweetedStatus = SafeGetStatus(obj, "retweeted_status", issuer),
                RetweetCount = obj["retweet_count"].ToObject<int>(),
                FavoriteCount = obj["favorite_count"].ToObject<int>()
            };
            var entities = obj["entities"].ToObject<JObject>();

            if (status.Text == null)
            {
                status.Text = obj["text"].ToString();
            }
            

            ParseBasicEntitesGroup(status, entities);
            if (entities.ContainsKey("polls"))
            {
                status.Polls = ParseArray(entities["polls"].ToObject<JArray>(), ParsePolls);
            }
            else if(obj.ContainsKey("card"))
            {
                var card = obj["card"];
                var cardName = card["name"].ToString();
                if(cardName.EndsWith("choice_text_only"))
                {
                    var polls = new Polls();

                    var values = card["binding_values"];

                    polls.DurationMinutes = values["duration_minutes"]["string_value"].ToObject<int>();
                    polls.EndDateTime = ParsePollsCardDateTime(values["end_datetime_utc"]["string_value"].ToString());
                    polls.URL = values["api"]["string_value"].ToString();

                    var count = 2;
                    if(cardName == "poll3choice_text_only")
                    {
                        count = 3;
                    }
                    else if(cardName == "poll4choice_text_only")
                    {
                        count = 4;
                    }
                    polls.Options = new Polls.Option[count];
                    for(int i = 0; i < count; i++)
                    {
                        polls.Options[i] = ParseCardPollsOption(values.ToObject<JObject>(), i + 1);
                        polls.TotalCount += polls.Options[i].Count;
                    }

                    status.Polls = new Polls[] { polls };
                }
            }
            if (obj.ContainsKey("extended_entities"))
            {
                status.ExtendMedias = ParseArray(obj["extended_entities"]["media"].ToObject<JArray>(), ParseExtendMedia);
            }
            status.IsFavortedByUser = SafeGetBool(obj, "favorited");
            status.IsRetweetedByUser = SafeGetBool(obj, "retweeted");
            status.PossiblySensitive = SafeGetBool(obj, "possibly_sensitive");

            return StatusFilter.ApplyFilter(status);
        }

        public Polls.Option ParseCardPollsOption(JObject binding, int inx)
        {
            return new Polls.Option()
            {
                Position = inx,
                Count = binding["choice" + inx + "_count"]["string_value"].ToObject<int>(),
                Name = binding["choice" + inx + "_label"]["string_value"].ToString()
            };
        }

        public Indices ParseIndices(JArray obj)
        {
            var indices = new Indices
            {
                Start = obj[0].ToObject<int>(),
                End = obj[1].ToObject<int>()
            };

            return indices;
        }

        public HashTag ParseHashTag(JObject obj)
        {
            var hashtag = new HashTag
            {
                Indices = ParseIndices(obj["indices"].ToObject<JArray>()),
                Text = obj["text"].ToString()
            };

            return hashtag;
        }

        public URL ParseURL(JObject obj)
        {
            var url = new URL
            {
                Indices = ParseIndices(obj["indices"].ToObject<JArray>()),
                RawURL = obj["url"].ToString(),
                DisplayURL = obj["display_url"]?.ToString(),
                ExpandedURL = obj["expanded_url"]?.ToString()
            };

            if(url.DisplayURL == null)
            {
                url.DisplayURL = url.RawURL;
            }

            return url;
        }

        public UserMention ParseUserMention(JObject obj)
        {
            var userMention = new UserMention
            {
                Indices = ParseIndices(obj["indices"].ToObject<JArray>()),
                Name = obj["name"].ToString(),
                ScreenNane = obj["screen_name"].ToString(),
                ID = obj["id"].ToObject<long>()
            };

            return userMention;
        }

        public Symbol ParseSymbol(JObject obj)
        {
            var symbol = new Symbol
            {
                Indices = ParseIndices(obj["indices"].ToObject<JArray>()),
                Text = obj["text"].ToString()
            };

            return symbol;
        }

        public Polls ParsePolls(JObject obj)
        {
            var polls = new Polls
            {
                EndDateTime = ParseTwitterDateTime(obj["end_datetime"].ToString()),
                DurationMinutes = obj["duration_minutes"].ToObject<int>()
            };

            var options = obj["options"].ToObject<JArray>();
            polls.Options = ParseArray(options, ParsePollsOption);

            return polls;
        }

        public Polls.Option ParsePollsOption(JObject obj)
        {
            var option = new Polls.Option
            {
                Position = obj["position"].ToObject<int>(),
                Name = obj["text"].ToString()
            };

            return option;
        }

        public ExtendMedia ParseExtendMedia(JObject obj)
        {
            var extendMedia = new ExtendMedia
            {
                ID = obj["id"].ToObject<long>(),
                Indices = ParseIndices(obj["indices"].ToObject<JArray>()),
                MediaURL = obj["media_url"].ToString(),
                MediaURLHttps = obj["media_url_https"].ToString(),
                URL = ParseURL(obj),
                Type = obj["type"].ToString()
            };
            if (obj.ContainsKey("video_info"))
            {
                var infoObj = obj["video_info"].ToObject<JObject>();
                var info = new VideoInformation
                {
                    AspectRatio = ParseIndices(infoObj["aspect_ratio"].ToObject<JArray>()),
                    //animated_gif doesn't have duration_millis data
                    Duration = SafeGetLong(infoObj, "duration_millis")
                };
                var variantArray = infoObj["variants"].ToObject<JArray>();
                info.Variants = ParseArray(variantArray, ParseVideoVariant);

                extendMedia.Video = info;
            }

            return extendMedia;
        }

        public VideoVariant ParseVideoVariant(JObject obj)
        {
            var variant = new VideoVariant
            {
                URL = obj["url"].ToString(),
                Bitrate = (int)SafeGetLong(obj, "bitrate"),
                ContentType = obj["content_type"].ToString()
            };

            return variant;
        }

        public SavedSearch ParseSavedSearch(JObject obj)
        {
            var savedSearch = new SavedSearch
            {
                CreatedAt = ParseTwitterDateTime(obj["created_at"].ToString()),
                ID = obj["id"].ToObject<long>(),
                Name = obj["name"].ToString(),
                Query = obj["query"].ToString()
            };

            return savedSearch;
        }

        public Collection ParseCollection(JObject obj)
        {
            var collection = new Collection
            {
                Type = obj["collection_type"].ToString(),
                URL = obj["collection_url"].ToString(),
                Description = obj["description"].ToString(),
                Name = obj["name"].ToString()
            };
            var order = obj["timeline_order"].ToString();
            if (order == "curation_reverse_chron")
            {
                collection.TimelineOrder = Collection.Order.AddTime;
            }
            else if (order == "tweet_chron")
            {
                collection.TimelineOrder = Collection.Order.Oldest;
            }
            else
            {
                collection.TimelineOrder = Collection.Order.Newest;
            }
            collection.UserId = obj["user_id"].ToObject<long>();
            collection.IsPrivate = obj["visibility"].ToString() == "private";
            
            return collection;
        }

        public Collection.CollectionTweet ParseCollectionTweet(JObject obj)
        {
            var collectionTweet = new Collection.CollectionTweet
            {
                FeatureContext = obj["feature_context"].ToString()
            };
            var tweet = obj["tweet"];
            collectionTweet.TweetId = tweet["id"].ToObject<long>();
            collectionTweet.TweetSortIndex = tweet["sort_index"].ToObject<long>();

            return collectionTweet;
        }

        public TwitterList ParseTwitterList(JObject obj, long issuer)
        {
            var twitterList = new TwitterList
            {
                Slug = obj["slug"].ToString(),
                Name = obj["name"].ToString(),
                CreatedAt = ParseTwitterDateTime(obj["created_at"].ToString()),
                URL = obj["url"].ToString(),
                SubscriberCount = obj["subscriber_count"].ToObject<long>(),
                MemberCount = obj["member_count"].ToObject<long>(),
                Mode = obj["mode"].ToString(),
                ID = obj["id"].ToObject<long>(),
                FullName = obj["full_name"].ToString(),
                Description = obj["description"].ToString(),
                User = ParseUser(obj["user"].ToObject<JObject>(), issuer)
            };

            return twitterList;
        }

        public Friendship ParseFriendship(JObject obj)
        {
            var friendship = new Friendship
            {
                ID = obj["id"].ToObject<long>(),
                ScreenName = obj["screen_name"].ToString(),
                Name = obj["name"].ToString()
            };

            foreach (var connection in obj["connections"])
            {
                switch(connection.ToString())
                {
                    case "following":
                        friendship.IsFollowing = true;
                        break;
                    case "following_requested":
                        friendship.IsFollowingRequested = true;
                        break;
                    case "followed_by":
                        friendship.IsFollowedBy = true;
                        break;
                    case "blocking":
                        friendship.IsBlocking = true;
                        break;
                    case "muting":
                        friendship.IsMuting = true;
                        break;
                }
            }

            return friendship;
        }

        public Relationship ParseRelationship(JObject obj)
        {
            var relationship = new Relationship();

            obj = obj["relationship"].ToObject<JObject>();

            var target = obj["target"];
            relationship.TargetId = target["id"].ToObject<long>();
            relationship.TargetScreen = target["screen_name"].ToString();

            var source = obj["source"];
            relationship.SourceId = source["id"].ToObject<long>();
            relationship.SourceScreen = source["screen_name"].ToString();

            relationship.IsCanDM = source["can_dm"].ToObject<bool>();
            relationship.IsBlocked = source["blocking"].ToObject<bool>();
            relationship.IsMuted = source["muting"].ToObject<bool>();
            //TODO: all_replies
            //TODO: want_retweets
            relationship.IsMarkedSpam = source["marked_spam"].ToObject<bool>();
            relationship.IsFollowing = source["following"].ToObject<bool>();
            relationship.IsFollower = source["followed_by"].ToObject<bool>();
            //TODO:notifications_enabled

            //TODO: Check this value Offical API Only things?
            relationship.IsBlockedBy = SafeGetBool(source.ToObject<JObject>(), "blocked_by");

            return relationship;
        }

        public Notification ParseNotification(JObject obj, long issuer)
        {
            switch( obj["action"].ToString() )
            {
                case "retweet":
                    return ParseNotification(obj, issuer, new Notification.Retweet(), typeof(User), typeof(Status), typeof(Status));
                case "retweeted_mention":
                    return ParseNotification(obj, issuer, new Notification.RetweetedMention(), typeof(User), typeof(Status), typeof(User));
                case "retweeted_retweet":
                    return ParseNotification(obj, issuer, new Notification.RetweetedRetweet(), typeof(User), typeof(Status), typeof(User));
                case "favorite":
                    return ParseNotification(obj, issuer, new Notification.Favorited(), typeof(User), typeof(Status), null);
                case "favorited_mention":
                    return ParseNotification(obj, issuer, new Notification.FavoritedMention(), typeof(User), typeof(Status), typeof(User));
                case "favorited_retweet":
                    return ParseNotification(obj, issuer, new Notification.FavoritedRetweet(), typeof(User), typeof(Status), typeof(User));
                case "mention":
                    return ParseNotification(obj, issuer, new Notification.Mention(), typeof(User), typeof(User), typeof(Status));
                case "reply":
                    return ParseNotification(obj, issuer, new Notification.Reply(), typeof(User), typeof(Status), typeof(Status));
                case "follow":
                    return ParseNotification(obj, issuer, new Notification.Follow(), typeof(User), typeof(User), null);
                case "quote":
                    return ParseNotification(obj, issuer, new Notification.Quote(), typeof(User), typeof(Status), typeof(Status));
            }
            //TODO: Check Unknown Type?
            return null;
        }

        private Notification ParseNotification(JObject obj, long issuer, Notification notification, Type source, Type target, Type targetObject)
        {
            notification.Action = obj["action"].ToString();
            notification.MaxPosition = obj["max_position"].ToObject<long>();
            notification.MinPosition = obj["min_position"].ToObject<long>();
            notification.CreatedAt = ParseTwitterDateTime(obj["created_at"].ToString());

            notification.Sources = ParseNotification(obj["sources"].ToObject<JArray>(), source, issuer);
            notification.Targets = ParseNotification(obj["targets"].ToObject<JArray>(), target, issuer);
            notification.TargetObjects = ParseNotification(obj["target_objects"].ToObject<JArray>(), targetObject, issuer);
            return notification;          
        }

        private object[] ParseNotification(JArray array, Type type, long issuer)
        {
            if (type == null) return null;

            if(type == typeof(Status))
            {
                return ParseArray(array, issuer, ParseStatus);
            }
            else if(type == typeof(User))
            {
                return ParseArray(array, issuer, ParseUser);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private Status GetStatusFromConversation(long request, long issuer, JObject global, Dictionary<long, Status> statusCache, Dictionary<long, User> userCache)
        {
            if(statusCache.ContainsKey(request))
            {
                return statusCache[request];
            }
            var tweet = global["tweets"][request.ToString()].ToObject<JObject>();
            var result = ParseStatus(tweet, issuer, GetUserFromConversation(tweet["user_id"].ToObject<long>(), issuer, global, userCache));
            if(result.IsQuote)
            {
                try
                {
                    result.QuotedStatus = GetStatusFromConversation(result.QuotedStatusId, issuer, global, statusCache, userCache);
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

        private User GetUserFromConversation(long request, long issuer, JObject global, Dictionary<long, User> cache)
        {
            if (cache.ContainsKey(request))
            {
                return cache[request];
            }

            var result = ParseUser(global["users"][request.ToString()].ToObject<JObject>(), issuer);
            cache[request] = result;
            return result;
        }

        public List<Status> ParseConversation(JObject obj, long issuer)
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
