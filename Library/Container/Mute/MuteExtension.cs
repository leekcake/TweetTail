using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

using DataMute = TwitterInterface.Data.Mute;

namespace Library.Container.Mute
{
    public static class MuteExtension
    {
        public static JObject Save(this DataMute mute)
        {
            var result = new JObject();

            if(mute.target is DataMute.StatusTarget)
            {
                var target = mute.target as DataMute.StatusTarget;
                result["type"] = "status";
                result["id"] = target.id;
            }
            else if(mute.target is DataMute.KeywordTarget)
            {
                var target = mute.target as DataMute.KeywordTarget;
                result["type"] = "keyword";
                result["keyword"] = target.keyword;
                if(target.replace != null)
                {
                    result["replace"] = target.replace;
                }
            }
            else if(mute.target is DataMute.UserTarget)
            {
                var target = mute.target as DataMute.UserTarget;
                result["type"] = "user";
                result["id"] = target.id;
                result["muteTweet"] = target.muteTweet;
                result["muteRetweet"] = target.muteRetweet;
                result["muteOutboundMention"] = target.muteOutboundMention;
                result["muteSingleInboundMention"] = target.muteSingleInboundMention;
                result["muteMultipleInboundMention"] = target.muteMultipleInboundMention;
                result["muteMultipleInboundMentionForcely"] = target.muteMultipleInboundMentionForcely;                
            }

            return result;
        }

        public static DataMute LoadMute(JObject data)
        {
            var result = new DataMute();

            var type = data["type"].ToString();
            if(type == "status")
            {
                var target = new DataMute.StatusTarget();
                target.id = data["id"].ToObject<long>();
                result.target = target;
            }
            else if(type == "keyword")
            {
                var target = new DataMute.KeywordTarget();
                target.keyword = data["keyword"].ToString();
                if(data.ContainsKey("replace"))
                {
                    target.replace = data["replace"].ToString();
                }
                result.target = target;
            }
            else if(type == "user")
            {
                var target = new DataMute.UserTarget();

                target.id = data["id"].ToObject<long>();
                target.muteTweet = data["muteTweet"].ToObject<bool>();
                target.muteRetweet = data["muteRetweet"].ToObject<bool>();
                target.muteOutboundMention = data["muteOutboundMention"].ToObject<bool>();
                target.muteSingleInboundMention = data["muteSingleInboundMention"].ToObject<bool>();
                target.muteMultipleInboundMention = data["muteMultipleInboundMention"].ToObject<bool>();
                target.muteMultipleInboundMentionForcely = data["muteMultipleInboundMentionForcely"].ToObject<bool>();

                result.target = target;
            }

            return result;
        }
    }
}
