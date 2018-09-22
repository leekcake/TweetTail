using FFImageLoading.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DataStatus = TwitterInterface.Data.Status;

namespace TweetTail.Status
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MediaPage : CarouselPage
    {
        private DataStatus viewing;

		public MediaPage (DataStatus status, int inx = 0)
		{
			InitializeComponent ();
            viewing = status;

            foreach(var media in status.extendMedias)
            {
                Children.Add(new ContentPage()
                {
                    Content = new CachedImage()
                    {
                        Source = new UriImageSource()
                        {
                            Uri = new Uri(media.mediaURLHttps)
                        }
                    }
                });
            }
            CurrentPage = Children[inx];
		}
	}
}