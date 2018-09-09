using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data
{
    public class TwitterList
    {
        public long id;

        public string slut;

        public string name;
        public string fullName;

        public string description;

        public DateTime createdAt;
        public string url;

        public long subscriberCount;
        public long memberCount;
        public string mode;

        public User user;
        public bool following;
    }
}
