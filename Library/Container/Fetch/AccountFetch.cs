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
            protected DataAccount account;

            public AccountSinceFetch(TweetTail tail, DataAccount account)
            {
                this.tail = tail;
                this.account = account;
            }
        }

        public abstract class AccountCursoredFetch<T> : CursoredFetch<T>
        {
            protected TweetTail tail;
            protected DataAccount account;

            public AccountCursoredFetch(TweetTail tail, DataAccount account)
            {
                this.tail = tail;
                this.account = account;
            }
        }

        public class Timeline : AccountSinceFetch<Status>
        {
            public Timeline(TweetTail tail, DataAccount account) : base(tail, account)
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
            public Mentionline(TweetTail tail, DataAccount account) : base(tail, account)
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

            public Userline(TweetTail tail, DataAccount account, User target) : base(tail, account)
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

            public Medialine(TweetTail tail, DataAccount account, User target) : base(tail, account)
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

            public Favorites(TweetTail tail, DataAccount account, User target) : base(tail, account)
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

            public Followers(TweetTail tail, DataAccount account, User target) : base(tail, account)
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

            public Followings(TweetTail tail, DataAccount account, User target) : base(tail, account)
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

            public Search(TweetTail tail, DataAccount account, string query, bool isRecent, string until = null) : base(tail, account)
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
            public Notifications(TweetTail tail, DataAccount account) : base(tail, account)
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
