using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using TweetTail.Controls.TabbedPageExpanded;
using TweetTail.iOS.Controls.TabbedPageExpanded;
using UIKit;
using Xamarin.Forms.Platform.iOS;

[assembly: Xamarin.Forms.ExportRenderer(typeof(TabbedPageEx), typeof(TabbedPageExRenderer))]
namespace TweetTail.iOS.Controls.TabbedPageExpanded
{
    public class TabbedPageExRenderer : TabbedRenderer
    {
        private UIKit.UITabBarItem _prevItem;

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (SelectedIndex < TabBar.Items.Length)
                _prevItem = TabBar.Items[SelectedIndex];
        }

        public override void ItemSelected(UIKit.UITabBar tabbar, UIKit.UITabBarItem item)
        {
            if (_prevItem == item)
            {
                (Element as TabbedPageEx).OnTabReselected();
            }
            _prevItem = item;
        }
    }
}