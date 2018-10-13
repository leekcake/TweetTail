using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetTail.Components.Menu;
using TweetTail.Components.Mute;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TweetTail.Pages.Mute
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MutePage : ContentPage
	{
        public ObservableCollection<MenuData> Items { get; set; }

        public MutePage ()
		{
            Title = "뮤트";
            InitializeComponent();

            Items = new ObservableCollection<MenuData>();
            
            Items.Add(new MenuData()
            {
                Title = "유저 뮤트",
                action = new Action(() =>
                {
                    var page = new MuteListPage( typeof(UserMuteCell) );
                    page.Title = "뮤트된 유저 목록";
                    page.refreshFunc = new Func<IEnumerable<TwitterInterface.Data.Mute>>(() =>
                    {
                        return App.tail.mute.ReadonlyKeywordMutes;
                    });
                    page.Refresh();
                    App.Navigation.PushAsync(page);
                })
            });

            Items.Add(new MenuData()
            {
                Title = "키워드 뮤트",
                action = new Action(() =>
                {
                    var page = new MuteListPage( typeof(KeywordMuteCell) );
                    page.Title = "뮤트된 키워드 목록";
                    page.refreshFunc = new Func<IEnumerable<TwitterInterface.Data.Mute>>(() =>
                    {
                        return App.tail.mute.ReadonlyKeywordMutes;
                    });
                    page.Refresh();
                    page.fabAction.IsVisible = true;
                    page.fabAction.Clicked += new EventHandler((sender, e) =>
                    {

                    });
                    App.Navigation.PushAsync(page);
                })
            });

            Items.Add(new MenuData()
            {
                Title = "트윗 뮤트",
                action = new Action(() =>
                {
                    var page = new MuteListPage( typeof(StatusMuteCell) );
                    page.Title = "뮤트된 트윗 목록";
                    page.refreshFunc = new Func<IEnumerable<TwitterInterface.Data.Mute>>(() =>
                    {
                        return App.tail.mute.ReadonlyStatusMutes;
                    });
                    page.Refresh();
                    App.Navigation.PushAsync(page);
                })
            });

            lvMenu.ItemTemplate = new DataTemplate(typeof(MenuCell));
            lvMenu.ItemsSource = Items;
            lvMenu.ItemTapped += ListView_ItemTapped;
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as MenuData;
            if (item.action != null)
            {
                item.action.Invoke();
            }
        }
    }
}