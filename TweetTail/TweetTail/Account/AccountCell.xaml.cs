using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DataAccount = TwitterInterface.Data.Account;

namespace TweetTail.Account
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
            if (BindingContext is DataAccount) { }
            else
            {
                return;
            }

            userView.BindingContext = (BindingContext as DataAccount).user;
            userView.Update();

            base.OnBindingContextChanged();
        }
    }
}