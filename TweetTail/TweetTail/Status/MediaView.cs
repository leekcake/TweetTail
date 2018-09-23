using FFImageLoading.Forms;
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
                        Content = new CachedImage()
                        {
                            Source = new UriImageSource()
                            {
                                Uri = new Uri(media.mediaURLHttps)
                            }
                        }
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