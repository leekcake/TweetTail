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
        public int RefreshRate = 7200; //큿
        
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

                if (!inRefresh && AutoRefresh)
                {
                    new Task(async () =>
                    {
                        System.Diagnostics.Debug.WriteLine("Refresh requested");
                        await Task.Delay(RefreshRate);
                        DoRefresh();
                    }).Start();
                }

                UpdateRefreshIcon();
                return;
            }
        }

        private void UpdateRefreshIcon()
        {
            RefreshIcon.Source = InRefresh ? "ic_repeat_green_300_24dp" : "ic_repeat_grey_500_24dp";
        }

        private bool autoRefresh = true;
        public bool AutoRefresh {
            get {
                return autoRefresh;
            }
            set {
                if (value == autoRefresh) return;
                autoRefresh = value;
                UpdateAutoRefreshIcon();
            }
        }
        private void UpdateAutoRefreshIcon()
        {
            AutoRefreshIcon.Source = AutoRefresh ? "ic_sync_green_300_24dp" : "ic_sync_disabled_grey_500_24dp";
        }

        public FetchableHeaderView()
        {
            InitializeComponent();

            UpdateRefreshIcon();
            UpdateAutoRefreshIcon();

            if (AutoRefresh) DoRefresh();

            RefreshIcon.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    if(!AutoRefresh)
                        DoRefresh();
                })
            });
        }

        private void DoRefresh()
        {
            if (InRefresh)
            {
                return;
            }
            InRefresh = true;
            if(OnRefreshRequested != null)
            {
                OnRefreshRequested.Invoke();
            }
            else
            {
                InRefresh = false;
            }
        }
    }
}