using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TwitterInterface.API;
using Library.Manager;

namespace Library
{
    public class TweetTail
    {
        public API twitter;

        public string saveDir, cacheDir;

        public AccountManager account;
        public MediaManager media;

        public TweetTail(API api, string saveDir, string cacheDir)
        {
            twitter = api;
            this.saveDir = saveDir;
            this.cacheDir = cacheDir;

            Directory.CreateDirectory(saveDir);
            Directory.CreateDirectory(cacheDir);

            account = new AccountManager(this);
            media = new MediaManager(this);
        }
    }
}
