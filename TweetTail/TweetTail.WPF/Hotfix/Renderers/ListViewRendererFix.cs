using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using TweetTail.WPF.Hotfix.Renderers.ListViewFix;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WPF;

using ListView = Xamarin.Forms.ListView;
using WList = System.Windows.Controls.ListView;
using Application = System.Windows.Application;

[assembly: ExportRenderer(typeof(ListView), typeof(ListViewRendererFix))]
namespace TweetTail.WPF.Hotfix.Renderers.ListViewFix
{
    public class ListViewRendererFix : ListViewRenderer
    {
        ITemplatedItemsView<Cell> TemplatedItemsView => Element;

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);
            if(Control != null)
            {
                Control.SetValue(VirtualizingPanel.ScrollUnitProperty, ScrollUnit.Pixel);
            }
        }

        private CancellationTokenSource token;

        //Dirty Refresh of Width
        protected override void UpdateWidth()
        {
            base.UpdateWidth();
            if (Control == null || Element == null)
                return;

            if(token != null)
            {
                token.Cancel();
                token = null;
            }
            token = new CancellationTokenSource();
            var holder = token;
            new Task(async () =>
            {
                await Task.Delay(100);
                if (holder.IsCancellationRequested)
                {
                    return;
                }
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var source = Element.ItemsSource;
                    Element.ItemsSource = null;
                    Element.ItemsSource = source;
                });
                token = null;
            }).Start();
        }
    }
}
