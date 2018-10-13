using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetTail.Components;
using TweetTail.Components.Menu;
using TweetTail.Pages.Account;
using TweetTail.Pages.Blend;
using TweetTail.Pages.Mute;
using TweetTail.Pages.Status;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TweetTail.Pages.Menu
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MenuPage : ContentPage
	{
        public ObservableCollection<MenuData> Items { get; set; }

		public MenuPage ()
		{
            Title = "Menu";
			InitializeComponent ();

            Items = new ObservableCollection<MenuData>();

            Items.Add(new MenuData() {
                action = new Action(() =>
                {
                    App.Navigation.PushAsync(new AccountPage());
                })
            });
            Update();

            Items.Add(new MenuData()
            {
                Title = "계정 전환",
                Description = "계정을 전환합니다",
                Icon = "ic_account_box_grey_500_48dp",
                action = new Action(() =>
                {
                    App.Navigation.PushAsync(new BlendListPage());
                })
            });

            Items.Add(new MenuData()
            {
                Title = "검색 하기",
                Description = "트위터를 검색합니다",
                Icon = "ic_search_grey_500_48dp",
                action = new Action(() =>
                {
                    App.Navigation.PushAsync(new SearchPage(App.tail.account.SelectedAccountGroup));
                })
            });

            Items.Add(new MenuData()
            {
                Title = "뮤트목록",
                Description = "뮤트된 대상들을 확인하고 관리합니다",
                Icon = "ic_visibility_off_grey_500_24dp",
                action = new Action(() =>
                {
                    App.Navigation.PushAsync(new MutePage());
                })
            });

            listView.ItemTemplate = new DataTemplate(typeof(MenuCell));
            listView.ItemsSource = Items;
            listView.ItemTapped += ListView_ItemTapped;
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as MenuData;
            if(item.action != null)
            {
                item.action.Invoke();
            }

            (Parent as MasterDetailPage).IsPresented = false;
        }

        public void Update()
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

            var name = App.tail.blend.SelectedBlendName;
            if(name == null || name == "")
            {
                lblBlendStatus.IsVisible = false;
            }
            else
            {
                lblBlendStatus.IsVisible = true;
                lblBlendStatus.Text = name + " 병합 계정을 사용하고 있습니다.";
            }
        }
        
    }
}