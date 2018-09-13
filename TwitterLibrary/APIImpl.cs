using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TwitterInterface.API;
using TwitterInterface.Container;
using TwitterInterface.Data;
using TwitterLibrary.Data;
using TwitterLibrary.Container;
using Newtonsoft.Json.Linq;

namespace TwitterLibrary
{
    public class APIImpl : AccountAPI
    {
        internal async Task<string> Get(string uri, Account account, KeyValuePair<string, string>[] query)
        {
            return await Utils.readStringFromTwitter(httpClient, HttpMethod.Get, new Uri(uri), query, account as LibAccount);
        }

        internal async Task<string> Get(string uri, Token consumer, Token? oauth, KeyValuePair<string, string>[] query)
        {
            return await Utils.readStringFromTwitter(httpClient, HttpMethod.Get, new Uri(uri), query, consumer, oauth);
        }

        internal async Task<string> Post(string uri, Account account, KeyValuePair<string, string>[] query)
        {
            return await Utils.readStringFromTwitter(httpClient, HttpMethod.Post, new Uri(uri), query, account as LibAccount);
        }

        internal async Task<string> Post(string uri, Token consumer, Token? oauth, KeyValuePair<string, string>[] query)
        {
            return await Utils.readStringFromTwitter(httpClient, HttpMethod.Post, new Uri(uri), query, consumer, oauth);
        }

        internal readonly HttpClient httpClient = new HttpClient();

        public Task<SavedSearch> CreateSavedSearch(Account account, string query)
        {
            throw new NotImplementedException();
        }

        public Task<SavedSearch> DestroySavedSearch(Account account, long id)
        {
            throw new NotImplementedException();
        }

        public Task<AccountSetting> GetAccountSetting(Account account)
        {
            throw new NotImplementedException();
        }

        public Task<string[]> GetBannerImageVariant(Account account, long userId)
        {
            throw new NotImplementedException();
        }

        public Task<string[]> GetBannerImageVariant(Account account, string screenName)
        {
            throw new NotImplementedException();
        }

        public async Task<LoginToken> GetLoginTokenAsync(Token consumerToken)
        {
            var response = await Post("https://api.twitter.com/oauth/request_token", consumerToken, null, new KeyValuePair<string, string>[] {

            });
            var data = HttpUtility.ParseQueryString(response);

            var token = new Token();
            token.key = data["oauth_token"];
            token.secret = data["oauth_token_secret"];

            return new LoginTokenImpl(this, consumerToken, token);
        }

        public Task<SavedSearch> GetSavedSearchById(Account account, long id)
        {
            throw new NotImplementedException();
        }

        public Task<List<SavedSearch>> GetSavedSearches(Account account)
        {
            throw new NotImplementedException();
        }

        public Account LoadAccount(Stream stream)
        {
            return LibAccount.Load(stream);
        }

        public Task RemoveProfileBanner(Account account)
        {
            throw new NotImplementedException();
        }

        public Task<User> UpdateProfile(Account account, string name, string url, string location, string description, string profileLinkColor, bool includeEntities = true, bool skipStatus = false)
        {
            throw new NotImplementedException();
        }

        public Task UpdateProfileBanner(Account account, Stream image)
        {
            throw new NotImplementedException();
        }

        public Task<User> UpdateProfileImage(Account account, Stream image, bool includeEntities = true, bool skipStatus = false)
        {
            throw new NotImplementedException();
        }

        public async Task<User> VerifyCredentials(Account account, bool includeEntities = true, bool skipStatus = false, bool includeEmail = false)
        {
            var response = await Get("https://api.twitter.com/1.1/account/verify_credentials.json", account, new KeyValuePair<string, string>[] {
                
            });

            return TwitterDataFactory.parseUser(JObject.Parse(response));
        }
    }
}
