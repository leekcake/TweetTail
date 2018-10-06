using Android.Content;
using Android.Support.Design.Widget;
using Android.Views;
using TweetTail.Controls.TabbedPageExpanded;
using Xamarin.Forms;
using TweetTail.Droid.Controls.TabbedPageExpanded;
using Xamarin.Forms.Platform.Android.AppCompat;
using Xamarin.Forms.Platform.Android;
using System;
using System.Reflection;

[assembly: ExportRenderer(typeof(TabbedPageEx), typeof(TabbedPageExRenderer))]
namespace TweetTail.Droid.Controls.TabbedPageExpanded
{
    public class TabbedPageExRenderer : TabbedPageRenderer, TabLayout.IOnTabSelectedListener, BottomNavigationView.IOnNavigationItemReselectedListener
    {
        public TabbedPageExRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<TabbedPage> e)
        {
            base.OnElementChanged(e);

            try
            {
                //https://github.com/xamarin/Xamarin.Forms/blob/master/Xamarin.Forms.Platform.Android/AppCompat/TabbedPageRenderer.cs#L37
                //Trickly way to access _bottomNavigationView
                //It may be failed to get...
                var field = typeof(TabbedPageRenderer).GetField("_bottomNavigationView", BindingFlags.NonPublic | BindingFlags.Instance);
                var bottom = field.GetValue(this) as BottomNavigationView;
                bottom.SetOnNavigationItemReselectedListener(this);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message + " " + ex.StackTrace);
            }
        }

        void TabLayout.IOnTabSelectedListener.OnTabReselected(TabLayout.Tab tab)
        {
            (Element as TabbedPageEx).OnTabReselected();
        }

        void BottomNavigationView.IOnNavigationItemReselectedListener.OnNavigationItemReselected(IMenuItem item)
        {
            (Element as TabbedPageEx).OnTabReselected();
        }
    }
}