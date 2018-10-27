using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace TweetTail.Controls.ListViewExpanded
{
    public class ListViewEx : ListView
    {
        public bool IsNotScrolled = true;

        public ListViewEx()
        {
        }

        public ListViewEx(ListViewCachingStrategy cachingStrategy) : base(cachingStrategy)
        {
        }
    }
}
