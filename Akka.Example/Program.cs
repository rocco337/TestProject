using Akka.Actor;
using Akka.Dispatch.SysMsg;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Akka.Example
{
    public class Item
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class WarehouseActor : ReceiveActor
    {
        private Random rnd = new Random();
        private IActorRef productionActor = Context.Watch(Context.ActorOf<ProductionActor>("ProductionActorWorker"));
        private IActorRef logActor = Context.Watch(Context.ActorOf<LogActor>("LogActorActorWorker"));

        public WarehouseActor()
        {
            Receive<Item>(m =>
            {
                AddToWarehouse(m).PipeTo(productionActor);
            });
        }

        private Task<Item> AddToWarehouse(Item m)
        {
            return Task.Factory.StartNew<Item>(() =>
            {
                var storageTime = rnd.Next(5, 10);
                Thread.Sleep(storageTime);

                Dashboard.AddToWarehouseActor();

                logActor.Tell(string.Format("{0}:{1} item received!Stored in {2} ms", DateTime.Now.TimeOfDay, m.Id, storageTime));

                return m;
            });
        }
    }

    public class ProductionActor : ReceiveActor
    {
        private Random rnd = new Random();
        private IActorRef shipmentActor = Context.Watch(Context.ActorOf<ShipmentActor>("ShipmentActorWorker"));
        private IActorRef logActor = Context.Watch(Context.ActorOf<LogActor>("LogActorActorWorker"));

        public ProductionActor()
        {
            Receive<Item>(m =>
            {
                AddToProduction(m).PipeTo(shipmentActor);
            });
        }

        private Task<Item> AddToProduction(Item m)
        {
            return Task.Factory.StartNew<Item>(() =>
            {
                Dashboard.AddToProduction();

                var productionTime = rnd.Next(100, 500);
                Thread.Sleep(productionTime);

                logActor.Tell(string.Format("{0}:{1} item producing!Produced in {2} ms!", DateTime.Now.TimeOfDay, m.Id, productionTime));

                return m;
            });
        }
    }

    public class ShipmentActor : ReceiveActor
    {
        private Random rnd = new Random();
        private IActorRef logActor = Context.Watch(Context.ActorOf<LogActor>("LogActorActorWorker"));

        public ShipmentActor()
        {
            Receive<Item>(m =>
            {
                AddToShippment(m);
            });
        }

        private Task<Item> AddToShippment(Item m)
        {
            return Task.Factory.StartNew<Item>(() =>
            {
                Dashboard.ShipItem();

                var shippmentTime = rnd.Next(50, 100);
                Thread.Sleep(shippmentTime);

                logActor.Tell(string.Format("{0}:{1} item shipped!", DateTime.Now.TimeOfDay, m.Id));
                return m;
            });
        }
    }

    public class LogActor : ReceiveActor
    {
        public LogActor()
        {
            Receive<string>(m => LogItem(m));
        }

        private void LogItem(string message)
        {
            Debug.WriteLine(message);
        }
    }

    public static class Dashboard
    {
        private static object lock1 = new object();
        private static object lock2 = new object();
        private static object lock3 = new object();

        private static int warehouseCount = 0;
        private static int productionCount = 0;
        private static int shipedCount = 0;
        private static bool started = false;

        public static void AddToWarehouseActor()
        {
            lock (lock1)
            {
                warehouseCount++;
                started = true;
            }
        }

        public static void AddToProduction()
        {
            lock (lock2)
            {
                warehouseCount--;
                productionCount++;
            }
        }

        public static void ShipItem()
        {
            lock (lock3)
            {
                productionCount--;
                shipedCount++;
            }
        }

        public static bool IsFinished()
        {
            return warehouseCount == 0 && productionCount == 0 && started;
        }

        public static void PrintDashboard()
        {
            Console.Clear();
            Console.Write("\r Warehouse: {0} Production:{1} Shipped:{2}", warehouseCount, productionCount, shipedCount);
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            var rnd = new Random();

            //create a new actor system (a container for your actors)
            var system = ActorSystem.Create("FactorySystem");

            var warehouse = system.ActorOf<WarehouseActor>("WarehouseActor");

            for (var ii = 0; ii < 100; ii++)
            {
                Task.Factory.StartNew<Item>(() =>
                {
                    return new Item()
                    {
                        Id = rnd.Next(),
                        Name = RandomString(10)
                    };
                }).PipeTo(warehouse);
            }

            Task.WaitAll(Task.Factory.StartNew(() =>
            {
                while (!Dashboard.IsFinished())
                {
                    Thread.Sleep(100);
                    Dashboard.PrintDashboard();
                }
            }));

            Console.ReadLine();
        }

        private static readonly Random _rng = new Random();
        private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private static string RandomString(int size)
        {
            char[] buffer = new char[size];

            for (int i = 0; i < size; i++)
            {
                buffer[i] = _chars[_rng.Next(_chars.Length)];
            }
            return new string(buffer);
        }
    }
}