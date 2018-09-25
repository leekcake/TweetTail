using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetTail.Account;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TweetTail.Menu
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MenuPage : ContentPage
	{
        public class Item
        {
            public string Title {
                get; set;
            }

            public string Description {
                get; set;
            }

            public string Icon {
                get; set;
            }

            public Action action;
        }
        public ObservableCollection<Item> Items { get; set; }

		public MenuPage ()
		{
            Title = "Menu";
			InitializeComponent ();

            Items = new ObservableCollection<Item>();


            Items.Add(new Item() {
                action = new Action(() =>
                {
                    App.Navigation.PushAsync(new AccountPage());
                })
            });
            UpdateUser();

            Items.Add(new Item()
            {
                Title = "계정 전환",
                Description = "계정을 전환합니다",
                Icon = "ic_account_box_black_48dp"
            });

            listView.ItemsSource = Items;
            listView.ItemTapped += ListView_ItemTapped;
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as Item;
            if(item.action != null)
            {
                item.action.Invoke();
            }

            (Parent as MasterDetailPage).IsPresented = false;
        }

        public void UpdateUser()
        {
            var item = Items[0];
            try
            {
                var user = App.tail.account.SelectedAccountGroup.accountForRead.user;
                item.Title = user.nickName + " @" + user.screenName;
                item.Description = user.description;
                item.Icon = user.profileHttpsImageURL;
            }
            catch(NullReferenceException nre)
            {
                item.Title = "알 수 없음";
                item.Description = "메인 유저 계정을 사용할 수 없습니다";
                item.Icon = "ic_delete_black_24dp.png";
            }

            //Notify to Refresh
            Items[0] = item;
        }
        
    }
}