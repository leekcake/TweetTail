using FFImageLoading.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DataStatus = TwitterInterface.Data.Status;
using FFImageLoading.Transformations;

namespace TweetTail.Components.Status
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatusCell : ViewCell
    {
        public StatusCell()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            statusView.UpdateImage();
        }

        protected override void OnBindingContextChanged()
        {
            statusView.BindingContext = BindingContext;
            statusView.Update();
            base.OnBindingContextChanged();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            statusView.ClearImage();
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();
            statusView.Update();
        }
    }
}