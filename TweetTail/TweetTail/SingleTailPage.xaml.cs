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
            timelinePage.Reload();
            mentionPage.Reload();
            notificationPage.Reload();
        }
    }
}