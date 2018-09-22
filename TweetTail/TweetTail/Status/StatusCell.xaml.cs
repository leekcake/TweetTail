﻿using FFImageLoading.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DataStatus = TwitterInterface.Data.Status;

namespace TweetTail.Status
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class StatusCell : ViewCell
	{
        private CachedImage getMediaView(int inx)
        {
            switch(inx)
            {
                case 0:
                    return imgMedia1;
                case 1:
                    return imgMedia2;
                case 2:
                    return imgMedia3;
                case 3:
                    return imgMedia4;
            }
            throw new IndexOutOfRangeException();
        }
        
		public StatusCell ()
		{
			InitializeComponent ();
            imgHeader.Source = new EmbeddedResourceImageSource("TweetTail.Res.ic_repeat_black_48dp.png", Assembly.GetExecutingAssembly());
            imgReply.Source = new EmbeddedResourceImageSource("TweetTail.Res.ic_reply_black_48dp.png", Assembly.GetExecutingAssembly());
            imgRetweet.Source = new EmbeddedResourceImageSource("TweetTail.Res.ic_repeat_black_48dp.png", Assembly.GetExecutingAssembly());
            imgFavorite.Source = new EmbeddedResourceImageSource("TweetTail.Res.ic_grade_black_48dp.png", Assembly.GetExecutingAssembly());
            imgDelete.Source = new EmbeddedResourceImageSource("TweetTail.Res.ic_delete_black_24dp.png", Assembly.GetExecutingAssembly());
            imgMore.Source = new EmbeddedResourceImageSource("TweetTail.Res.ic_more_horiz_black_48dp.png", Assembly.GetExecutingAssembly());
		}

        private DataStatus getDisplayStatus(DataStatus status)
        {
            if(status.retweetedStatus != null)
            {
                return status.retweetedStatus;
            }
            else
            {
                return status;
            }
        }

        protected void UpdateImage()
        {
            var display = getDisplayStatus( BindingContext as DataStatus );

            imgProfile.Source = null;
            for(int i = 0; i < 4; i++)
            {
                getMediaView(i).Source = null;
            }

            imgProfile.Source = display.creater.profileHttpsImageURL;

            if (display.extendMedias != null)
            {
                for (int i = 0; i < display.extendMedias.Length; i++)
                {
                    getMediaView(i).Source = display.extendMedias[i].mediaURLHttps;
                }
            }
        }

        protected void Update()
        {
            if (BindingContext is DataStatus) { }
            else
            {
                return;
            }
            var status = BindingContext as DataStatus;
            var display = getDisplayStatus(status);

            if(display != status)
            {
                viewHeader.IsVisible = true;
                lblHeader.Text = string.Format("{0} 님이 리트윗 하셨습니다", status.creater.nickName);
            }
            else
            {
                viewHeader.IsVisible = false;
            }

            imgLock.IsVisible = display.creater.isProtected;
            lblCreatedAt.Text = display.createdAt.ToString();
            lblName.Text = string.Format("{0} @{1}", display.creater.nickName, display.creater.screenName);
            lblText.Text = display.text;
            
            imgProfile.Source = null;
            for (int i = 0; i < 4; i++)
            {
                getMediaView(i).Source = null;
            }
            if(display.extendMedias != null)
            {
                viewMedias.IsVisible = true;
            }
            else
            {
                viewMedias.IsVisible = false;
            }
            UpdateImage();
        }

        protected override void OnBindingContextChanged()
        {
            Update();
            base.OnBindingContextChanged();
        }
    }
}