using Library.Container.Fetch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DataNotification = TwitterInterface.Data.Notification;

namespace TweetTail.Pages.Notification
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NotificationPage : ContentPage
	{
		public NotificationPage ()
		{
			InitializeComponent ();

            Reload();
        }

        public void ScrollToRoot()
        {
            NotificationListView.ScrollToRoot();
        }

        public void Reload()
        {
            if (App.Tail.Blend.SelectedBlendedAccount != null)
            {
                NotificationListView.Fetchable = new BlendAccountFetch<DataNotification>.Notifications(App.Tail.Blend.SelectedBlendedAccount);
            }
            else
            {
                NotificationListView.Fetchable = new AccountFetch.Notifications(App.Tail, App.Tail.Account.SelectedAccountGroup);
            }
        }

    }
}