using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TweetTail
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SingleTailPage : TabbedPage
    {
        public SingleTailPage ()
        {
            InitializeComponent();

            if (App.tail.blend.SelectedBlendedAccount != null)
            {
                Title = App.tail.blend.SelectedBlendedAccount.name + " 병합계정";
            }
            else
            {
                Title = App.tail.account.SelectedAccountGroup.accountForRead.user.nickName + " 계정";
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
            if(App.tail.blend.SelectedBlendedAccount != null)
            {
                Title = App.tail.blend.SelectedBlendedAccount.name + " 병합계정";
            }
            else
            {
                Title = App.tail.account.SelectedAccountGroup.accountForRead.user.nickName + " 계정";
            }

            timelinePage.Reload();
            mentionPage.Reload();
            notificationPage.Reload();
        }
    }
}