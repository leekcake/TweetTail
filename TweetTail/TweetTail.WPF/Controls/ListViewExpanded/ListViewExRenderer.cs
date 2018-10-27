using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TweetTail.Controls.ListViewExpanded;
using TweetTail.WPF.Controls.ListViewExpanded;
using TweetTail.WPF.Hotfix.Renderers.ListViewFix;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WPF;
using ListView = Xamarin.Forms.ListView;
using WList = System.Windows.Controls.ListView;

[assembly: ExportRenderer(typeof(ListViewEx), typeof(ListViewExRenderer))]
namespace TweetTail.WPF.Controls.ListViewExpanded
{
    public class ListViewExRenderer : ListViewRendererFix
    {
        public static ScrollViewer FindScrollViewer(DependencyObject obj)
        {
            Decorator border = VisualTreeHelper.GetChild(obj, 0) as Decorator;
            return border?.Child as ScrollViewer;
        }

        public static T GetChildOfType<T>(DependencyObject depObj)
    where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null) return result;
            }
            return null;
        }

        private ScrollViewer scrollViewer;

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);
            if(Control != null)
            {
                Control.ItemContainerGenerator.ItemsChanged += ItemContainerGenerator_ItemsChanged;
            }
        }

        private void ItemContainerGenerator_ItemsChanged(object sender, System.Windows.Controls.Primitives.ItemsChangedEventArgs e)
        {
            CheckScroll();
        }

        private void CheckScroll()
        {
            if (scrollViewer != null) return;

            scrollViewer = GetChildOfType<ScrollViewer>(Control);
            if(scrollViewer != null)
            {
                scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
            }
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (Element is ListViewEx listview)
            {
                listview.IsNotScrolled = e.VerticalOffset == 0;
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
                scrollViewer = null;
            }
            catch
            {

            }
            base.Dispose(disposing);
        }
    }
}
