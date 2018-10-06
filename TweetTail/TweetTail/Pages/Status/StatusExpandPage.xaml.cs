using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DataUser = TwitterInterface.Data.User;
using DataStatus = TwitterInterface.Data.Status;
using TweetTail.Utils;

namespace TweetTail.Pages.Status
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class StatusExpandPage : ContentPage
	{
        private DataStatus parent;

		public StatusExpandPage (DataStatus parent)
		{
            this.parent = parent;
			InitializeComponent();
            statusLV.Items.Add(parent);

            Fetch();
		}

        public async Task Fetch()
        {
            try
            {
                var fetch = await App.tail.twitter.GetConversation(App.tail.account.getAccountGroup(parent.issuer[0]).accountForRead, parent.id);

                statusLV.Items.Clear();
                DataStatus orig = null;
                foreach(var status in fetch)
                {
                    if(status.id == parent.id)
                    {
                        orig = status;
                    }
                    statusLV.Items.Add(status);
                }
                if (orig != null) {
                    statusLV.ScrollTo(orig, ScrollToPosition.MakeVisible, true);
                }
            }
            catch(Exception e)
            {
                Util.HandleException(e);
            }
        }
	}
}