using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Greentube.Test.ConsoleApp
{
    public class ListPerformanceTest
    {
        private const int _listCount = 10000;

        private int[] _array = new int[_listCount];
        private List<int> _list = new List<int>();
        private LinkedList<int> _linkedList = new LinkedList<int>();

        public ListPerformanceTest()
        {
            Add();
            Iterate();
            Remove();
        }

        public void Add()
        {
            var s = Stopwatch.StartNew();
            for (var ii = 0; ii < _listCount; ii++)
            {
                _array[ii] = ii;
            }
            s.Stop();
            Console.WriteLine("Array add: {0}", s.ElapsedTicks);
            s.Start();
            for (var ii = 0; ii < _listCount; ii++)
            {
                _list.Add(ii);
            }
            s.Stop();
            Console.WriteLine("List add: {0}", s.ElapsedTicks);

            s.Start();
            for (var ii = 0; ii < _listCount; ii++)
            {
                _linkedList.AddFirst(new LinkedListNode<int>(ii));
            } s.Stop();
            Console.WriteLine("LinkedList add: {0}", s.ElapsedTicks);
        }

        public void Remove()
        {
            var s = Stopwatch.StartNew();

            Console.WriteLine("Array remove: {0}", s.ElapsedTicks);
            s.Start();
            for (var ii = 0; ii < _listCount; ii++)
            {
                _list.Remove(ii);
            }
            s.Stop();
            Console.WriteLine("List remove: {0}", s.ElapsedTicks);
            s.Start();
            for (var ii = 0; ii < _listCount; ii++)
            {
                _linkedList.Remove(ii);
            }
            s.Stop();
            Console.WriteLine("LinkedList remove: {0}", s.ElapsedTicks);
        }

        public void Iterate()
        {
            var s = Stopwatch.StartNew();

            foreach (var i in _array)
            {
            }
            s.Stop();
            Console.WriteLine("Array iteration: {0}", s.ElapsedTicks);
            s.Start();
            foreach (var i in _list)
            {
            }
            s.Stop();
            Console.WriteLine("List iteration: {0}", s.ElapsedTicks);
            s.Start();
            foreach (var i in _linkedList)
            {
            }
            s.Stop();
            Console.WriteLine("LinkedList iteration: {0}", s.ElapsedTicks);
        }
    }
}