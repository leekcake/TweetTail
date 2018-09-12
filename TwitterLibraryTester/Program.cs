using System;
using System.Threading.Tasks;
using TwitterInterface.Data;
using TwitterLibrary;
using System.IO;

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
            var api = new APIImpl();
            LoginTest(api).Wait();

            Console.WriteLine("End of Test");
            Console.ReadLine();
        }

        static async Task LoginTest(APIImpl api)
        {
            Console.WriteLine("Login Test");

            if (File.Exists("Account.bin"))
            {
                if (AskConsole("Use exist account?"))
                {
                    var read = new FileStream("Account.bin", FileMode.Open);
                    account = api.LoadAccount(read);
                    read.Close();
                    return;
                }
            }

            var consumer = new Token(HiddenStore.sampleConsumerKey, HiddenStore.sampleConsumerSecret);

            var token = await api.GetLoginTokenAsync(consumer);

            Console.WriteLine("Auth API: " + token.loginURL);
            Console.WriteLine("Wait for Pin: ");
            var line = Console.ReadLine();

            account = await token.login(line);

            var stream = new FileStream("Account.bin", FileMode.Create);
            account.Save(stream);
            stream.Close();
        }
    }
}
