using System;
using System.Collections.Generic;
using System.Net;
using TweetTail.Pages.User;
using TwitterInterface.Data;
using TwitterInterface.Data.Entity;
using Xamarin.Forms;

using DataStatus = TwitterInterface.Data.Status;

namespace TweetTail.Utils
{
    public class TwitterFormater
    {
        public class Builder
        {
            private class Hyperlink
            {
                public Indices Indices;
                public Command Command;
                public string Replace;

                public bool IsVisible = false;

                public Hyperlink(Indices indices, Command command, string replace = null)
                {
                    Indices = indices;
                    Command = command;
                    Replace = replace;
                }
            }

            private string text;
            private List<Hyperlink> hyperLinks = new List<Hyperlink>();

            public Builder(string text)
            {
                this.text = text;
            }

            private void InsertHyperlink(Hyperlink link)
            {
                for (int i = 0; i < hyperLinks.Count; i++)
                {
                    if (hyperLinks[i].Indices.Start > link.Indices.Start)
                    {
                        hyperLinks.Insert(i, link);
                        return;
                    }
                }
                hyperLinks.Add(link);
            }

            public void RegisterHyperlink(Indices indices, Command command, string replace = null)
            {
                InsertHyperlink( new Hyperlink(indices, command, replace));
            }

            public void RegisterRemover(Indices indices)
            {
                InsertHyperlink(new Hyperlink(indices, null)
                {
                    IsVisible = true
                });
            }

            public FormattedString ToFormattedString()
            {
                var result = new FormattedString();

                //No hyperlink
                if (hyperLinks.Count == 0)
                {
                    result.Spans.Add(new Span() { Text = WebUtility.HtmlDecode(text) });
                    return result;
                }

                int pos = 0;
                for (int i = 0; i < hyperLinks.Count; i++)
                {
                    var link = hyperLinks[i];

                    result.Spans.Add(new Span() { Text = WebUtility.HtmlDecode(text.Substring(pos, link.Indices.Start - pos)) });

                    if (!link.IsVisible)
                    {
                        var span = new Span()
                        {
                            Text = link.Replace ?? WebUtility.HtmlDecode(text.Substring(link.Indices.Start, link.Indices.Length)),
                            TextColor = Color.Blue
                        };
                        span.GestureRecognizers.Add(new TapGestureRecognizer()
                        {
                            Command = link.Command
                        });

                        result.Spans.Add(span);
                    }

                    pos = link.Indices.End;
                }

                if (pos < text.Length)
                {
                    result.Spans.Add(new Span() { Text = WebUtility.HtmlDecode(text.Substring(pos, text.Length - pos)) });
                }

                return result;
            }
        }

        public static FormattedString ParseFormattedString(DataStatus status)
        {
            var builder = new Builder(status.Text);

            ParseURLEntity(builder, status.URLs);
            ParseMediaEntity(builder, status.ExtendMedias);
            ParseMentionEntity(builder, status.UserMentions, status.Issuer);

            return builder.ToFormattedString();
        }

        public static FormattedString ParseFormattedString(string text, URL[] urls)
        {
            var builder = new Builder(text);

            ParseURLEntity(builder, urls);

            return builder.ToFormattedString();
        }

        public static FormattedString ParseFormattedString(string text, BasicEntitiesGroup group, List<long> issuer)
        {
            var builder = new Builder(text);

            ParseURLEntity(builder, group.URLs);
            ParseMentionEntity(builder, group.UserMentions, issuer);

            return builder.ToFormattedString();
        }

        public static void ParseURLEntity(Builder builder, URL[] entities)
        {
            if (entities == null)
            {
                return;
            }

            foreach (var url in entities)
            {
                builder.RegisterHyperlink(url.Indices, new Command(() =>
                {
                    Device.OpenUri(new Uri(url.RawURL));
                }), url.DisplayURL);
            }
        }

        public static void ParseMediaEntity(Builder builder, ExtendMedia[] entities)
        {
            if (entities == null)
            {
                return;
            }

            //Media already handled in StatusView, so hide it.
            builder.RegisterRemover(entities[0].Indices);
        }

        public static void ParseMentionEntity(Builder builder, UserMention[] entities, List<long> issuer)
        {
            if (entities == null)
            {
                return;
            }

            foreach (var mention in entities)
            {
                builder.RegisterHyperlink(mention.Indices, new Command(async () =>
                {
                    var account = issuer == null ?
                        App.Tail.Account.SelectedAccountGroup.AccountForRead :
                        (await Util.SelectAccount("유저페이지를 열때 사용할 계정", issuer)).AccountForRead;
                    var user = await App.Tail.TwitterAPI.GetUserAsync(account, mention.ID);
                    if (user != null)
                    {
#pragma warning disable CS4014 // 이 호출을 대기하지 않으므로 호출이 완료되기 전에 현재 메서드가 계속 실행됩니다.
                        App.Navigation.PushAsync(new UserDetailPage(user, account));
#pragma warning restore CS4014 // 이 호출을 대기하지 않으므로 호출이 완료되기 전에 현재 메서드가 계속 실행됩니다.
                    }
                }), null);
            }
        }
    }
}
