using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DataUser = TwitterInterface.Data.User;
using DataMute = TwitterInterface.Data.Mute;

namespace TweetTail.Pages.Mute
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UserMutePage : ContentPage
	{
        private DataMute mute;
        private DataMute.UserTarget target {
            get {
                return mute.target as DataMute.UserTarget;
            }
        }

        private DataUser user;

        public UserMutePage (DataUser user)
		{
			InitializeComponent ();
            this.user = user;
            viewTarget.BindingContext = user;
            viewTarget.Update();

            mute = App.tail.mute.GetUserMute(user);

            if(mute != null)
            {
                swiGoAway.IsToggled = target.muteGoAway;
                swiTweet.IsToggled = target.muteTweet;
                swiRetweet.IsToggled = target.muteRetweet;
                swiOutboundMention.IsToggled = target.muteOutboundMention;
                swiSingleInboundMention.IsToggled = target.muteSingleInboundMention;
                swiMultipleInboundMention.IsToggled = target.muteMultipleInboundMention;
                swiMultipleInboundMentionForcely.IsToggled = target.muteMultipleInboundMentionForcely;
            }
		}

        private void btnOK_Clicked(object sender, EventArgs e)
        {
            var isNew = false;
            if (mute == null)
            {
                isNew = true;
                mute = new DataMute();
                mute.target = new DataMute.UserTarget();
            }

            target.id = user.id;
            target.user = user;
            target.muteGoAway = swiGoAway.IsToggled;
            target.muteTweet = swiTweet.IsToggled;
            target.muteRetweet = swiRetweet.IsToggled;
            target.muteOutboundMention = swiOutboundMention.IsToggled;
            target.muteSingleInboundMention = swiSingleInboundMention.IsToggled;
            target.muteMultipleInboundMention = swiMultipleInboundMention.IsToggled;
            target.muteMultipleInboundMentionForcely = swiMultipleInboundMentionForcely.IsToggled;

            if (!target.isNeedless)
            {
                if (isNew)
                {
                    App.tail.mute.RegisterMute(mute);
                }
                else
                {
                    App.tail.mute.UpdateMute(mute);
                }
            }
            else
            {
                if(!isNew)
                {
                    App.tail.mute.UnregisterMute(mute);
                }
            }
            App.Navigation.RemovePage(this);
        }
    }
}