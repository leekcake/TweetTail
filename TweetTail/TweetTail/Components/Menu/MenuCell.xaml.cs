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

            if (BindingContext == null) return;

            var data = BindingContext as MenuData;

            try
            {
                lblTitle.Text = data.Title;
                if (data.Description != null)
                {
                    lblDescription.IsVisible = true;
                    lblDescription.Text = data.Description;
                }
                else
                {
                    lblDescription.IsVisible = false;
                }
                
                if (data.Icon != null)
                {
                    imgIcon.IsVisible = true;
                    imgIcon.Source = data.Icon;
                }
                else
                {
                    imgIcon.Source = null;
                    imgIcon.IsVisible = false;
                }
            }
            catch
            {

            }
        }
    }
}