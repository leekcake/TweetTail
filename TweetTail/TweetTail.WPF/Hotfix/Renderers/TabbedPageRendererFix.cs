using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.WPF.Controls;
using Xamarin.Forms.Platform.WPF;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WPF.Interfaces;
using System.Threading;
using TweetTail.WPF.Hotfix.Renderers.TabbedPageFix;

[assembly: ExportRenderer(typeof(TabbedPage), typeof(TabbedPageRendererFix))]
namespace TweetTail.WPF.Hotfix.Renderers.TabbedPageFix
{
    public class TabbedPageRendererFix : TabbedPageRenderer
    {
        private IContentLoader CleanupHelper = new FormsContentLoader();
        protected override void Dispose(bool disposing)
        {
            if (Control != null && Control.ItemsSource != null)
            {
                foreach (var page in Control.ItemsSource)
                {
                    CleanupHelper.LoadContentAsync(Control, page, null, CancellationToken.None);
                }
            }
            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<TabbedPage> e)
        {
            if (e.NewElement != null && Control == null)
            {
                SetNativeControl(new FormsTabbedPage() { ContentLoader = new StackingContentLoader() });
            }
        }
    }
}
