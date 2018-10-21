using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterInterface.Container;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TweetTail.Pages.Login
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginPage : ContentPage
	{
        private ILoginToken token;

        private bool isRegisterWrite = false;

		public LoginPage ()
		{
			InitializeComponent ();
		}

        private async void LoginCompleteButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                var account = await token.LoginAsync(PinEditor.Text);
                App.Tail.Account.AddAccount(account);
                if (!isRegisterWrite)
                {
                    var answer = await DisplayAlert("쓰기용 정보 등록", "트윗테일은 트윗 작성등을 위한 쓰기용 토큰과 읽기용 토큰을 구분할 수 있습니다. 지금 등록합니까?", "예", "아니요");
                    if (answer)
                    {
                        isRegisterWrite = true;
                        PinEditor.Text = "";
                        token = null;
                        LoginTryButton_Clicked(sender, e);
                        return;
                    }
                }

                if (App.Navigation.NavigationStack[0] == this)
                {
                    App.Navigation.PushAsync(new SingleTailPage());
                }
                App.Navigation.RemovePage(this);
            }
            catch (Exception ex)
            {
                StatusLabel.Text = ex.Message;
            }
        }

        private async void LoginTryButton_Clicked(object sender, EventArgs e)
        {
            if (token != null)
            {
                Device.OpenUri(new Uri(token.LoginURL));
                return;
            }
            if (isRegisterWrite)
            {
                token = await App.Tail.TwitterAPI.GetLoginTokenAsync(HiddenStore.ConsumerTokenForWrite);
            }
            else
            {
                token = await App.Tail.TwitterAPI.GetLoginTokenAsync(HiddenStore.ConsumerTokenForRead);
            }
            Device.OpenUri(new Uri(token.LoginURL));
        }

        private void LoginTDButton_Clicked(object sender, EventArgs e)
        {
            App.Navigation.PushAsync(new TDLoginPage());
            App.Navigation.RemovePage(this);
        }
    }
}