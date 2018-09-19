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
	public partial class StatusCell : ViewCell
	{
        private DataStatus beforeStatus;

        private Image getMediaView(int inx)
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

        protected void Clear()
        {
            if(beforeStatus == null)
            {
                return;
            }

            var display = getDisplayStatus(beforeStatus);
            imgProfile.Source = null;
            App.tail.media.profile.release(display.creater);
            if(display.extendMedias != null)
            {
                App.tail.media.mediaThumb.release(display);
                for(int i = 0; i < 4; i++)
                {
                    getMediaView(i).Source = null;
                }
            }

            beforeStatus = null;
        }

        protected async Task UpdateImage()
        {
            var display = getDisplayStatus(beforeStatus);

            var profileTask = App.tail.media.profile.request(display.creater);
            Task<ImageSource>[] mediaTasks;

            if(display.extendMedias != null)
            {
                mediaTasks = new Task<ImageSource>[display.extendMedias.Length];
                for(int i = 0; i < mediaTasks.Length; i++)
                {
                    mediaTasks[i] = App.tail.media.mediaThumb.request(display, i);
                }
            }
            else
            {
                mediaTasks = new Task<ImageSource>[] { };
            }

            imgProfile.Source = await profileTask;

            for(int i = 0; i < mediaTasks.Length; i++)
            {
                getMediaView(i).Source = await mediaTasks[i];
            }
        }

        protected async Task Update()
        {
            if (BindingContext is DataStatus) { }
            else
            {
                return;
            }
            var status = BindingContext as DataStatus;
            if(beforeStatus == status)
            {
                return;
            }
            Clear();
            beforeStatus = status;
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
            await UpdateImage();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            Update();
        }
    }
}