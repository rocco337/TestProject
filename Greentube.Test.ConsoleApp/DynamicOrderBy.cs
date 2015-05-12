using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Greentube.Test.ConsoleApp
{
    public class DynamicOrderBy
    {
        private List<TestClass> _result;

        public void Run()
        {
            _result = GenerateData();

            Console.WriteLine("------ ID ------");
            for (var i = 0; i < 3; i++)
            {
                Console.WriteLine(i + ".");

                var s = Stopwatch.StartNew();
                EmptyIteration();
                s.Stop();
                Console.WriteLine("Empty: " + s.ElapsedMilliseconds);

                s = Stopwatch.StartNew();
                _result.OrderBy(m => m.Id).ToList();
                s.Stop();
                Console.WriteLine("ASC: " + s.ElapsedMilliseconds);

                s = Stopwatch.StartNew();
                _result.OrderBy(m => m.Id).ToList();
                s.Stop();

                s = Stopwatch.StartNew();
                _result.DynamicOrderBy("Id", true).ToList();
                s.Stop();
                Console.WriteLine("ASC DYN: " + s.ElapsedMilliseconds);

                s = Stopwatch.StartNew();
                _result.DynamicOrderBy("Id", false).ToList();
                s.Stop();
                Console.WriteLine("DESC DYN: " + s.ElapsedMilliseconds);

                var ordering = new DynamicLinqOrdering<TestClass>("Id");
                s = Stopwatch.StartNew();
                ordering.DynamicOrderBy(_result, true).ToList();
                s.Stop();
                Console.WriteLine("ASC DYN2: " + s.ElapsedMilliseconds);

                s = Stopwatch.StartNew();
                ordering.DynamicOrderBy(_result, false).ToList();
                s.Stop();
                Console.WriteLine("DESC DYN2: " + s.ElapsedMilliseconds);

                Console.WriteLine("");
            }

            Console.WriteLine("------ Name ------");
            for (var i = 0; i < 3; i++)
            {
                Console.WriteLine(i + ".");

                var s = Stopwatch.StartNew();
                EmptyIteration();
                s.Stop();
                Console.WriteLine("Empty: " + s.ElapsedMilliseconds);

                s = Stopwatch.StartNew();
                _result.OrderBy(m => m.Name).ToList();
                s.Stop();
                Console.WriteLine("ASC: " + s.ElapsedMilliseconds);

                s = Stopwatch.StartNew();
                _result.OrderBy(m => m.Name).ToList();
                s.Stop();

                s = Stopwatch.StartNew();
                _result.DynamicOrderBy("Name", true).ToList();
                s.Stop();
                Console.WriteLine("ASC DYN: " + s.ElapsedMilliseconds);

                s = Stopwatch.StartNew();
                _result.DynamicOrderBy("Name", false).ToList();
                s.Stop();
                Console.WriteLine("DESC DYN: " + s.ElapsedMilliseconds);

                var ordering = new DynamicLinqOrdering<TestClass>("Name");
                s = Stopwatch.StartNew();
                ordering.DynamicOrderBy(_result, true).ToList();
                s.Stop();
                Console.WriteLine("ASC DYN2: " + s.ElapsedMilliseconds);

                s = Stopwatch.StartNew();
                ordering.DynamicOrderBy(_result, false).ToList();
                s.Stop();
                Console.WriteLine("DESC DYN2: " + s.ElapsedMilliseconds);

                Console.WriteLine("");
            }

            Console.WriteLine("------ Created ------");
            for (var i = 0; i < 3; i++)
            {
                Console.WriteLine(i + ".");

                var s = Stopwatch.StartNew();
                EmptyIteration();
                s.Stop();
                Console.WriteLine("Empty: " + s.ElapsedMilliseconds);

                s = Stopwatch.StartNew();
                _result.OrderBy(m => m.Created).ToList();
                s.Stop();
                Console.WriteLine("ASC: " + s.ElapsedMilliseconds);

                s = Stopwatch.StartNew();
                _result.OrderBy(m => m.Created).ToList();
                s.Stop();

                s = Stopwatch.StartNew();
                _result.DynamicOrderBy("Created", true).ToList();
                s.Stop();
                Console.WriteLine("ASC DYN: " + s.ElapsedMilliseconds);

                s = Stopwatch.StartNew();
                _result.DynamicOrderBy("Created", false).ToList();
                s.Stop();
                Console.WriteLine("DESC DYN: " + s.ElapsedMilliseconds);

                var ordering = new DynamicLinqOrdering<TestClass>("Created");
                s = Stopwatch.StartNew();
                ordering.DynamicOrderBy(_result, true).ToList();
                s.Stop();
                Console.WriteLine("ASC DYN2: " + s.ElapsedMilliseconds);

                s = Stopwatch.StartNew();
                ordering.DynamicOrderBy(_result, false).ToList();
                s.Stop();
                Console.WriteLine("DESC DYN2: " + s.ElapsedMilliseconds);

                Console.WriteLine("");
            }
        }

        private void EmptyIteration()
        {
            foreach (var item in _result.ToList())
            {
                //do nothing
            }
        }

        private List<TestClass> GenerateData()
        {
            var result = new List<TestClass>();
            Random rnd = new Random();

            for (var ii = 0; ii < 1000000; ii++)
            {
                var entity = new TestClass()
                {
                    Id = rnd.Next(Int32.MaxValue),
                    Name = Guid.NewGuid().ToString(),
                    Created = RandomDay()
                };

                result.Add(entity);
            }

            return result;
        }

        private DateTime RandomDay()
        {
            DateTime start = new DateTime(1995, 1, 1);
            Random gen = new Random();

            int range = (DateTime.Today - start).Days;
            return start.AddDays(gen.Next(range));
        }

        private class TestClass
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public DateTime Created { get; set; }
        }
    }

    public static class Helper
    {
        public static IEnumerable<T> DynamicOrderBy<T>(this IEnumerable<T> list, string sortFieldName, bool isAscending)
        {
            Type enumType = typeof(T);
            PropertyInfo sortProperty = enumType.GetProperties().FirstOrDefault(m => m.Name.Equals(sortFieldName, StringComparison.OrdinalIgnoreCase));

            return isAscending ? list.OrderBy(m => sortProperty.GetValue(m)) : list.OrderByDescending(m => sortProperty.GetValue(m));
        }
    }

    public class DynamicLinqOrdering<T> where T : class
    {
        private Type classType;
        private PropertyInfo sortProperty;

        public DynamicLinqOrdering(string fieldName)
        {
            classType = typeof(T);
            sortProperty = classType.GetProperties().FirstOrDefault(m => m.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<T> DynamicOrderBy<T>(IEnumerable<T> list, bool isAscending)
        {
            return isAscending ? list.OrderBy(m => sortProperty.GetValue(m)) : list.OrderByDescending(m => sortProperty.GetValue(m));
        }
    }
}