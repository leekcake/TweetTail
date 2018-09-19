using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DataStatus = TwitterInterface.Data.Status;

namespace TweetTail.Status
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatusPage : ContentPage
    {
        public ObservableCollection<DataStatus> Items { get; set; }

        public StatusPage()
        {
            InitializeComponent();

            Items = new ObservableCollection<DataStatus>();
			
			MyListView.ItemsSource = Items;

            LoadData();
        }

        async void LoadData()
        {
            var data = await App.tail.twitter.GetTimeline(App.tail.account.readOnlyAccountGroups[0].accountForRead);
            foreach(var status in data)
            {
                Items.Add(status);
            }
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}
