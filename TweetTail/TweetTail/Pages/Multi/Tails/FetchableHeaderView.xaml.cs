using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TweetTail.Pages.Multi.Tails
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FetchableHeaderView : ContentView
    {
        public delegate void RefreshRequest();
        public event RefreshRequest OnRefreshRequested;

        public bool IsAutomaticRefresh {
            get {
                return false;
            }
        }

        private bool inRefresh = false;
        public bool InRefresh {
            get {
                return inRefresh;
            }
            set {
                if (value == inRefresh) return;
                inRefresh = value;
                UpdateRefreshIcon();
                return;
            }
        }

        private void UpdateRefreshIcon()
        {
            RefreshIcon.Source = InRefresh ? "ic_repeat_green_300_24dp" : "ic_repeat_grey_500_24dp";
        }

        public FetchableHeaderView()
        {
            InitializeComponent();
            UpdateRefreshIcon();
            RefreshIcon.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    if(InRefresh)
                    {
                        return;
                    }
                    InRefresh = true;
                    OnRefreshRequested?.Invoke();
                })
            });
        }
    }
}