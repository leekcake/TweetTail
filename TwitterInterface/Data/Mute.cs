using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data
{
    public class Mute
    {
        public MuteTarget Target;

        public JObject Save()
        {
            var result = new JObject();
            Target.Save(result);
            return result;
        }

        public static Mute Load(JObject obj)
        {
            return new Mute
            {
                Target = MuteTarget.Load(obj)
            }; ;
        }

        public abstract class MuteTarget {
            public abstract void Save(JObject obj);

            public static MuteTarget Load(JObject obj)
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

        public class StatusTarget : MuteTarget
        {
            public long ID;
            public Status Status;

            public override void Save(JObject obj)
            {
                obj["type"] = "status";

                obj["target"] = JObject.FromObject(this);
            }

            public new static StatusTarget Load(JObject obj)
            {
                return obj["target"].ToObject<StatusTarget>();
            }
        }

        public class KeywordTarget : MuteTarget
        {
            public string Keyword;
            public string Replace;

            public override void Save(JObject obj)
            {
                obj["type"] = "keyword";
                obj["target"] = JObject.FromObject(this);
            }

            public new static KeywordTarget Load(JObject obj)
            {
                return obj["target"].ToObject<KeywordTarget>();
            }
        }

        public class UserTarget : MuteTarget
        {
            public long ID;
            public User User;

            /// <summary>
            /// 이 뮤트 타겟이 아무것도 대상으로 삼지 않고 있는지의 여부를 확인합니다
            /// </summary>
            public bool IsNeedless {
                get {
                    return !MuteGoAway && !MuteTweet && !MuteRetweet && !MuteOutboundMention 
                        && !MuteSingleInboundMention && !MuteMultipleInboundMention && !MuteMultipleInboundMentionForcely;
                }
            }

            /// <summary>
            /// 이 유저가 포함되어 있다면 거의 모든 항목에서 차단하려 시도합니다
            /// </summary>
            public bool MuteGoAway;

            /// <summary>
            /// 대상 유저의 순수 트윗을 뮤트합니다
            /// </summary>
            public bool MuteTweet;

            /// <summary>
            /// 대상 유저의 리트윗을 뮤트합니다
            /// </summary>
            public bool MuteRetweet;

            /// <summary>
            /// 대상 유저에게서 출발한 멘션을 뮤트합니다
            ///-@muted: @person 멘션 -> 뮤트됨
            ///@muted: 트윗 @person -> 뮤트됨
            /// </summary>
            public bool MuteOutboundMention;

            public bool MuteSingleInboundMention;
            public bool MuteMultipleInboundMention;
            public bool MuteMultipleInboundMentionForcely;

            public override void Save(JObject obj)
            {
                obj["type"] = "user";
                obj["target"] = JObject.FromObject(this);
            }

            public new static UserTarget Load(JObject obj)
            {
                return obj["target"].ToObject<UserTarget>();
            }
        }
    }
}
