using Library.Container.Fetch;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using TwitterInterface.Data;
using Xamarin.Forms;

namespace TweetTail.Utils
{
    public abstract class TwitterListView<Data, Cell> : ListView where Cell : ViewCell
    {
        private Fetchable<Data> fetchable;
        public Fetchable<Data> Fetchable {
            get {
                return fetchable;
            }
            set {
                fetchable = value;
                Items.Clear();
                if (value != null)
                {
                    IsPullToRefreshEnabled = value.IsSupportRefresh;
                }
                else
                {
                    IsPullToRefreshEnabled = false;
                }
                Reload();
            }
        }

        public ObservableCollection<Data> Items { get; set; }

        private bool isLoading;
        private bool isNoMoreData;
        private void FlagNoMoreData() {
            isNoMoreData = true;
            Footer = "더이상 자료가 없습니다";
        }

        public TwitterListView() : base(ListViewCachingStrategy.RecycleElement)
        {
            HasUnevenRows = true;
            ItemTemplate = new DataTemplate(typeof(Cell));
            Items = new ObservableCollection<Data>();
            ItemsSource = Items;
            SelectionMode = ListViewSelectionMode.None;
            RefreshCommand = new Command(Refresh);

            if (Footer == null)
            {
                Footer = "불러오는중...";
                ItemAppearing += TwitterListView_ItemAppearing;
            }
        }

        public void ScrollToRoot()
        {
            ScrollTo(Items[0], ScrollToPosition.Start, true);
        }

        public void Reload()
        {
            isNoMoreData = false;
            if(Footer is string)
            {
                if((string) Footer == "더이상 자료가 없습니다")
                {
                    Footer = "불러오는중...";
                }
            }

            Items.Clear();
            BeginRefresh();
        }

        private void TwitterListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            if (isLoading || isNoMoreData || Items.Count == 0)
                return;

            //hit bottom!
            if ( e.Item == Items[Items.Count - 1] as object)
            {
                LoadMore();
            }
        }

        public async void LoadMore()
        {
            try
            {
                if (fetchable != null)
                {
                    var datas = await fetchable.FetchOld();
                    if (datas.Count == 0)
                    {
                        isLoading = false;
                        FlagNoMoreData();
                        return;
                    }
                    
                    foreach(var data in datas)
                    {
                        Items.Add(data);
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message + "\n" + e.StackTrace);
                EndRefresh();
            }
        }

        public async void Refresh() //Method to Get latest data
        {
            try
            {
                if (fetchable != null)
                {
                    var datas = await fetchable.FetchNew();
                    if (datas.Count == 0)
                    {
                        if(Items.Count == 0)
                        {
                            Footer = "이 목록이 비어있는것 같습니다!";
                        }
                        EndRefresh();
                        return;
                    }

                    Data topItem;
                    if(Items.Count != 0)
                    {
                        topItem = Items[0];
                    }
                    else
                    {
                        topItem = datas[0];
                    }
                    
                    for (int i = datas.Count - 1; i >= 0; i--)
                    {
                        Items.Insert(0, datas[i]);
                    }

                    //갑자기 아래로 내려가 있으면 이상하기 때문에 유저 경험을 위해 추가되기 전에 맨 위에 있었을 아이템을 맨 위로 팍 올린뒤
                    //스크롤을 자연스럽게 내려줌
                    ScrollTo(topItem, ScrollToPosition.Start, false);
                    ScrollTo(topItem, ScrollToPosition.End, true);
                }

                EndRefresh();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message + "\n" + e.StackTrace);
                EndRefresh();
            }
        }
    }
}
