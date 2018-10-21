using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TweetTail.WPF.Hotfix.Renderers.NavigationPageFix;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.WPF;
using Xamarin.Forms.Platform.WPF.Interfaces;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(NavigationPageRendererFix))]
namespace TweetTail.WPF.Hotfix.Renderers.NavigationPageFix
{
    /// <summary>
    /// Hotfix for Keeping Pages from Dispose
    /// </summary>
    public class NavigationPageRendererFix : NavigationPageRenderer
    {
        public class FormsLightNavigationPageFix : FormsLightNavigationPage
        {
            public FormsLightNavigationPageFix(NavigationPage navigationPage) : base(navigationPage)
            {
                ContentLoader = new StackingContentLoader();
            }
        }

        //Dirty dispose of not used controls
        private IContentLoader CleanupHelper = new FormsContentLoader();
        private List<object> LiveCache = new List<object>();
        public void CheckForChange()
        {
            if (Control == null) return;

            foreach (var obj in LiveCache)
            {
                if( !Control.InternalChildren.Any(dest => obj == dest) )
                {
                    //Dirty Clean-Up calling
                    CleanupHelper.LoadContentAsync(Control, obj, null, CancellationToken.None);
                }
            }

            LiveCache.Clear();
            foreach(var obj in Control.InternalChildren)
            {
                LiveCache.Add(obj);
            }
        }
        
        protected override void OnElementChanged(ElementChangedEventArgs<NavigationPage> e)
        {
            if(e.NewElement != null && Control == null)
            {
                SetNativeControl(new FormsLightNavigationPageFix(e.NewElement));
            }
            base.OnElementChanged(e);

            if (e.OldElement != null) // Clear old element event
            {
                e.OldElement.PushRequested -= Element_PushRequested;
                e.OldElement.PopRequested -= Element_PopRequested;
                e.OldElement.PopToRootRequested -= Element_PopToRootRequested;
                e.OldElement.RemovePageRequested -= Element_RemovePageRequested;
                e.OldElement.InsertPageBeforeRequested -= Element_InsertPageBeforeRequested;
            }

            if (e.NewElement != null)
            {
                if (Control == null) // construct and SetNativeControl and suscribe control event
                {
                    SetNativeControl(new FormsLightNavigationPage(Element));
                }

                // Suscribe element event
                Element.PushRequested += Element_PushRequested;
                Element.PopRequested += Element_PopRequested;
                Element.PopToRootRequested += Element_PopToRootRequested;
                Element.RemovePageRequested += Element_RemovePageRequested;
                Element.InsertPageBeforeRequested += Element_InsertPageBeforeRequested;
            }
        }

        private void Element_InsertPageBeforeRequested(object sender, NavigationRequestedEventArgs e)
        {
            CheckForChange();
        }

        private void Element_RemovePageRequested(object sender, NavigationRequestedEventArgs e)
        {
            CheckForChange();
        }

        private void Element_PopToRootRequested(object sender, NavigationRequestedEventArgs e)
        {
            CheckForChange();
        }

        private void Element_PopRequested(object sender, NavigationRequestedEventArgs e)
        {
            CheckForChange();
        }

        private void Element_PushRequested(object sender, NavigationRequestedEventArgs e)
        {
            CheckForChange();
        }
    }
}
