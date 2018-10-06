using System;
using System.Collections.Generic;
using System.Text;

using DataAccount = TwitterInterface.Data.Account;

namespace TweetTail.Components.Account.Checkable
{
    public class CheckableAccount
    {
        public DataAccount account;
        public bool isChecked = false;

        public CheckableAccount(DataAccount account)
        {
            this.account = account;
        }
    }
}
