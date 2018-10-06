using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            StartVerify();
		}

        public async Task StartVerify()
        {
            await App.tail.account.VerifyAccounts();

            try
            {
                await App.Navigation.PushAsync(new SingleTailPage());
                App.Navigation.RemovePage(this);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message + " " + e.StackTrace);
            }
        }
	}
}