using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WPF;

namespace TweetTail.WPF.Hotfix
{
    public class CellControlFix : CellControl
    {
        public CellControlFix()
        {
            Loaded += CellControlFix_Loaded;
        }
        
        private void CellControlFix_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is ICellController cell)
            {
                cell.SendAppearing();
            }
        }
    }
}
