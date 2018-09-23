using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }
        public ObservableCollection<Item> Items { get; set; }

		public MenuPage ()
		{
            Title = "Menu";
			InitializeComponent ();

            Items = new ObservableCollection<Item>();

            Items.Add(new Item()
            {
                Title = "계정 전환",
                Description = "계정을 전환합니다",
                Icon = "ic_account_box_black_48dp"
            });

            listView.ItemsSource = Items;
        }
	}
}