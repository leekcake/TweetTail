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

            var mute = BindingContext as DataMute;
            var target = mute.target as DataMute.KeywordTarget;

            var builder = new StringBuilder();
            builder.Append(target.keyword);
            if (target.replace != null)
            {
                builder.Append(" => ");
                builder.Append(target.replace);
            }

            label.Text = builder.ToString();
        }
    }
}