using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TweetTail.Controls.CookiesWebView;
using TweetTail.UWP.Controls.CookiesWebView;
using Windows.Web.Http.Filters;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(CookieWebView), typeof(CookieWebViewRenderer))]
namespace TweetTail.UWP.Controls.CookiesWebView
{
    public class CookieWebViewRenderer : WebViewRenderer
    {
        const string LocalScheme = "ms-appx-web:///";

        public CookieWebView CookieWebView {
            get { return Element as CookieWebView; }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                Control.NavigationStarting -= Control_NavigationStarting;
                Control.NavigationCompleted -= Control_NavigationCompleted;
            }
            catch
            {

            }
            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                Control.NavigationStarting += Control_NavigationStarting;
                Control.NavigationCompleted += Control_NavigationCompleted;
            }
        }

        private void Control_NavigationCompleted(Windows.UI.Xaml.Controls.WebView sender, Windows.UI.Xaml.Controls.WebViewNavigationCompletedEventArgs args)
        {
            var httpBaseProtocolFilter = new HttpBaseProtocolFilter();
            var cookieManager = httpBaseProtocolFilter.CookieManager;
            var cookieCollection = cookieManager.GetCookies(args.Uri);

            var cookies = new CookieCollection();
            foreach(var cookie in cookieCollection)
            {
                cookies.Add(new Cookie()
                {
                    Domain = cookie.Domain,
                    HttpOnly = cookie.HttpOnly,
                    Name = cookie.Name,
                    Path = cookie.Path,
                    Secure = cookie.Secure,
                    Value = cookie.Value,
                });
            }

            CookieWebView.OnNavigated(new CookieNavigatedEventArgs()
            {
                Url = args.Uri.ToString(),
                Cookies = cookies
            });
        }

        private void Control_NavigationStarting(Windows.UI.Xaml.Controls.WebView sender, Windows.UI.Xaml.Controls.WebViewNavigationStartingEventArgs args)
        {
            CookieWebView.OnNavigating(new CookieNavigationEventArgs()
            {
                Url = args.Uri.ToString()
            });
        }
    }
}
