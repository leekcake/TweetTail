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
                var target = origin.Target as DataMute.KeywordTarget;
                KeywordEditor.Text = target.Keyword;
                if (target.Replace != null)
                {
                    ReplaceEditor.Text = target.Replace;
                }
            }
		}

        private void ConfirmButton_Clicked(object sender, EventArgs e)
        {
            var isNew = false;
            if(origin == null)
            {
                isNew = true;
                origin = new DataMute();
                origin.Target = new DataMute.KeywordTarget();
            }
            var target = origin.Target as DataMute.KeywordTarget;
            target.Keyword = KeywordEditor.Text;
            target.Replace = ReplaceEditor.Text;
            if(target.Replace == "")
            {
                target.Replace = null;
            }

            if (isNew)
            {
                App.Tail.Mute.RegisterMute(origin);
            }
            else
            {
                App.Tail.Mute.UpdateMute(origin);
            }

            App.Navigation.RemovePage(this);
        }
    }
}