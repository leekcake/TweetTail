using System;
using System.Collections.Generic;
using System.Text;
using Library.Manager;

namespace Library.Container.Media
{
    public class MediaRequester : BaseMediaRequester
    {
        public MediaRequester(MediaManager owner) : base(owner, "media")
        {
        }

        public override string tailOfURL => "";
    }
}
