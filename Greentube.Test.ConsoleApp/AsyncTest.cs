using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Greentube.Test.ConsoleApp
{
    public class AsyncTest
    {
        public AsyncTest()
        {
        }

        public void Test()
        {
            var task1 = FirstMethod();
            task1.GetAwaiter().OnCompleted(() =>
            {
                var aa = task1.GetAwaiter().GetResult();
                Console.WriteLine(aa);
            });

            Task.WaitAll(new[] { task1 });
        }

        private Task<string> FirstMethod()
        {
            return Task<string>.Factory.StartNew(() =>
            {
                Thread.Sleep(1000);
                return DateTime.Now.ToString();
            });
        }

        private Task<string> SecondMethod()
        {
            return Task<string>.Factory.StartNew(() =>
            {
                Thread.Sleep(1000);
                return DateTime.Now.ToString();
            });
        }

        private Task<string> ThirdMethod()
        {
            return Task<string>.Factory.StartNew(() =>
            {
                Thread.Sleep(1000);
                return DateTime.Now.ToString();
            });
        }
    }
}