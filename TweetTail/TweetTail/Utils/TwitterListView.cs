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
        public abstract long GetID(Data data);

        private long sinceIndex = -1, maxIndex = -1;
        public ObservableCollection<Data> Items { get; set; }

        private Func<long, long, Task<List<Data>>> sinceMaxGetter;
        public Func<long, long, Task<List<Data>>> SinceMaxGetter {
            get {
                return sinceMaxGetter;
            }
            set {
                sinceMaxGetter = value;

                if (value != null)
                {
                    IsPullToRefreshEnabled = true;
                    RefreshCommand = new Command(Refresh);
                }
                else
                {
                    IsPullToRefreshEnabled = false;
                }
            }
        }
        public Func<long, Task<CursoredList<Data>>> cursoredGetter = null;

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

            Footer = "불러오는중...";
            ItemAppearing += TwitterListView_ItemAppearing;
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
                if (sinceMaxGetter != null)
                {
                    var datas = await sinceMaxGetter(-1, maxIndex - 1);
                    if (datas.Count == 0)
                    {
                        isLoading = false;
                        FlagNoMoreData();
                        return;
                    }

                    sinceIndex = GetID(datas[0]);
                    maxIndex = GetID(datas[datas.Count - 1]);
                    foreach(var data in datas)
                    {
                        Items.Add(data);
                    }
                }
                else if(cursoredGetter != null)
                {
                    var datas = await cursoredGetter(maxIndex);

                    if (datas.Count == 0)
                    {
                        isLoading = false;
                        FlagNoMoreData();
                        return;
                    }

                    maxIndex = datas.nextCursor;

                    foreach (var data in datas)
                    {
                        Items.Add(data);
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message + "\n" + e.StackTrace);
            }
        }

        public async void Refresh() //Method to Get latest data
        {
            try
            {
                if (sinceMaxGetter != null)
                {
                    var datas = await sinceMaxGetter(sinceIndex, -1);
                    if (datas.Count == 0)
                    {
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

                    sinceIndex = GetID(datas[0]);
                    maxIndex = GetID(datas[datas.Count - 1]);
                    for (int i = datas.Count - 1; i >= 0; i--)
                    {
                        Items.Insert(0, datas[i]);
                    }
                    
                    ScrollTo(topItem, ScrollToPosition.End, false);
                }
                else if(cursoredGetter != null)
                {
                    var datas = await cursoredGetter(-1);
                    if (datas.Count == 0)
                    {
                        EndRefresh();
                        return;
                    }

                    maxIndex = datas.nextCursor;
                    for (int i = datas.Count - 1; i >= 0; i--)
                    {
                        Items.Insert(0, datas[i]);
                    }
                }

                EndRefresh();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message + "\n" + e.StackTrace);
            }
        }
    }
}
