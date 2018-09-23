using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DataStatus = TwitterInterface.Data.Status;

namespace TweetTail.Status
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimelinePage : ContentPage
    {
        public TimelinePage()
        {
            InitializeComponent();

            StatusListView.statusGetter = TimelineGetter;
            StatusListView.Refresh();
            
        }

        public Task<List<DataStatus>> TimelineGetter(long sinceId, long maxId)
        {
            return App.tail.twitter.GetTimeline(App.tail.account.readOnlyAccountGroups[0].accountForRead, 200, sinceId, maxId);
        }

        private void fabTweet_Clicked(object sender, EventArgs e)
        {
            var page = new ContentPage();
            page.Content = new StatusWriterView() { BindingContext = page };
            page.Title = "트윗작성";
            App.Navigation.PushAsync(page);
        }
    }
}
