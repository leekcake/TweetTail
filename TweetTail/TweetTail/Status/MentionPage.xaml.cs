using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterInterface.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TweetTail.Status
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MentionPage : ContentPage
	{
		public MentionPage ()
		{
			InitializeComponent ();

            StatusListView.statusGetter = MentionGetter;
            StatusListView.Refresh();
        }

        private Task<List<TwitterInterface.Data.Status>> MentionGetter(long sinceId, long maxId)
        {
            return App.tail.twitter.GetMentionline(App.tail.account.SelectedAccountGroup.accountForRead, 200, sinceId, maxId);
        }
    }
}