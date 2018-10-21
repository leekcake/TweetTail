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

        private List<AccountGroup> accountGroups = new List<AccountGroup>();
        private Dictionary<long, AccountGroup> accountDict = new Dictionary<long, AccountGroup>();

        private void AddAccountGroup(AccountGroup group)
        {
            accountGroups.Add(group);
            accountDict[group.id] = group;
        }

        private void RemoveAccountGroup(AccountGroup group)
        {
            accountGroups.Remove(group);
            accountDict.Remove(group.id);
        }

        public ReadOnlyCollection<AccountGroup> readOnlyAccountGroups => accountGroups.AsReadOnly();
        public AccountGroup SelectedAccountGroup => getAccountGroup(selectedAccountId);

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
                AddAccountGroup(group);
            }

            group.accounts.Add(account);

            save();
        }

        public AccountGroup getAccountGroup(long id)
        {
            AccountGroup group;
            if (accountDict.TryGetValue(id, out group))
            {
                return group;
            }
            return null;
        }

        public void removeAccount(Account account)
        {
            var group = getAccountGroup(account.id);
            if (group == null) return;

            group.accounts.RemoveAll((data) => { return data == account; });
            if(group.accounts.Count == 0)
            {
                RemoveAccountGroup(group);
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
            for(int i = 0; i < accountGroups.Count; i++)
            {
                var group = accountGroups[i];
                foreach(var account in group.accounts)
                {
                    if(account.IsTweetdeck && !account.IsShadowcopy)
                    {
                        var shadows = await owner.twitter.GetContributees(account);
                        
                        foreach(var shadow in shadows)
                        {
                            var check = getAccountGroup(shadow.id);
                            if(check != null)
                            {
                                check.accounts.Add(shadow);
                            }
                            else
                            {
                                var generated = new AccountGroup(owner, shadow.id);
                                generated.accounts.Add(shadow);
                                AddAccountGroup(generated);
                            }
                        }

                        break;
                    }
                }
            }

            foreach(var group in accountGroups)
            {
                try
                {
                    if (group.accounts[0].user == null)
                    {
                        var user = await owner.twitter.VerifyCredentials(group.accountForRead);
                        foreach (var account in group.accounts)
                        {
                            account.user = user;
                        }
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
                AddAccountGroup(new AccountGroup(owner, accountGroup.ToObject<JObject>()));
            }
        }

        private void save()
        {
            var data = new JObject();
            data["selectedAccountId"] = SelectedAccountId;

            var accounts = new JArray();
            foreach(var accountGroup in accountGroups)
            {
                var save = accountGroup.save();
                if (save != null)
                {
                    accounts.Add(save);
                }
            }
            data["accounts"] = accounts;

            File.WriteAllText(savePath, data.ToString());
        }
    }
}
