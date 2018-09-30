using Library.Container.Account;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace TweetTail.Utils
{
    public class Util
    {
        public static void HandleException(Exception exception)
        {
            System.Diagnostics.Debug.WriteLine(exception.Message + " - " + exception.StackTrace);
            Application.Current.MainPage.DisplayAlert("오류: " + exception.Message, exception.StackTrace, "확인");
        }

        public static async Task<AccountGroup> SelectAccount(string description, ReadOnlyCollection<AccountGroup> group)
        {
            if(group.Count == 1)
            {
                return group[0];
            }

            var actionSheet = new string[group.Count];
            for (int i = 0; i < group.Count; i++)
            {
                actionSheet[i] = group[i].accountForRead.user.nickName;
            }

            var selected = await Application.Current.MainPage.DisplayActionSheet(description, "취소", null, actionSheet);

            if (selected == null)
            {
                return null;
            }

            for (int i = 0; i < group.Count; i++)
            {
                if(selected == actionSheet[i])
                {
                    return group[i];
                }
            }

            //?????????
            throw new InvalidOperationException();
        }

        public static Task<AccountGroup> SelectAccount(string description)
        {
            return SelectAccount(description, App.tail.account.readOnlyAccountGroups);
        }

        public static Task<AccountGroup> SelectAccount(string description, List<long> issuer)
        {
            var list = new List<AccountGroup>(issuer.Count);
            foreach(var groupId in issuer)
            {
                list.Add(App.tail.account.getAccountGroup(groupId));
            }

            return SelectAccount(description, list.AsReadOnly());
        }
    }
}
