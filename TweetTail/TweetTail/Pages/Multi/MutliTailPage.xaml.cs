using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetTail.Pages.Multi.Tails;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TweetTail.Pages.Multi
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MutliTailPage : ContentPage
    {
        public MutliTailPage()
        {
            InitializeComponent();

            foreach (var group in App.Tail.Account.ReadOnlyAccountGroups)
            {
                if(!group.AccountForRead.User.IsProtected)
                    TailContainer.Children.Add( new TimelineTail(group) );
            }
        }
    }
}