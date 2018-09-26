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

        public class Timeline : SinceFetch<Status>
        {
            private TweetTail tail;
            private DataAccount account;

            public Timeline(TweetTail tail, DataAccount account)
            {
                this.tail = tail;
                this.account = account;
            }

            protected override Task<List<Status>> GetDatas(long sinceId, long maxId)
            {
                return tail.twitter.GetTimeline(account, 200, sinceId, maxId);
            }

            protected override long GetID(Status data)
            {
                return data.id;
            }
        }

        public class Mentionline : SinceFetch<Status>
        {
            private TweetTail tail;
            private DataAccount account;

            public Mentionline(TweetTail tail, DataAccount account)
            {
                this.tail = tail;
                this.account = account;
            }

            protected override Task<List<Status>> GetDatas(long sinceId, long maxId)
            {
                return tail.twitter.GetMentionline(account, 200, sinceId, maxId);
            }

            protected override long GetID(Status data)
            {
                return data.id;
            }
        }

        public class Notifications : SinceFetch<Notification>
        {
            private TweetTail tail;
            private DataAccount account;

            public Notifications(TweetTail tail, DataAccount account)
            {
                this.tail = tail;
                this.account = account;
            }

            protected override Task<List<Notification>> GetDatas(long sinceId, long maxId)
            {
                return tail.twitter.GetNotifications(account, 40, sinceId, maxId);
            }

            protected override long GetID(Notification data)
            {
                return data.maxPosition;
            }
        }
    }
}
