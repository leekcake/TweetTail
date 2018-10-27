using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetTail.Pages.Multi;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TweetTail.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoadingPage : ContentPage
	{
		public LoadingPage ()
		{
			InitializeComponent ();
#pragma warning disable CS4014 // 이 호출을 대기하지 않으므로 호출이 완료되기 전에 현재 메서드가 계속 실행됩니다.
            StartVerify();
#pragma warning restore CS4014 // 이 호출을 대기하지 않으므로 호출이 완료되기 전에 현재 메서드가 계속 실행됩니다.
        }

        public async Task StartVerify()
        {
            await App.Tail.Account.VerifyAccounts();
            await Task.Delay(100); //for WPF

            try
            {
                await App.Navigation.PushAsync(new MutliTailPage());
                App.Navigation.RemovePage(this);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message + " " + e.StackTrace);
            }
        }
	}
}