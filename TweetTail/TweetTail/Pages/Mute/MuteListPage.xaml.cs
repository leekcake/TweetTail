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

        public Func<IEnumerable<DataMute>> refreshFunc;

        public MuteListPage (Type cell)
		{
			InitializeComponent ();

            Style = Application.Current.Resources["backgroundStyle"] as Style;

            Items = new ObservableCollection<DataMute>();
            lv.ItemTemplate = new DataTemplate(cell);
            lv.ItemsSource = Items;
        }

        public void Refresh()
        {
            Items.Clear();
            foreach(var item in refreshFunc())
            {
                Items.Add(item);
            }
        }
	}
}