using SimpleValidator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greentube.Test.ConsoleApp
{
    internal class Program
    {
        private static SomeService service1 = new SomeService();

        private static void Main(string[] args)
        {
            //var list = new List<BussinessObject>();
            //for (var ii = 0; ii < 9999999; ii++)
            //{
            //    list.Add(new BussinessObject()
            //    {
            //        Id = (new Random()).Next(),
            //        Name = "Roko",
            //        Email = "roko@greentube.com",
            //        Age = (new Random()).Next(1,50)
            //    });
            //}

            //Stopwatch s1 = new Stopwatch();
            //s1.Start();
            //var result = list.Select(m => m.Id);
            //s1.Stop();

            //Stopwatch s2 = new Stopwatch();
            //s2.Start();
            //var result2 = list.Where(m=>m.Age>=18).Select(m => m.Id);
            //s2.Stop();

            //Console.WriteLine("S1:" + s1.ElapsedTicks);
            //Console.WriteLine("S2:" + s2.ElapsedTicks);
            //Console.ReadLine();

            //var respawn = new RespawnTest();
            //respawn.DoOneMinuteTestThanReSpawn();

            //var asyncMethod = new AsyncTest();
            //asyncMethod.Test();

            //(new DynamicOrderBy()).Run();
            //Console.ReadLine();

            //var test = new SimpleAsyncTest();
            //var length = test.GetLength().Result;

            //Console.WriteLine("length: " + length);

            new ListPerformanceTest();
        }
    }

    public class BussinessObject
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public int Age { get; set; }
    }

    public class SomeService
    {
        public void Proccess1(BussinessObject input)
        {
            var validator = new BussinessObjectValidator(input);
            if (validator.IsValid())
            {
                //do something
            }
            else
            {
                throw new Exception(string.Join(",", validator.ErrorMessages.ToArray()));
            }
        }
    }

    /// <summary>
    /// Implementation of validaiton for our simple object
    /// </summary>
    public class BussinessObjectValidator : ObjectValidatorBase<BussinessObject>
    {
        public BussinessObjectValidator(BussinessObject model)
            : base(model)
        {
            AddRule(o => o.Age >= 18, "User is underage!");
            AddRule(o => !string.IsNullOrEmpty(o.Name), "Enter name!");
            AddRule(o => !string.IsNullOrEmpty(o.Email), "Enter email!");
            AddRule(o => isEMail(o.Email), o => !string.IsNullOrEmpty(o.Email), "Enter valid email!");
        }

        private bool isEMail(string email)
        {
            return true;
        }
    }
}