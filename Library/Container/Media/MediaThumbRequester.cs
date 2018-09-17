using System;
using System.Collections.Generic;
using System.Text;
using Library.Manager;

namespace Library.Container.Media
{
    public class MediaThumbRequester : BaseMediaRequester
    {
        public MediaThumbRequester(MediaManager owner) : base(owner, "media_thumb")
        {
        }

        public override string tailOfURL => ":thumb";
    }
}
