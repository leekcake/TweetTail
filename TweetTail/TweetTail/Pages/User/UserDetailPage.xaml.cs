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
            RelationshipStatusLabel.IsVisible = !enable;
            FollowButton.IsEnabled = enable;
            BlockButton.IsEnabled = enable;
            MuteButton.IsEnabled = enable;
            InternalMuteButton.IsEnabled = enable;
        }

        private async void UpdateRelationship()
        {
            try
            {
                SetRelationshipButtons(false);

                //My account
                if(issuer.ID == binding.ID)
                {
                    SetRelationshipButtons(true);
                    RelationshipGrid.IsVisible = false;
                    FollowMeFrame.IsVisible = false;
                    EditButton.IsVisible = true;
                    return;
                }
                EditButton.IsVisible = false;

                RelationshipGrid.IsVisible = true;

                relationship = await App.Tail.TwitterAPI.GetRelationshipAsync(issuer, issuer.ID, binding.ID);
                UpdateUser();

                if (relationship.IsBlocked)
                {
                    BlockButton.Text = "언블락";
                }
                else
                {
                    BlockButton.Text = "블락";
                }

                if (relationship.IsMuted)
                {
                    MuteButton.Text = "언뮤트";
                }
                else
                {
                    MuteButton.Text = "뮤트";
                }

                if (relationship.IsBlockedBy)
                {
                    SetRelationshipButtons(true);
                    FollowButton.Text = "차단됨";
                    FollowButton.IsEnabled = false;
                    
                    UserActionGrid.IsVisible = false;
                    FollowMeFrame.IsVisible = false;

                    DescriptionLabel.Text = string.Format("{0}님을 팔로우 하거나 {0} 님의 트윗을 볼 수 없도록 차단되었습니다.", binding.NickName);
                    LinkLabel.IsVisible = false;
                    LocationLabel.IsVisible = false;
                    StatusLabel.IsVisible = false;
                    return;
                }
                UserActionGrid.IsVisible = true;

                if (relationship.IsFollowing)
                {
                    FollowButton.Text = "언팔로우";
                }
                else
                {
                    FollowButton.Text = "팔로우";
                }

                FollowMeFrame.IsVisible = relationship.IsFollower;
                SetRelationshipButtons(true);
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine( e.Message + " " + e.StackTrace );
            }
        }

        public void UpdateUser()
        {
            HeaderImage.Source = binding.ProfileBannerURL;
            ProfileImage.Source = binding.ProfileHttpsImageURL;

            if(SetVisibleByText(DescriptionLabel, binding.Description))
            {
                DescriptionLabel.FormattedText = TwitterFormater.ParseFormattedString(binding.Description, binding.descriptionEntities, new List<long>() { issuer.ID });
            }
            if(SetVisibleByText(LinkLabel, binding.URL))
            {
                LinkLabel.FormattedText = TwitterFormater.ParseFormattedString(binding.URL, binding.URLEntities);
            }
            SetTextHideEmpty(LocationLabel, binding.Location);
            NickNameLabel.Text = binding.NickName;
            ScreenNameLabel.Text = "@" + binding.ScreenName;
            StatusLabel.Text = string.Format("{0} 트윗 {1} 마음에 들어요 {2} 팔로워 {3} 팔로잉", binding.StatusesCount, binding.FavouritesCount, binding.FollowerCount, binding.FollowingCount);
        }

        public void UpdateBinding(DataUser user)
        {
            binding = user;
            Title = binding.NickName + "님의 프로필";
            
            UpdateUser();
            UpdateRelationship();
        }

        public UserDetailPage (DataUser binding, DataAccount issuer)
		{
			InitializeComponent ();
            this.issuer = issuer;

            IssuerView.BindingContext = issuer.User;
            IssuerView.Update();

            UpdateBinding(binding);

            IssuerGroupView.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    try
                    {
                        var account = await Util.SelectAccount("전환할 계정을 선택하세요");
                        this.issuer = account.AccountForRead;

                        IssuerView.BindingContext = this.issuer.User;
                        IssuerView.Update();
                        UpdateRelationship();
                    }
                    catch(Exception e)
                    {
                        Util.HandleException(e);
                    }
                })
            });
        }

        private void TweetButton_Clicked(object sender, EventArgs e)
        {
            var listview = new StatusListView();
            listview.Fetchable = new AccountFetch.Userline(App.Tail, issuer, binding);
            App.Navigation.PushAsync(new ContentPage() { Style = (Style) Application.Current.Resources["backgroundStyle"], Content = listview, Title = binding.NickName + "님의 트윗" });
        }

        private void MediaButton_Clicked(object sender, EventArgs e)
        {
            var listview = new StatusListView();
            listview.Fetchable = new AccountFetch.Medialine(App.Tail, issuer, binding);
            App.Navigation.PushAsync(new ContentPage() { Style = (Style)Application.Current.Resources["backgroundStyle"], Content = listview, Title = binding.NickName + "님의 미디어" });
        }

        private void MentionButton_Clicked(object sender, EventArgs e)
        {
            var listview = new StatusListView();
            listview.Fetchable = new AccountFetch.Search(App.Tail, issuer, "to:@" + binding.ScreenName, true);
            App.Navigation.PushAsync(new ContentPage() { Style = (Style)Application.Current.Resources["backgroundStyle"], Content = listview, Title = binding.NickName + "님에게 가고있는 멘션" });
        }

        private void FavoriteButton_Clicked(object sender, EventArgs e)
        {
            var listview = new StatusListView();
            listview.Fetchable = new AccountFetch.Favorites(App.Tail, issuer, binding);
            App.Navigation.PushAsync(new ContentPage() { Style = (Style)Application.Current.Resources["backgroundStyle"], Content = listview, Title = binding.NickName + "님의 관심글" });
        }

        private void FollowerButton_Clicked(object sender, EventArgs e)
        {
            var listview = new UserListView();
            listview.Fetchable = new AccountFetch.Followers(App.Tail, issuer, binding);
            App.Navigation.PushAsync(new ContentPage() { Style = (Style)Application.Current.Resources["backgroundStyle"], Content = listview, Title = binding.NickName + "님의 팔로워" });
        }

        private void FollowingButton_Clicked(object sender, EventArgs e)
        {
            var listview = new UserListView();
            listview.Fetchable = new AccountFetch.Followings(App.Tail, issuer, binding);
            App.Navigation.PushAsync(new ContentPage() { Style = (Style)Application.Current.Resources["backgroundStyle"], Content = listview, Title = binding.NickName + "님의 팔로잉" });
        }

        private async void FollowButton_Clicked(object sender, EventArgs e)
        {
            SetRelationshipButtons(false);
            try
            {
                if(relationship.IsFollowing)
                {
                    await App.Tail.TwitterAPI.DestroyFriendshipAsync(issuer, binding.ID);
                }
                else
                {
                    await App.Tail.TwitterAPI.CreateFriendshipAsync(issuer, binding.ID);
                }
                UpdateRelationship();
            }
            catch(Exception ex)
            {
                Util.HandleException(ex);
            }
            SetRelationshipButtons(true);
        }

        private async void BlockButton_Clicked(object sender, EventArgs e)
        {
            SetRelationshipButtons(false);
            try
            {
                if (relationship.IsBlocked)
                {
                    await App.Tail.TwitterAPI.UnblockAsync(issuer, binding.ID);
                }
                else
                {
                    await App.Tail.TwitterAPI.BlockAsync(issuer, binding.ID);
                }
                UpdateRelationship();
            }
            catch (Exception ex)
            {
                Util.HandleException(ex);
            }
            SetRelationshipButtons(true);
        }

        private async void MuteButton_Clicked(object sender, EventArgs e)
        {
            SetRelationshipButtons(false);
            try
            {
                if (relationship.IsMuted)
                {
                    await App.Tail.TwitterAPI.UnmuteAsync(issuer, binding.ID);
                }
                else
                {
                    await App.Tail.TwitterAPI.MuteAsync(issuer, binding.ID);
                }
                UpdateRelationship();
            }
            catch (Exception ex)
            {
                Util.HandleException(ex);
            }
            SetRelationshipButtons(true);
        }

        private void InternalMuteButton_Clicked(object sender, EventArgs e)
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

        private void EditButton_Clicked(object sender, EventArgs e)
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