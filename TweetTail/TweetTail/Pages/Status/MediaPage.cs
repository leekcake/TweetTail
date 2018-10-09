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
            private MediaPage parent;
            public AbsoluteLayout layout = new AbsoluteLayout();
            public CachedImage image = new CachedImage();
            public ProgressBar pb = new ProgressBar();

            public MediaView(MediaPage parent, ExtendMedia media)
            {
                this.parent = parent;

                image.Source = media.mediaURLHttps;
                image.DownloadProgress += Image_DownloadProgress;
                image.Finish += Image_Finish;

                pb.Progress = 0;

                layout.Children.Add(image);
                layout.Children.Add(pb);

                AbsoluteLayout.SetLayoutBounds(image, new Rectangle(0, 0, 1, 1));
                AbsoluteLayout.SetLayoutFlags(image, AbsoluteLayoutFlags.All);

                AbsoluteLayout.SetLayoutBounds(pb, new Rectangle(0.1, 0.4, 0.7, 0.2));
                AbsoluteLayout.SetLayoutFlags(pb, AbsoluteLayoutFlags.All);

                var gesture = new PanGestureRecognizer();
                gesture.PanUpdated += (sender, e) =>
                {
                    switch (e.StatusType)
                    {
                        case GestureStatus.Running:
                            image.TranslationX = e.TotalX;
                            image.TranslationY = e.TotalY;
                            break;
                        case GestureStatus.Completed:
                            if ( Math.Abs(image.TranslationY) > (parent.Height / 8))
                            {
                                App.Navigation.RemovePage(parent);
                            }
                            image.TranslateTo(0, 0);
                            break;
                    }
                };
                image.GestureRecognizers.Add(gesture);
            }

            private void Image_Finish(object sender, CachedImageEvents.FinishEventArgs e)
            {
                Device.BeginInvokeOnMainThread(() => {
                    pb.IsVisible = false;
                });                
            }

            private void Image_DownloadProgress(object sender, CachedImageEvents.DownloadProgressEventArgs e)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    pb.Progress = e.DownloadProgress.Current / e.DownloadProgress.Total;
                });                
            }
        };

        private DataStatus viewing;

        private VideoPlayer videoView;

        public MediaPage(DataStatus status, int inx = 0)
        {
            viewing = status;
            Style = (Style) Application.Current.Resources["backgroundStyle"];

            if (status.extendMedias[0].type == "photo")
            {
                foreach (var media in status.extendMedias)
                {
                    Children.Add(new ContentPage()
                    {
                        Content = makeMediaView(media)
                    });
                }
                CurrentPage = Children[inx];
            }
            else
            {
                videoView = new VideoPlayer();
                if (status.extendMedias[0].type == "animated_gif")
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
                videoView.Source = VideoSource.FromUri(pickVideoVariant(status.extendMedias[0].video.variants).url);
            }
        }

        private View makeMediaView(ExtendMedia media)
        {
            var view = new MediaView(this, media);
            return view.layout;
        }

        private VideoVariant pickVideoVariant(VideoVariant[] variants)
        {
            var best = variants[0];
            foreach (var variant in variants)
            {
                if (best.bitrate < variant.bitrate)
                {
                    best = variant;
                }
            }
            return best;
        }
    }
}