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

using DataStatus = TwitterInterface.Data.Status;

namespace TweetTail.Status
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatusView : ContentView
    {
        private static TintTransformation retweetTransformation = new TintTransformation("#009900");
        private static TintTransformation favoriteTransformation = new TintTransformation("#FF0000");

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

        private CachedImage getMediaView(int inx)
        {
            switch (inx)
            {
                case 0:
                    return imgMedia1;
                case 1:
                    return imgMedia2;
                case 2:
                    return imgMedia3;
                case 3:
                    return imgMedia4;
            }
            throw new IndexOutOfRangeException();
        }

        public StatusView()
        {
            InitializeComponent();

            imgProfile.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
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
                    getMediaView(i).Source = display.extendMedias[i].mediaURLHttps;
                }
            }
        }

        protected void UpdateButton()
        {
            var display = getDisplayStatus(status);

            imgRetweet.Transformations.Clear();
            if (display.isRetweetedByUser)
            {
                imgRetweet.Transformations.Add(retweetTransformation);
            }

            imgFavorite.Transformations.Clear();
            if (display.isFavortedByUser)
            {
                imgFavorite.Transformations.Add(favoriteTransformation);
            }
            imgRetweet.ReloadImage();
            imgFavorite.ReloadImage();

            if (statuses != null)
            {
                imgDelete.IsVisible = true;
                var group = App.tail.account.getAccountGroup(status.creater.id);
                if (group != null)
                {
                    imgDelete.Source = "ic_delete_black_24dp";
                }
                else
                {
                    imgDelete.Source = "ic_visibility_off_black_24dp";
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
            lblCreatedAt.Text = display.createdAt.ToString();
            lblName.Text = string.Format("{0} @{1}", display.creater.nickName, display.creater.screenName);
            lblText.Text = display.text;

            imgProfile.Source = null;
            for (int i = 0; i < 4; i++)
            {
                getMediaView(i).Source = null;
            }
            if (display.extendMedias != null)
            {
                viewMedias.IsVisible = true;
            }
            else
            {
                viewMedias.IsVisible = false;
            }

            viewIssuer.BindingContext = status.issuer;
            viewIssuer.Update();
            UpdateImage();
            UpdateButton();
        }
    }
}