using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using TwitterInterface.Container;
using TwitterInterface.Data;

namespace TwitterInterface.API
{
    public interface IAccountAPI
    {
        Account LoadAccount(JObject data);

        //POST oauth/request_token
        //POST oauth/access_token
        Task<ILoginToken> GetLoginTokenAsync(Token consumerToken);
        
        Task<Account> GetAccountFromTweetdeckCookieAsync(CookieCollection cookieData);
        Task<List<Account>> GetContributeesAsync(Account account);

        //GET account/settings
        Task<AccountSetting> GetAccountSettingAsync(Account account);
        //GET account/verify_credentials
        Task<User> VerifyCredentialsAsync(Account account);
        
        /* TODO:
        //GET users/profile_banner
        Task<string[]> GetBannerImageVariant(Account account, long userId);
        Task<string[]> GetBannerImageVariant(Account account, string screenName);
        */

        //POST account/remove_profile_banner
        Task RemoveProfileBannerAsync(Account account);
        //TODO: POST account/settings

        //POST account/update_profile
        Task<User> UpdateProfileAsync(Account account, string name, string url, string location, string description, string profileLinkColor);
        //POST account/update_profile_banner
        Task UpdateProfileBannerAsync(Account account, Stream image);
        //POST account/update_profile_image
        Task<User> UpdateProfileImageAsync(Account account, Stream image);


        //GET saved_searches/list
        Task<List<SavedSearch>> GetSavedSearchesAsync(Account account);
        //GET saved_searches/show/:id
        Task<SavedSearch> GetSavedSearchByIdAsync(Account account, long id);

        //POST saved_searches/create
        Task<SavedSearch> CreateSavedSearchAsync(Account account, string query);
        //POST saved_searches/destroy/:id
        Task<SavedSearch> DestroySavedSearchAsync(Account account, long id);

        //HIDDEN API
        //GET activity/about_me
        Task<List<Notification>> GetNotificationsAsync(Account account, int count = 40, long sinceId =- 1, long maxId = -1);
    }
}
