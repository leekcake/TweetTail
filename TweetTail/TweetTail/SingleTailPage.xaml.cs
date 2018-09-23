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
    }
}