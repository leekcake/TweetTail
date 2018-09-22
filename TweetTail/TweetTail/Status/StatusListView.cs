using Library.Container.Account;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using DataStatus = TwitterInterface.Data.Status;

namespace TweetTail.Status
{
    public class StatusListView : ListView
    {
        public ObservableCollection<DataStatus> Items { get; set; }

        public long lastSinceId = -1;
        public Func<long, long, Task<List<DataStatus>>> statusGetter = null;

        public StatusListView() : base( ListViewCachingStrategy.RecycleElement )
        {
            HasUnevenRows = true;
            ItemTemplate = new DataTemplate(typeof(StatusCell));
            Items = new ObservableCollection<DataStatus>();
            ItemsSource = Items;
            SelectionMode = ListViewSelectionMode.None;

            IsPullToRefreshEnabled = true;
            RefreshCommand = new Command(Refresh);
        }

        public async void Refresh()
        {
            try
            {
                if(statusGetter == null)
                {
                    EndRefresh();
                    return;
                }

                var statuses = await statusGetter(lastSinceId, -1);
                if(statuses.Count == 0)
                {
                    EndRefresh();
                    return;
                }

                lastSinceId = statuses[0].id;
                for(int i = statuses.Count - 1; i >= 0; i--)
                {
                    Items.Insert(0, statuses[i]);
                }

                EndRefresh();
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message + "\n" + e.StackTrace);
            }
        }
    }
}
