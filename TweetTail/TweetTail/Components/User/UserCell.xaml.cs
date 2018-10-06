using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TweetTail.Components.User
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserCell : ViewCell
    {
        public UserCell()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            userView.BindingContext = BindingContext;
            userView.Update();
            base.OnBindingContextChanged();
        }
    }
}