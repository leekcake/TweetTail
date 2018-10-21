using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterInterface.Data
{
    public class AccountSetting
    {
        public class SleepTimeData
        {
            public bool IsEnabled;
            //TODO: Replace with (custom) class
            public string StartTime, EndTime;
        }

        public class TrendLocationData
        {
            public string Country;
            public string CountryCode;
            public string Name;

            public long ParentId;

            //placeType {
            public long PlaceTypeCode;
            public string PlaceTypeName;
            //}

            public string URL;
            public long Woeid;
        }

        public bool IsAlwaysUseHttps;
        public bool IsDiscoverableByEmail;
        public bool IsGeoEnabled;

        public string Language;

        public bool IsProtected;

        public string ScreenName;

        public bool ShowAllInlineMedia;

        public SleepTimeData SleepTime;
        public TimeZoneInfo TimeZone;
        public TrendLocationData TrendLocation;

        public bool UseCookiePersonalization;
        public string AllowContributorRequest;
    }
}
