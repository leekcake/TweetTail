using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TwitterInterface.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TweetTail.Status
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatusWriterView : ContentView
    {
        private List<MediaFile> mediaFiles = new List<MediaFile>();

        public StatusWriterView()
        {
            InitializeComponent();
        }

        private async void btnTweet_Clicked(object sender, EventArgs e)
        {
            if (editText.Text.Trim() == "")
            {
                //TODO: Notify
                return;
            }

            var update = new StatusUpdate();

            update.text = editText.Text;

            if (mediaFiles.Count != 0)
            {
                long[] mediaIDs = new long[mediaFiles.Count];
                for (int i = 0; i < mediaIDs.Length; i++)
                {
                    mediaIDs[i] = await App.tail.twitter.uploadMedia(App.tail.account.SelectedAccountGroup.accountForWrite, Path.GetFileName(mediaFiles[i].Path), mediaFiles[i].GetStream());
                }
                update.mediaIDs = mediaIDs;
            }

            await App.tail.twitter.CreateStatus(App.tail.account.SelectedAccountGroup.accountForWrite, update);

            //TODO: Close Parent if Need
        }

        private async void btnAddImage_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                //TODO: Notify
                return;
            }

            var media = await CrossMedia.Current.PickPhotoAsync();

            if (media == null)
            {
                return;
            }

            mediaFiles.Add(media);
            var image = new Image()
            {
                Source = ImageSource.FromStream(() => { return media.GetStream(); })
            };
            image.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    var inx = mediaFiles.FindIndex((destMedia) => { return media == destMedia; });
                    mediaFiles.RemoveAt(inx);
                    slSelectedImages.Children.RemoveAt(inx);
                }),
                NumberOfTapsRequired = 1
            });

            slSelectedImages.Children.Add(image);
        }
    }
}