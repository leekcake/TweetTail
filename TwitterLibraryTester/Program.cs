using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterInterface.Data;
using TwitterLibrary;

namespace TwitterLibraryTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var api = new APIImpl();
            LoginTest(api);

            System.Console.ReadLine();
        }

        static async void LoginTest(APIImpl api)
        {
            var consumer = new Token(HiddenStore.sampleConsumerKey, HiddenStore.sampleConsumerSecret);

            var token = await api.GetLoginTokenAsync(consumer);
        }
    }
}
