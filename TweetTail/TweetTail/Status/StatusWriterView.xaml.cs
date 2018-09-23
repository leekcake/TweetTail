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
	public partial class StatusWriterView : ContentView
	{
		public StatusWriterView ()
		{
			InitializeComponent ();
		}

        private async void btnTweet_Clicked(object sender, EventArgs e)
        {
            if(editText.Text.Trim() == "")
            {
                //TODO: Notify
                return;
            }

            var update = new StatusUpdate();

            update.text = editText.Text;

            await App.tail.twitter.CreateStatus(App.tail.account.SelectedAccountGroup.accountForWrite, update);

            //TODO: Close Parent
        }
    }
}