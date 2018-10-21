using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Library;
using TwitterLibrary;
using TweetTail.Pages;
using TweetTail.Pages.Login;
using TweetTail.Pages.Menu;
using Plugin.Media.Abstractions;
using Plugin.Media;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace TweetTail
{
    public partial class App : Application
    {
        public static Library.TweetTail Tail;
        private static IMedia media;
        public static IMedia Media {
            get {
                if(media == null)
                {
                    media = CrossMedia.Current;
                }
                return media;
            }
            set {
                media = value;
            }
        }

        public static INavigation Navigation {
            get {
                return (Current as App).NavigationPage.Navigation;
            }
        }

        public NavigationPage NavigationPage { get; private set; }

        public App()
        {
            InitializeComponent();

            var localData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            
            Tail = new Library.TweetTail(new API(), 
                Path.Combine(localData, "save"));
            
            if (Tail.Account.ReadOnlyAccountGroups.Count != 0)
            {
                NavigationPage = new NavigationPage( new LoadingPage() );
            }
            else
            {
                NavigationPage = new NavigationPage( new LoginPage() );
            }

            var rootPage = new RootPage();
            rootPage.Master = new MenuPage();
            rootPage.Detail = NavigationPage;

            MainPage = rootPage;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
