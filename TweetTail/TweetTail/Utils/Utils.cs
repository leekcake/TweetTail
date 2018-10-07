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
        public static string TimespanToString(TimeSpan span)
        {
            var result = new StringBuilder();

            if(span.Days != 0)
            {
                result.Append(span.Days);
                result.Append("일 ");
            }

            if (span.Hours != 0)
            {
                result.Append(span.Hours);
                result.Append("시 ");
            }

            if(span.Minutes != 0)
            {
                result.Append(span.Minutes);
                result.Append("분 ");
            }

            if(result.Length == 0)
            {
                return string.Format("{0}초 남음", span.Seconds);
            }

            result.Remove(result.Length - 1, 1);

            return result.ToString();
        }
        
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

            if (selected == null || selected == "취소")
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
