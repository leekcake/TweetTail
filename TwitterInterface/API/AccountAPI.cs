using System;
using System.Collections.Generic;
using System.IO;
using TwitterInterface.Container;
using TwitterInterface.Data;

namespace TwitterInterface.API
{
    public interface AccountAPI
    {
        //POST oauth/request_token
        //POST oauth/access_token
        LoginToken GetLoginToken();

        //GET account/settings
        AccountSetting GetAccountSetting(Account account);
        //GET account/verify_credentials
        User VerifyCredentials(Account account, bool includeEntities = true, bool skipStatus = false, bool includeEmail = false);
        //GET users/profile_banner
        string[] GetBannerImageVariant(Account account, long userId);
        string[] GetBannerImageVariant(Account account, string screenName);

        //POST account/remove_profile_banner
        void RemoveProfileBanner(Account account);
        //TODO: POST account/settings

        //POST account/update_profile
        User UpdateProfile(Account account, string name, string url, string location, string description, string profileLinkColor, bool includeEntities = true, bool skipStatus = false);
        //POST account/update_profile_banner
        void UpdateProfileBanner(Account account, Stream image);
        //POST account/update_profile_image
        User UpdateProfileImage(Account account, Stream image, bool includeEntities = true, bool skipStatus = false);


        //GET saved_searches/list
        List<SavedSearch> GetSavedSearches(Account account);
        //GET saved_searches/show/:id
        SavedSearch GetSavedSearchById(Account account, long id);

        //POST saved_searches/create
        SavedSearch CreateSavedSearch(Account account, string query);
        //POST saved_searches/destroy/:id
        SavedSearch DestroySavedSearch(Account account, long id);
    }
}
