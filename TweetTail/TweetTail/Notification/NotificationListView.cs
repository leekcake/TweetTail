using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using DataNotification = TwitterInterface.Data.Notification;

namespace TweetTail.Notification
{
    public class NotificationListView : ListView
    {
        public ObservableCollection<DataNotification> Items { get; set; }

        public long lastSinceId = -1;
        public Func<long, long, Task<List<DataNotification>>> notificationGetter = null;

        public NotificationListView() : base(ListViewCachingStrategy.RecycleElement)
        {
            HasUnevenRows = true;
            ItemTemplate = new DataTemplate(typeof(NotificationCell));
            Items = new ObservableCollection<DataNotification>();
            ItemsSource = Items;
            SelectionMode = ListViewSelectionMode.None;
        }

        public async void Refresh()
        {
            try
            {
                if (notificationGetter == null)
                {
                    EndRefresh();
                    return;
                }

                var notifications = await notificationGetter(lastSinceId, -1);
                if (notifications.Count == 0)
                {
                    EndRefresh();
                    return;
                }

                lastSinceId = notifications[0].maxPosition;
                for (int i = notifications.Count - 1; i >= 0; i--)
                {
                    Items.Insert(0, notifications[i]);
                }

                EndRefresh();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message + "\n" + e.StackTrace);
            }
        }
    }
}
