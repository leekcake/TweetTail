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
        private readonly TweetTail tail;

        public long ID;
        public AccountGroup(TweetTail tail, long id)
        {
            this.tail = tail;
            this.ID = id;
        }

        public AccountGroup(TweetTail tail, JObject data)
        {
            this.tail = tail;
            ID = data["id"].ToObject<long>();

            foreach (var account in data["accounts"].ToObject<JArray>())
            {
                accounts.Add(tail.TwitterAPI.LoadAccount(account.ToObject<JObject>()));
            }
        }

        internal List<DataAccount> accounts = new List<DataAccount>();
        public DataAccount AccountForRead {
            get {
                return accounts[0];
            }
        }

        public DataAccount AccountForWrite {
            get {
                return accounts.Count != 1 ? accounts[1] : accounts[0];
            }
        }

        public JObject Save()
        {
            var result = new JObject();
            result["id"] = ID;

            var accounts = new JArray();
            foreach (var account in this.accounts)
            {
                if (!account.IsShadowcopy)
                {
                    accounts.Add(account.Save());
                }
            }
            if(accounts.Count == 0) //It's shadow only account group
            {
                return null;
            }
            result["accounts"] = accounts;

            return result;
        }
    }
}
