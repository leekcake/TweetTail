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

        public string saveDir;

        public AccountManager account;
        public BlendManager blend;
        public MuteManager mute;

        public TweetTail(API api, string saveDir)
        {
            twitter = api;
            this.saveDir = saveDir;

            Directory.CreateDirectory(saveDir);

            account = new AccountManager(this);
            blend = new BlendManager(this);
            mute = new MuteManager(this);
        }
    }
}
