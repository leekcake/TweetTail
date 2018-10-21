using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data
{
    public class TwitterList
    {
        public long ID;

        public string Slug;

        public string Name;
        public string FullName;

        public string Description;

        public DateTime CreatedAt;
        public string URL;

        public long SubscriberCount;
        public long MemberCount;
        public string Mode;

        public User User;
        public bool Following;
    }
}
