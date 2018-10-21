using Library.Container.Blend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetTail.Components.Account.Checkable;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TweetTail.Pages.Blend
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BlendEditPage : ContentPage
	{
        private BlendedAccount target;

		public BlendEditPage (string editTarget = null)
		{
			InitializeComponent ();

            if(editTarget != null)
            {
                target = App.Tail.Blend.GetBlendedAccount(editTarget);
                ConfirmButton.Text = "수정하기";
            }
            else
            {
                ConfirmButton.Text = "만들기";
            }

            CheckableAccountListView.Footer = null;

            foreach(var accountGroup in App.Tail.Account.ReadOnlyAccountGroups)
            {
                CheckableAccountListView.Items.Add(new CheckableAccount(accountGroup.AccountForRead));
            }
		}

        private bool VerifyName()
        {
            if (App.Tail.Blend.GetBlendedAccount(NameEditor.Text) != null)
            {
                Application.Current.MainPage.DisplayAlert("오류", "동일한 이름을 가진 병합계정이 있습니다", "확인");
                return false;
            }
            return true;
        }

        private void ConfirmButton_Clicked(object sender, EventArgs e)
        {
            var count = 0;
            foreach (var item in CheckableAccountListView.Items)
            {
                if (item.IsChecked)
                {
                    count++;
                }
            }
            if (count < 2)
            {
                Application.Current.MainPage.DisplayAlert("오류", "계정을 병합하기 위해서는 두개 이상의 계정을 선택해야 합니다", "확인");
                return;
            }

            if (target == null)
            {
                if (!VerifyName()) return;

                target = new BlendedAccount(App.Tail)
                {
                    Name = NameEditor.Text
                };

                App.Tail.Blend.RegisterBlendedAccount(target);
            }
            target.IDs = new long[count];
            int i = 0;
            foreach (var item in CheckableAccountListView.Items)
            {
                if (item.IsChecked)
                {
                    target.IDs[i++] = item.Account.ID;
                }
            }

            App.Tail.Blend.Save();

            foreach(var page in App.Navigation.NavigationStack)
            {
                if(page is BlendListPage)
                {
                    (page as BlendListPage).Reload();
                    break;
                }
            }

            App.Navigation.RemovePage(this);
        }
    }
}