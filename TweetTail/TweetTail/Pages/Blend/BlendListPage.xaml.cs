using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TweetTail.Pages.Blend
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
            
            BlendListView.ItemsSource = Items;
            BlendListView.ItemTapped += BlendListView_ItemTapped;

            HeaderLabel.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    if(App.Tail.Blend.SelectedBlendName == null)
                    {
                        return;
                    }
                    App.Tail.Blend.SelectedBlendName = null;
                    SingleTailPage.ReloadInNavigationStack();
                    App.Navigation.RemovePage(this);
                })
            });

            FooterLabel.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    App.Navigation.PushAsync(new BlendEditPage());
                })
            });
        }

        private void BlendListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (App.Tail.Blend.SelectedBlendName == (string) e.Item)
            {
                return;
            }

            App.Tail.Blend.SelectedBlendName = e.Item as string;
            SingleTailPage.ReloadInNavigationStack();
            App.Navigation.RemovePage(this);
        }

        public void Reload()
        {
            Items.Clear();
            foreach (var blend in App.Tail.Blend.ReadOnlyBlendedAccounts)
            {
                Items.Add(blend.Name);
            }
        }
	}
}