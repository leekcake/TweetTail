using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data
{
    public class Notification
    {
        public string action;
        public DateTime createdAt;

        public long maxPosition, minPosition;

        public object[] targetObjects;
        public object[] targets;
        public object[] sources;

        public class Retweet : Notification
        {
            //TargetObject: 리트윗 대상 트윗
            //Target: 리트윗 된 트윗
            //Source: 리트윗 한 사람

            public Status RetweetTargetTweet {
                get {
                    return targetObjects[0] as Status;
                }
            }

            public Status RetweetedTweet {
                get {
                    return targets[0] as Status;
                }
            }

            public User Performer {
                get {
                    return sources[0] as User;
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
                    return targetObjects[0] as User;
                }
            }

            public Status RetweetedTweet {
                get {
                    return targets[0] as Status;
                }
            }

            public User Performer {
                get {
                    return sources[0] as User;
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
                    return targetObjects[0] as User;
                }
            }

            public Status RetweetedTweet {
                get {
                    return targets[0] as Status;
                }
            }

            public User Performer {
                get {
                    return sources[0] as User;
                }
            }
        }

        public class Favorited : Notification
        {
            //Source: 마음 찍은 사람
            //Target: 마음 찍힌 트윗
            public Status FavoritedTweet {
                get {
                    return targets[0] as Status;
                }
            }

            public User Performer {
                get {
                    return sources[0] as User;
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
                    return targetObjects[0] as User;
                }
            }

            public Status RetweetedTweet {
                get {
                    return targets[0] as Status;
                }
            }

            public User Performer {
                get {
                    return sources[0] as User;
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
                    return targetObjects[0] as User;
                }
            }

            public Status FavoritedTweet {
                get {
                    return targets[0] as Status;
                }
            }

            public User Performer {
                get {
                    return sources[0] as User;
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
                    return targetObjects[0] as Status;
                }
            }

            public User MentionedUser {
                get {
                    return targets[0] as User;
                }
            }

            public User Performer {
                get {
                    return sources[0] as User;
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
                    return targetObjects[0] as Status;
                }
            }

            public Status ReplyTweet {
                get {
                    return targets[0] as Status;
                }
            }

            public User Replyer {
                get {
                    return sources[0] as User;
                }
            }
        }

        public class Follow : Notification
        {
            //Target: 팔로우 대상
            //Source: 팔로우 한 사람

            public User Followed {
                get {
                    return targets[0] as User;
                }
            }

            public User Performer {
                get {
                    return sources[0] as User;
                }
            }
        }
    }
}
