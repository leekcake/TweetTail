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
using Library.Container.Account;
using TweetTail.Utils;

namespace TweetTail.Status
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatusWriterView : ContentView
    {
        private List<MediaFile> mediaFiles = new List<MediaFile>();
        private DataStatus replyStatus;

        private AccountGroup writer;

        public void SetReplyStatus(DataStatus reply)
        {
            replyStatus = reply;
            viewReplyStatus.IsVisible = true;
            viewReplyStatus.BindingContext = replyStatus;
            viewReplyStatus.Update();

            editText.Text = "@" + (reply.retweetedStatus != null ? reply.retweetedStatus.creater.screenName : reply.creater.screenName) + " " + editText.Text;
        }

        int currentImageInx = 0;

        private GridImageWrapper gridImageWrapper;

        public StatusWriterView()
        {
            InitializeComponent();
            gridImageWrapper = new GridImageWrapper(gridMedias);
            writer = App.tail.account.SelectedAccountGroup;

            for (int i = 0; i < 4; i++)
            {
                //Value Copy
                var inx = i;
                gridImageWrapper[i].GestureRecognizers.Add(new TapGestureRecognizer() {
                    Command = new Command(() =>
                    {
                        mediaFiles.RemoveAt(inx);
                        gridImageWrapper[inx].Source = null;
                        gridImageWrapper.setCount(--currentImageInx);
                        //Shift Source Prop 
                        //removed at 0 cause
                        //[0] = [1]
                        //[1] = [2]
                        //[2] = [3]
                        for(int icpy = inx + 1; icpy < 4; icpy++)
                        {
                            gridImageWrapper[icpy - 1].Source = gridImageWrapper[icpy].Source;
                        }
                    })
                });
            }

            displayWriter();
            viewWriter.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    var selected = await Util.SelectAccount("트윗을 쓸 계정을 선택하세요");
                    if (selected == null) return;

                    writer = selected;
                    displayWriter();
                })
            });
        }

        public void displayWriter()
        {
            var user = writer.accountForRead.user;
            imgProfile.Source = user.profileHttpsImageURL;
            lblName.Text = user.nickName + " @" + user.screenName;
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
                        mediaIDs[i] = await App.tail.twitter.uploadMedia(writer.accountForWrite, Path.GetFileName(mediaFiles[i].Path), mediaFiles[i].GetStream());
                    }
                    update.mediaIDs = mediaIDs;
                }

                await App.tail.twitter.CreateStatus(writer.accountForWrite, update);
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
            for(int i = 0; i < 4; i++)
            {
                gridImageWrapper[i].Source = null;
            }
            currentImageInx = 0;
            gridImageWrapper.setCount(0);
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
            gridImageWrapper[currentImageInx].Source = ImageSource.FromStream(() => { return media.GetStream(); });

            gridImageWrapper.setCount(currentImageInx + 1);
            currentImageInx++;
        }
    }
}