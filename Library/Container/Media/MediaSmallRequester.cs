using System;
using System.Collections.Generic;
using System.Text;
using Library.Manager;

namespace Library.Container.Media
{
    public class MediaSmallRequester : BaseMediaRequester
    {
        public MediaSmallRequester(MediaManager owner) : base(owner, "media_small")
        {
        }

        public override string tailOfURL => ":small";
    }
}
