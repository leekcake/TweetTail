using Library.Container.Fetch;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using TweetTail.Components.Status;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DataStatus = TwitterInterface.Data.Status;

namespace TweetTail.Pages.Status
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimelinePage : ContentPage
    {
        public TimelinePage()
        {
            InitializeComponent();

            Reload();
        }

        public void ScrollToRoot()
        {
            StatusListView.ScrollToRoot();
        }

        public void Reload()
        {
            if (App.Tail.Blend.SelectedBlendedAccount != null)
            {
                StatusListView.Fetchable = new BlendAccountFetch<DataStatus>.Timeline( App.Tail.Blend.SelectedBlendedAccount );
            }
            else
            {
                StatusListView.Fetchable = new AccountFetch.Timeline(App.Tail, App.Tail.Account.SelectedAccountGroup);
            }
        }

        private void TweetButton_Clicked(object sender, EventArgs e)
        {
            var page = new ContentPage() { Style = (Style)Application.Current.Resources["backgroundStyle"] };
            page.Content = new StatusWriterView(App.Tail.Account.SelectedAccountGroup) { BindingContext = page };
            page.Title = "트윗작성";
            App.Navigation.PushAsync(page);
        }
    }
}
