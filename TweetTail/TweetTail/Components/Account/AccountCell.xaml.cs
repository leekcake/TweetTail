using Library.Container.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DataAccount = TwitterInterface.Data.Account;

namespace TweetTail.Components.Account
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AccountCell : ViewCell
	{
		public AccountCell ()
		{
			InitializeComponent ();
		}

        protected override void OnBindingContextChanged()
        {
            if (BindingContext is AccountGroup) { }
            else
            {
                return;
            }

            UserView.BindingContext = (BindingContext as AccountGroup).User;
            UserView.Update();

            base.OnBindingContextChanged();
        }
    }
}