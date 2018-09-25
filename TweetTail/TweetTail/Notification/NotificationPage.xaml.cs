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

            notificationListView.SinceMaxGetter = NotificationGetter;
            notificationListView.Refresh();
        }

        public void Reload()
        {
            notificationListView.Reload();
        }

        public Task<List<DataNotification>> NotificationGetter(long sinceId, long maxId)
        {
            if(App.tail.blend.SelectedBlendedAccount != null)
            {
                return App.tail.blend.SelectedBlendedAccount.getNotifications(40, sinceId, maxId);
            }
            return App.tail.twitter.GetNotifications(App.tail.account.SelectedAccountGroup.accountForRead, 200, sinceId, maxId);
        }
    }
}