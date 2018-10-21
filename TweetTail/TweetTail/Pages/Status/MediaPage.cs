using FFImageLoading.Forms;
using FFImageLoading.Transformations;
using FFImageLoading.Work;
using TweetTail.Controls.FormsVideoLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterInterface.Data.Entity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DataStatus = TwitterInterface.Data.Status;

namespace TweetTail.Pages.Status
{
    public partial class MediaPage : CarouselPage
    {
        private class MediaView
        {
            private readonly MediaPage parent;
            public AbsoluteLayout Layout = new AbsoluteLayout();
            public CachedImage Image = new CachedImage();
            public ProgressBar ImageProgressBar = new ProgressBar();

            public MediaView(MediaPage parent, ExtendMedia media)
            {
                this.parent = parent;

                Image.Source = media.MediaURLHttps;
                Image.DownloadProgress += Image_DownloadProgress;
                Image.Finish += Image_Finish;

                ImageProgressBar.Progress = 0;

                Layout.Children.Add(Image);
                Layout.Children.Add(ImageProgressBar);

                AbsoluteLayout.SetLayoutBounds(Image, new Rectangle(0, 0, 1, 1));
                AbsoluteLayout.SetLayoutFlags(Image, AbsoluteLayoutFlags.All);

                AbsoluteLayout.SetLayoutBounds(ImageProgressBar, new Rectangle(0.1, 0.45, 0.8, 0.15));
                AbsoluteLayout.SetLayoutFlags(ImageProgressBar, AbsoluteLayoutFlags.All);

                var gesture = new PanGestureRecognizer();
                gesture.PanUpdated += (sender, e) =>
                {
                    switch (e.StatusType)
                    {
                        case GestureStatus.Running:
                            Image.TranslationX = e.TotalX;
                            Image.TranslationY = e.TotalY;
                            break;
                        case GestureStatus.Completed:
                            if ( Math.Abs(Image.TranslationY) > (parent.Height / 8))
                            {
                                App.Navigation.RemovePage(parent);
                            }
                            Image.TranslateTo(0, 0);
                            break;
                    }
                };
                Image.GestureRecognizers.Add(gesture);
            }

            private void Image_Finish(object sender, CachedImageEvents.FinishEventArgs e)
            {
                Device.BeginInvokeOnMainThread(() => {
                    ImageProgressBar.IsVisible = false;
                });                
            }

            private void Image_DownloadProgress(object sender, CachedImageEvents.DownloadProgressEventArgs e)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ImageProgressBar.Progress = e.DownloadProgress.Current / e.DownloadProgress.Total;
                });                
            }
        };

        private DataStatus viewing;

        private VideoPlayer videoView;

        public MediaPage(DataStatus status, int inx = 0)
        {
            viewing = status;
            Style = (Style) Application.Current.Resources["backgroundStyle"];

            if (status.ExtendMedias[0].Type == "photo")
            {
                foreach (var media in status.ExtendMedias)
                {
                    Children.Add(new ContentPage()
                    {
                        Content = MakeMediaView(media)
                    });
                }
                CurrentPage = Children[inx];
            }
            else
            {
                videoView = new VideoPlayer();
                if (status.ExtendMedias[0].Type == "animated_gif")
                {
                    videoView.IsVideoOnly = true;
                }
                var gesture = new PanGestureRecognizer();
                gesture.PanUpdated += (sender, e) =>
                {
                    switch (e.StatusType)
                    {
                        case GestureStatus.Running:
                            videoView.TranslationX = e.TotalX;
                            videoView.TranslationY = e.TotalY;
                            break;
                        case GestureStatus.Completed:
                            if (Math.Abs(videoView.TranslationY) > (Height / 8))
                            {
                                App.Navigation.RemovePage(this);
                            }
                            videoView.TranslateTo(0, 0);
                            break;
                    }
                };
                videoView.GestureRecognizers.Add(gesture);
                Children.Add(new ContentPage()
                {
                    Content = videoView
                });
                videoView.Source = VideoSource.FromUri(PickVideoVariant(status.ExtendMedias[0].Video.Variants).URL);
            }
        }

        private View MakeMediaView(ExtendMedia media)
        {
            var view = new MediaView(this, media);
            return view.Layout;
        }

        private VideoVariant PickVideoVariant(VideoVariant[] variants)
        {
            var best = variants[0];
            foreach (var variant in variants)
            {
                if (best.Bitrate < variant.Bitrate)
                {
                    best = variant;
                }
            }
            return best;
        }
    }
}