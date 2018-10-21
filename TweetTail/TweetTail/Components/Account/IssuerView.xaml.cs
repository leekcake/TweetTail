using FFImageLoading.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DataUser = TwitterInterface.Data.User;

namespace TweetTail.Components.Account
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IssuerView : ContentView
    {
        private List<CachedImage> issuerImages = new List<CachedImage>();
        private void AddNewIssuer()
        {
            var image = new CachedImage();
            image.WidthRequest = 16;
            image.HeightRequest = 16;
            image.Margin = new Thickness(0, 0, 4, 0);
            RootView.Children.Add(image);
            issuerImages.Add(image);
        }

        private void ResetAll()
        {
            foreach(var image in issuerImages)
            {
                image.Source = null;
                image.IsVisible = false;
            }
        }

        private void ShowIssuer(int inx, DataUser issuer)
        {
            if(inx == issuerImages.Count)
            {
                AddNewIssuer();
            }
            var image = issuerImages[inx];
            image.IsVisible = true;
            if(issuer == null)
            {
                return;
            }
            image.Source = issuer.ProfileHttpsImageURL;
        }

        public IssuerView()
        {
            InitializeComponent();
        }

        public void Update()
        {
            ResetAll();
            if (BindingContext is List<long>) { }
            else
            {
                return;
            }
            var array = BindingContext as List<long>;
            for(int i = 0; i < array.Count; i++)
            {
                ShowIssuer(i, App.Tail.Account.GetAccountGroup(array[i]).AccountForRead.User);
            }
        }
    }
}