using FFImageLoading.Forms;
using Library.Container.Account;
using Library.Container.Fetch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetTail.Components.Status;
using TweetTail.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TweetTail.Pages.Multi.Tails
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TimelineTail : Tail
	{
        private CachedImage WriteIcon;
        private StatusWriterView WriterView;

		public TimelineTail (AccountGroup group)
		{
			InitializeComponent ();

            HeaderView.Icon.Source = "ic_timeline_green_300_48dp";
            HeaderView.HeaderLabel.Text = "타임라인 @" + group.AccountForRead.User.ScreenName;

            WriterView = new StatusWriterView(group);
            WriterView.IsVisible = false;
            RootView.Children.Insert(1, WriterView);

            WriteIcon = HeaderView.AddIcon("ic_create_black_48dp");
            WriteIcon.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    WriterView.IsVisible = !WriterView.IsVisible;
                })
            });

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