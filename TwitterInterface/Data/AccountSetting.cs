using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data
{
    class AccountSetting
    {
        public class SleepTime
        {
            public bool isEnabled;
            //TODO: Replace with (custom) class
            public string startTime, endTime;
        }

        public class TrendLocation
        {
            public string country;
            public string countryCode;
            public string name;

            public long parentId;

            //placeType {
            public long placeTypeCode;
            public string placeTypeName;
            //}

            public string url;
            public string woeid;
        }

        public bool isAlwaysUseHttps;
        public bool isDiscoverableByEmail;
        public bool isGeoEnabled;

        public string language;

        public bool isProtected;

        public string screenName;

        public bool showAllInlineMedia;

        public SleepTime sleepTime;
        public TimeZoneInfo timeZone;
        public TrendLocation trendLocation;

        public bool useCookiePersonalization;
        public string allowContributorRequest;
    }
}
