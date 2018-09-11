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
            var message = Utils.generateHttpRequest(HttpMethod.Post, new Uri("https://api.twitter.com/oauth/access_token"), new KeyValuePair<string, string>[] {
                //new KeyValuePair<string, string>("oauth_verifier", pin)
            }, consumer, null);
            
            //Utils.authorizeHttpRequest(message, consumer, oauth);
            var response = await owner.httpClient.SendAsync(message);
            Utils.VerifyTwitterResponse(response);

            var data = HttpUtility.ParseQueryString(response.Content.ToString());

            var account = new LibAccount();
            account.token = new Token(data["oauth_token"], data["oauth_token_secret"]);
            account.id = long.Parse(data["user_id"]);

            return account;
        }
    }
}
