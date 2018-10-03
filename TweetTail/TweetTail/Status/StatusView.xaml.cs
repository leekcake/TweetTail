using FFImageLoading.Forms;
using FFImageLoading.Transformations;
using Library.Container.Account;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetTail.User;
using TweetTail.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DataUser = TwitterInterface.Data.User;
using DataStatus = TwitterInterface.Data.Status;

namespace TweetTail.Status
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatusView : ContentView
    {
        private DataStatus status {
            get {
                return BindingContext as DataStatus;
            }
        }

        public ObservableCollection<DataStatus> statuses {
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

        private List<CachedImage> images;

        private CachedImage getMediaView(int inx)
        {
            return images[inx];
        }

        public StatusView()
        {
            InitializeComponent();

            images = new List<CachedImage>();
            for (int i = 0; i < 4; i++)
            {
                var cached = new CachedImage();
                cached.Aspect = Aspect.AspectFill;
                cached.WidthRequest = 10000;
                images.Add(cached);
            }

            imgProfile.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    //NotificationCell reuse
                    if(BindingContext is DataUser)
                    {
                        var user = BindingContext as DataUser;
                        App.Navigation.PushAsync(new UserDetailPage(user, App.tail.account.getAccountGroup(user.issuer[0]).accountForRead));
                        return;
                    }
                    App.Navigation.PushAsync(new UserDetailPage(getDisplayStatus(status).creater, App.tail.account.getAccountGroup(status.issuer[0]).accountForRead));
                }),
                NumberOfTapsRequired = 1
            });

            imgReply.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    Application.Current.MainPage.DisplayAlert("TODO", "Reply Button", "OK");
                }),
                NumberOfTapsRequired = 1
            });

            imgRetweet.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    try
                    {
                        var selected = await Util.SelectAccount("리트윗할 계정을 선택하세요", status.issuer);
                        if (selected == null)
                        {
                            return;
                        }

                        await App.tail.twitter.RetweetStatus(selected.accountForWrite, status.id);
                        getDisplayStatus(status).isRetweetedByUser = true;
                        UpdateButton();
                    }
                    catch (Exception e)
                    {
                        Util.HandleException(e);
                    }

                }),
                NumberOfTapsRequired = 1
            });

            imgFavorite.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    try
                    {
                        var selected = await Util.SelectAccount("관심글할 계정을 선택하세요", status.issuer);
                        if (selected == null)
                        {
                            return;
                        }

                        await App.tail.twitter.CreateFavorite(selected.accountForWrite, status.id);
                        getDisplayStatus(status).isFavortedByUser = true;
                        UpdateButton();
                    }
                    catch (Exception e)
                    {
                        Util.HandleException(e);
                    }
                }),
                NumberOfTapsRequired = 1
            });

            imgDelete.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    var group = App.tail.account.getAccountGroup(status.creater.id);
                    if (group != null)
                    {
                        if (await Application.Current.MainPage.DisplayAlert("제거 확인", "이 트윗이 제거됩니다, 진행합니까?", "네", "아니오"))
                        {
                            try
                            {
                                await App.tail.twitter.DestroyStatus(group.accountForWrite, status.id);
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
                    statuses.Remove(status);
                }),
                NumberOfTapsRequired = 1
            });

            imgMore.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    try
                    {
                        DataStatus target = status;
                        if (status.retweetedStatus != null)
                        {
                            if (!await Application.Current.MainPage.DisplayAlert("리트윗된 트윗", "이 트윗은 다른 유저가 리트윗한 트윗입니다. 어떤 트윗을 사용합니까?" +
                                "이 트윗을 사용해 다른 계정에서 리트윗/마음을 찍는경우 리트윗한 사람에게까지 알림이 갈 수 있습니다", "이 트윗", "원본트윗"))
                            {
                                target = status.retweetedStatus;
                            }
                        }

                        string[] moreActionSheet = {
                            "다른 계정으로 리트윗",
                            "다른 계정으로 관심글"
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

                                await App.tail.twitter.RetweetStatus(account.accountForWrite, target.id);
                                break;
                            case "다른 계정으로 관심글":
                                account = await Util.SelectAccount("관심글할 계정을 선택하세요");
                                if (account == null)
                                {
                                    return;
                                }

                                await App.tail.twitter.CreateFavorite(account.accountForWrite, target.id);
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
                getMediaView(i).GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() =>
                    {
                        App.Navigation.PushAsync(new MediaPage(getDisplayStatus(status), inx));
                    }),
                    NumberOfTapsRequired = 1
                });
            }
        }

        private DataStatus getDisplayStatus(DataStatus status)
        {
            if (status.retweetedStatus != null)
            {
                return status.retweetedStatus;
            }
            else
            {
                return status;
            }
        }

        protected void UpdateImage()
        {
            var display = getDisplayStatus(status);

            imgProfile.Source = null;
            for (int i = 0; i < 4; i++)
            {
                getMediaView(i).Source = null;
            }

            imgProfile.Source = display.creater.profileHttpsImageURL;

            if (display.extendMedias != null)
            {
                for (int i = 0; i < display.extendMedias.Length; i++)
                {
                    getMediaView(i).Source = display.extendMedias[i].mediaURLHttps + ":thumb";
                }
                
                gridMedias.RowDefinitions.Clear();
                gridMedias.ColumnDefinitions.Clear();
                gridMedias.Children.Clear();

                switch (display.extendMedias.Length)
                {
                    case 1:
                        gridMedias.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                        gridMedias.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                        gridMedias.Children.Add(getMediaView(0), 0, 0);
                        break;
                    case 2:
                        gridMedias.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                        gridMedias.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                        gridMedias.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                        gridMedias.Children.Add(getMediaView(0), 0, 0);
                        gridMedias.Children.Add(getMediaView(1), 1, 0);
                        break;
                    case 3:
                        gridMedias.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                        gridMedias.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                        gridMedias.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                        gridMedias.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                        gridMedias.Children.Add(getMediaView(0), 0, 0);
                        gridMedias.Children.Add(getMediaView(1), 1, 0);
                        gridMedias.Children.Add(getMediaView(2), 0, 1);
                        break;
                    case 4:
                        gridMedias.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                        gridMedias.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                        gridMedias.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                        gridMedias.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                        gridMedias.Children.Add(getMediaView(0), 0, 0);
                        gridMedias.Children.Add(getMediaView(1), 1, 0);
                        gridMedias.Children.Add(getMediaView(2), 0, 1);
                        gridMedias.Children.Add(getMediaView(3), 1, 1);
                        break;
                }
            }
        }

        protected void UpdateButton()
        {
            var display = getDisplayStatus(status);
            
            if (display.isRetweetedByUser)
            {
                imgRetweet.Source = "ic_repeat_green_300_24dp";
            }
            else
            {
                imgRetweet.Source = "ic_repeat_grey_500_24dp";
            }
            
            if (display.isFavortedByUser)
            {
                imgFavorite.Source = "ic_grade_yellow_light_24dp";
            }
            else
            {
                imgFavorite.Source = "ic_grade_grey_500_24dp";
            }

            if (statuses != null)
            {
                imgDelete.IsVisible = true;
                var group = App.tail.account.getAccountGroup(status.creater.id);
                if (group != null)
                {
                    imgDelete.Source = "ic_delete_grey_500_24dp";
                }
                else
                {
                    imgDelete.Source = "ic_visibility_off_grey_500_24dp";
                }
            }
            else
            {
                imgDelete.IsVisible = false;
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
            var display = getDisplayStatus(status);

            if (display != status)
            {
                viewHeader.IsVisible = true;
                lblHeader.Text = string.Format("{0} 님이 리트윗 하셨습니다", status.creater.nickName);
            }
            else
            {
                viewHeader.IsVisible = false;
            }

            imgLock.IsVisible = display.creater.isProtected;
            lblCreatedAt.Text = display.createdAt.ToLocalTime().ToString();
            lblName.Text = string.Format("{0} @{1}", display.creater.nickName, display.creater.screenName);
            lblText.FormattedText = TwitterFormater.ParseFormattedString(display);

            imgProfile.Source = null;
            for (int i = 0; i < 4; i++)
            {
                getMediaView(i).Source = null;
            }
            if (display.extendMedias != null)
            {
                gridMedias.IsVisible = true;
            }
            else
            {
                gridMedias.IsVisible = false;
            }

            viewIssuer.BindingContext = status.issuer;
            viewIssuer.Update();
            UpdateImage();
            UpdateButton();
        }
    }
}