using Library.Container.Fetch;
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

            Reload();
        }

        public void Reload()
        {
            if (App.tail.blend.SelectedBlendedAccount != null)
            {
                StatusListView.Fetchable = new BlendAccountFetch<DataStatus>.Timeline( App.tail.blend.SelectedBlendedAccount );
            }
            else
            {
                StatusListView.Fetchable = new AccountFetch.Timeline(App.tail, App.tail.account.SelectedAccountGroup.accountForRead);
            }
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
