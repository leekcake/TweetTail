using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
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
        public AccountGroup SelectedAccountGroup => accountGroups.Find((data) => { return data.id == selectedAccountId; });

        private long selectedAccountId;
        public long SelectedAccountId {
            get {
                return selectedAccountId;
            }
            set {
                selectedAccountId = value;
                save();
            }
        }

        public void addAccount(Account account)
        {
            var group = accountGroups.Find((data) => { return data.id == account.id; });
            if(group == null)
            {
                if(accountGroups.Count == 0)
                {
                    selectedAccountId = account.id;
                }
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
                if(account.id == selectedAccountId)
                {
                    if(accountGroups.Count == 0)
                    {
                        selectedAccountId = -1;
                    }
                    else
                    {
                        selectedAccountId = accountGroups[0].id;
                    }
                }
            }

            save();
        }

        public async Task VerifyAccounts()
        {
            foreach(var group in accountGroups)
            {
                try
                {
                    var user = await owner.twitter.VerifyCredentials(group.accountForRead);
                    foreach (var account in group.accounts)
                    {
                        account.user = user;
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message + " " + e.StackTrace);
                }
            }
        }

        private void load()
        {
            accountGroups.Clear();
            if(!File.Exists(savePath))
            {
                return;
            }

            var data = JObject.Parse(File.ReadAllText(savePath));

            selectedAccountId = data["selectedAccountId"].ToObject<long>();
            var accounts = data["accounts"];

            foreach(var accountGroup in accounts)
            {
                accountGroups.Add(new AccountGroup(owner, accountGroup.ToObject<JObject>()));
            }
        }

        private void save()
        {
            var data = new JObject();
            data["selectedAccountId"] = SelectedAccountId;

            var accounts = new JArray();
            foreach(var accountGroup in accountGroups)
            {
                accounts.Add(accountGroup.save());
            }
            data["accounts"] = accounts;

            File.WriteAllText(savePath, data.ToString());
        }
    }
}
