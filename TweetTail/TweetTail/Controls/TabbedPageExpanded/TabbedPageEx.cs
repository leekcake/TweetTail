using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace TweetTail.Controls.TabbedPageExpanded
{
    public delegate void TabReselectedHandler(object sender, EventArgs args);

    public class TabbedPageEx : TabbedPage
    {
        public event TabReselectedHandler TabReselected;

        public void OnTabReselected()
        {
            TabReselected?.Invoke(this, new EventArgs());
        }
    }
}
