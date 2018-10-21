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
                TitleLabel.Text = data.Title;
                if (data.Description != null)
                {
                    DescriptionLabel.IsVisible = true;
                    DescriptionLabel.Text = data.Description;
                }
                else
                {
                    DescriptionLabel.IsVisible = false;
                }
                
                if (data.Icon != null)
                {
                    IconImage.IsVisible = true;
                    IconImage.Source = data.Icon;
                }
                else
                {
                    IconImage.Source = null;
                    IconImage.IsVisible = false;
                }
            }
            catch
            {

            }
        }
    }
}