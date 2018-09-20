using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Library;
using TweetTail.Login;
using TwitterLibrary;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace TweetTail
{
    public partial class App : Application
    {
        public static Library.TweetTail tail;

        public App()
        {
            InitializeComponent();

            var localData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            
            tail = new Library.TweetTail(new APIImpl(), 
                Path.Combine(localData, "save"));

            if (tail.account.readOnlyAccountGroups.Count != 0)
            {
                MainPage = new Status.StatusPage();
            }
            else
            {
                MainPage = new LoginView();
            }
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
