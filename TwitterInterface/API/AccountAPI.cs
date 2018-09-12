using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TwitterInterface.Container;
using TwitterInterface.Data;

namespace TwitterInterface.API
{
    public interface AccountAPI
    {
        Account LoadAccount(Stream stream);

        //POST oauth/request_token
        //POST oauth/access_token
        Task<LoginToken> GetLoginTokenAsync(Token consumerToken);

        //GET account/settings
        Task<AccountSetting> GetAccountSetting(Account account);
        //GET account/verify_credentials
        Task<User> VerifyCredentials(Account account, bool includeEntities = true, bool skipStatus = false, bool includeEmail = false);
        //GET users/profile_banner
        Task<string[]> GetBannerImageVariant(Account account, long userId);
        Task<string[]> GetBannerImageVariant(Account account, string screenName);

        //POST account/remove_profile_banner
        Task RemoveProfileBanner(Account account);
        //TODO: POST account/settings

        //POST account/update_profile
        Task<User> UpdateProfile(Account account, string name, string url, string location, string description, string profileLinkColor, bool includeEntities = true, bool skipStatus = false);
        //POST account/update_profile_banner
        Task UpdateProfileBanner(Account account, Stream image);
        //POST account/update_profile_image
        Task<User> UpdateProfileImage(Account account, Stream image, bool includeEntities = true, bool skipStatus = false);


        //GET saved_searches/list
        Task<List<SavedSearch>> GetSavedSearches(Account account);
        //GET saved_searches/show/:id
        Task<SavedSearch> GetSavedSearchById(Account account, long id);

        //POST saved_searches/create
        Task<SavedSearch> CreateSavedSearch(Account account, string query);
        //POST saved_searches/destroy/:id
        Task<SavedSearch> DestroySavedSearch(Account account, long id);
    }
}
