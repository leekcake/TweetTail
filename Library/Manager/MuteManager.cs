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
        private readonly string savePath;

        public MuteManager(TweetTail owner)
        {
            this.owner = owner;
            savePath = Path.Combine(owner.SaveDir, "mutes.json");
            Load();

            TwitterDataFactory.StatusFilter.RegisterFilter(new TwitterLibrary.Container.FilterStore<Status>.Filter(Filter));
            TwitterDataFactory.UserFilter.RegisterFilter(new TwitterLibrary.Container.FilterStore<User>.Filter(Filter));
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

        public Mute GetUserMute(User user)
        {
            if (user == null) return null;
            if (!userMutes.ContainsKey(user.ID)) return null;
            return userMutes[user.ID];
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
            var data = new JObject
            {
                ["keywords"] = Save(keywordMutes),
                ["users"] = Save(userMutes.Values),
                ["statuses"] = Save(statusMutes.Values)
            };

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
                userMutes[(mute.Target as Mute.UserTarget).ID] = mute;
            }

            statusMutes.Clear();
            foreach (var mute in Load(data["statuses"]))
            {
                statusMutes[(mute.Target as Mute.StatusTarget).ID] = mute;
            }
        }

        public void RegisterMute(Mute mute)
        {
            if (mute.Target is Mute.KeywordTarget)
            {
                keywordMutes.Add(mute);
            }
            else if (mute.Target is Mute.UserTarget)
            {
                var id = (mute.Target as Mute.UserTarget).ID;
                if (userMutes.ContainsKey(id))
                {
                    userMutes.Remove(id);
                }
                userMutes[id] = mute;
            }
            else if (mute.Target is Mute.StatusTarget)
            {
                var id = (mute.Target as Mute.StatusTarget).ID;
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
            if (mute.Target is Mute.KeywordTarget)
            {
                keywordMutes.RemoveAll((amute) => { return amute == mute; });
            }
            else if (mute.Target is Mute.UserTarget)
            {
                var id = (mute.Target as Mute.UserTarget).ID;
                if (userMutes.ContainsKey(id))
                {
                    userMutes.Remove(id);
                }
            }
            else if (mute.Target is Mute.StatusTarget)
            {
                var id = (mute.Target as Mute.StatusTarget).ID;
                if (statusMutes.ContainsKey(id))
                {
                    statusMutes.Remove(id);
                }
            }
            Save();
        }

        public Status Filter(Status data)
        {
            Status display = data;
            if (data.RetweetedStatus != null)
            {
                display = data.RetweetedStatus;
            }

            if (statusMutes.ContainsKey(display.ID))
            {
                return null;
            }

            if (FilterUser(display) || FilterUser(data))
            {
                return null;
            }

            if (display.UserMentions != null)
            {
                var rebuild = new List<UserMention>();

                foreach (var mention in display.UserMentions)
                {
                    if(!userMutes.ContainsKey(mention.ID))
                    {
                        rebuild.Add(mention);
                        continue;
                    }
                    var user = userMutes[mention.ID];

                    var target = user.Target as Mute.UserTarget;

                    if (target.MuteGoAway)
                    {
                        return null;
                    }

                    if (target.MuteSingleInboundMention && display.UserMentions.Length == 1)
                    {
                        return null;
                    }

                    if (target.MuteMultipleInboundMention)
                    {
                        if (target.MuteMultipleInboundMentionForcely)
                        {
                            return null;
                        }
                        CutText(display, mention.Indices);
                        ChangeLength(display, mention.Indices.Start, mention.Indices.End - mention.Indices.Start, 0);
                        continue;
                    }

                    rebuild.Add(mention);
                }

                display.UserMentions = rebuild.ToArray();
            }

            foreach (var keyword in keywordMutes)
            {
                var target = keyword.Target as Mute.KeywordTarget;

                var inx = display.Text.IndexOf(target.Keyword);
                if (inx == -1) continue;

                if (target.Replace == null)
                {
                    return null;
                }

                CutText(display, new Indices() { Start = inx, End = inx + (target.Replace.Length - 1) }, target.Replace);
                ChangeLength(display, inx, target.Keyword.Length, target.Replace.Length);
            }

            return data;
        }

        public void CutText(Status status, Indices indices, string insert = null)
        {
            var builder = new StringBuilder(status.Text.Length);
            builder.Append(status.Text, 0, indices.Start);
            if (insert != null)
            {
                builder.Append(insert);
            }
            builder.Append(status.Text, indices.End, status.Text.Length - (indices.End));

            status.Text = builder.ToString();
        }

        public void ChangeLength(Status status, int inx, int origLen, int newLen)
        {
            var diff = origLen - newLen;

            var check = new Action<Indices>((Indices indics) =>
            {
                if (indics.Start > inx)
                {
                    indics.Start -= diff;
                    indics.End -= diff;
                }
            });

            if (status.UserMentions != null)
            {
                foreach (var mention in status.UserMentions)
                {
                    check(mention.Indices);
                }
            }

            if (status.ExtendMedias != null)
            {
                foreach (var media in status.ExtendMedias)
                {
                    check(media.Indices);
                }
            }

            if (status.Symbols != null)
            {
                foreach (var symbol in status.Symbols)
                {
                    check(symbol.Indices);
                }
            }

            if (status.Hashtags != null)
            {
                foreach (var hashtag in status.Hashtags)
                {
                    check(hashtag.Indices);
                }
            }

            if (status.URLs != null)
            {
                foreach (var url in status.URLs)
                {
                    check(url.Indices);
                }
            }
        }

        public bool FilterUser(Status status)
        {
            if (status == null) return true;

            if (!userMutes.ContainsKey(status.Creater.ID))
            {
                return false;
            }

            var mute = userMutes[status.Creater.ID];

            var target = mute.Target as Mute.UserTarget;

            if (target.MuteGoAway)
            {
                return true;
            }

            if (target.MuteTweet && status.RetweetedStatus == null && status.UserMentions == null)
            {
                return true;
            }

            if (target.MuteRetweet && status.RetweetedStatus != null)
            {
                return true;
            }

            if (target.MuteOutboundMention && status.UserMentions != null)
            {
                return true;
            }

            return false;
        }

        public User Filter(User user)
        {
            if (!userMutes.ContainsKey(user.ID))
            {
                return user;
            }
            var mute = userMutes[user.ID];

            var target = mute.Target as Mute.UserTarget;

            if (target.MuteGoAway)
            {
                return null;
            }

            return user;
        }
    }
}
