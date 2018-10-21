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

        private async void ExtractButton_Clicked(object sender, EventArgs e)
        {
            var account = await App.Tail.TwitterAPI.GetAccountFromTweetdeckCookieAsync(TDCookieWebView.Cookies);
            App.Tail.Account.AddAccount(account);

            if (App.Navigation.NavigationStack[0] == this)
            {
#pragma warning disable CS4014 // 이 호출을 대기하지 않으므로 호출이 완료되기 전에 현재 메서드가 계속 실행됩니다.
                App.Navigation.PushAsync(new SingleTailPage());
#pragma warning restore CS4014 // 이 호출을 대기하지 않으므로 호출이 완료되기 전에 현재 메서드가 계속 실행됩니다.
            }
            App.Navigation.RemovePage(this);
        }
    }
}