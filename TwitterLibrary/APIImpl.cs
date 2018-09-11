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
using TwitterLibrary.Container;

namespace TwitterLibrary
{
    public class APIImpl : AccountAPI
    {
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
            var message = Utils.generateHttpRequest(HttpMethod.Post, new Uri("https://api.twitter.com/oauth/request_token"), new KeyValuePair<string, string>[] {
                //new KeyValuePair<string, string>("oauth_callback", "obb")
            }, consumerToken, null);

            var response = await httpClient.SendAsync(message);
            Utils.VerifyTwitterResponse(response);

            var responseText = await response.Content.ReadAsStringAsync();
            var data = HttpUtility.ParseQueryString(responseText);

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

        public Task<User> VerifyCredentials(Account account, bool includeEntities = true, bool skipStatus = false, bool includeEmail = false)
        {
            throw new NotImplementedException();
        }
    }
}
