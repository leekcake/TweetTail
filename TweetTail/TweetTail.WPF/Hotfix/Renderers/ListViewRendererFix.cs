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

[assembly: ExportRenderer(typeof(ListView), typeof(ListViewRendererFix))]
namespace TweetTail.WPF.Hotfix.Renderers.ListViewFix
{
    public class ListViewRendererFix : ViewRenderer<ListView, WList>
    {
        private class TemplatedItemsListProxy : IList<Cell>, INotifyCollectionChanged
        {
            private ITemplatedItemsList<Cell> templated;

            public TemplatedItemsListProxy(ITemplatedItemsList<Cell> templated)
            {
                this.templated = templated;

                templated.CollectionChanged += Templated_CollectionChanged;
            }

            private void Templated_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                CollectionChanged?.Invoke(sender, e);
            }

            public Cell this[int index] { get => templated[index]; set { } }

            public int Count => templated.Count;

            public bool IsReadOnly => true;

            public event NotifyCollectionChangedEventHandler CollectionChanged;

            public void Add(Cell item)
            {
                return;
            }

            public void Clear()
            {
                return;
            }

            public bool Contains(Cell item)
            {
                return true;
            }

            public void CopyTo(Cell[] array, int arrayIndex)
            {
                return;
            }

            public IEnumerator<Cell> GetEnumerator()
            {
                return templated.GetEnumerator();
            }

            public int IndexOf(Cell item)
            {
                return -1;
            }

            public void Insert(int index, Cell item)
            {
                return;
            }

            public bool Remove(Cell item)
            {
                return false;
            }

            public void RemoveAt(int index)
            {
                return;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return templated.GetEnumerator();
            }
        }

        ITemplatedItemsView<Cell> TemplatedItemsView => Element;

        public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            SizeRequest result = base.GetDesiredSize(widthConstraint, heightConstraint);
            result.Minimum = new Size(40, 40);
            return result;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            if(e.OldElement != null)
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
                        ItemTemplate = (DataTemplate)System.Windows.Application.Current.Resources["CellTemplateFix"],
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

            var scrollArgs = (ITemplatedItemsListScrollToRequestedEventArgs) e;

            Control.ScrollIntoView(scrollArgs.Item);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            
            /*if (e.PropertyName == ListView.SelectedItemProperty.PropertyName)
				OnItemSelected(Element.SelectedItem);
			else if (e.PropertyName == "HeaderElement")
				UpdateHeader();
			else if (e.PropertyName == "FooterElement")
				UpdateFooter();
			else if ((e.PropertyName == ListView.IsRefreshingProperty.PropertyName) || (e.PropertyName == ListView.IsPullToRefreshEnabledProperty.PropertyName) || (e.PropertyName == "CanRefresh"))
				UpdateIsRefreshing();
			else if (e.PropertyName == "GroupShortNameBinding")
				UpdateJumpList();*/
        }
        
        void UpdateItemSource()
        {
            Control.ItemsSource = new TemplatedItemsListProxy(TemplatedItemsView.TemplatedItems);
        }

        void OnNativeKeyUp(object sender, KeyEventArgs e)
            => Element.NotifyRowTapped(Control.SelectedIndex, cell: null);

        void OnNativeMouseUp(object sender, MouseButtonEventArgs e)
            => Element.NotifyRowTapped(Control.SelectedIndex, cell: null);

        void OnNativeTouchUp(object sender, TouchEventArgs e)
            => Element.NotifyRowTapped(Control.SelectedIndex, cell: null);

        void OnNativeStylusUp(object sender, StylusEventArgs e)
            => Element.NotifyRowTapped(Control.SelectedIndex, cell: null);

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
