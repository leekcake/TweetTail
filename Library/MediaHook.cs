using System;
using System.Collections.Generic;
using System.Text;
using TwitterInterface.Data;
using TwitterInterface.Data.Entity;

namespace Library
{
    public class MediaHook
    {
        private TweetTail owner;
        public MediaHook(TweetTail owner)
        {
            this.owner = owner;
        }

        public Status checkMissingMedia(Status status)
        {
            foreach(URL url in status.urls)
            {
                if(url.displayURL.StartsWith("pic.twitter.com"))
                {

                }
            }
            return status;
        }
    }
}
