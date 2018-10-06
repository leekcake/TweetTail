using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TweetTail.Pages.Login
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TDLoginPage : ContentPage
	{
		public TDLoginPage ()
		{
			InitializeComponent ();
		}

        private async void btnExtract_Clicked(object sender, EventArgs e)
        {
            var account = await App.tail.twitter.GetAccountFromTweetdeckCookie(webTD.Cookies);
            App.tail.account.addAccount(account);

            if (App.Navigation.NavigationStack[0] == this)
            {
                App.Navigation.PushAsync(new SingleTailPage());
            }
            App.Navigation.RemovePage(this);
        }
    }
}