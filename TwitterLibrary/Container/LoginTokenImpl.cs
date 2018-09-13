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
    class LoginTokenImpl : LoginToken
    {
        private APIImpl owner;
        private Token consumer, oauth;

        public LoginTokenImpl(APIImpl owner, Token consumer, Token oauth)
        {
            this.owner = owner;
            this.consumer = consumer;
            this.oauth = oauth;
            loginURL = "https://api.twitter.com/oauth/authenticate?oauth_token=" + oauth.key;
        }
        public string loginURL { get; }

        public async Task<Account> login(string pin)
        {
            var response = await owner.Post("https://api.twitter.com/oauth/access_token", consumer, oauth, new KeyValuePair<string, string>[] {
                new KeyValuePair<string, string>("oauth_verifier", pin)
            });
            var data = HttpUtility.ParseQueryString(response);

            var account = new LibAccount();
            account.consumer = consumer;
            account.oauth = new Token(data["oauth_token"], data["oauth_token_secret"]);
            account.id = long.Parse(data["user_id"]);

            return account;
        }
    }
}
