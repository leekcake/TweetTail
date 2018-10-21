using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetTail.Controls.TabbedPageExpanded;
using TweetTail.Pages.Notification;
using TweetTail.Pages.Status;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace TweetTail.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SingleTailPage : TabbedPageEx
    {
        public SingleTailPage ()
        {
            InitializeComponent();

            if (App.Tail.Blend.SelectedBlendedAccount != null)
            {
                Title = App.Tail.Blend.SelectedBlendedAccount.Name + " 병합계정";
            }
            else
            {
                Title = App.Tail.Account.SelectedAccountGroup.AccountForRead.User.NickName + " 계정";
            }

            On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);

            TabReselected += SingleTailPage_TabReselected;
        }

        private void SingleTailPage_TabReselected(object sender, EventArgs args)
        {
            if( CurrentPage is TimelinePage )
            {
                (CurrentPage as TimelinePage).ScrollToRoot();
            }
            else if( CurrentPage is MentionPage )
            {
                (CurrentPage as MentionPage).ScrollToRoot();
            }
            else if( CurrentPage is NotificationPage )
            {
                (CurrentPage as NotificationPage).ScrollToRoot();
            }
        }

        public static void ReloadInNavigationStack()
        {
            foreach (var page in App.Navigation.NavigationStack)
            {
                if (page is SingleTailPage)
                {
                    (page as SingleTailPage).Reload();
                    break;
                }
            }
        }

        public void Reload()
        {
            if(App.Tail.Blend.SelectedBlendedAccount != null)
            {
                Title = App.Tail.Blend.SelectedBlendedAccount.Name + " 병합계정";
            }
            else
            {
                Title = App.Tail.Account.SelectedAccountGroup.AccountForRead.User.NickName + " 계정";
            }

            TimelinePage.Reload();
            MentionPage.Reload();
            NotificationPage.Reload();
        }
    }
}