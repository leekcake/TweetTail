using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TweetTail.WPF.Hotfix.Renderers.ListViewFix;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WPF;

using ListView = Xamarin.Forms.ListView;
using WList = System.Windows.Controls.ListView;

[assembly: ExportRenderer(typeof(ListView), typeof(ListViewRendererFix))]
namespace TweetTail.WPF.Hotfix.Renderers.ListViewFix
{
    public class ListViewRendererFix : ListViewRenderer
    {
        public ListViewRendererFix()
        {
            System.Diagnostics.Debug.WriteLine("Creation of ListViewRenderer");
        }

        protected override void Disappearing()
        {
            base.Disappearing();
            System.Diagnostics.Debug.WriteLine("Disappearing of ListViewRenderer");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            System.Diagnostics.Debug.WriteLine("Dispose of ListViewRenderer");
        }

        private ScrollViewer FindScrollViewer(DependencyObject d)
        {
            if (d is ScrollViewer)
                return d as ScrollViewer;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(d); i++)
            {
                var sw = FindScrollViewer(VisualTreeHelper.GetChild(d, i));
                if (sw != null) return sw;
            }
            return null;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            ScrollViewer scrollViewer = null;
            double verticalOffset = 0;
            
            if(Control != null)
            {
                Border border = (Border)VisualTreeHelper.GetChild(Control, 0);
                scrollViewer = VisualTreeHelper.GetChild(border, 0) as ScrollViewer;
                verticalOffset = scrollViewer.VerticalOffset;
            }
            base.OnElementChanged(e);
            if(scrollViewer != null)
            {
                scrollViewer.ScrollToVerticalOffset(verticalOffset);
            }
        }
    }
}
