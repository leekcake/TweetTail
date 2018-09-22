using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DataAccount = TwitterInterface.Data.Account;
using TwitterInterface.API;
using System.IO;
using System.Threading.Tasks;
using TwitterInterface.API.Result;
using TwitterInterface.Container;
using TwitterInterface.Data;

namespace Library.Container.Account
{
    public class AccountGroup
    {
        private TweetTail owner;

        public long id;
        public AccountGroup(TweetTail owner, long id)
        {
            this.owner = owner;
            this.id = id;
        }

        public AccountGroup(TweetTail owner, JObject data)
        {
            this.owner = owner;
            id = data["id"].ToObject<long>();

            foreach (var account in data["accounts"].ToObject<JArray>())
            {
                accounts.Add(owner.twitter.LoadAccount(account.ToObject<JObject>()));
            }   
        }

        internal List<DataAccount> accounts = new List<DataAccount>();
        public DataAccount accountForRead {
            get {
                return accounts[0];
            }
        }

        public DataAccount accountForWrite {
            get {
                return accounts.Count != 1 ? accounts[1] : accounts[0];
            }
        }

        public JObject save()
        {
            var result = new JObject();
            result["id"] = id;

            var accounts = new JArray();
            foreach (var account in this.accounts)
            {
                accounts.Add(account.Save());
            }
            result["accounts"] = accounts;

            return result;
        }
    }
}
