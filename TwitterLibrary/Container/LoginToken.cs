using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TwitterInterface.Container;
using TwitterInterface.Data;
using TwitterLibrary.Data;

namespace TwitterLibrary.Container
{
    class LoginToken : ILoginToken
    {
        private API owner;
        private Token consumer, oauth;

        public LoginToken(API owner, Token consumer, Token oauth)
        {
            this.owner = owner;
            this.consumer = consumer;
            this.oauth = oauth;
            LoginURL = "https://api.twitter.com/oauth/authenticate?oauth_token=" + oauth.Key;
        }
        public string LoginURL { get; }

        public async Task<Account> LoginAsync(string pin)
        {
            var query = new List<KeyValuePair<string, string>>();
            query.Add(new KeyValuePair<string, string>("oauth_verifier", pin));
            var response = await owner.PostAsync("https://api.twitter.com/oauth/access_token", consumer, oauth, query);
            var data = HttpUtility.ParseQueryString(response);

            var account = new LibAccount();
            account.Consumer = consumer;
            account.Oauth = new Token(data["oauth_token"], data["oauth_token_secret"]);
            account.ID = long.Parse(data["user_id"]);

            account.User = await owner.VerifyCredentialsAsync(account);

            return account;
        }
    }
}
