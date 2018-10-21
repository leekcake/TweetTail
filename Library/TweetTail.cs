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
        public IAPI TwitterAPI;

        public string SaveDir;

        public AccountManager Account;
        public BlendManager Blend;
        public MuteManager Mute;

        public TweetTail(IAPI api, string saveDir)
        {
            TwitterAPI = api;
            SaveDir = saveDir;

            Directory.CreateDirectory(saveDir);

            Account = new AccountManager(this);
            Blend = new BlendManager(this);
            Mute = new MuteManager(this);
        }
    }
}
