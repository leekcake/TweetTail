using Library.Container.Account;
using Library.Container.Fetch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            HeaderLabel.Text = "타임라인 @" + group.AccountForRead.User.ScreenName;
            TimelineListView.Fetchable = new AccountFetch.Timeline(App.Tail, group.AccountForRead);
        }
	}
}