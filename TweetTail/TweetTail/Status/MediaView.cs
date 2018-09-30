using FFImageLoading.Forms;
using FFImageLoading.Transformations;
using FFImageLoading.Work;
using FormsVideoLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterInterface.Data.Entity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DataStatus = TwitterInterface.Data.Status;

namespace TweetTail.Status
{
    public partial class MediaPage : CarouselPage
    {
        private class MediaView
        {
            private MediaPage parent;
            public CachedImage image = new CachedImage();

            public MediaView(MediaPage parent, ExtendMedia media)
            {
                this.parent = parent;
                image.Source = media.mediaURLHttps;

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
        };

        private DataStatus viewing;

        private VideoPlayer videoView;

        public MediaPage(DataStatus status, int inx = 0)
        {
            viewing = status;

            if (status.extendMedias[0].type == "photo")
            {
                foreach (var media in status.extendMedias)
                {
                    Children.Add(new ContentPage()
                    {
                        Content = makeCachedImage(media)
                    });
                }
                CurrentPage = Children[inx];
            }
            else
            {
                videoView = new VideoPlayer();
                Children.Add(new ContentPage()
                {
                    Content = videoView
                });
                videoView.Source = VideoSource.FromUri(pickVideoVariant(status.extendMedias[0].video.variants).url);
            }
        }

        private CachedImage makeCachedImage(ExtendMedia media)
        {
            var view = new MediaView(this, media);
            return view.image;
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