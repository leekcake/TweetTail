using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace TweetTail.Menu
{
    public class RootPage : MasterDetailPage
    {
        public RootPage()
        {
            MasterBehavior = MasterBehavior.Popover;

            IsPresentedChanged += RootPage_IsPresentedChanged;
        }

        private void RootPage_IsPresentedChanged(object sender, EventArgs e)
        {
            (Master as MenuPage).UpdateUser();
        }
    }
}
