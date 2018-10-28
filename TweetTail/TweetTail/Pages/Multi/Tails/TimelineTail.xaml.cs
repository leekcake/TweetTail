using Library.Container.Account;
using Library.Container.Fetch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetTail.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TweetTail.Pages.Multi.Tails
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TimelineTail : Tail
	{
		public TimelineTail (AccountGroup group)
		{
			InitializeComponent ();

            HeaderView.Icon.Source = "ic_timeline_green_300_48dp";
            HeaderView.HeaderLabel.Text = "타임라인 @" + group.AccountForRead.User.ScreenName;

            TimelineListView.Fetchable = new AccountFetch.Timeline(App.Tail, group);

            HeaderView.RefreshAction += new Action(async () =>
            {
                try
                {
                    await TimelineListView.Refresh();
                }
                catch (Exception e)
                {
                    Util.HandleException(e);
                }
                HeaderView.InRefresh = false;
            });
        }
    }
}