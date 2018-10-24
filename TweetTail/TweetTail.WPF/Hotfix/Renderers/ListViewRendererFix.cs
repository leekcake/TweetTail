using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using TweetTail.WPF.Hotfix.Renderers.ListViewFix;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WPF;
using System.Collections.Specialized;

using ListView = Xamarin.Forms.ListView;
using WList = System.Windows.Controls.ListView;
using Application = System.Windows.Application;
using Size = Xamarin.Forms.Size;
using System.Windows.Input;
using System.Collections.Generic;
using System.ComponentModel;
using DataTemplate = System.Windows.DataTemplate;
using System.Collections;
using Xamarin.Forms.Internals;
using DataTemplateSelector = System.Windows.Controls.DataTemplateSelector;

[assembly: ExportRenderer(typeof(ListView), typeof(ListViewRendererFix))]
namespace TweetTail.WPF.Hotfix.Renderers.ListViewFix
{
    public class ListViewRendererFix : ViewRenderer<ListView, WList>
    {
        protected class ListViewDataTemplateSelector : DataTemplateSelector
        {
            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                if (item is Cell)
                {
                    return (DataTemplate)System.Windows.Application.Current.Resources["CellTemplateFix"];
                }
                else
                {
                    return (DataTemplate)System.Windows.Application.Current.Resources["View"];
                }
            }
        }

        protected class TemplatedItemsListProxy : IList<object>, INotifyCollectionChanged
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
                if(headerExist)
                {
                    switch(e.Action)
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
                    if(index == 0 && header != null)
                    {
                        return header;
                    }
                    else if(index == Count - 1 && footer != null)
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

        ITemplatedItemsView<Cell> TemplatedItemsView => Element;
        private TemplatedItemsListProxy proxy;

        public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            SizeRequest result = base.GetDesiredSize(widthConstraint, heightConstraint);
            result.Minimum = new Size(40, 40);
            return result;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            if (e.OldElement != null)
            {
                e.OldElement.ScrollToRequested -= Element_ScrollToRequested;
            }

            if (e.NewElement != null)
            {
                if (Control == null) // construct and SetNativeControl and suscribe control event
                {
                    var listView = new WList
                    {
                        DataContext = Element,
                        ItemTemplateSelector = new ListViewDataTemplateSelector(),
                        Style = (System.Windows.Style)System.Windows.Application.Current.Resources["ListViewTemplate"]
                    };
                    SetNativeControl(listView);
                    Control.MouseUp += OnNativeMouseUp;
                    Control.KeyUp += OnNativeKeyUp;
                    Control.TouchUp += OnNativeTouchUp;
                    Control.StylusUp += OnNativeStylusUp;
                }

                e.NewElement.ScrollToRequested += Element_ScrollToRequested;

                UpdateItemSource();
            }

            Control.SetValue(VirtualizingPanel.ScrollUnitProperty, ScrollUnit.Pixel);
            base.OnElementChanged(e);
        }

        private void Element_ScrollToRequested(object sender, ScrollToRequestedEventArgs e)
        {
            //TODO: Animate Support
            //TODO: Support Position

            var scrollArgs = (ITemplatedItemsListScrollToRequestedEventArgs)e;

            Control.ScrollIntoView(scrollArgs.Item);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == ListView.SelectedItemProperty.PropertyName)
            {
                UpdateSelectedItem();
            }
            else if(e.PropertyName == "HeaderElement")
            {
                proxy.UpdateHeader();
            }
            else if(e.PropertyName == "FooterElement")
            {
                proxy.UpdateFooter();
            }
        }

        protected void UpdateSelectedItem()
        {
            Control.SelectedItem = Element.SelectedItem;
        }

        void UpdateItemSource()
        {
            Control.ItemsSource = proxy = new TemplatedItemsListProxy(Element);
        }

        void OnNativeKeyUp(object sender, KeyEventArgs e)
            => HandleRowTapped();

        void OnNativeMouseUp(object sender, MouseButtonEventArgs e)
            => HandleRowTapped();

        void OnNativeTouchUp(object sender, TouchEventArgs e)
            => HandleRowTapped();

        void OnNativeStylusUp(object sender, StylusEventArgs e)
            => HandleRowTapped();

        private void HandleRowTapped()
        {
            var headerTapped = proxy.HeaderExist && Control.SelectedIndex == 0;
            var footerTapped = proxy.FooterExist && Control.SelectedIndex == proxy.Count - 1;
            if (headerTapped || footerTapped)
            {
                //Cause Out of Index if continue calling because user tapped virtual item and ListView doesn't know about that.
                return;
            }
            Element.NotifyRowTapped(Control.SelectedIndex - (proxy.HeaderExist ? 1 : 0), cell: null);
        }

        bool _isDisposed;

        protected override void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
            {
                if (Control != null)
                {
                    Control.MouseUp -= OnNativeMouseUp;
                    Control.KeyUp -= OnNativeKeyUp;
                    Control.TouchUp -= OnNativeTouchUp;
                    Control.StylusUp -= OnNativeStylusUp;
                }
            }
            _isDisposed = true;
            base.Dispose(disposing);
        }

        //Dirty Refresh of Width
        protected override void UpdateWidth()
        {
            if (_isDisposed)
                return;

            base.UpdateWidth();
            if (Control == null || Element == null)
                return;

            try
            {
                foreach (var item in TemplatedItemsView.TemplatedItems)
                {
                    if (item is ViewCell viewCell)
                    {
                        var element = Platform.GetRenderer(viewCell.View)?.GetNativeElement();
                        if (element != null)
                        {
                            element.Width = Control.Width - 15;
                        }
                    }
                }
            }
            catch
            {

            }
        }
    }
}
