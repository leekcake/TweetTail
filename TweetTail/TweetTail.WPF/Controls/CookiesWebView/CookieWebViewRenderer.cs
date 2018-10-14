using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TweetTail.Controls.CookiesWebView;
using TweetTail.WPF.Controls.CookiesWebView;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WPF;

[assembly: ExportRenderer(typeof(CookieWebView), typeof(CookieWebViewRenderer))]
namespace TweetTail.WPF.Controls.CookiesWebView
{
    public class CookieWebViewRenderer : WebViewRenderer
    {
        [DllImport("wininet.dll", SetLastError = true)]
        public static extern bool InternetGetCookieEx(
    string url,
    string cookieName,
    StringBuilder cookieData,
    ref int size,
    Int32 dwFlags,
    IntPtr lpReserved);

        private const Int32 InternetCookieHttponly = 0x2000;

        /// <summary>
        /// Gets the URI cookie container.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        public static CookieContainer GetUriCookieContainer(Uri uri)
        {
            CookieContainer cookies = null;
            // Determine the size of the cookie
            int datasize = 8192 * 16;
            StringBuilder cookieData = new StringBuilder(datasize);
            if (!InternetGetCookieEx(uri.ToString(), null, cookieData, ref datasize, InternetCookieHttponly, IntPtr.Zero))
            {
                if (datasize < 0)
                    return null;
                // Allocate stringbuilder large enough to hold the cookie
                cookieData = new StringBuilder(datasize);
                if (!InternetGetCookieEx(
                    uri.ToString(),
                    null, cookieData,
                    ref datasize,
                    InternetCookieHttponly,
                    IntPtr.Zero))
                    return null;
            }
            if (cookieData.Length > 0)
            {
                cookies = new CookieContainer();
                cookies.SetCookies(uri, cookieData.ToString().Replace(';', ','));
            }
            return cookies;
        }

        public CookieWebView CookieWebView {
            get { return Element as CookieWebView; }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                Control.Navigating += Control_Navigating;
                Control.Navigated += Control_Navigated;
            }
        }

        private void Control_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            CookieWebView.OnNavigated(new CookieNavigatedEventArgs()
            {
                Cookies = GetUriCookieContainer(e.Uri).GetCookies(e.Uri),
                Url = e.Uri.ToString()
            });
        }

        private void Control_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            CookieWebView.OnNavigating(new CookieNavigationEventArgs()
            {
                Url = e.Uri.ToString()
            });
        }
    }
}
