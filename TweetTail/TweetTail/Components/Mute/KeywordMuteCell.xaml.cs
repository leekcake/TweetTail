using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DataMute = TwitterInterface.Data.Mute;

namespace TweetTail.Components.Mute
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class KeywordMuteCell : ViewCell
	{
		public KeywordMuteCell ()
		{
			InitializeComponent ();
		}

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext == null) return;

            var mute = BindingContext as DataMute;
            var target = mute.Target as DataMute.KeywordTarget;

            var builder = new StringBuilder();
            builder.Append(target.Keyword);
            if (target.Replace != null)
            {
                builder.Append(" => ");
                builder.Append(target.Replace);
            }

            KeywordLabel.Text = builder.ToString();
        }
    }
}