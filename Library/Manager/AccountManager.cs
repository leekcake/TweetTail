using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Library.Container.Account;
using Newtonsoft.Json.Linq;
using TwitterInterface.Data;

namespace Library.Manager
{
    public class AccountManager
    {
        internal TweetTail owner;
        private string savePath;
        public AccountManager(TweetTail owner)
        {
            this.owner = owner;
            savePath = Path.Combine(owner.saveDir, "accounts.json");
            load();
        }

        internal List<AccountGroup> accountGroups = new List<AccountGroup>();
        public ReadOnlyCollection<AccountGroup> readOnlyAccountGroups => accountGroups.AsReadOnly();

        public void addAccount(Account account)
        {
            var group = accountGroups.Find((data) => { return data.id == account.id; });
            if(group == null)
            {
                group = new AccountGroup(owner, account.id);
                accountGroups.Add(group);
            }

            group.accounts.Add(account);

            save();
        }

        public AccountGroup getAccountGroup(long id)
        {
            return accountGroups.Find((data) => { return data.id == id; });
        }

        public void removeAccount(Account account)
        {
            var group = accountGroups.Find((data) => { return data.id == account.id; });
            if (group == null) return;

            group.accounts.RemoveAll((data) => { return data == account; });
            if(group.accounts.Count == 0)
            {
                accountGroups.RemoveAll((data) => data.id == group.id);
            }

            save();
        }

        private void load()
        {
            accountGroups.Clear();
            if(!File.Exists(savePath))
            {
                return;
            }

            var data = JArray.Parse(File.ReadAllText(savePath));

            foreach(var accountGroup in data)
            {
                accountGroups.Add(new AccountGroup(owner, accountGroup.ToObject<JObject>()));
            }
        }

        private void save()
        {
            var data = new JArray();
            foreach(var accountGroup in accountGroups)
            {
                data.Add(accountGroup.save());
            }

            File.WriteAllText(savePath, data.ToString());
        }
    }
}
