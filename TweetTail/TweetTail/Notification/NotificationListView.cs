using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using TweetTail.Utils;
using TwitterInterface.Data;
using Xamarin.Forms;
using DataNotification = TwitterInterface.Data.Notification;

namespace TweetTail.Notification
{
    public class NotificationListView : TwitterListView<DataNotification, NotificationCell>
    {
        public override long GetID(DataNotification data)
        {
            return data.maxPosition;
        }
    }
}
