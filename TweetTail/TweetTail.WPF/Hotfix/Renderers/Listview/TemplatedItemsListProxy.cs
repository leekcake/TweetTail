using TweetTail.WPF.Hotfix.Renderers.Listview;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WPF;
using System.Collections.Specialized;

using ListView = Xamarin.Forms.ListView;
using System.Collections.Generic;
using System.Collections;

[assembly: ExportRenderer(typeof(ListView), typeof(ListViewRendererFix))]
namespace TweetTail.WPF.Hotfix.Renderers.Listview
{
    public class TemplatedItemsListProxy : IList<object>, INotifyCollectionChanged
    {
        private class Enumerator : IEnumerator<object>
        {
            private IEnumerator<object> enumerator;
            private bool inited = false, parentFinished = false;
            private bool headerProvided, footerProvided;

            private TemplatedItemsListProxy parent;

            public Enumerator(TemplatedItemsListProxy parent, IEnumerator<object> enumerator)
            {
                this.parent = parent;
                this.enumerator = enumerator;

                Reset();
            }

            public object Current {
                get {
                    if (!headerProvided) return parent.header;
                    if (parentFinished)
                    {
                        if (!footerProvided) return parent.footer;
                    }
                    else
                    {
                        return enumerator.Current;
                    }
                    return null;
                }
            }

            public void Dispose()
            {
                enumerator.Dispose();
            }

            public bool MoveNext()
            {
                if (!headerProvided)
                {
                    if (!inited)
                    {
                        inited = true;
                    }
                    else
                    {
                        headerProvided = true;
                        return MoveNext();
                    }
                    return true;
                }
                else if (parentFinished)
                {
                    if (footerProvided)
                    {
                        return false;
                    }
                    footerProvided = true;
                    return true;
                }
                var result = enumerator.MoveNext();
                if (!result)
                {
                    parentFinished = true;
                    if (footerProvided)
                    {
                        return false;
                    }
                }
                return true;
            }

            public void Reset()
            {
                headerProvided = parent.header == null;
                parentFinished = false;
                footerProvided = parent.footer == null;
                //enumerator.Reset();
            }
        }

        private ListView listview;

        private object header, footer;
        private bool headerExist, footerExist;

        private ITemplatedItemsList<Cell> templated;

        public bool HeaderExist {
            get {
                return headerExist;
            }
        }

        public bool FooterExist {
            get {
                return footerExist;
            }
        }

        public TemplatedItemsListProxy(ListView listview)
        {
            this.listview = listview;
            templated = ((ITemplatedItemsView<Cell>)listview).TemplatedItems;
            templated.CollectionChanged += Templated_CollectionChanged;

            UpdateHeader();
            UpdateFooter();
        }

        private void UpdateVirtualItems(object replace, int inx, ref bool existBuffer, ref object into)
        {
            if (replace == null)
            {
                var old = into;
                into = null;
                if (existBuffer)
                {
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, null, new List<object>()
                        {
                            old
                        }));
                    existBuffer = false;
                }
            }
            else
            {
                var old = into;
                into = replace;
                if (!existBuffer)
                {
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, null, new List<object>()
                        {
                            replace
                        }, inx));
                    existBuffer = true;
                }
                else
                {
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, replace, old, inx));
                }
            }
        }

        public void UpdateHeader()
        {
            UpdateVirtualItems(listview.HeaderElement, 0, ref headerExist, ref header);
        }

        public void UpdateFooter()
        {
            UpdateVirtualItems(listview.FooterElement, Count - 1, ref footerExist, ref footer);
        }

        private void Templated_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (headerExist)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Reset:
                        CollectionChanged?.Invoke(sender, e);
                        return;
                    case NotifyCollectionChangedAction.Add:
                        CollectionChanged?.Invoke(sender, new NotifyCollectionChangedEventArgs(e.Action, e.NewItems, e.NewStartingIndex + 1));
                        return;
                    case NotifyCollectionChangedAction.Move:
                        CollectionChanged?.Invoke(sender, new NotifyCollectionChangedEventArgs(e.Action, e.OldItems, e.NewStartingIndex + 1, e.OldStartingIndex + 1));
                        return;
                    case NotifyCollectionChangedAction.Remove:
                        CollectionChanged?.Invoke(sender, new NotifyCollectionChangedEventArgs(e.Action, e.OldItems, e.OldStartingIndex + 1));
                        return;
                    case NotifyCollectionChangedAction.Replace:
                        CollectionChanged?.Invoke(sender, new NotifyCollectionChangedEventArgs(e.Action, e.NewItems, e.OldItems, e.OldStartingIndex + 1));
                        return;
                }
            }
            CollectionChanged?.Invoke(sender, e);
        }

        public object this[int index] {
            get {
                if (index == 0 && header != null)
                {
                    return header;
                }
                else if (index == Count - 1 && footer != null)
                {
                    return footer;
                }

                return templated[index - (header == null ? 0 : 1)];
            }
            set { }
        }

        public int Count => templated.Count + (header == null ? 0 : 1) + (footer == null ? 0 : 1);

        public bool IsReadOnly => true;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Add(object item)
        {
            return;
        }

        public void Clear()
        {
            return;
        }

        public bool Contains(object item)
        {
            return true;
        }

        public void CopyTo(object[] array, int arrayIndex)
        {
            return;
        }

        public IEnumerator<object> GetEnumerator()
        {
            return new Enumerator(this, templated.GetEnumerator());
        }

        public int IndexOf(object item)
        {
            return -1;
        }

        public void Insert(int index, object item)
        {
            return;
        }

        public bool Remove(object item)
        {
            return false;
        }

        public void RemoveAt(int index)
        {
            return;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this, templated.GetEnumerator());
        }
    }
}
