using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Greentube.Test.ConsoleApp
{
    public class SimpleAsyncTest
    {
        public SimpleAsyncTest()
        {
        }

        public async Task<int> GetLength()
        {
            string result = "";

            var client = new HttpClient();
            Task<string> task = client.GetStringAsync(new Uri("http://admin.greentube.int/payout/"));

            for (var ii = 0; ii < 8000000; ii++)
            {
                var aa = 1;
            }

            result = await task;

            return result.Length;
        }
    }
}