using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DataMute = TwitterInterface.Data.Mute;

namespace TweetTail.Pages.Mute
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MuteListPage : ContentPage
	{
        public ObservableCollection<DataMute> Items { get; set; }

        public Func<IEnumerable<DataMute>> RefreshFunc;

        public MuteListPage (Type cell)
		{
			InitializeComponent ();

            Style = Application.Current.Resources["backgroundStyle"] as Style;

            Items = new ObservableCollection<DataMute>();
            ListView.ItemTemplate = new DataTemplate(cell);
            ListView.ItemsSource = Items;
        }

        public void Refresh()
        {
            Items.Clear();
            foreach(var item in RefreshFunc())
            {
                Items.Add(item);
            }
        }

        protected override void OnAppearing()
        {
            Refresh();
            base.OnAppearing();
        }
    }
}