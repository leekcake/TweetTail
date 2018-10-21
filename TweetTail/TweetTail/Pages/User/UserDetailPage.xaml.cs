using Library.Container.Fetch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DataUser = TwitterInterface.Data.User;
using DataAccount = TwitterInterface.Data.Account;
using TwitterInterface.Data;
using TweetTail.Utils;
using TweetTail.Components.Status;
using TweetTail.Components.User;
using TweetTail.Pages.Mute;

namespace TweetTail.Pages.User
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UserDetailPage : ContentPage
	{
        private DataUser binding;
        private DataAccount issuer;
        private Relationship relationship;

        private bool SetVisibleByText(Label label, string text)
        {
            if (text == null || text.Trim() == "")
            {
                label.IsVisible = false;
                return false;
            }
            else
            {
                label.IsVisible = true;
                return true;
            }
        }

        private void SetTextHideEmpty(Label label, string text)
        {
            if(text == null || text.Trim() == "")
            {
                label.IsVisible = false;
            }
            else
            {
                label.IsVisible = true;
                label.Text = text;
            }
        }

        private void SetRelationshipButtons(bool enable)
        {
            lblRelationshipStatus.IsVisible = !enable;
            btnFollow.IsEnabled = enable;
            btnBlock.IsEnabled = enable;
            btnMute.IsEnabled = enable;
            btnInternalMute.IsEnabled = enable;
        }

        private async void updateRelationship()
        {
            try
            {
                SetRelationshipButtons(false);

                //My account
                if(issuer.id == binding.id)
                {
                    SetRelationshipButtons(true);
                    viewRelationship.IsVisible = false;
                    viewFollowMe.IsVisible = false;
                    btnEdit.IsVisible = true;
                    return;
                }
                btnEdit.IsVisible = false;

                viewRelationship.IsVisible = true;

                relationship = await App.tail.twitter.GetRelationship(issuer, issuer.id, binding.id);
                updateUser();

                if (relationship.isBlocked)
                {
                    btnBlock.Text = "언블락";
                }
                else
                {
                    btnBlock.Text = "블락";
                }

                if (relationship.isMuted)
                {
                    btnMute.Text = "언뮤트";
                }
                else
                {
                    btnMute.Text = "뮤트";
                }

                if (relationship.isBlockedBy)
                {
                    SetRelationshipButtons(true);
                    btnFollow.Text = "차단됨";
                    btnFollow.IsEnabled = false;
                    
                    viewUserAction.IsVisible = false;
                    viewFollowMe.IsVisible = false;

                    lblDescription.Text = string.Format("{0}님을 팔로우 하거나 {0} 님의 트윗을 볼 수 없도록 차단되었습니다.", binding.nickName);
                    lblLink.IsVisible = false;
                    lblLocation.IsVisible = false;
                    lblStatus.IsVisible = false;
                    return;
                }
                viewUserAction.IsVisible = true;

                if (relationship.isFollowing)
                {
                    btnFollow.Text = "언팔로우";
                }
                else
                {
                    btnFollow.Text = "팔로우";
                }

                viewFollowMe.IsVisible = relationship.isFollower;
                SetRelationshipButtons(true);
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine( e.Message + " " + e.StackTrace );
            }
        }

        public void updateUser()
        {
            imgHeader.Source = binding.profileBannerURL;
            imgProfile.Source = binding.profileHttpsImageURL;

            if(SetVisibleByText(lblDescription, binding.description))
            {
                lblDescription.FormattedText = TwitterFormater.ParseFormattedString(binding.description, binding.descriptionEntities, new List<long>() { issuer.id });
            }
            if(SetVisibleByText(lblLink, binding.url))
            {
                lblLink.FormattedText = TwitterFormater.ParseFormattedString(binding.url, binding.urlURLEntity);
            }
            SetTextHideEmpty(lblLocation, binding.location);
            lblNickname.Text = binding.nickName;
            lblScreenName.Text = "@" + binding.screenName;
            lblStatus.Text = string.Format("{0} 트윗 {1} 마음에 들어요 {2} 팔로워 {3} 팔로잉", binding.statusesCount, binding.favouritesCount, binding.followerCount, binding.followingCount);
        }

        public void UpdateBinding(DataUser user)
        {
            binding = user;
            Title = binding.nickName + "님의 프로필";
            
            updateUser();
            updateRelationship();
        }

        public UserDetailPage (DataUser binding, DataAccount issuer)
		{
			InitializeComponent ();
            this.issuer = issuer;

            viewIssuer.BindingContext = issuer.user;
            viewIssuer.Update();

            UpdateBinding(binding);

            viewIssuerGroup.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    try
                    {
                        var account = await Util.SelectAccount("전환할 계정을 선택하세요");
                        this.issuer = account.accountForRead;

                        viewIssuer.BindingContext = this.issuer.user;
                        viewIssuer.Update();
                        updateRelationship();
                    }
                    catch(Exception e)
                    {
                        Util.HandleException(e);
                    }
                })
            });
        }

        private void btnTweet_Clicked(object sender, EventArgs e)
        {
            var listview = new StatusListView();
            listview.Fetchable = new AccountFetch.Userline(App.tail, issuer, binding);
            App.Navigation.PushAsync(new ContentPage() { Style = (Style) Application.Current.Resources["backgroundStyle"], Content = listview, Title = binding.nickName + "님의 트윗" });
        }

        private void btnMedia_Clicked(object sender, EventArgs e)
        {
            var listview = new StatusListView();
            listview.Fetchable = new AccountFetch.Medialine(App.tail, issuer, binding);
            App.Navigation.PushAsync(new ContentPage() { Style = (Style)Application.Current.Resources["backgroundStyle"], Content = listview, Title = binding.nickName + "님의 미디어" });
        }

        private void btnMention_Clicked(object sender, EventArgs e)
        {
            var listview = new StatusListView();
            listview.Fetchable = new AccountFetch.Search(App.tail, issuer, "to:@" + binding.screenName, true);
            App.Navigation.PushAsync(new ContentPage() { Style = (Style)Application.Current.Resources["backgroundStyle"], Content = listview, Title = binding.nickName + "님에게 가고있는 멘션" });
        }

        private void btnFavorite_Clicked(object sender, EventArgs e)
        {
            var listview = new StatusListView();
            listview.Fetchable = new AccountFetch.Favorites(App.tail, issuer, binding);
            App.Navigation.PushAsync(new ContentPage() { Style = (Style)Application.Current.Resources["backgroundStyle"], Content = listview, Title = binding.nickName + "님의 관심글" });
        }

        private void btnFollower_Clicked(object sender, EventArgs e)
        {
            var listview = new UserListView();
            listview.Fetchable = new AccountFetch.Followers(App.tail, issuer, binding);
            App.Navigation.PushAsync(new ContentPage() { Style = (Style)Application.Current.Resources["backgroundStyle"], Content = listview, Title = binding.nickName + "님의 팔로워" });
        }

        private void btnFollowing_Clicked(object sender, EventArgs e)
        {
            var listview = new UserListView();
            listview.Fetchable = new AccountFetch.Followings(App.tail, issuer, binding);
            App.Navigation.PushAsync(new ContentPage() { Style = (Style)Application.Current.Resources["backgroundStyle"], Content = listview, Title = binding.nickName + "님의 팔로잉" });
        }

        private async void btnFollow_Clicked(object sender, EventArgs e)
        {
            SetRelationshipButtons(false);
            try
            {
                if(relationship.isFollowing)
                {
                    await App.tail.twitter.DestroyFriendship(issuer, binding.id);
                }
                else
                {
                    await App.tail.twitter.CreateFriendship(issuer, binding.id);
                }
                updateRelationship();
            }
            catch(Exception ex)
            {
                Util.HandleException(ex);
            }
            SetRelationshipButtons(true);
        }

        private async void btnBlock_Clicked(object sender, EventArgs e)
        {
            SetRelationshipButtons(false);
            try
            {
                if (relationship.isBlocked)
                {
                    await App.tail.twitter.Unblock(issuer, binding.id);
                }
                else
                {
                    await App.tail.twitter.Block(issuer, binding.id);
                }
                updateRelationship();
            }
            catch (Exception ex)
            {
                Util.HandleException(ex);
            }
            SetRelationshipButtons(true);
        }

        private async void btnMute_Clicked(object sender, EventArgs e)
        {
            SetRelationshipButtons(false);
            try
            {
                if (relationship.isMuted)
                {
                    await App.tail.twitter.Unmute(issuer, binding.id);
                }
                else
                {
                    await App.tail.twitter.Mute(issuer, binding.id);
                }
                updateRelationship();
            }
            catch (Exception ex)
            {
                Util.HandleException(ex);
            }
            SetRelationshipButtons(true);
        }

        private void btnInternalMute_Clicked(object sender, EventArgs e)
        {
            try
            {
                App.Navigation.PushAsync(new UserMutePage(binding));
            }
            catch (Exception ex)
            {
                Util.HandleException(ex);
            }
        }

        private void btnEdit_Clicked(object sender, EventArgs e)
        {
            try
            {
                App.Navigation.PushAsync(new UserEditPage(issuer, this));
            }
            catch (Exception ex)
            {
                Util.HandleException(ex);
            }
        }
    }
}