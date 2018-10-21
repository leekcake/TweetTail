using Library.Container.Account;
using Library.Container.Fetch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TweetTail.Pages.Status
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SearchPage : ContentPage
	{
        private AccountGroup issuer;

		public SearchPage (AccountGroup issuer)
		{
			InitializeComponent ();
            this.issuer = issuer;
        }

        private void SearchButton_Clicked(object sender, EventArgs e)
        {
            StatusListView.Fetchable = new AccountFetch.Search(App.Tail, issuer.AccountForRead, editKeyword.Text, true);
        }
    }
}