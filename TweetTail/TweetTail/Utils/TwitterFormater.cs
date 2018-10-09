using System;
using System.Collections.Generic;
using System.Net;
using TweetTail.Pages.User;
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
                public Indices indices;
                public Command command;
                public string replace;

                public bool invisible = false;

                public Hyperlink(Indices indices, Command command, string replace = null)
                {
                    this.indices = indices;
                    this.command = command;
                    this.replace = replace;
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
                    if (hyperLinks[i].indices.start > link.indices.start)
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
                    invisible = true
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

                    result.Spans.Add(new Span() { Text = WebUtility.HtmlDecode(text.Substring(pos, link.indices.start - pos)) });

                    if (!link.invisible)
                    {
                        var span = new Span()
                        {
                            Text = link.replace == null ? WebUtility.HtmlDecode(text.Substring(link.indices.start, link.indices.Length)) : link.replace,
                            TextColor = Color.Blue
                        };
                        span.GestureRecognizers.Add(new TapGestureRecognizer()
                        {
                            Command = link.command
                        });

                        result.Spans.Add(span);
                    }

                    pos = link.indices.end;
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
            var builder = new Builder(status.text);

            ParseURLEntity(builder, status.urls);
            ParseMediaEntity(builder, status.extendMedias);
            ParseMentionEntity(builder, status.userMentions, status.issuer);

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
                builder.RegisterHyperlink(url.indices, new Command(() =>
                {
                    Device.OpenUri(new Uri(url.url));
                }), url.displayURL);
            }
        }

        public static void ParseMediaEntity(Builder builder, ExtendMedia[] entities)
        {
            if (entities == null)
            {
                return;
            }

            //Media already handled in StatusView, so hide it.
            builder.RegisterRemover(entities[0].indices);
        }

        public static void ParseMentionEntity(Builder builder, UserMention[] entities, List<long> issuer)
        {
            if (entities == null)
            {
                return;
            }

            foreach (var mention in entities)
            {
                builder.RegisterHyperlink(mention.indices, new Command(async () =>
                {
                    var account = issuer == null ?
                        App.tail.account.SelectedAccountGroup.accountForRead :
                        (await Util.SelectAccount("유저페이지를 열때 사용할 계정", issuer)).accountForRead;
                    var user = await App.tail.twitter.GetUser(account, mention.id);
                    if (user != null)
                    {
                        App.Navigation.PushAsync(new UserDetailPage(user, account));
                    }
                }), null);
            }
        }
    }
}
