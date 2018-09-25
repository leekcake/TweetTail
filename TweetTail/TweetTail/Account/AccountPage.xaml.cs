using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetTail.Login;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DataAccount = TwitterInterface.Data.Account;

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
            accountListView.ItemTapped += AccountListView_ItemTapped;

            var view = accountListView.Footer as StackLayout;
            view.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    App.Navigation.PushAsync(new LoginView());
                })
            });
		}

        private void AccountListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as DataAccount;
            if(item.id != App.tail.account.SelectedAccountId)
            {
                App.tail.account.SelectedAccountId = item.id;
                SingleTailPage.ReloadInNavigationStack();
                App.Navigation.RemovePage(this);
            }
        }
    }
}