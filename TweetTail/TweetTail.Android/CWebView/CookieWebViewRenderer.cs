/*
The MIT License (MIT)

Copyright (c) 2014 Sean Sparkman

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System.Net;
using Android.Content;
using Android.Graphics;
using Android.Webkit;
using TweetTail.Droid.CWebView;
using TweetTail.CWebView;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using WebView = Xamarin.Forms.WebView;
using System.Collections.Generic;

[assembly: ExportRenderer(typeof(CookieWebView), typeof(CookieWebViewRenderer))]
namespace TweetTail.Droid.CWebView
{
    public class CookieWebViewRenderer
           : WebViewRenderer
    {
        public CookieWebViewRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                Control.SetWebViewClient(new CookieWebViewClient(CookieWebView));
            }
        }

        public CookieWebView CookieWebView => Element as CookieWebView;
    }

    internal class CookieWebViewClient
        : WebViewClient
    {
        private readonly CookieWebView _cookieWebView;
        internal CookieWebViewClient(CookieWebView cookieWebView)
        {
            _cookieWebView = cookieWebView;
        }

        public override void OnPageStarted(global::Android.Webkit.WebView view, string url, Bitmap favicon)
        {
            base.OnPageStarted(view, url, favicon);

            _cookieWebView.OnNavigating(new CookieNavigationEventArgs
            {
                Url = url
            });
        }

        public static Dictionary<string, string> parseCookiesFromHeader(string cookie)
        {
            cookie = cookie.Replace("\"", "");
            var result = new Dictionary<string, string>();

            int inx = 0;
            while (inx != -1)
            {
                inx = parseCookieFromJavascript(cookie, inx, result);
            }

            return result;
        }

        private static int parseCookieFromJavascript(string cookie, int inx, Dictionary<string, string> result)
        {
            int last = -1;
            try
            {
                last = cookie.IndexOf(';', inx);
            }
            catch { }
            var value = cookie.Substring(inx, last == -1 ? cookie.Length - inx : last - inx);
            var split = value.Split('=');

            result[split[0].Trim()] = split[1].Trim();

            return last == -1 ? -1 : last + 1;
        }

        public override void OnPageFinished(global::Android.Webkit.WebView view, string url)
        {
            var cookieHeader = CookieManager.Instance.GetCookie(url);
            var cookies = new CookieCollection();
            var cookiePairs = parseCookiesFromHeader(cookieHeader); //cookieHeader.Split('&');
            foreach (var cookiePair in cookiePairs)
            {
                cookies.Add(new Cookie
                {
                    Name = cookiePair.Key,
                    Value = cookiePair.Value
                });
            }

            _cookieWebView.OnNavigated(new CookieNavigatedEventArgs
            {
                Cookies = cookies,
                Url = url
            });
        }
    }
}