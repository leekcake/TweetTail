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

namespace TweetTail.Components.Status
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
            ReplyStatusView.IsVisible = true;
            ReplyStatusView.BindingContext = replyStatus;
            ReplyStatusView.Update();

            TextEditor.Text = "@" + (reply.RetweetedStatus != null ? reply.RetweetedStatus.Creater.ScreenName : reply.Creater.ScreenName) + " " + TextEditor.Text;
        }

        int currentImageInx = 0;

        private GridImageWrapper gridImageWrapper;

        private void SetProgressVisible(bool visible)
        {
            WriteProgressLabel.IsVisible = visible;
            WriteProgressBar.IsVisible = visible;
        }

        public StatusWriterView(AccountGroup issuer)
        {
            InitializeComponent();
            gridImageWrapper = new GridImageWrapper(MediaGrid);
            writer = issuer;
            SetProgressVisible(false);

            for (int i = 0; i < 4; i++)
            {
                //Value Copy
                var inx = i;
                gridImageWrapper[i].GestureRecognizers.Add(new TapGestureRecognizer() {
                    Command = new Command(() =>
                    {
                        mediaFiles.RemoveAt(inx);
                        gridImageWrapper[inx].Source = null;
                        gridImageWrapper.SetCount(--currentImageInx);
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

            DisplayWriter();
            WriterView.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    var selected = await Util.SelectAccount("트윗을 쓸 계정을 선택하세요");
                    if (selected == null) return;

                    writer = selected;
                    DisplayWriter();
                })
            });
        }

        public void DisplayWriter()
        {
            var user = writer.AccountForRead.User;
            ProfileImage.Source = user.ProfileHttpsImageURL;
            NameLabel.Text = user.NickName + " @" + user.ScreenName;
        }

        private void SetInputStatus(bool enable)
        {
            AddImageButton.IsEnabled = enable;
            TweetButton.IsEnabled = enable;
            TextEditor.IsEnabled = enable;
        }

        private async void TweetButton_Clicked(object sender, EventArgs e)
        {
            if (TextEditor.Text.Trim() == "" && mediaFiles.Count == 0)
            {
                await Application.Current.MainPage.DisplayAlert("트윗 실패", "트윗은 하나 이상의 미디어 혹은 글자를 포함해야 합니다", "확인");
                return;
            }

            SetInputStatus(false);
            SetProgressVisible(true);
            WriteProgressBar.Progress = 0d;
            WriteProgressLabel.Text = "초기화...";

            var update = new StatusUpdate
            {
                Text = TextEditor.Text
            };

            if (replyStatus != null)
            {
                update.InReplyToStatusId = replyStatus.ID;
            }

            try
            {
                if (mediaFiles.Count != 0)
                {
                    long[] mediaIDs = new long[mediaFiles.Count];
                    for (int i = 0; i < mediaIDs.Length; i++)
                    {
                        WriteProgressBar.Progress = ((double) i / mediaIDs.Length) * 0.9d;
                        WriteProgressLabel.Text = (i+1) + "번째 미디어 업로드 중";
                        mediaIDs[i] = await App.Tail.TwitterAPI.UploadMediaAsync(writer.AccountForWrite, Path.GetFileName(mediaFiles[i].Path), mediaFiles[i].GetStream());
                    }
                    update.MediaIDs = mediaIDs;
                }
                WriteProgressBar.Progress = 0.9d;
                WriteProgressLabel.Text = "트윗 발송중...";
                await App.Tail.TwitterAPI.CreateStatusAsync(writer.AccountForWrite, update);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("트윗 실패", ex.Message, "확인");
                SetInputStatus(true);
                return;
            }
            SetInputStatus(true);
            SetProgressVisible(false);

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
            gridImageWrapper.SetCount(0);
            TextEditor.Text = "";
        }

        private async void AddImageButton_Clicked(object sender, EventArgs e)
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
            
            mediaFiles.Add(media);
            gridImageWrapper[currentImageInx].Source = ImageSource.FromStream(() => { return media.GetStream(); });

            gridImageWrapper.SetCount(currentImageInx + 1);
            currentImageInx++;
        }
    }
}