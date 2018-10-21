using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Library.Container.Account;
using Newtonsoft.Json.Linq;
using TwitterInterface.Data;
using TwitterLibrary;

namespace Library.Manager
{
    public class AccountManager
    {
        internal TweetTail owner;
        private string savePath;
        public AccountManager(TweetTail owner)
        {
            this.owner = owner;
            savePath = Path.Combine(owner.SaveDir, "accounts.json");
            Load();

            TwitterDataFactory.UserFilter.RegisterFilter(new TwitterLibrary.Container.FilterStore<User>.Filter( UpdateIfCan ) );
        }

        public User UpdateIfCan(User user)
        {
            var group = GetAccountGroup(user.ID);
            if (group != null)
            {
                foreach(var account in group.accounts)
                {
                    account.User = user;
                }
            }
            return user;
        }

        private List<AccountGroup> accountGroups = new List<AccountGroup>();
        private Dictionary<long, AccountGroup> accountDict = new Dictionary<long, AccountGroup>();

        private void AddAccountGroup(AccountGroup group)
        {
            accountGroups.Add(group);
            accountDict[group.ID] = group;
        }

        private void RemoveAccountGroup(AccountGroup group)
        {
            accountGroups.Remove(group);
            accountDict.Remove(group.ID);
        }

        public ReadOnlyCollection<AccountGroup> ReadOnlyAccountGroups => accountGroups.AsReadOnly();
        public AccountGroup SelectedAccountGroup => GetAccountGroup(selectedAccountId);

        private long selectedAccountId;
        public long SelectedAccountId {
            get {
                return selectedAccountId;
            }
            set {
                selectedAccountId = value;
                Save();
            }
        }

        public void AddAccount(Account account)
        {
            var group = accountGroups.Find((data) => { return data.ID == account.ID; });
            if(group == null)
            {
                if(accountGroups.Count == 0)
                {
                    selectedAccountId = account.ID;
                }
                group = new AccountGroup(owner, account.ID);
                AddAccountGroup(group);
            }

            group.accounts.Add(account);

            Save();
        }

        public AccountGroup GetAccountGroup(long id)
        {
            AccountGroup group;
            if (accountDict.TryGetValue(id, out group))
            {
                return group;
            }
            return null;
        }

        public void RemoveAccount(Account account)
        {
            var group = GetAccountGroup(account.ID);
            if (group == null) return;

            group.accounts.RemoveAll((data) => { return data == account; });
            if(group.accounts.Count == 0)
            {
                RemoveAccountGroup(group);
                if(account.ID == selectedAccountId)
                {
                    if(accountGroups.Count == 0)
                    {
                        selectedAccountId = -1;
                    }
                    else
                    {
                        selectedAccountId = accountGroups[0].ID;
                    }
                }
            }

            Save();
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
                        var shadows = await owner.TwitterAPI.GetContributeesAsync(account);
                        
                        foreach(var shadow in shadows)
                        {
                            var check = GetAccountGroup(shadow.ID);
                            if(check != null)
                            {
                                check.accounts.Add(shadow);
                            }
                            else
                            {
                                var generated = new AccountGroup(owner, shadow.ID);
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
                    if (group.accounts[0].User == null)
                    {
                        var user = await owner.TwitterAPI.VerifyCredentialsAsync(group.AccountForRead);
                        foreach (var account in group.accounts)
                        {
                            account.User = user;
                        }
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message + " " + e.StackTrace);
                }
            }
        }

        private void Load()
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

        private void Save()
        {
            var data = new JObject();
            data["selectedAccountId"] = SelectedAccountId;

            var accounts = new JArray();
            foreach(var accountGroup in accountGroups)
            {
                var save = accountGroup.Save();
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
