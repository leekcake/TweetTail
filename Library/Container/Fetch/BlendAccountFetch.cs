using Library.Container.Blend;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TwitterInterface.Data;

namespace Library.Container.Fetch
{
    public abstract class BlendAccountFetch<Data> : Fetchable<Data>
    {
        protected TweetTail tail;

        private Fetchable<Data>[] fetchables;

        protected abstract Fetchable<Data> GetFetchable(long id);
        protected abstract long GetID(Data data);
        protected abstract List<Data> GetBlendedData(List<Data>[] datas);

        private BlendAccountFetch(BlendedAccount account)
        {
            tail = account.tail;
            fetchables = new Fetchable<Data>[account.ids.Length];
            for (int i = 0; i < fetchables.Length; i++)
            {
                fetchables[i] = GetFetchable(account.ids[i]);
            }
        }

        public bool IsSupportRefresh => fetchables[0].IsSupportRefresh;

        public async Task<List<Data>> FetchNew()
        {
            var datas = new List<Data>[fetchables.Length];
            for (int i = 0; i < datas.Length; i++)
            {
                datas[i] = await fetchables[i].FetchNew();
            }
            return GetBlendedData(datas);
        }

        public async Task<List<Data>> FetchOld()
        {
            var datas = new List<Data>[fetchables.Length];
            for (int i = 0; i < datas.Length; i++)
            {
                datas[i] = await fetchables[i].FetchOld();
            }
            return GetBlendedData(datas);
        }

        public class Timeline : BlendAccountFetch<Status>
        {
            public Timeline(BlendedAccount account) : base(account)
            {
            }

            protected override List<Status> GetBlendedData(List<Status>[] datas)
            {
                return BlendedAccount.blendStatus(datas);
            }

            protected override Fetchable<Status> GetFetchable(long id)
            {
                return new AccountFetch.Timeline(tail, tail.account.getAccountGroup(id).accountForRead);
            }

            protected override long GetID(Status data)
            {
                return data.id;
            }
        }

        public class Mentionline : BlendAccountFetch<Status>
        {
            public Mentionline(BlendedAccount account) : base(account)
            {
            }

            protected override List<Status> GetBlendedData(List<Status>[] datas)
            {
                return BlendedAccount.blendStatus(datas);
            }

            protected override Fetchable<Status> GetFetchable(long id)
            {
                return new AccountFetch.Mentionline(tail, tail.account.getAccountGroup(id).accountForRead);
            }

            protected override long GetID(Status data)
            {
                return data.id;
            }
        }

        public class Notifications : BlendAccountFetch<Notification>
        {
            public Notifications(BlendedAccount account) : base(account)
            {
            }

            protected override List<Notification> GetBlendedData(List<Notification>[] datas)
            {
                return BlendedAccount.blendNotification(datas);
            }

            protected override Fetchable<Notification> GetFetchable(long id)
            {
                return new AccountFetch.Notifications(tail, tail.account.getAccountGroup(id).accountForRead);
            }

            protected override long GetID(Notification data)
            {
                return data.maxPosition;
            }
        }
    }
}
