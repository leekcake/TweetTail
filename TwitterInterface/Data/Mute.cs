using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data
{
    public class Mute
    {
        public Target target;

        public class Target { }

        public class StatusTarget : Target
        {
            public long id;
        }

        public class KeywordTarget : Target
        {
            public string keyword;
            public string replace;
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
        }
    }
}
