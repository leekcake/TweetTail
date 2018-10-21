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
        internal TweetTail tail;

        public long[] IDs;
        public string Name;

        public BlendedAccount(TweetTail tail)
        {
            this.tail = tail;
        }

        public static BlendedAccount Load(TweetTail tail, JObject obj)
        {
            var value = new BlendedAccount(tail);
            value.Name = obj["name"].ToString();
            var ids = obj["ids"].ToObject<JArray>();
            value.IDs = new long[ids.Count];
            for (int i = 0; i < ids.Count; i++)
            {
                value.IDs[i] = ids[i].ToObject<long>();
            }

            return value;
        }

        public JObject Save()
        {
            var result = new JObject();
            result["name"] = Name;
            result["ids"] = new JArray(IDs);

            return result;
        }

        public static List<Status> BlendStatus(List<Status>[] statuses)
        {
            var dict = new Dictionary<long, Status>();
            var list = new List<Status>();

            foreach (var statusList in statuses)
            {
                foreach (var status in statusList)
                {
                    if (dict.ContainsKey(status.ID))
                    {
                        dict[status.ID].Issuer.Add(status.Issuer[0]);
                    }
                    else
                    {
                        dict[status.ID] = status;
                        list.Add(status);
                    }
                }
            }
            list.Sort(delegate (Status s1, Status s2)
            {
                return s2.ID.CompareTo(s1.ID);
            });

            return list;
        }

        public static List<Notification> BlendNotification(List<Notification>[] notifications)
        {
            var list = new List<Notification>();
            foreach(var notificationList in notifications)
            {
                list.AddRange(notificationList);
            }

            list.Sort(delegate (Notification s1, Notification s2)
            {
                return s2.MaxPosition.CompareTo(s1.MaxPosition);
            });

            return list;
        }

        public async Task<List<Status>> GetTimeline(int count = 200, long sinceId = -1, long maxId = -1)
        {
            var statuses = new List<Status>[IDs.Length];
            for (int i = 0; i < statuses.Length; i++)
            {
                statuses[i] = await tail.TwitterAPI.GetTimelineAsync(tail.Account.GetAccountGroup(IDs[i]).AccountForRead, count, sinceId, maxId);
            }

            return BlendStatus(statuses);
        }

        public async Task<List<Status>> GetMentionline(int count = 200, long sinceId = -1, long maxId = -1)
        {
            var statuses = new List<Status>[IDs.Length];
            for (int i = 0; i < statuses.Length; i++)
            {
                statuses[i] = await tail.TwitterAPI.GetMentionlineAsync(tail.Account.GetAccountGroup(IDs[i]).AccountForRead, count, sinceId, maxId);
            }

            return BlendStatus(statuses);
        }

        public async Task<List<Notification>> GetNotifications(int count = 40, long sinceId = -1, long maxId = -1)
        {
            var notifications = new List<Notification>[IDs.Length];
            for (int i = 0; i < notifications.Length; i++)
            {
                notifications[i] = await tail.TwitterAPI.GetNotificationsAsync(tail.Account.GetAccountGroup(IDs[i]).AccountForRead, count, sinceId, maxId);
            }

            return BlendNotification(notifications);
        }
    }
}
