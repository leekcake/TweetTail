using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Data
{
    public class Mute
    {
        public MuteTarget target;
        public Place[] places;

        public enum Place : short
        {
            Unknown = -1,
            Timeline = 0,
            UserPage = 1,
            Search = 2,
            FollowList = 3,
            NoticeList = 4,
            List = 5,
            DirectMessage = 6
        }

        public interface MuteTarget
        {
            JObject save();
        }

        public class StatusTarget : MuteTarget
        {
            public long id;
            public StatusTarget(long id)
            {
                this.id = id;
            }

            public StatusTarget(JObject data)
            {
                id = data["id"].ToObject<long>();
            }

            public JObject save()
            {
                var result = new JObject();
                result["type"] = "status";
                result["id"] = id;
                return result;
            }
        }

        public class KeywordTarget : MuteTarget
        {
            public string keyword, replace;
            public KeywordTarget(string keyword, string replace)
            {
                this.keyword = keyword;
                this.replace = replace;
            }

            public KeywordTarget(JObject data)
            {
                keyword = data["keyword"].ToString();
                if (data.ContainsKey("replace") && data["replace"].ToString() != "null")
                {
                    replace = data["replace"].ToString();
                }
            }

            public JObject save()
            {
                var result = new JObject();
                result["type"] = "keyword";
                result["keyword"] = keyword;
                if (replace != null)
                {
                    result["replace"] = replace;
                }
                return result;
            }
        }

        public class UserTarget : MuteTarget
        {
            public long id;

            public bool muteTweet;
            public bool muteRetweet;
            public bool muteOutboundMention;
            public bool muteSingleInboundMention;
            public bool muteMultiplyInboundMention;
            public bool muteMultiplyInboundMentionForcely;

            public UserTarget(long id)
            {
                this.id = id;
            }

            public UserTarget(JObject data)
            {
                id = data["id"].ToObject<long>();

                muteTweet = data["muteTweet"].ToObject<bool>();
                muteRetweet = data["muteRetweet"].ToObject<bool>();
                muteOutboundMention = data["muteOutboundMention"].ToObject<bool>();
                muteSingleInboundMention = data["muteSingleInboundMention"].ToObject<bool>();
                muteMultiplyInboundMention = data["muteMultiplyInboundMention"].ToObject<bool>();
                muteMultiplyInboundMentionForcely = data["muteMultiplyInboundMentionForcely"].ToObject<bool>();
            }

            public JObject save()
            {
                var result = new JObject();

                result["id"] = id;
                result["muteTweet"] = muteTweet;
                result["muteRetweet"] = muteRetweet;
                result["muteOutboundMention"] = muteOutboundMention;
                result["muteSingleInboundMention"] = muteSingleInboundMention;
                result["muteMultiplyInboundMention"] = muteMultiplyInboundMention;
                result["muteMultiplyInboundMentionForcely"] = muteMultiplyInboundMentionForcely;

                return result;
            }
        }
    }
}
