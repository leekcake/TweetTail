using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using Library.Container.Media;

namespace Library.Manager
{
    public class MediaManager
    {
        public string cacheDir;

        internal HttpClient client = new HttpClient();

        public BannerRequester banner;
        public ProfileRequester profile;

        public MediaRequester media;
        public MediaThumbRequester mediaThumb;
        public MediaSmallRequester mediaSmall;
        public MediaLargeRequester mediaLarge;

        public MediaManager(TweetTail tail)
        {
            cacheDir = Path.Combine(tail.cacheDir, "media");

            banner = new BannerRequester(this);
            profile = new ProfileRequester(this);

            media = new MediaRequester(this);
            mediaSmall = new MediaSmallRequester(this);
            mediaThumb = new MediaThumbRequester(this);
            mediaLarge = new MediaLargeRequester(this);
        }
    }
}
