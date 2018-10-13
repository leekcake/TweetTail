using Newtonsoft.Json;
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
            public Status status;

            public override void Save(JObject obj)
            {
                obj["type"] = "status";

                obj["target"] = new JObject(this);
            }

            public new static StatusTarget Load(JObject obj)
            {
                return obj["target"].ToObject<StatusTarget>();
            }
        }

        public class KeywordTarget : Target
        {
            public string keyword;
            public string replace;

            public override void Save(JObject obj)
            {
                obj["type"] = "keyword";
                obj["target"] = new JObject(this);
            }

            public new static KeywordTarget Load(JObject obj)
            {
                return obj["target"].ToObject<KeywordTarget>();
            }
        }

        public class UserTarget : Target
        {
            public long id;
            public User user;

            /// <summary>
            /// 이 유저가 포함되어 있다면 거의 모든 항목에서 차단하려 시도합니다
            /// </summary>
            public bool muteGoAway;

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
                obj["target"] = new JObject(this);
            }

            public new static UserTarget Load(JObject obj)
            {
                return obj["target"].ToObject<UserTarget>();
            }
        }
    }
}
