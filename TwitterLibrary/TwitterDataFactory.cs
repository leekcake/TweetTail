using System;
using System.Collections.Generic;
using System.Text;
using TwitterInterface.Data;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace TwitterLibrary
{
    class TwitterDataFactory
    {
        public const string TwitterDateTemplate = "ddd MMM dd HH:mm:ss +ffff yyyy";

        private static string SafeGetString(JObject obj, string key)
        {
            if(obj.ContainsKey(key))
            {
                return obj[key].ToString();
            } else
            {
                return null;
            }
        }

        public static User parseUser(JObject obj)
        {
            var user = new User();

            user.id = obj["id"].ToObject<long>();
            user.nickName = obj["name"].ToString();
            user.screenName = obj["screen_name"].ToString();
            user.location = SafeGetString(obj, "location");
            user.url = SafeGetString(obj, "url");
            user.description = SafeGetString(obj, "description");
            //TOOD: derived
            user.isProtected = obj["protected"].ToObject<bool>();
            user.isVerified = obj["verified"].ToObject<bool>();
            user.followerCount = obj["followers_count"].ToObject<long>();
            user.followingCount = obj["friends_count"].ToObject<long>();
            user.listedCount = obj["listed_count"].ToObject<long>();
            user.favouritesCount = obj["favourites_count"].ToObject<long>();
            user.statusesCount = obj["statuses_count"].ToObject<long>();
            user.createdAt = DateTime.ParseExact(obj["created_at"].ToString(), TwitterDateTemplate, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            user.geoEnabled = obj["geo_enabled"].ToObject<bool>();
            user.language = obj["lang"].ToString();
            //TODO: contributors_enabled
            user.profileBackgroundColor = obj["profile_background_color"].ToString();
            user.profileBackgroundImageURL = obj["profile_background_image_url"].ToString();
            user.profileHttpsBackgroundImageURL = obj["profile_background_image_url_https"].ToString();
            user.profileBackgroundTile = obj["profile_background_tile"].ToObject<bool>();
            user.profileBannerURL = obj["profile_banner_url"].ToString();
            user.profileImageURL = obj["profile_image_url"].ToString();
            user.profileHttpsImageURL = obj["profile_image_url_https"].ToString();
            //TODO: profile_link_color
            //TODO: profile_sidebar_border_color
            //TODO: profile_sidebar_fill_color
            //TODO: profile_text_color
            //TODO: profile_use_background_image
            //TODO: default_profile
            //TODO: default_profile_image
            //TODO: withheld_in_countries
            //TODO: withheld_scope

            return user;
        }
    }
}
