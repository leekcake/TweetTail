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
                target = App.tail.blend.GetBlendedAccount(editTarget);
                btnConfirm.Text = "수정하기";
            }
            else
            {
                btnConfirm.Text = "만들기";
            }

            checkableAccountListView.Footer = null;

            foreach(var accountGroup in App.tail.account.readOnlyAccountGroups)
            {
                checkableAccountListView.Items.Add(new CheckableAccount(accountGroup.accountForRead));
            }
		}

        private bool VerifyName()
        {
            if (App.tail.blend.GetBlendedAccount(editName.Text) != null)
            {
                Application.Current.MainPage.DisplayAlert("오류", "동일한 이름을 가진 병합계정이 있습니다", "확인");
                return false;
            }
            return true;
        }

        private void btnConfirm_Clicked(object sender, EventArgs e)
        {
            var count = 0;
            foreach (var item in checkableAccountListView.Items)
            {
                if (item.isChecked)
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

                target = new BlendedAccount(App.tail);
                target.name = editName.Text;

                App.tail.blend.registerBlendedAccount(target);
            }
            target.ids = new long[count];
            int i = 0;
            foreach (var item in checkableAccountListView.Items)
            {
                if (item.isChecked)
                {
                    target.ids[i++] = item.account.id;
                }
            }

            App.tail.blend.save();

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