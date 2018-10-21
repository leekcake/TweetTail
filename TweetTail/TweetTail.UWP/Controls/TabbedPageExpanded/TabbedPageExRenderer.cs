using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetTail.Controls.TabbedPageExpanded;
using TweetTail.UWP.Controls.TabbedPageExpanded;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(TabbedPageEx), typeof(TabbedPageExRenderer))]
namespace TweetTail.UWP.Controls.TabbedPageExpanded
{
    public class TabbedPageExRenderer : TabbedPageRenderer
    {
        private Xamarin.Forms.Page prevPage;

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            Control.Tapped += Control_Tapped;
            prevPage = Control.SelectedItem as Xamarin.Forms.Page;
        }

        private void Control_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var src = e.OriginalSource as TextBlock;
            if (src != null && src.Name == "TabbedPageHeaderTextBlock")
            {
                var newPage = src.DataContext as Xamarin.Forms.Page;
                if (newPage == prevPage)
                {
                    (Element as TabbedPageEx).OnTabReselected();
                }
                prevPage = newPage;
            }
        }
    }
}
