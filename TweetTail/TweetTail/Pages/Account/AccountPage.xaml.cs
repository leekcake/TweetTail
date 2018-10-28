using Library.Container.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetTail.Pages.Login;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TweetTail.Pages.Account
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AccountPage : ContentPage
	{
		public AccountPage ()
		{
			InitializeComponent ();

            foreach(var accountGroup in App.Tail.Account.ReadOnlyAccountGroups)
            {
                AccountListView.Items.Add(accountGroup);
            }
            AccountListView.ItemTapped += AccountListView_ItemTapped;

            var view = AccountListView.Footer as StackLayout;
            view.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    App.Navigation.PushAsync(new LoginPage());
                })
            });
		}

        private void AccountListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as AccountGroup;
            if(item.ID != App.Tail.Account.SelectedAccountId)
            {
                App.Tail.Account.SelectedAccountId = item.ID;
                SingleTailPage.ReloadInNavigationStack();
                App.Navigation.RemovePage(this);
            }
        }
    }
}