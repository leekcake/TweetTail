using System;
using System.Threading.Tasks;
using TwitterInterface.Data;
using TwitterLibrary;
using System.IO;
using System.Runtime.ExceptionServices;
using Newtonsoft.Json.Linq;

namespace TwitterLibraryTester
{
    class Program
    {
        static bool AskConsole(string message)
        {
            Console.WriteLine(message + " (y/n)");
            var value = Console.ReadLine();

            if(value.ToLower() == "y")
            {
                return true;
            }
            return false;
        }

        private static Account account;

        static void Main(string[] args)
        {
            var task = TestPerform();

            while (!task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
            {
                System.Threading.Thread.Sleep(1000);
            }
            ThrowError(task);

            Console.WriteLine("End of Test");
            Console.ReadLine();
        }

        private static void ThrowError(Task t)
        {
            if (t.IsFaulted)
            {
                Exception exception =
                    t.Exception.InnerExceptions != null && t.Exception.InnerExceptions.Count == 1
                        ? t.Exception.InnerExceptions[0]
                        : t.Exception;

                ExceptionDispatchInfo.Capture(exception).Throw();
            }
        }

        static async Task TestPerform()
        {
            var api = new APIImpl();

            await LoginTest(api);
            await VerifyTest(api);
            await ReadTweetTest(api);
            await PostTweetTest(api);
        }

        static async Task LoginTest(APIImpl api)
        {
            Console.WriteLine("Login Test");

            if (File.Exists("Account.bin"))
            {
                if (AskConsole("Use exist account?"))
                {
                    account = api.LoadAccount( JObject.Parse( File.ReadAllText("Account.bin")) );
                    return;
                }
            }

            var consumer = new Token(HiddenStore.sampleConsumerKey, HiddenStore.sampleConsumerSecret);

            var token = await api.GetLoginTokenAsync(consumer);

            Console.WriteLine("Auth API: " + token.loginURL);
            Console.WriteLine("Wait for Pin: ");
            var line = Console.ReadLine();

            account = await token.login(line);

            File.WriteAllText("Account.bin", account.Save().ToString());

            Console.WriteLine("Login Test OK");
        }

        static async Task VerifyTest(APIImpl api)
        {
            Console.WriteLine("Verify Test");

            var user = await api.VerifyCredentials(account);

            Console.WriteLine(user.nickName + " (@" + user.screenName + ") verify complete");


            Console.WriteLine("Verify Test OK");
        }

        static async Task ReadTweetTest(APIImpl api)
        {
            Console.WriteLine("ReadTweetTest Test");

            var line = await api.GetTimeline(account);

            Console.WriteLine("Latest Timeline tweet text: " + line[0].text);

            Console.WriteLine("ReadTweetTest Test OK");
        }

        static async Task PostTweetTest(APIImpl api)
        {
            Console.WriteLine("Post Tweet Test");

            var update = new StatusUpdate();
            update.text = "샘플 데-스!";

            var status = await api.CreateStatus(account, update);

            Console.WriteLine("Post Tweet Test OK");
        }
    }
}
