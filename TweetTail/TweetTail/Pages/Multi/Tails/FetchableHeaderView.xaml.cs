using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TweetTail.Pages.Multi.Tails
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FetchableHeaderView : ContentView
    {
        public int RefreshRate = 7200; //큿

        private Action refreshAction;
        public Action RefreshAction {
            get {
                return refreshAction;
            }
            set {
                refreshAction = value;
                DoRefresh();
            }
        }

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
                    AutoRefreshAsync();
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

        private Task autoRefreshWaiter;
        private CancellationTokenSource autoRefreshToken = new CancellationTokenSource();
        private async void AutoRefreshAsync()
        {
            if(autoRefreshWaiter != null)
            {
                return;
            }
            
            await Task.Delay(RefreshRate);
            autoRefreshWaiter = null;
            if (autoRefreshToken.Token.IsCancellationRequested)
            {
                autoRefreshToken = new CancellationTokenSource();
                return;
            }
            DoRefresh();
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

            RefreshIcon.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    if(!AutoRefresh)
                        DoRefresh();
                })
            });

            AutoRefreshIcon.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    AutoRefresh = !AutoRefresh;
                    if (AutoRefresh) AutoRefreshAsync();
                    else autoRefreshToken.Cancel();
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
            refreshAction?.Invoke();
        }
    }
}