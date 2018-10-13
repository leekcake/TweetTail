using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DataMute = TwitterInterface.Data.Mute;

namespace TweetTail.Pages.Mute
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class KeywordMutePage : ContentPage
	{
        private DataMute origin;

		public KeywordMutePage (DataMute origin = null)
		{
			InitializeComponent ();
            this.origin = origin;

            if(origin != null)
            {
                var target = origin.target as DataMute.KeywordTarget;
                editKeyword.Text = target.keyword;
                if (target.replace != null)
                {
                    editReplace.Text = target.replace;
                }
            }
		}

        private void btnConfirm_Clicked(object sender, EventArgs e)
        {
            var isNew = false;
            if(origin == null)
            {
                isNew = true;
                origin = new DataMute();
                origin.target = new DataMute.KeywordTarget();
            }
            var target = origin.target as DataMute.KeywordTarget;
            target.keyword = editKeyword.Text;
            target.replace = editReplace.Text;
            if(target.replace == "")
            {
                target.replace = null;
            }

            if (isNew)
            {
                App.tail.mute.RegisterMute(origin);
            }
            else
            {
                App.tail.mute.UpdateMute(origin);
            }

            App.Navigation.RemovePage(this);
        }
    }
}