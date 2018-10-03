/*
The MIT License (MIT)

Copyright (c) 2014 Sean Sparkman

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using Xamarin.Forms;

namespace TweetTail.CWebView
{
    public delegate void WebViewNavigatedHandler(object sender, CookieNavigatedEventArgs args);
    public delegate void WebViewNavigatingHandler(object sender, CookieNavigationEventArgs args);

    public class CookieWebView : WebView
    {
        public CookieCollection Cookies {
            get; protected set;
        }

        public new event WebViewNavigatedHandler Navigated;
        public new event WebViewNavigatingHandler Navigating;

        public virtual void OnNavigated(CookieNavigatedEventArgs args)
        {
            var eventHandler = Navigated;

            if (eventHandler != null)
            {
                eventHandler(this, args);
            }
            Cookies = args.Cookies;
        }

        public virtual void OnNavigating(CookieNavigationEventArgs args)
        {
            var eventHandler = Navigating;

            if (eventHandler != null)
            {
                eventHandler(this, args);
            }
        }
    }

    public class CookieNavigationEventArgs
: EventArgs
    {
        public string Url { get; set; }
    }

    public class CookieNavigatedEventArgs
        : CookieNavigationEventArgs
    {
        public CookieCollection Cookies { get; set; }
        public CookieNavigationMode NavigationMode { get; set; }
    }

    public enum CookieNavigationMode
    {
        Back,
        Forward,
        New,
        Refresh,
        Reset
    }
}