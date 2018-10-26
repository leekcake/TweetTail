using FFImageLoading.Forms;
using FFImageLoading.Transformations;
using Library.Container.Account;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetTail.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DataUser = TwitterInterface.Data.User;
using DataStatus = TwitterInterface.Data.Status;
using DataMute = TwitterInterface.Data.Mute;
using TweetTail.Pages.Status;
using TweetTail.Pages.User;

namespace TweetTail.Components.Status
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatusView : ContentView
    {
        private bool NeedToPreventDisplay => Status.Creater.IsProtected || DisplayStatus.Creater.IsProtected;

        private DataStatus Status => BindingContext as DataStatus;

        private DataStatus DisplayStatus => Status.RetweetedStatus ?? Status;

        public ObservableCollection<DataStatus> Statuses {
            get {
                if (Parent == null)
                {
                    return null;
                }
                if (Parent is StatusCell)
                {
                    if (Parent.Parent is StatusListView)
                    {
                        return (Parent.Parent as StatusListView).Items;
                    }
                }
                return null;
            }
        }

        private GridImageWrapper gridImageWrapper;
        private PollView[] pollViews;

        private StatusView quoteView;
        
        public StatusView() : this(true)
        {

        }

        public StatusView(bool hasQuoteView)
        {
            InitializeComponent();
            gridImageWrapper = new GridImageWrapper(MediaGrid);

            RootView.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    if (Status == null) return;
                    App.Navigation.PushAsync(new StatusExpandPage( DisplayStatus ));
                })
            });

            HeaderView.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () => 
                {
                    if (Status == null) return;
                    var selected = await Util.SelectAccount("유저를 확인할 계정을 선택하세요", Status.Issuer);
                    if (selected == null)
                    {
                        return;
                    }
                    await App.Navigation.PushAsync(new UserDetailPage(Status.Creater, selected.AccountForRead));
                })
            });

            if(hasQuoteView)
            {
                quoteView = new StatusView(false);
                QuoteViewFrame.Content = (quoteView);
            }

            ProfileImage.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    //NotificationCell reuse
                    if(BindingContext is DataUser)
                    {
                        var user = BindingContext as DataUser;
                        App.Navigation.PushAsync(new UserDetailPage(user, App.Tail.Account.GetAccountGroup(user.Issuer[0]).AccountForRead));
                        return;
                    }
                    App.Navigation.PushAsync(new UserDetailPage(DisplayStatus.Creater, App.Tail.Account.GetAccountGroup(Status.Issuer[0]).AccountForRead));
                }),
                NumberOfTapsRequired = 1
            });

            ReplyImage.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    var page = new ContentPage() { Style = (Style) Application.Current.Resources["backgroundStyle"] };
                    var selected = await Util.SelectAccount("어떤 계정으로 답글을 작성할까요?", Status.Issuer);
                    if(selected == null)
                    {
                        return;
                    }
                    var view = new StatusWriterView( selected ) { BindingContext = page };
                    view.SetReplyStatus(Status);

                    page.Content = view;
                    page.Title = "트윗작성";
#pragma warning disable CS4014 // 이 호출을 대기하지 않으므로 호출이 완료되기 전에 현재 메서드가 계속 실행됩니다.
                    App.Navigation.PushAsync(page);
#pragma warning restore CS4014 // 이 호출을 대기하지 않으므로 호출이 완료되기 전에 현재 메서드가 계속 실행됩니다.
                }),
                NumberOfTapsRequired = 1
            });

            RetweetImage.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    try
                    {
                        var selected = await Util.SelectAccount("리트윗할 계정을 선택하세요", Status.Issuer);
                        if (selected == null)
                        {
                            return;
                        }
                        var animation = new Animation(v => RetweetImage.Rotation = v, 0, 360);
                        RetweetImage.Animate("Spin", animation, 16, 500, null, null, () => { return true; });
                        await App.Tail.TwitterAPI.RetweetStatusAsync(selected.AccountForWrite, Status.ID);
                        DisplayStatus.IsRetweetedByUser = true;
                        UpdateButton();
                        RetweetImage.AbortAnimation("Spin");
#pragma warning disable CS4014 // 이 호출을 대기하지 않으므로 호출이 완료되기 전에 현재 메서드가 계속 실행됩니다.
                        RetweetImage.RotateTo(360);
#pragma warning restore CS4014 // 이 호출을 대기하지 않으므로 호출이 완료되기 전에 현재 메서드가 계속 실행됩니다.
                    }
                    catch (Exception e)
                    {
                        Util.HandleException(e);
                    }

                }),
                NumberOfTapsRequired = 1
            });

            FavoriteImage.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    try
                    {
                        var selected = await Util.SelectAccount("관심글할 계정을 선택하세요", Status.Issuer);
                        if (selected == null)
                        {
                            return;
                        }

                        var animation = new Animation(v => FavoriteImage.Rotation = v, 0, 360);
                        FavoriteImage.Animate("Spin", animation, 16, 500, null, null, () => { return true; });

                        await App.Tail.TwitterAPI.CreateFavoriteAsync(selected.AccountForWrite, Status.ID);
                        DisplayStatus.IsFavortedByUser = true;
                        UpdateButton();

                        FavoriteImage.AbortAnimation("Spin");
#pragma warning disable CS4014 // 이 호출을 대기하지 않으므로 호출이 완료되기 전에 현재 메서드가 계속 실행됩니다.
                        FavoriteImage.RotateTo(360);
#pragma warning restore CS4014 // 이 호출을 대기하지 않으므로 호출이 완료되기 전에 현재 메서드가 계속 실행됩니다.
                    }
                    catch (Exception e)
                    {
                        Util.HandleException(e);
                    }
                }),
                NumberOfTapsRequired = 1
            });

            DeleteImage.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    var group = App.Tail.Account.GetAccountGroup(Status.Creater.ID);
                    if (group != null)
                    {
                        if (await Application.Current.MainPage.DisplayAlert("제거 확인", "이 트윗이 제거됩니다, 진행합니까?", "네", "아니오"))
                        {
                            try
                            {
                                await App.Tail.TwitterAPI.DestroyStatusAsync(group.AccountForWrite, Status.ID);
                            }
                            catch (Exception e)
                            {
                                await Application.Current.MainPage.DisplayAlert("오류", e.Message, "확인");
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                    Statuses?.Remove(Status);
                }),
                NumberOfTapsRequired = 1
            });

            MoreImage.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    try
                    {
                        DataStatus target = Status;
                        if (Status.RetweetedStatus != null)
                        {
                            if (!await Application.Current.MainPage.DisplayAlert("리트윗된 트윗", "이 트윗은 다른 유저가 리트윗한 트윗입니다. 어떤 트윗을 사용합니까?" +
                                "이 트윗을 사용해 다른 계정에서 리트윗/마음을 찍는경우 리트윗한 사람에게까지 알림이 갈 수 있습니다", "이 트윗", "원본트윗"))
                            {
                                target = Status.RetweetedStatus;
                            }
                        }

                        string[] moreActionSheet = {
                            "다른 계정으로 리트윗",
                            "다른 계정으로 관심글",
                            "트윗 뮤트"
                        };
                        var selected = await Application.Current.MainPage.DisplayActionSheet("이 트윗으로...", "취소", null, moreActionSheet);
                        AccountGroup account;
                        switch(selected)
                        {
                            case "다른 계정으로 리트윗":
                                account = await Util.SelectAccount("리트윗할 계정을 선택하세요");
                                if (account == null)
                                {
                                    return;
                                }

                                await App.Tail.TwitterAPI.RetweetStatusAsync(account.AccountForWrite, target.ID);
                                break;
                            case "다른 계정으로 관심글":
                                account = await Util.SelectAccount("관심글할 계정을 선택하세요");
                                if (account == null)
                                {
                                    return;
                                }

                                await App.Tail.TwitterAPI.CreateFavoriteAsync(account.AccountForWrite, target.ID);
                                break;
                            case "트윗 뮤트":
                                App.Tail.Mute.RegisterMute(new DataMute()
                                {
                                    Target = new DataMute.StatusTarget()
                                    {
                                        ID = Status.ID,
                                        Status = Status
                                    }
                                });
                                Statuses?.Remove(Status);
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Util.HandleException(e);
                    }
                }),
                NumberOfTapsRequired = 1
            });

            for (int i = 0; i < 4; i++)
            {
                int inx = i; //Value Copy
                gridImageWrapper[i].GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() =>
                    {
                        App.Navigation.PushAsync(new MediaPage(DisplayStatus, inx));
                    }),
                    NumberOfTapsRequired = 1
                });
            }

            pollViews = new PollView[4];
            for(int i = 0; i < 4; i++)
            {
                pollViews[i] = new PollView();
            }
        }

        public void ClearImage()
        {
            ProfileImage.Source = null;
            for (int i = 0; i < 4; i++)
            {
                gridImageWrapper[i].Source = null;
            }
        }

        public void UpdateImage()
        {
            ClearImage();

            if (NeedToPreventDisplay) return;

            var display = DisplayStatus;

            ProfileImage.Source = display.Creater.ProfileHttpsImageURL;

            if (display.ExtendMedias != null)
            {
                for (int i = 0; i < display.ExtendMedias.Length; i++)
                {
                    gridImageWrapper[i].Source = display.ExtendMedias[i].MediaURLHttps + ":thumb";
                }
                gridImageWrapper.SetCount(display.ExtendMedias.Length);
            }
        }

        protected void UpdateButton()
        {
            var display = DisplayStatus;
            
            if (display.IsRetweetedByUser)
            {
                RetweetImage.Source = "ic_repeat_green_300_24dp";
            }
            else
            {
                RetweetImage.Source = "ic_repeat_grey_500_24dp";
            }
            
            if (display.IsFavortedByUser)
            {
                FavoriteImage.Source = "ic_grade_yellow_light_24dp";
            }
            else
            {
                FavoriteImage.Source = "ic_grade_grey_500_24dp";
            }

            if (Statuses != null)
            {
                DeleteImage.IsVisible = true;
                var group = App.Tail.Account.GetAccountGroup(Status.Creater.ID);
                if (group != null)
                {
                    DeleteImage.Source = "ic_delete_grey_500_24dp";
                }
                else
                {
                    DeleteImage.Source = "ic_visibility_off_grey_500_24dp";
                }
            }
            else
            {
                DeleteImage.IsVisible = false;
            }
        }

        public void Update()
        {
            if (BindingContext is DataStatus) { }
            else
            {
                return;
            }
            var status = BindingContext as DataStatus;
            var display = DisplayStatus;

            if (display != status && !NeedToPreventDisplay)
            {
                HeaderView.IsVisible = true;
                HeaderLabel.Text = string.Format("{0} 님이 리트윗 하셨습니다", status.Creater.NickName);
            }
            else
            {
                HeaderView.IsVisible = false;
            }
            
            ClearImage();
            LockImage.IsVisible = display.Creater.IsProtected;
            CreatedAtLabel.Text = display.CreatedAt.ToLocalTime().ToString();

            if (NeedToPreventDisplay)
            {
                NameLabel.Text = "이 유저는 보호된 유저입니다";
                TextLabel.Text = "이 트윗은 보호된 유저의 트윗입니다";
                MediaGrid.IsVisible = false;
                return;
            }
            
            NameLabel.Text = string.Format("{0} @{1}", display.Creater.NickName, display.Creater.ScreenName);
            TextLabel.FormattedText = TwitterFormater.ParseFormattedString(display);

            if (display.ExtendMedias != null)
            {
                MediaGrid.IsVisible = true;
            }
            else
            {
                MediaGrid.IsVisible = false;
            }

            IssuerView.BindingContext = status.Issuer;
            IssuerView.Update();
            UpdateImage();
            UpdateButton();

            if(quoteView != null)
            {
                if (display.QuotedStatus != null)
                {
                    quoteView.BindingContext = display.QuotedStatus;
                    quoteView.Update();
                    QuoteViewFrame.IsVisible = true;
                }
                else
                {
                    quoteView.ClearImage();
                    QuoteViewFrame.IsVisible = false;
                }
            }

            if(display.Polls != null)
            {
                var poll = display.Polls[0];
                PollGroupView.IsVisible = true;
                for(int i = 0; i < poll.Options.Length; i++)
                {
                    pollViews[i].Update(poll, i);
                    PollsView.Children.Add(pollViews[i]);
                }
                if(poll.EndDateTime < DateTime.UtcNow)
                {
                    var leftTime = poll.EndDateTime - DateTime.UtcNow;
                    PollStatusLabel.Text = string.Format("{0}표 • 투표가 끝났습니다", poll.TotalCount);
                }
                else
                {
                    var leftTime = poll.EndDateTime - DateTime.UtcNow;
                    PollStatusLabel.Text = string.Format("{0}표 • {1} 남았습니다.", poll.TotalCount, Util.TimespanToString(leftTime));
                }
            }
            else
            {
                PollsView.Children.Clear();
                PollGroupView.IsVisible = false;
            }
        }
    }
}