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
                return mute.Target as DataMute.UserTarget;
            }
        }

        private DataUser user;

        public UserMutePage (DataUser user)
		{
			InitializeComponent ();
            this.user = user;
            viewTarget.BindingContext = user;
            viewTarget.Update();

            mute = App.Tail.Mute.GetUserMute(user);

            if(mute != null)
            {
                GoAwaySwitch.IsToggled = target.MuteGoAway;
                TweetSwitch.IsToggled = target.MuteTweet;
                RetweetSwitch.IsToggled = target.MuteRetweet;
                OutboundMentionSwitch.IsToggled = target.MuteOutboundMention;
                SingleInboundMentionSwitch.IsToggled = target.MuteSingleInboundMention;
                MultipleInboundMentionSwitch.IsToggled = target.MuteMultipleInboundMention;
                MultipleInboundMentionForcelySwitch.IsToggled = target.MuteMultipleInboundMentionForcely;
            }
		}

        private void OKButton_Clicked(object sender, EventArgs e)
        {
            var isNew = false;
            if (mute == null)
            {
                isNew = true;
                mute = new DataMute();
                mute.Target = new DataMute.UserTarget();
            }

            target.ID = user.ID;
            target.User = user;
            target.MuteGoAway = GoAwaySwitch.IsToggled;
            target.MuteTweet = TweetSwitch.IsToggled;
            target.MuteRetweet = RetweetSwitch.IsToggled;
            target.MuteOutboundMention = OutboundMentionSwitch.IsToggled;
            target.MuteSingleInboundMention = SingleInboundMentionSwitch.IsToggled;
            target.MuteMultipleInboundMention = MultipleInboundMentionSwitch.IsToggled;
            target.MuteMultipleInboundMentionForcely = MultipleInboundMentionForcelySwitch.IsToggled;

            if (!target.IsNeedless)
            {
                if (isNew)
                {
                    App.Tail.Mute.RegisterMute(mute);
                }
                else
                {
                    App.Tail.Mute.UpdateMute(mute);
                }
            }
            else
            {
                if(!isNew)
                {
                    App.Tail.Mute.UnregisterMute(mute);
                }
            }
            App.Navigation.RemovePage(this);
        }
    }
}