using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DataStatus = TwitterInterface.Data.Status;
using DataUser = TwitterInterface.Data.User;
using DataNotification = TwitterInterface.Data.Notification;
using TweetTail.Pages.User;

namespace TweetTail.Components.Notification
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NotificationCell : ViewCell
	{
		public NotificationCell ()
		{
			InitializeComponent ();
            StatusView.HeaderView.GestureRecognizers.Clear();
            StatusView.HeaderView.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    var notification = BindingContext as DataNotification;

                    DataUser performer = null;

                    if (notification is DataNotification.Follow)
                    {
                        performer = (notification as DataNotification.Follow).Performer;
                    }
                    else if(notification is DataNotification.Retweet)
                    {
                        performer = (notification as DataNotification.Retweet).Performer;
                    }
                    else if (notification is DataNotification.RetweetedMention)
                    {
                        performer = (notification as DataNotification.RetweetedMention).Performer;
                    }
                    else if (notification is DataNotification.RetweetedRetweet)
                    {
                        performer = (notification as DataNotification.RetweetedRetweet).Performer;
                    }
                    else if (notification is DataNotification.Favorited)
                    {
                        performer = (notification as DataNotification.Favorited).Performer;
                    }
                    else if (notification is DataNotification.FavoritedMention)
                    {
                        performer = (notification as DataNotification.FavoritedMention).Performer;
                    }
                    else if (notification is DataNotification.FavoritedRetweet)
                    {
                        performer = (notification as DataNotification.FavoritedRetweet).Performer;
                    }

                    if(performer == null)
                    {
                        return;
                    }
                    App.Navigation.PushAsync(new UserDetailPage(performer, App.Tail.Account.GetAccountGroup(performer.Issuer[0]).AccountForRead));
                })
            });
		}

        protected override void OnBindingContextChanged()
        {
            var notification = BindingContext as DataNotification;
            
            //Display User with StatusCell
            if(notification is DataNotification.Follow)
            {
                var follow = notification as DataNotification.Follow;
                StatusView.ButtonsView.IsVisible = false;
                StatusView.MediaGrid.IsVisible = false;
                StatusView.HeaderView.IsVisible = true;

                StatusView.BindingContext = follow.Performer;
                StatusView.HeaderImage.Source = "ic_person_add_light_green_500_24dp";
                StatusView.HeaderLabel.Text = follow.Performer.NickName + " 님이 팔로우 하셨습니다.";

                StatusView.ProfileImage.Source = follow.Performer.ProfileHttpsImageURL;
                StatusView.NameLabel.Text = follow.Performer.NickName + " @" + follow.Performer.ScreenName;
                StatusView.TextLabel.Text = follow.Performer.Description;

                return;
            }
            if(notification is DataNotification.Retweet)
            {
                var data = notification as DataNotification.Retweet;
                DisplayNotification(data.RetweetTargetTweet, "ic_repeat_green_300_24dp",
                    data.Performer.NickName + " 님이 리트윗 하였습니다");
            }
            else if(notification is DataNotification.RetweetedMention)
            {
                var data = notification as DataNotification.RetweetedMention;
                DisplayNotification(data.RetweetedTweet, "ic_repeat_green_300_24dp",
                    data.Performer.NickName + " 님이 내가 멘션된 트윗을 리트윗 하였습니다");
            }
            else if(notification is DataNotification.RetweetedRetweet)
            {
                var data = notification as DataNotification.RetweetedRetweet;
                DisplayNotification(data.RetweetedTweet, "ic_repeat_green_300_24dp",
                    data.Performer.NickName + " 님이 내가 리트윗한 트윗을 리트윗 하였습니다");
            }
            else if(notification is DataNotification.Favorited)
            {
                var data = notification as DataNotification.Favorited;
                DisplayNotification(data.FavoritedTweet, "ic_grade_yellow_light_24dp",
                    data.Performer.NickName + " 님이 마음에 들어합니다");
            }
            else if (notification is DataNotification.FavoritedMention)
            {
                var data = notification as DataNotification.FavoritedMention;
                DisplayNotification(data.RetweetedTweet, "ic_grade_yellow_light_24dp",
                    data.Performer.NickName + " 님이 내가 멘션된 트윗을 좋아합니다");
            }
            else if (notification is DataNotification.FavoritedRetweet)
            {
                var data = notification as DataNotification.FavoritedRetweet;
                DisplayNotification(data.FavoritedTweet, "ic_grade_yellow_light_24dp",
                    data.Performer.NickName + " 님이 내가 리트윗 한 트윗을 마음에 들어합니다");
            }
            StatusView.ButtonsView.IsVisible = true;
            StatusView.HeaderView.IsVisible = true;

            if (notification is DataNotification.Mention)
            {
                var data = notification as DataNotification.Mention;
                DisplayNotification(data.MentionedTweet, null, null);
            }
            else if (notification is DataNotification.Reply)
            {
                var data = notification as DataNotification.Reply;
                DisplayNotification(data.ReplyTweet, null, null);
            }
            else if(notification is DataNotification.Quote)
            {
                var data = notification as DataNotification.Quote;
                DisplayNotification(data.QuoteTweet, null, null);
            }

            base.OnBindingContextChanged();
        }

        private void DisplayNotification(DataStatus status, string headerIcon, string headerText)
        {
            StatusView.BindingContext = status;
            StatusView.Update();

            if(headerIcon != null)
            {
                StatusView.HeaderImage.Source = headerIcon;
            }

            if (headerText != null)
            {
                StatusView.HeaderLabel.Text = headerText;
            }
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();
            OnBindingContextChanged();
        }
    }
}