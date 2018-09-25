using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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
            userView.BindingContext = BindingContext;
            userView.Update();

            base.OnBindingContextChanged();
        }
    }
}