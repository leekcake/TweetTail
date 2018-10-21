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

[assembly: ExportRenderer(typeof(ListView), typeof(ListViewRendererFix))]
namespace TweetTail.WPF.Hotfix.Renderers.ListViewFix
{
    public class ListViewRendererFix : ViewRenderer<ListView, WList>
    {
        private ObservableCollection<object> InternalItems = new ObservableCollection<object>();

        ITemplatedItemsView<Cell> TemplatedItemsView => Element;

        public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            SizeRequest result = base.GetDesiredSize(widthConstraint, heightConstraint);
            result.Minimum = new Size(40, 40);
            return result;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            if (e.OldElement != null) // Clear old element event
            {
                var templatedItems = ((ITemplatedItemsView<Cell>)e.OldElement).TemplatedItems;
                templatedItems.CollectionChanged -= TemplatedItems_CollectionChanged;
                templatedItems.GroupedCollectionChanged -= TemplatedItems_GroupedCollectionChanged;
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

                TemplatedItemsView.TemplatedItems.CollectionChanged += TemplatedItems_CollectionChanged;
                TemplatedItemsView.TemplatedItems.GroupedCollectionChanged += TemplatedItems_GroupedCollectionChanged;

                UpdateItemSource();
            }

            Control.SetValue(VirtualizingPanel.ScrollUnitProperty, ScrollUnit.Pixel);
            base.OnElementChanged(e);
        }

        private void TemplatedItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 0)
                        goto case NotifyCollectionChangedAction.Reset;

                    // if a NewStartingIndex that's too high is passed in just add the items.
                    // I realize this is enforcing bad behavior but prior to this synchronization
                    // code being added it wouldn't cause the app to crash whereas now it does
                    // so this code accounts for that in order to ensure smooth sailing for the user
                    if (e.NewStartingIndex >= InternalItems.Count)
                    {
                        for (int i = 0; i < e.NewItems.Count; i++)
                            InternalItems.Add(e.NewItems[i]);
                    }
                    else
                    {
                        for (int i = e.NewItems.Count - 1; i >= 0; i--)
                            InternalItems.Insert(e.NewStartingIndex, e.NewItems[i]);
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    for (int i = e.OldItems.Count - 1; i >= 0; i--)
                        InternalItems.RemoveAt(e.OldStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Move:
                    for (var i = 0; i < e.OldItems.Count; i++)
                    {
                        var oldi = e.OldStartingIndex;
                        var newi = e.NewStartingIndex;

                        if (e.NewStartingIndex < e.OldStartingIndex)
                        {
                            oldi += i;
                            newi += i;
                        }

                        // we know that wrapped collection is an ObservableCollection<object>
                        InternalItems.Move(oldi, newi);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    try
                    {
                        InternalItems[e.OldStartingIndex] = e.NewItems[0];
                    }
                    catch
                    {
                        goto case NotifyCollectionChangedAction.Reset;
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                default:
                    UpdateItemSource();
                    break;
            }
        }

        private void TemplatedItems_GroupedCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //TODO: Dynamic Update of InternalItem
            UpdateItemSource();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == ListView.IsGroupingEnabledProperty.PropertyName)
            {
                //UpdateGrouping();
            }


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
            InternalItems.Clear();
            if (Element.IsGroupingEnabled)
            {
                int index = 0;
                foreach (var groupItem in TemplatedItemsView.TemplatedItems)
                {
                    var group = TemplatedItemsView.TemplatedItems.GetGroup(index);

                    if (group.Count != 0)
                    {
                        InternalItems.Add(group.HeaderContent);

                        /*if (HasHeader(group))
							_cells.Add(GetCell(group.HeaderContent));
						else
							_cells.Add(CreateEmptyHeader());*/

                        foreach(var item in group)
                        {
                            InternalItems.Add(item);
                        }
                    }

                    index++;
                }
            }
            else
            {
                foreach (var item in TemplatedItemsView.TemplatedItems)
                {
                    InternalItems.Add(item);
                }
            }
            Control.ItemsSource = InternalItems;
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

                if (Element != null)
                {
                    TemplatedItemsView.TemplatedItems.CollectionChanged -= TemplatedItems_GroupedCollectionChanged;
                    TemplatedItemsView.TemplatedItems.GroupedCollectionChanged -= TemplatedItems_GroupedCollectionChanged;
                }
            }
            Token?.Dispose();
            _isDisposed = true;
            base.Dispose(disposing);
        }

        private CancellationTokenSource Token;

        //Dirty Refresh of Width
        protected override void UpdateWidth()
        {
            if (_isDisposed)
                return;

            base.UpdateWidth();
            if (Control == null || Element == null)
                return;

            if (Token != null)
            {
                Token.Cancel();
                Token = null;
            }
            Token = new CancellationTokenSource();
            var holder = Token;

            Application.Current.Dispatcher.BeginInvoke(new Action(async () =>
            {
                try
                {
                    await Task.Delay(100, holder.Token);
                }
                catch
                {
                    return;
                }

                var source = Element.ItemsSource;

                TemplatedItemsView.TemplatedItems.CollectionChanged -= TemplatedItems_CollectionChanged;
                TemplatedItemsView.TemplatedItems.GroupedCollectionChanged -= TemplatedItems_GroupedCollectionChanged;

                Element.ItemsSource = null;
                Element.ItemsSource = source;

                TemplatedItemsView.TemplatedItems.CollectionChanged += TemplatedItems_CollectionChanged;
                TemplatedItemsView.TemplatedItems.GroupedCollectionChanged += TemplatedItems_GroupedCollectionChanged;
                
                UpdateItemSource();
                Token = null;
            }));
        }
    }
}
