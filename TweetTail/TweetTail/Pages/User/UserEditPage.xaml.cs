using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetTail.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


using DataAccount = TwitterInterface.Data.Account;

namespace TweetTail.Pages.User
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UserEditPage : ContentPage
	{
        private DataAccount issuer;
        private UserDetailPage parent;

		public UserEditPage (DataAccount issuer, UserDetailPage parent)
		{
			InitializeComponent ();
            this.issuer = issuer;
            this.parent = parent;

            imgBanner.Source = issuer.user.profileBannerURL;
            imgProfile.Source = issuer.user.profileHttpsImageURL;
            editNickName.Text = issuer.user.nickName;
            editDescription.Text = issuer.user.description;
            editLocation.Text = issuer.user.location;
            if (issuer.user.urlURLEntity != null && issuer.user.urlURLEntity.Length != 0)
            {
                editURL.Text = issuer.user.urlURLEntity[0].displayURL;
            }
            else
            {
                editURL.Text = issuer.user.url;
            }


            imgBanner.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    var mediaProxy = App.Media;
                    await mediaProxy.Initialize();

                    if (!mediaProxy.IsPickPhotoSupported)
                    {
                        //TODO: Notify
                        return;
                    }

                    var media = await mediaProxy.PickPhotoAsync();

                    if (media == null)
                    {
                        return;
                    }

                    await App.tail.twitter.UpdateProfileBanner(issuer, media.GetStream());

                    var user = await App.tail.twitter.GetUser(issuer, issuer.id);
                    imgBanner.Source = user.profileBannerURL;
                    parent.UpdateBinding(user);
                })
            });
            imgProfile.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    var mediaProxy = App.Media;
                    await mediaProxy.Initialize();

                    if (!mediaProxy.IsPickPhotoSupported)
                    {
                        //TODO: Notify
                        return;
                    }

                    var media = await mediaProxy.PickPhotoAsync();

                    if (media == null)
                    {
                        return;
                    }

                    var user = await App.tail.twitter.UpdateProfileImage(issuer, media.GetStream());
                    
                    imgProfile.Source = user.profileHttpsImageURL;
                    parent.UpdateBinding(user);
                })
            });
        }

        private async void btnUpdate_Clicked(object sender, EventArgs e)
        {
            try
            {
                var user = await App.tail.twitter.UpdateProfile(issuer, editNickName.Text, editURL.Text, editLocation.Text, editDescription.Text, null);
                parent.UpdateBinding(user);
                await App.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                Util.HandleException(ex);
            }
        }
    }
}