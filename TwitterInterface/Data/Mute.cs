using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data
{
    public class Mute
    {
        public Target target;

        public JObject Save()
        {
            var result = new JObject();
            target.Save(result);
            return result;
        }

        public static Mute Load(JObject obj)
        {
            var mute = new Mute();
            mute.target = Target.Load(obj);
            return mute;
        }

        public abstract class Target {
            public abstract void Save(JObject obj);

            public static Target Load(JObject obj)
            {
                var type = obj["type"].ToString();
                if(type == "status")
                {
                    return StatusTarget.Load(obj);
                }
                if(type == "keyword")
                {
                    return KeywordTarget.Load(obj);
                }
                if(type == "user")
                {
                    return UserTarget.Load(obj);
                }
                return null;
            }
        }

        public class StatusTarget : Target
        {
            public long id;

            public override void Save(JObject obj)
            {
                obj["type"] = "status";
                obj["id"] = id;
            }

            public new static StatusTarget Load(JObject obj)
            {
                var result = new StatusTarget();

                result.id = obj["id"].ToObject<long>();

                return result;
            }
        }

        public class KeywordTarget : Target
        {
            public string keyword;
            public string replace;

            public override void Save(JObject obj)
            {
                obj["type"] = "keyword";
                obj["keyword"] = keyword;
                if (replace != null)
                {
                    obj["replace"] = replace;
                }
            }

            public new static KeywordTarget Load(JObject obj)
            {
                var result = new KeywordTarget();
                result.keyword = obj["keyword"].ToString();
                if (obj.ContainsKey("replace"))
                {
                    result.replace = obj["replace"].ToString();
                }
                return result;
            }
        }

        public class UserTarget : Target
        {
            public long id;

            /// <summary>
            /// 대상 유저의 순수 트윗을 뮤트합니다
            /// </summary>
            public bool muteTweet;

            /// <summary>
            /// 대상 유저의 리트윗을 뮤트합니다
            /// </summary>
            public bool muteRetweet;

            /// <summary>
            /// 대상 유저에게서 출발한 멘션을 뮤트합니다
            ///-@muted: @person 멘션 -> 뮤트됨
            ///@muted: 트윗 @person -> 뮤트됨
            /// </summary>
            public bool muteOutboundMention;

            public bool muteSingleInboundMention;
            public bool muteMultipleInboundMention;
            public bool muteMultipleInboundMentionForcely;

            public override void Save(JObject obj)
            {
                obj["type"] = "user";
                obj["id"] = id;
                obj["muteTweet"] = muteTweet;
                obj["muteRetweet"] = muteRetweet;
                obj["muteOutboundMention"] = muteOutboundMention;
                obj["muteSingleInboundMention"] = muteSingleInboundMention;
                obj["muteMultipleInboundMention"] = muteMultipleInboundMention;
                obj["muteMultipleInboundMentionForcely"] = muteMultipleInboundMentionForcely;
            }

            public new static UserTarget Load(JObject obj)
            {
                var target = new UserTarget();

                target.id = obj["id"].ToObject<long>();
                target.muteTweet = obj["muteTweet"].ToObject<bool>();
                target.muteRetweet = obj["muteRetweet"].ToObject<bool>();
                target.muteOutboundMention = obj["muteOutboundMention"].ToObject<bool>();
                target.muteSingleInboundMention = obj["muteSingleInboundMention"].ToObject<bool>();
                target.muteMultipleInboundMention = obj["muteMultipleInboundMention"].ToObject<bool>();
                target.muteMultipleInboundMentionForcely = obj["muteMultipleInboundMentionForcely"].ToObject<bool>();

                return target;
            }
        }
    }
}
