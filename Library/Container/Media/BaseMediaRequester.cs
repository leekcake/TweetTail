using Library.Manager;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TwitterInterface.Data;
using Xamarin.Forms;

namespace Library.Container.Media
{
    public abstract class BaseMediaRequester : Requester
    {
        public abstract string tailOfURL {
            get;
        }

        public BaseMediaRequester(MediaManager owner, string type) : base(owner, type)
        {
        }

        private Task<ImageSource> request(Status status, int inx)
        {
            var media = status.extendMedias[inx];
            return request(media.mediaURLHttps + tailOfURL, media.id);
        }

        public void release(Status status, int inx)
        {
            var media = status.extendMedias[inx];
            release(media.id);
        }

        public void release(Status status)
        {
            foreach(var media in status.extendMedias)
            {
                release(media.id);
            }
        }
    }
}
