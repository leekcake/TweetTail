using System;
using System.Collections.Generic;
using System.Text;
using TweetTail.Utils;
using TwitterInterface.Data;
using DataUser = TwitterInterface.Data.User;

namespace TweetTail.User
{
    public class UserListView : TwitterListView<DataUser, UserCell>
    {
        public override long GetID(DataUser data)
        {
            return data.id;
        }
    }
}
