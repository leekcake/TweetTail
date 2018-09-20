using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data
{
    /// <summary>
    /// 트위터에 존재하는 유저
    /// </summary>
    public class User
    {
        //이 유저정보를 얻기 위해 사용된 계정 아이디
        public long[] issuer;

        public long id;
        public string nickName;
        public string screenName;

        public string location;
        public string url;
        public string description;

        //TODO: derived

        public bool isProtected;
        public bool isVerified;

        public long followerCount;
        public long followingCount;
        public long listedCount;

        public long favouritesCount;
        public long statusesCount;
        public DateTime createdAt;

        public bool geoEnabled;
        public string language;

        public string profileBackgroundColor; //TODO: make Color Class
        public string profileBackgroundImageURL;
        public string profileHttpsBackgroundImageURL;
        public bool profileBackgroundTile;

        public string profileBannerURL;
        public string profileImageURL;
        public string profileHttpsImageURL;
    }
}
