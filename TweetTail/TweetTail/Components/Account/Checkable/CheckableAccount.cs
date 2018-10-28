using Library.Container.Account;
using System;
using System.Collections.Generic;
using System.Text;

using DataAccount = TwitterInterface.Data.Account;

namespace TweetTail.Components.Account.Checkable
{
    public class CheckableAccount
    {
        public AccountGroup Account;
        public bool IsChecked = false;

        public CheckableAccount(AccountGroup account)
        {
            this.Account = account;
        }
    }
}
