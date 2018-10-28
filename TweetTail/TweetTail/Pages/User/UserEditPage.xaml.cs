using Library.Container.Account;
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
        private AccountGroup issuer;
        private UserDetailPage parent;

		public UserEditPage (AccountGroup issuer, UserDetailPage parent)
		{
			InitializeComponent ();
            this.issuer = issuer;
            this.parent = parent;

            BannerImage.Source = issuer.User.ProfileBannerURL;
            ProfileImage.Source = issuer.User.ProfileHttpsImageURL;
            NickNameEditor.Text = issuer.User.NickName;
            DescriptionEditor.Text = issuer.User.Description;
            LocationEditor.Text = issuer.User.Location;
            if (issuer.User.URLEntities != null && issuer.User.URLEntities.Length != 0)
            {
                URLEditor.Text = issuer.User.URLEntities[0].DisplayURL;
            }
            else
            {
                URLEditor.Text = issuer.User.URL;
            }


            BannerImage.GestureRecognizers.Add(new TapGestureRecognizer()
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

                    await App.Tail.TwitterAPI.UpdateProfileBannerAsync(issuer.AccountForWrite, media.GetStream());

                    var user = await App.Tail.TwitterAPI.GetUserAsync(issuer.AccountForRead, issuer.ID);
                    BannerImage.Source = user.ProfileBannerURL;
                    parent.UpdateBinding(user);
                })
            });
            ProfileImage.GestureRecognizers.Add(new TapGestureRecognizer()
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

                    var user = await App.Tail.TwitterAPI.UpdateProfileImageAsync(issuer.AccountForWrite, media.GetStream());
                    
                    ProfileImage.Source = user.ProfileHttpsImageURL;
                    parent.UpdateBinding(user);
                })
            });
        }

        private async void UpdateButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                var user = await App.Tail.TwitterAPI.UpdateProfileAsync(issuer.AccountForWrite, NickNameEditor.Text, URLEditor.Text, LocationEditor.Text, DescriptionEditor.Text, null);
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