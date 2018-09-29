using Library.Container.Fetch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetTail.Status;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DataUser = TwitterInterface.Data.User;
using DataAccount = TwitterInterface.Data.Account;

namespace TweetTail.User
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UserDetailPage : ContentPage
	{
        private DataUser binding;
        private DataAccount issuer;

		public UserDetailPage (DataUser binding, DataAccount issuer)
		{
			InitializeComponent ();

            this.binding = binding;
            this.issuer = issuer;

            viewIssuer.BindingContext = issuer.user;
            viewIssuer.Update();

            imgHeader.Source = binding.profileBannerURL;
            imgProfile.Source = binding.profileHttpsImageURL;

            lblDescription.Text = binding.description;
            lblLink.Text = binding.url;
            lblLocation.Text = binding.location;
            lblNickname.Text = binding.nickName;
            lblScreenName.Text = binding.screenName;
            lblStatus.Text = string.Format("{0} 트윗 {1} 마음에 들어요 {2} 팔로워 {3} 팔로잉", binding.statusesCount, binding.favouritesCount, binding.followerCount, binding.followingCount);
		}

        private void btnTweet_Clicked(object sender, EventArgs e)
        {
            var listview = new StatusListView();
            listview.Fetchable = new AccountFetch.Userline(App.tail, issuer, binding);
            App.Navigation.PushAsync(new ContentPage() { Content = listview, Title = binding.nickName + "님의 트윗" });
        }

        private void btnMedia_Clicked(object sender, EventArgs e)
        {
            //TODO: Wait for Medialine API
        }

        private void btnMention_Clicked(object sender, EventArgs e)
        {
            //TODO: Wait for Search API

        }

        private void btnFavorite_Clicked(object sender, EventArgs e)
        {
            var listview = new StatusListView();
            listview.Fetchable = new AccountFetch.Favorites(App.tail, issuer, binding);
            App.Navigation.PushAsync(new ContentPage() { Content = listview, Title = binding.nickName + "님의 관심글" });
        }

        private void btnFollower_Clicked(object sender, EventArgs e)
        {
            var listview = new UserListView();
            listview.Fetchable = new AccountFetch.Followers(App.tail, issuer, binding);
            App.Navigation.PushAsync(new ContentPage() { Content = listview, Title = binding.nickName + "님의 팔로워" });
        }

        private void btnFollowing_Clicked(object sender, EventArgs e)
        {
            var listview = new UserListView();
            listview.Fetchable = new AccountFetch.Followings(App.tail, issuer, binding);
            App.Navigation.PushAsync(new ContentPage() { Content = listview, Title = binding.nickName + "님의 팔로잉" });
        }
    }
}