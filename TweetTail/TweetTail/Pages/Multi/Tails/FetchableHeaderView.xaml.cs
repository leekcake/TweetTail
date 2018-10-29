using FFImageLoading.Forms;
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
                    StartAutoRefreshWaiter();
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

        private object autoRefreshWaiter = new object();
        private void StartAutoRefreshWaiter()
        {
            new Task(() =>
            {
                lock (autoRefreshWaiter)
                {
                    if (!Monitor.Wait(autoRefreshWaiter, RefreshRate))
                    {
                        DoRefresh();
                    }
                }
            }).Start();
        }

        private void UpdateAutoRefreshIcon()
        {
            AutoRefreshIcon.Source = AutoRefresh ? "ic_sync_green_300_24dp" : "ic_sync_disabled_grey_500_24dp";
        }

        public CachedImage AddIcon(string image)
        {
            var result = new CachedImage();
            result.WidthRequest = 16;
            result.HeightRequest = 16;
            ExtraIconsView.Children.Add(result);
            result.Source = image;

            return result;
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
                    if (AutoRefresh) StartAutoRefreshWaiter();
                    else lock(autoRefreshWaiter) Monitor.PulseAll(autoRefreshWaiter);
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