using System;
using System.Collections.Generic;
using System.Text;
using TwitterInterface.Data.Entity;

namespace TwitterInterface.Data
{
    /// <summary>
    /// 트위터에 존재하는 유저
    /// </summary>
    public class User
    {
        //이 유저정보를 얻기 위해 사용된 계정 아이디
        public List<long> Issuer;

        public long ID;
        public string NickName;
        public string ScreenName;

        public string Location;
        public string URL;
        public string Description;

        public URL[] URLEntities;
        public BasicEntitiesGroup descriptionEntities;

        //TODO: derived

        public bool IsProtected;
        public bool IsVerified;

        public long FollowerCount;
        public long FollowingCount;
        public long ListedCount;

        public long FavouritesCount;
        public long StatusesCount;
        public DateTime CreatedAt;

        public bool GeoEnabled;
        public string Language;

        public string ProfileBackgroundColor; //TODO: make Color Class
        public string ProfileBackgroundImageURL;
        public string ProfileHttpsBackgroundImageURL;
        public bool ProfileBackgroundTile;

        public string ProfileBannerURL;
        public string ProfileImageURL;
        public string ProfileHttpsImageURL;
    }
}
