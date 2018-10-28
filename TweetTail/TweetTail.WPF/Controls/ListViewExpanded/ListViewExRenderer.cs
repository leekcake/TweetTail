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
using TweetTail.WPF.Hotfix.Renderers.Listview;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WPF;
using ListView = Xamarin.Forms.ListView;
using WList = System.Windows.Controls.ListView;

[assembly: ExportRenderer(typeof(ListViewEx), typeof(ListViewExRenderer))]
namespace TweetTail.WPF.Controls.ListViewExpanded
{
    public class ListViewExRenderer : ListViewRendererFix
    {
        protected override void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            base.ScrollViewer_ScrollChanged(sender, e);
            if (Element is ListViewEx listview)
            {
                listview.IsNotScrolled = e.VerticalOffset == 0;
            }
        }
    }
}
