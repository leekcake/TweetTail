using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TweetTail.Blend
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BlendListPage : ContentPage
	{
        public ObservableCollection<string> Items { get; set; }

        public BlendListPage ()
		{
			InitializeComponent ();

            Items = new ObservableCollection<string>();
            Reload();

            blendListView.ItemsSource = Items;

            lblHeader.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {

                })
            });

            lblFooter.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    App.Navigation.PushAsync(new BlendEditPage());
                })
            });
        }

        public void Reload()
        {
            Items.Clear();
            foreach (var blend in App.tail.blend.readOnlyBlendedAccounts)
            {
                Items.Add(blend.name);
            }
        }
	}
}