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
            blendListView.ItemTapped += BlendListView_ItemTapped;

            lblHeader.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    if(App.tail.blend.SelectedBlendName == null)
                    {
                        return;
                    }
                    App.tail.blend.SelectedBlendName = null;
                    SingleTailPage.ReloadInNavigationStack();
                    App.Navigation.RemovePage(this);
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

        private void BlendListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (App.tail.blend.SelectedBlendName == (string) e.Item)
            {
                return;
            }

            App.tail.blend.SelectedBlendName = e.Item as string;
            SingleTailPage.ReloadInNavigationStack();
            App.Navigation.RemovePage(this);
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