using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TwitterInterface.API;
using TwitterInterface.Data;

namespace Library.Container.Blend
{
    public class BlendedAccount
    {
        private TweetTail tail;

        public long[] ids;
        public string name;

        public BlendedAccount(TweetTail tail)
        {
            this.tail = tail;
        }

        public static BlendedAccount load(TweetTail tail, JObject obj)
        {
            var value = new BlendedAccount(tail);
            value.name = obj["name"].ToString();
            var ids = obj["ids"].ToObject<JArray>();
            value.ids = new long[ids.Count];
            for (int i = 0; i < ids.Count; i++)
            {
                value.ids[i] = ids[i].ToObject<long>();
            }

            return value;
        }

        public JObject save()
        {
            var result = new JObject();
            result["name"] = name;
            result["ids"] = new JArray(ids);

            return result;
        }

        private List<Status> blendStatus(List<Status>[] statuses)
        {
            var dict = new Dictionary<long, Status>();
            var list = new List<Status>();

            foreach (var statusList in statuses)
            {
                foreach (var status in statusList)
                {
                    if (dict.ContainsKey(status.id))
                    {
                        dict[status.id].issuer.Add(status.issuer[0]);
                    }
                    else
                    {
                        dict[status.id] = status;
                        list.Add(status);
                    }
                }
            }
            list.Sort(delegate (Status s1, Status s2)
            {
                return s2.id.CompareTo(s1.id);
            });

            return list;
        }

        private List<Notification> blendNotification(List<Notification>[] notifications)
        {
            var list = new List<Notification>();
            foreach(var notificationList in notifications)
            {
                list.AddRange(notificationList);
            }

            list.Sort(delegate (Notification s1, Notification s2)
            {
                return s2.maxPosition.CompareTo(s1.maxPosition);
            });

            return list;
        }

        public async Task<List<Status>> getTimeline(int count = 200, long sinceId = -1, long maxId = -1)
        {
            var statuses = new List<Status>[ids.Length];
            for (int i = 0; i < statuses.Length; i++)
            {
                statuses[i] = await tail.twitter.GetTimeline(tail.account.getAccountGroup(ids[i]).accountForRead, count, sinceId, maxId);
            }

            return blendStatus(statuses);
        }

        public async Task<List<Status>> getMentionline(int count = 200, long sinceId = -1, long maxId = -1)
        {
            var statuses = new List<Status>[ids.Length];
            for (int i = 0; i < statuses.Length; i++)
            {
                statuses[i] = await tail.twitter.GetMentionline(tail.account.getAccountGroup(ids[i]).accountForRead, count, sinceId, maxId);
            }

            return blendStatus(statuses);
        }

        public async Task<List<Notification>> getNotifications(int count = 40, long sinceId = -1, long maxId = -1)
        {
            var notifications = new List<Notification>[ids.Length];
            for (int i = 0; i < notifications.Length; i++)
            {
                notifications[i] = await tail.twitter.GetNotifications(tail.account.getAccountGroup(ids[i]).accountForRead, count, sinceId, maxId);
            }

            return blendNotification(notifications);
        }
    }
}
