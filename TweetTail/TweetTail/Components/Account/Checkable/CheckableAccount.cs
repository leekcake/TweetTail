using System;
using System.Collections.Generic;
using System.Text;

using DataAccount = TwitterInterface.Data.Account;

namespace TweetTail.Components.Account.Checkable
{
    public class CheckableAccount
    {
        public DataAccount Account;
        public bool IsChecked = false;

        public CheckableAccount(DataAccount account)
        {
            this.Account = account;
        }
    }
}
