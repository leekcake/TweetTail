using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data
{
    /// <summary>
    /// 트위터에 존재하는 유저
    /// </summary>
    class User
    {
        public long id;
        public string nickName;
        public string screenName;

        public string location;
        public string url;
        public string description;

        //TODO: derived

        public bool isProtected;
        public bool isVerified;

        public int follerCount;
        public int followingCount;
        public int listedCount;

        public int favouritesCount;
        public int statusesCount;
        public DateTime createdAt;

        public bool geoEnabled;
        public string language;

        public string profileBackgroundColor; //TODO: make Color Class
        public string profileBackgroundImageURL;
        public string profileHttpsBackgroundImageURL;
        public bool profileBackgroundTile;

        public string profileBannerURL;
        public string profileImageURL;
        public string profileHtppsImageURL;
    }
}
