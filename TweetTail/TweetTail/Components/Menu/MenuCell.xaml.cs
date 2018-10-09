using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TweetTail.Components.Menu
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MenuCell : ViewCell
	{
		public MenuCell ()
		{
			InitializeComponent ();
		}

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            var data = BindingContext as MenuData;

            try
            {
                lblTitle.Text = data.Title;
                lblDescription.Text = data.Description;
                imgIcon.Source = data.Icon;
            }
            catch
            {

            }
        }
    }
}