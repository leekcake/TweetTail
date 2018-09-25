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

            StatusListView.SinceMaxGetter = MentionGetter;
            StatusListView.Refresh();
        }

        public void Reload()
        {
            StatusListView.Reload();
        }

        private Task<List<TwitterInterface.Data.Status>> MentionGetter(long sinceId, long maxId)
        {
            if(App.tail.blend.SelectedBlendedAccount != null)
            {
                return App.tail.blend.SelectedBlendedAccount.getMentionline(200, sinceId, maxId);
            }
            return App.tail.twitter.GetMentionline(App.tail.account.SelectedAccountGroup.accountForRead, 200, sinceId, maxId);
        }
    }
}