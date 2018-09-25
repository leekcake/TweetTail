using System;
using System.Collections.Generic;
using System.Text;
using TweetTail.Utils;
using Xamarin.Forms;

namespace TweetTail.Account.Checkable
{
    public class CheckableAccountListView : TwitterListView<CheckableAccount, CheckableAccountCell>
    {
        public override long GetID(CheckableAccount data)
        {
            return data.account.id;
        }
    }
}
