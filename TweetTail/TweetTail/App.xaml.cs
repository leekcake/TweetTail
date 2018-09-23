using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Library;
using TweetTail.Login;
using TwitterLibrary;
using TweetTail.Menu;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace TweetTail
{
    public partial class App : Application
    {
        public static Library.TweetTail tail;

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
            
            tail = new Library.TweetTail(new APIImpl(), 
                Path.Combine(localData, "save"));
            
            if (tail.account.readOnlyAccountGroups.Count != 0)
            {
                NavigationPage = new NavigationPage( new SingleTailPage() );
            }
            else
            {
                NavigationPage = new NavigationPage( new LoginView() );
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
