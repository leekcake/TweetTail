using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetTail.Pages.Multi.Tails;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TweetTail.Pages.Multi
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TailSideMenuView : ContentView
	{
		public TailSideMenuView (Tail tail)
		{
			InitializeComponent ();
            IconImage.Source = tail.Icon;
		}
	}
}