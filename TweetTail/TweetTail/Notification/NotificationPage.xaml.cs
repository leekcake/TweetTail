using Library.Container.Fetch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DataNotification = TwitterInterface.Data.Notification;

namespace TweetTail.Notification
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NotificationPage : ContentPage
	{
		public NotificationPage ()
		{
			InitializeComponent ();

            Reload();
        }

        public void Reload()
        {
            if (App.tail.blend.SelectedBlendedAccount != null)
            {
                notificationListView.Fetchable = new BlendAccountFetch<DataNotification>.Notifications(App.tail.blend.SelectedBlendedAccount);
            }
            else
            {
                notificationListView.Fetchable = new AccountFetch.Notifications(App.tail, App.tail.account.SelectedAccountGroup.accountForRead);
            }
        }

    }
}