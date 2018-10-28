using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using TweetTail.WPF.Hotfix.Renderers.Listview;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WPF;

using ListView = Xamarin.Forms.ListView;
using WList = System.Windows.Controls.ListView;
using Application = System.Windows.Application;
using Size = Xamarin.Forms.Size;
using System.Windows.Input;
using System.ComponentModel;
using DataTemplate = System.Windows.DataTemplate;
using Xamarin.Forms.Internals;
using DataTemplateSelector = System.Windows.Controls.DataTemplateSelector;

[assembly: ExportRenderer(typeof(ListView), typeof(ListViewRendererFix))]
namespace TweetTail.WPF.Hotfix.Renderers.Listview
{
    public class ListViewRendererFix : ViewRenderer<ListView, WList>
    {
        public static T GetChildOfType<T>(DependencyObject depObj)
where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null) return result;
            }
            return null;
        }

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
                    Control.ItemContainerGenerator.ItemsChanged += ItemContainerGenerator_ItemsChanged;
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

        private void ItemContainerGenerator_ItemsChanged(object sender, System.Windows.Controls.Primitives.ItemsChangedEventArgs e)
        {
            CheckScroll();
        }

        private ScrollViewer scrollViewer;

        private void CheckScroll()
        {
            if (scrollViewer != null) return;

            scrollViewer = GetChildOfType<ScrollViewer>(Control);
            if (scrollViewer != null)
            {
                scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
            }
        }

        protected virtual void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;

            //try to Filter height changes by late loaded item (virtualization)
            if (e.ExtentHeightChange != 0 && e.VerticalOffset != 0 && e.VerticalChange == 0)
            {
                System.Diagnostics.Debug.WriteLine(e.VerticalChange + " / " + e.ExtentHeightChange);
                scrollViewer.ScrollToVerticalOffset(e.VerticalOffset + e.ExtentHeightChange);
            }
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

                try
                {
                    scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
                    scrollViewer = null;
                }
                catch
                {

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
