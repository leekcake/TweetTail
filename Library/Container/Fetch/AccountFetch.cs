using Library.Container.Account;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TwitterInterface.Data;

using DataAccount = TwitterInterface.Data.Account;

namespace Library.Container.Fetch
{
    public sealed class AccountFetch
    {
        private AccountFetch()
        {

        }

        public abstract class AccountSinceFetch<T> : SinceFetch<T>
        {
            protected TweetTail tail;
            protected AccountGroup accountGroup;
            protected DataAccount account {
                get {
                    return accountGroup.AccountForRead; 
                }
            }

            public AccountSinceFetch(TweetTail tail, AccountGroup accountGroup)
            {
                this.tail = tail;
                this.accountGroup = accountGroup;
            }
        }

        public abstract class AccountCursoredFetch<T> : CursoredFetch<T>
        {
            protected TweetTail tail;
            protected AccountGroup accountGroup;
            protected DataAccount account {
                get {
                    return accountGroup.AccountForRead;
                }
            }

            public AccountCursoredFetch(TweetTail tail, AccountGroup accountGroup)
            {
                this.tail = tail;
                this.accountGroup = accountGroup;
            }
        }

        public class Timeline : AccountSinceFetch<Status>
        {
            public Timeline(TweetTail tail, AccountGroup accountGroup) : base(tail, accountGroup)
            {
            }

            protected override Task<List<Status>> GetDatas(long sinceId, long maxId)
            {
                return tail.TwitterAPI.GetTimelineAsync(account, 100, sinceId, maxId);
            }

            protected override long GetID(Status data)
            {
                return data.ID;
            }
        }

        public class Mentionline : AccountSinceFetch<Status>
        {
            public Mentionline(TweetTail tail, AccountGroup accountGroup) : base(tail, accountGroup)
            {
            }

            protected override Task<List<Status>> GetDatas(long sinceId, long maxId)
            {
                return tail.TwitterAPI.GetMentionlineAsync(account, 100, sinceId, maxId);
            }

            protected override long GetID(Status data)
            {
                return data.ID;
            }
        }

        public class Userline : AccountSinceFetch<Status>
        {
            private User target;

            public Userline(TweetTail tail, AccountGroup accountGroup, User target) : base(tail, accountGroup)
            {
                this.target = target;
            }

            protected override Task<List<Status>> GetDatas(long sinceId, long maxId)
            {
                return tail.TwitterAPI.GetUserlineAsync(account, target.ID, 100, sinceId, maxId);
            }

            protected override long GetID(Status data)
            {
                return data.ID;
            }
        }

        public class Medialine : AccountSinceFetch<Status>
        {
            private User target;

            public Medialine(TweetTail tail, AccountGroup accountGroup, User target) : base(tail, accountGroup)
            {
                this.target = target;
            }

            protected override Task<List<Status>> GetDatas(long sinceId, long maxId)
            {
                return tail.TwitterAPI.GetMedialineAsync(account, target.ID, 100, sinceId, maxId);
            }

            protected override long GetID(Status data)
            {
                return data.ID;
            }
        }

        public class Favorites : AccountSinceFetch<Status>
        {
            private User target;

            public Favorites(TweetTail tail, AccountGroup accountGroup, User target) : base(tail, accountGroup)
            {
                this.target = target;
            }

            protected override Task<List<Status>> GetDatas(long sinceId, long maxId)
            {
                return tail.TwitterAPI.GetFavoritesAsync(account, target.ID, 100, sinceId, maxId);
            }

            protected override long GetID(Status data)
            {
                return data.ID;
            }
        }

        public class Followers : AccountCursoredFetch<User>
        {
            private User target;

            public Followers(TweetTail tail, AccountGroup accountGroup, User target) : base(tail, accountGroup)
            {
                this.target = target;
            }

            protected override Task<CursoredList<User>> GetDatas(long cursor)
            {
                return tail.TwitterAPI.GetFollowersAsync(account, target.ID, cursor);
            }

            protected override long GetID(User data)
            {
                return data.ID;
            }
        }

        public class Followings : AccountCursoredFetch<User>
        {
            private User target;

            public Followings(TweetTail tail, AccountGroup accountGroup, User target) : base(tail, accountGroup)
            {
                this.target = target;
            }

            protected override Task<CursoredList<User>> GetDatas(long cursor)
            {
                return tail.TwitterAPI.GetFriendsAsync(account, target.ID, cursor);
            }

            protected override long GetID(User data)
            {
                return data.ID;
            }
        }

        public class Search : AccountSinceFetch<Status>
        {
            private readonly string query;
            private readonly bool isRecent;
            private readonly string until;

            public Search(TweetTail tail, AccountGroup accountGroup, string query, bool isRecent, string until = null) : base(tail, accountGroup)
            {
                this.query = query;
                this.isRecent = isRecent;
                this.until = until;
            }

            protected override Task<List<Status>> GetDatas(long sinceId, long maxId)
            {
                return tail.TwitterAPI.SearchTweetAsync(account, query, isRecent, 100, until, sinceId, maxId);
            }

            protected override long GetID(Status data)
            {
                return data.ID;
            }
        }

        public class Notifications : AccountSinceFetch<Notification>
        {
            public Notifications(TweetTail tail, AccountGroup accountGroup) : base(tail, accountGroup)
            {
            }

            protected override Task<List<Notification>> GetDatas(long sinceId, long maxId)
            {
                return tail.TwitterAPI.GetNotificationsAsync(account, 40, sinceId, maxId);
            }

            protected override long GetID(Notification data)
            {
                return data.MaxPosition;
            }
        }
    }
}
