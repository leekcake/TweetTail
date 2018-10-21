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

using DataMute = TwitterInterface.Data.Mute;

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
                Action = new Action(() =>
                {
                    var page = new MuteListPage( typeof(UserMuteCell) );
                    page.Title = "뮤트된 유저 목록";
                    page.RefreshFunc = new Func<IEnumerable<TwitterInterface.Data.Mute>>(() =>
                    {
                        return App.Tail.Mute.ReadonlyUserMutes;
                    });
                    page.ListView.ItemTapped += new EventHandler<ItemTappedEventArgs>((sender, e) =>
                    {
                        App.Navigation.PushAsync(new UserMutePage( ((e.Item as DataMute).Target as DataMute.UserTarget).User ));
                    });
                    App.Navigation.PushAsync(page);
                })
            });

            Items.Add(new MenuData()
            {
                Title = "키워드 뮤트",
                Action = new Action(() =>
                {
                    var page = new MuteListPage( typeof(KeywordMuteCell) );
                    page.Title = "뮤트된 키워드 목록";
                    page.ListView.RowHeight = 48;
                    page.RefreshFunc = new Func<IEnumerable<TwitterInterface.Data.Mute>>(() =>
                    {
                        return App.Tail.Mute.ReadonlyKeywordMutes;
                    });
                    page.ListView.ItemTapped += new EventHandler<ItemTappedEventArgs>(async (sender, e) =>
                    {
                        var mute = (e.Item as DataMute);
                        var target = (mute.Target as DataMute.KeywordTarget);

                        //TODO: Replace with Long press
                        if (await DisplayAlert("작업 확인", target.Keyword + " 단어 뮤트를 삭제할까요?", "네", "아니요"))
                        {
                            App.Tail.Mute.UnregisterMute(mute);
                            page.Refresh();
                            return;
                        }
                        if (await DisplayAlert("작업 확인", target.Keyword + " 단어 뮤트를 편집할까요?", "네", "아니요"))
                        {
                            App.Navigation.PushAsync(new KeywordMutePage(mute));
                            return;
                        }
                    });
                    page.ActionButton.IsVisible = true;
                    page.ActionButton.Clicked += new EventHandler((sender, e) =>
                    {
                        App.Navigation.PushAsync(new KeywordMutePage());
                    });
                    App.Navigation.PushAsync(page);
                })
            });

            Items.Add(new MenuData()
            {
                Title = "트윗 뮤트",
                Action = new Action(() =>
                {
                    var page = new MuteListPage( typeof(StatusMuteCell) );
                    page.Title = "뮤트된 트윗 목록";
                    page.RefreshFunc = new Func<IEnumerable<TwitterInterface.Data.Mute>>(() =>
                    {
                        return App.Tail.Mute.ReadonlyStatusMutes;
                    });
                    page.ListView.ItemTapped += new EventHandler<ItemTappedEventArgs>(async (sender, e) =>
                    {
                        var mute = (e.Item as DataMute);
                        var target = (mute.Target as DataMute.StatusTarget);

                        //TODO: Replace with Long press
                        if (await DisplayAlert("작업 확인", target.Status.Text + " 트윗 뮤트를 삭제할까요?", "네", "아니요"))
                        {
                            App.Tail.Mute.UnregisterMute(mute);
                            page.Refresh();
                            return;
                        }
                    });
                    App.Navigation.PushAsync(page);
                })
            });

            MenuListView.ItemTemplate = new DataTemplate(typeof(MenuCell));
            MenuListView.ItemsSource = Items;
            MenuListView.ItemTapped += ListView_ItemTapped;
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as MenuData;
            if (item.Action != null)
            {
                item.Action.Invoke();
            }
        }
    }
}