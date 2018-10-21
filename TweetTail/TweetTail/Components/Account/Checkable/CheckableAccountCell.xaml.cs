using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TweetTail.Components.Account.Checkable
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CheckableAccountCell : ViewCell
	{
		public CheckableAccountCell ()
		{
			InitializeComponent ();

            CheckedSwitch.Toggled += SwitchChecked_Toggled;
		}

        private void SwitchChecked_Toggled(object sender, ToggledEventArgs e)
        {
            if (BindingContext is CheckableAccount) { }
            else
            {
                return;
            }
            (BindingContext as CheckableAccount).IsChecked = CheckedSwitch.IsToggled;
        }

        protected override void OnBindingContextChanged()
        {
            if (BindingContext is CheckableAccount) { }
            else
            {
                return;
            }

            userView.BindingContext = (BindingContext as CheckableAccount).Account.User;
            userView.Update();

            base.OnBindingContextChanged();
        }
    }
}