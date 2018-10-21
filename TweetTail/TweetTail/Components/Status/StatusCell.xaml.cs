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
            StatusView.UpdateImage();
        }

        protected override void OnBindingContextChanged()
        {
            StatusView.BindingContext = BindingContext;
            StatusView.Update();
            base.OnBindingContextChanged();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            StatusView.ClearImage();
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();
            StatusView.Update();
        }
    }
}