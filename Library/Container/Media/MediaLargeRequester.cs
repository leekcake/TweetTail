using System;
using System.Collections.Generic;
using System.Text;
using Library.Manager;

namespace Library.Container.Media
{
    public class MediaLargeRequester : BaseMediaRequester
    {
        public MediaLargeRequester(MediaManager owner) : base(owner, "media_large")
        {
        }

        public override string tailOfURL => ":large";
    }
}
