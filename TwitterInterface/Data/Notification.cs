using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data
{
    public class Notification
    {
        public string Action;
        public DateTime CreatedAt;

        public long MaxPosition, MinPosition;

        public object[] TargetObjects;
        public object[] Targets;
        public object[] Sources;

        public class Retweet : Notification
        {
            //TargetObject: 리트윗 대상 트윗
            //Target: 리트윗 된 트윗
            //Source: 리트윗 한 사람

            public Status RetweetTargetTweet {
                get {
                    return TargetObjects[0] as Status;
                }
            }

            public Status RetweetedTweet {
                get {
                    return Targets[0] as Status;
                }
            }

            public User Performer {
                get {
                    return Sources[0] as User;
                }
            }
        }

        public class RetweetedMention : Notification
        {
            //TargetObject: 멘션을 받은 사람
            //Target: 원본 트윗
            //Source: 리트윗을 한 사람

            public User Retweeter {
                get {
                    return TargetObjects[0] as User;
                }
            }

            public Status RetweetedTweet {
                get {
                    return Targets[0] as Status;
                }
            }

            public User Performer {
                get {
                    return Sources[0] as User;
                }
            }
        }

        public class RetweetedRetweet : Notification
        {
            //TargetObject: 리트윗한 사람
            //Target: 원본 트윗
            //Source: 마음 찍은 사람

            public User Retweeter {
                get {
                    return TargetObjects[0] as User;
                }
            }

            public Status RetweetedTweet {
                get {
                    return Targets[0] as Status;
                }
            }

            public User Performer {
                get {
                    return Sources[0] as User;
                }
            }
        }

        public class Favorited : Notification
        {
            //Source: 마음 찍은 사람
            //Target: 마음 찍힌 트윗
            public Status FavoritedTweet {
                get {
                    return Targets[0] as Status;
                }
            }

            public User Performer {
                get {
                    return Sources[0] as User;
                }
            }
        }

        public class FavoritedMention : Notification
        {
            //TargetObject: 멘션을 받은 사람
            //Target: 원본 트윗
            //Source: 마음 찍은 사람

            public User Retweeter {
                get {
                    return TargetObjects[0] as User;
                }
            }

            public Status RetweetedTweet {
                get {
                    return Targets[0] as Status;
                }
            }

            public User Performer {
                get {
                    return Sources[0] as User;
                }
            }
        }

        public class FavoritedRetweet : Notification
        {
            //TargetObject: 리트윗한 사람
            //Target: 원본 트윗
            //Source: 마음 찍은 사람

            public User Retweeter {
                get {
                    return TargetObjects[0] as User;
                }
            }

            public Status FavoritedTweet {
                get {
                    return Targets[0] as Status;
                }
            }

            public User Performer {
                get {
                    return Sources[0] as User;
                }
            }
        }

        public class Mention : Notification
        {
            //TargetObject: 멘션의 원천이 된 트윗
            //Target: 멘션 받은 사람
            //Source: 멘션한 사람

            public Status MentionedTweet {
                get {
                    return TargetObjects[0] as Status;
                }
            }

            public User MentionedUser {
                get {
                    return Targets[0] as User;
                }
            }

            public User Performer {
                get {
                    return Sources[0] as User;
                }
            }
        }

        public class Reply : Notification
        {
            //TargetObject: 답글이 달린 트윗
            //Target: 답글로 온 트윗
            //Source: 답글 단 사람

            public Status ReplyTargetTweet {
                get {
                    return TargetObjects[0] as Status;
                }
            }

            public Status ReplyTweet {
                get {
                    return Targets[0] as Status;
                }
            }

            public User Replyer {
                get {
                    return Sources[0] as User;
                }
            }
        }

        public class Quote : Notification
        {
            //TargetObject: 인용이 된 트윗
            //Target: 인용을 한 트윗
            //Source: 인용 한 사람

            public Status QuoteTargetTweet {
                get {
                    return TargetObjects[0] as Status;
                }
            }

            public Status QuoteTweet {
                get {
                    return Targets[0] as Status;
                }
            }

            public User Quoter {
                get {
                    return Sources[0] as User;
                }
            }
        }

        public class Follow : Notification
        {
            //Target: 팔로우 대상
            //Source: 팔로우 한 사람

            public User Followed {
                get {
                    return Targets[0] as User;
                }
            }

            public User Performer {
                get {
                    return Sources[0] as User;
                }
            }
        }
    }
}
