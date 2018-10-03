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

using DataStatus = TwitterInterface.Data.Status;

namespace TweetTail.Status
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatusWriterView : ContentView
    {
        private List<MediaFile> mediaFiles = new List<MediaFile>();
        private DataStatus replyStatus;

        public void SetReplyStatus(DataStatus reply)
        {
            replyStatus = reply;
            viewReplyStatus.IsVisible = true;
            viewReplyStatus.BindingContext = replyStatus;
            viewReplyStatus.Update();

            editText.Text = "@" + (reply.retweetedStatus != null ? reply.retweetedStatus.creater.screenName : reply.creater.screenName) + " " + editText.Text;
        }

        public StatusWriterView()
        {
            InitializeComponent();
        }

        private void SetInputStatus(bool enable)
        {
            btnAddImage.IsEnabled = enable;
            btnTweet.IsEnabled = enable;
            editText.IsEnabled = enable;
        }

        private async void btnTweet_Clicked(object sender, EventArgs e)
        {
            if (editText.Text.Trim() == "" && mediaFiles.Count == 0)
            {
                await Application.Current.MainPage.DisplayAlert("트윗 실패", "트윗은 하나 이상의 미디어 혹은 글자를 포함해야 합니다", "확인");
                return;
            }

            SetInputStatus(false);

            var update = new StatusUpdate();

            update.text = editText.Text;

            if(replyStatus != null)
            {
                update.inReplyToStatusId = replyStatus.id;
            }

            try
            {
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
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("트윗 실패", ex.Message, "확인");
                SetInputStatus(true);
                return;
            }
            SetInputStatus(true);

            if (BindingContext != null && BindingContext is Page)
            {
                App.Navigation.RemovePage(BindingContext as Page);
                return;
            }

            mediaFiles.Clear();
            slSelectedImages.Children.Clear();
            editText.Text = "";
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