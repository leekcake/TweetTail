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
        private DataMute.UserTarget Target {
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
                GoAwaySwitch.IsToggled = Target.MuteGoAway;
                TweetSwitch.IsToggled = Target.MuteTweet;
                RetweetSwitch.IsToggled = Target.MuteRetweet;
                OutboundMentionSwitch.IsToggled = Target.MuteOutboundMention;
                SingleInboundMentionSwitch.IsToggled = Target.MuteSingleInboundMention;
                MultipleInboundMentionSwitch.IsToggled = Target.MuteMultipleInboundMention;
                MultipleInboundMentionForcelySwitch.IsToggled = Target.MuteMultipleInboundMentionForcely;
            }
		}

        private void OKButton_Clicked(object sender, EventArgs e)
        {
            var isNew = false;
            if (mute == null)
            {
                isNew = true;
                mute = new DataMute
                {
                    Target = new DataMute.UserTarget()
                };
            }

            Target.ID = user.ID;
            Target.User = user;
            Target.MuteGoAway = GoAwaySwitch.IsToggled;
            Target.MuteTweet = TweetSwitch.IsToggled;
            Target.MuteRetweet = RetweetSwitch.IsToggled;
            Target.MuteOutboundMention = OutboundMentionSwitch.IsToggled;
            Target.MuteSingleInboundMention = SingleInboundMentionSwitch.IsToggled;
            Target.MuteMultipleInboundMention = MultipleInboundMentionSwitch.IsToggled;
            Target.MuteMultipleInboundMentionForcely = MultipleInboundMentionForcelySwitch.IsToggled;

            if (!Target.IsNeedless)
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