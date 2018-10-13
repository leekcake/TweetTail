using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using TwitterInterface.Data;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using TwitterInterface.Data.Entity;

using TwitterLibrary;
using System.Collections.ObjectModel;

namespace Library.Manager
{
    public class MuteManager
    {
        internal TweetTail owner;
        private string savePath;

        public MuteManager(TweetTail owner)
        {
            this.owner = owner;
            savePath = Path.Combine(owner.saveDir, "mutes.json");
            Load();

            TwitterDataFactory.statusFilter.RegisterFilter(new TwitterLibrary.Container.FilterStore<Status>.Filter(Filter));
            TwitterDataFactory.userFilter.RegisterFilter(new TwitterLibrary.Container.FilterStore<User>.Filter(Filter));
        }

        private List<Mute> keywordMutes = new List<Mute>();
        private Dictionary<long, Mute> userMutes = new Dictionary<long, Mute>();
        private Dictionary<long, Mute> statusMutes = new Dictionary<long, Mute>();

        public IEnumerable<Mute> ReadonlyKeywordMutes {
            get {
                return keywordMutes;
            }
        }

        public IEnumerable<Mute> ReadonlyUserMutes {
            get {
                return userMutes.Values;
            }
        }

        public IEnumerable<Mute> ReadonlyStatusMutes {
            get {
                return statusMutes.Values;
            }
        }

        private JArray Save(IEnumerable<Mute> mutes)
        {
            var result = new JArray();
            foreach (var mute in mutes)
            {
                result.Add(mute.Save());
            }
            return result;
        }

        private IEnumerable<Mute> Load(IEnumerable<JToken> tokens)
        {
            return tokens.Select((token) =>
            {
                return Mute.Load(token.ToObject<JObject>());
            });
        }

        public void Save()
        {
            var data = new JObject();

            data["keywords"] = Save(keywordMutes);
            data["users"] = Save(userMutes.Values);
            data["statuses"] = Save(statusMutes.Values);

            File.WriteAllText(savePath, data.ToString(Formatting.None));
        }

        public void Load()
        {
            if (!File.Exists(savePath))
            {
                return;
            }

            var loaded = File.ReadAllText(savePath);
            var data = JObject.Parse(loaded);

            keywordMutes.Clear();
            foreach (var mute in Load(data["keywords"]))
            {
                keywordMutes.Add(mute);
            }

            userMutes.Clear();
            foreach (var mute in Load(data["users"]))
            {
                userMutes[(mute.target as Mute.UserTarget).id] = mute;
            }

            statusMutes.Clear();
            foreach (var mute in Load(data["statuses"]))
            {
                statusMutes[(mute.target as Mute.StatusTarget).id] = mute;
            }
        }

        public void RegisterMute(Mute mute)
        {
            if (mute.target is Mute.KeywordTarget)
            {
                keywordMutes.Add(mute);
            }
            else if (mute.target is Mute.UserTarget)
            {
                var id = (mute.target as Mute.UserTarget).id;
                if (userMutes.ContainsKey(id))
                {
                    userMutes.Remove(id);
                }
                userMutes[id] = mute;
            }
            else if (mute.target is Mute.StatusTarget)
            {
                var id = (mute.target as Mute.StatusTarget).id;
                if (statusMutes.ContainsKey(id))
                {
                    statusMutes.Remove(id);
                }
                statusMutes[id] = mute;
            }

            Save();
        }

        public void UpdateMute(Mute mute)
        {
            Save();
        } 

        public void UnregisterMute(Mute mute)
        {
            if (mute.target is Mute.KeywordTarget)
            {
                keywordMutes.RemoveAll((amute) => { return amute == mute; });
            }
            else if (mute.target is Mute.UserTarget)
            {
                var id = (mute.target as Mute.UserTarget).id;
                if (userMutes.ContainsKey(id))
                {
                    userMutes.Remove(id);
                }
            }
            else if (mute.target is Mute.StatusTarget)
            {
                var id = (mute.target as Mute.StatusTarget).id;
                if (statusMutes.ContainsKey(id))
                {
                    statusMutes.Remove(id);
                }
            }
        }

        public Status Filter(Status data)
        {
            Status display = data;
            if (data.retweetedStatus != null)
            {
                display = data.retweetedStatus;
            }

            if (statusMutes.ContainsKey(display.id))
            {
                return null;
            }

            if (FilterUser(display) || FilterUser(data))
            {
                return null;
            }

            if (display.userMentions != null)
            {
                var rebuild = new List<UserMention>();

                foreach (var mention in display.userMentions)
                {
                    if(!userMutes.ContainsKey(mention.id))
                    {
                        rebuild.Add(mention);
                        continue;
                    }
                    var user = userMutes[mention.id];

                    var target = user.target as Mute.UserTarget;

                    if (target.muteGoAway)
                    {
                        return null;
                    }

                    if (target.muteSingleInboundMention && display.userMentions.Length == 1)
                    {
                        return null;
                    }

                    if (target.muteMultipleInboundMention)
                    {
                        if (target.muteMultipleInboundMentionForcely)
                        {
                            return null;
                        }
                        CutText(display, mention.indices);
                        ChangeLength(display, mention.indices.start, mention.indices.end - mention.indices.start, 0);
                        continue;
                    }

                    rebuild.Add(mention);
                }

                display.userMentions = rebuild.ToArray();
            }

            foreach (var keyword in keywordMutes)
            {
                var target = keyword.target as Mute.KeywordTarget;

                var inx = display.text.IndexOf(target.keyword);
                if (inx == -1) continue;

                if (target.replace == null)
                {
                    return null;
                }

                CutText(display, new Indices() { start = inx, end = inx + (target.replace.Length - 1) }, target.replace);
                ChangeLength(display, inx, target.keyword.Length, target.replace.Length);
            }

            return data;
        }

        public void CutText(Status status, Indices indices, string insert = null)
        {
            var builder = new StringBuilder(status.text.Length);
            builder.Append(status.text, 0, indices.start);
            if (insert != null)
            {
                builder.Append(insert);
            }
            builder.Append(status.text, indices.end, status.text.Length - (indices.end));

            status.text = builder.ToString();
        }

        public void ChangeLength(Status status, int inx, int origLen, int newLen)
        {
            var diff = origLen - newLen;

            var check = new Action<Indices>((Indices indics) =>
            {
                if (indics.start > inx)
                {
                    indics.start -= diff;
                    indics.end -= diff;
                }
            });

            foreach (var mention in status.userMentions)
            {
                check(mention.indices);
            }

            foreach (var media in status.extendMedias)
            {
                check(media.indices);
            }

            foreach (var symbol in status.symbols)
            {
                check(symbol.indices);
            }

            foreach (var hashtag in status.hashtags)
            {
                check(hashtag.indices);
            }

            foreach (var url in status.urls)
            {
                check(url.indices);
            }
        }

        public bool FilterUser(Status status)
        {
            if (status == null) return true;

            if (!userMutes.ContainsKey(status.creater.id))
            {
                return false;
            }

            var mute = userMutes[status.creater.id];

            var target = mute.target as Mute.UserTarget;

            if (target.muteGoAway)
            {
                return true;
            }

            if (target.muteTweet && status.retweetedStatus == null && status.userMentions == null)
            {
                return true;
            }

            if (target.muteRetweet && status.retweetedStatus != null)
            {
                return true;
            }

            if (target.muteOutboundMention && status.userMentions != null)
            {
                return true;
            }

            return false;
        }

        public User Filter(User user)
        {
            if (!userMutes.ContainsKey(user.id))
            {
                return user;
            }
            var mute = userMutes[user.id];

            var target = mute.target as Mute.UserTarget;

            if (target.muteGoAway)
            {
                return null;
            }

            return user;
        }
    }
}
