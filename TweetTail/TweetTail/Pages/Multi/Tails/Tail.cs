using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace TweetTail.Pages.Multi.Tails
{
    /// <summary>
    /// Base Class for Attachable Tail for Multi Tail Page
    /// </summary>
	public abstract class Tail : ContentView
	{
        public abstract string Icon {
            get;
        }

        public abstract string Description {
            get;
        }

		public Tail ()
		{
            WidthRequest = 340;
            HorizontalOptions = LayoutOptions.Fill;
		}
	}
}