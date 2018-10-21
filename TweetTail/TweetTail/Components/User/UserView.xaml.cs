using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DataUser = TwitterInterface.Data.User;

namespace TweetTail.Components.User
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserView : ContentView
    {
        public UserView()
        {
            InitializeComponent();
        }

        public void Update()
        {
            if (BindingContext is DataUser) { }
            else
            {
                return;
            }

            var user = BindingContext as DataUser;

            ProfileImage.Source = user.ProfileHttpsImageURL;
            LockImage.IsVisible = user.IsProtected;
            NameLabel.Text = user.NickName + " @" + user.ScreenName;
            TextLabel.Text = user.Description;
        }
    }
}