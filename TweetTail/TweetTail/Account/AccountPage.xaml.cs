using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TweetTail.Account
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AccountPage : ContentPage
	{
		public AccountPage ()
		{
			InitializeComponent ();

            foreach(var accountGroup in App.tail.account.readOnlyAccountGroups)
            {
                accountListView.Items.Add(accountGroup.accountForRead);
            }
		}
	}
}