using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Akka.Bootcamp.Unit1
{
    public class ProccessUsage
    {
        public ProccessUsage()
        {
            var system = ActorSystem.Create("ProcessUsageSystem");
            system.ActorOf<Logger>("Logger");

            var uiNotifier = system.ActorOf<UiNotifierActor>("notifyActor");

            var someActor = system.ActorOf<ResourceRetrieverActor>("ResourceRetrieverActor");

            system.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(150), someActor, RetrieverTypes.Cpu, uiNotifier);
            system.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(150), someActor, RetrieverTypes.Hdd, uiNotifier);
            system.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(150), someActor, RetrieverTypes.Memory, uiNotifier);

            system.AwaitTermination();
        }
    }

    #region Contracts

    public enum RetrieverTypes
    {
        Cpu,
        Memory,
        Hdd
    }

    public interface IResourceRetriever
    {
        RetrieverResponse GetUsage(RetrieverTypes type);
    }

    public class RetrieverResponse
    {
        public RetrieverTypes Type { get; set; }

        public float Usage { get; set; }

        public string Unit { get; set; }

        public override string ToString()
        {
            return string.Format("{0} usage: {1} {2}", Type, Usage, Unit);
        }

        public RetrieverResponse(float usage, string unit, RetrieverTypes type)
        {
            Usage = usage;
            Unit = unit;
            Type = type;
        }
    }

    #endregion Contracts

    #region actors

    public class UiNotifierActor : ReceiveActor
    {
        private static Dictionary<RetrieverTypes, RetrieverResponse> _dashboard = new Dictionary<RetrieverTypes, RetrieverResponse>();

        public UiNotifierActor()
        {
            Receive<RetrieverResponse>((response) =>
            {
                NotifyDashboard(response);

                Context.ActorSelection("../Logger").Tell(response);
            });
        }

        public static void DrawDashboard()
        {
            var result = new StringBuilder();
            result.AppendLine(string.Empty);
            foreach (var dashboardItem in _dashboard)
            {
                result.AppendLine(dashboardItem.Value.ToString());
            }

            result.AppendLine(string.Empty);
            foreach (var dashboardItem in _dashboard)
            {
                var percent = (int)(dashboardItem.Value.Usage / 1.4);
                result.AppendLine(string.Format("[{0}]:{1}", dashboardItem.Key, ".".Repeat(percent)));
            }
            result.AppendLine(string.Empty);

            Console.Clear();
            Console.Write(result);
        }

        public static void NotifyDashboard(RetrieverResponse response)
        {
            if (_dashboard.ContainsKey(response.Type))
            {
                _dashboard[response.Type] = response;
            }
            else
            {
                _dashboard.Add(response.Type, response);
            }

            DrawDashboard();
        }
    }

    public class Logger : ReceiveActor
    {
        private IActorRef debugLogger = Context.ActorOf<DebugLogger>("DebugLogger");
        private IActorRef fileLogger = Context.ActorOf<DebugLogger>("FileLogger");

        public Logger()
        {
            Receive<object>((value) =>
            {
                debugLogger.Tell(value);
                fileLogger.Tell(value);
            });
        }
    }

    public class DebugLogger : ReceiveActor
    {
        public DebugLogger()
        {
            Receive<RetrieverResponse>((response) =>
            {
                Debug.WriteLine(response.ToString());
            });
        }
    }

    public class FileLogger : ReceiveActor
    {
        private List<string> _logFile = new List<string>();

        public FileLogger()
        {
            Receive<RetrieverResponse>((response) =>
            {
                _logFile.Add(response.ToString());
            });
        }
    }

    public class ResourceRetrieverActor : ReceiveActor
    {
        public ResourceRetrieverActor()
        {
            Receive<RetrieverTypes>((type) => Sender.Tell(RetrieverFactory.Initiate(type).GetUsage(type)));
        }
    }

    #endregion actors

    #region Retreivers

    public class RetrieverFactory
    {
        public static IResourceRetriever Initiate(RetrieverTypes type)
        {
            switch (type)
            {
                case RetrieverTypes.Cpu:
                    return new CpuRetriever();

                case RetrieverTypes.Memory:
                    return new MemoryRetriever();

                case RetrieverTypes.Hdd:
                    return new HddRetriever();

                default:
                    throw new NotSupportedException();
            }
        }
    }

    public class CpuRetriever : IResourceRetriever
    {
        public RetrieverResponse GetUsage(RetrieverTypes type)
        {
            var counter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            counter.NextValue();
            System.Threading.Thread.Sleep(100);
            var usage = counter.NextValue();

            return new RetrieverResponse(usage, "%", type);
        }
    }

    public class MemoryRetriever : IResourceRetriever
    {
        public RetrieverResponse GetUsage(RetrieverTypes type)
        {
            var counter = new PerformanceCounter("Memory", "% Committed Bytes In Use", true);
            counter.NextValue();
            System.Threading.Thread.Sleep(100); // wait a second to get a valid reading
            var usage = counter.NextValue();

            return new RetrieverResponse(usage, "%", type);
        }
    }

    public class HddRetriever : IResourceRetriever
    {
        public RetrieverResponse GetUsage(RetrieverTypes type)
        {
            var counter = new PerformanceCounter("LogicalDisk", "% Disk Time", "_Total", true);
            counter.NextValue();
            System.Threading.Thread.Sleep(100); // wait a second to get a valid reading
            var usage = counter.NextValue();

            return new RetrieverResponse(usage, "%", type);
        }
    }

    #endregion Retreivers

    #region Other

    public static class Helper
    {
        public static string Repeat(this string stringToRepeat, int repeat)
        {
            var builder = new StringBuilder(repeat * stringToRepeat.Length);
            for (int i = 0; i < repeat; i++)
            {
                builder.Append(stringToRepeat);
            }
            return builder.ToString();
        }
    }

    #endregion Other
}