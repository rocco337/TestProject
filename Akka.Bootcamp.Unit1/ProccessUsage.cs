using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;

namespace Akka.Bootcamp.Unit1
{
    public class ProccessUsage
    {
        public ProccessUsage()
        {
            var system = ActorSystem.Create("ProcessUsageSystem");

            system.ActorOf<Logger>("Logger");
            var aggregationActor = system.ActorOf<AggregationActor>("Aggregation");

            var uiNotifier = system.ActorOf<UiNotifierActor>("NotifyActor");
            var resourceRetrieverActor = system.ActorOf<ResourceRetrieverActor>("ResourceRetrieverActor");

            var start = TimeSpan.FromMilliseconds(0);
            var interval = TimeSpan.FromMilliseconds(150);

            foreach (RetrieverTypes type in Enum.GetValues(typeof(RetrieverTypes)))
            {
                if (type == RetrieverTypes.NetworkSpeed)
                {
                    interval = TimeSpan.FromMilliseconds(1000);
                }

                system.Scheduler.ScheduleTellRepeatedly(start, interval, resourceRetrieverActor, type, uiNotifier);
            }

            system.Scheduler.ScheduleTellRepeatedly(start, TimeSpan.FromMilliseconds(2000), aggregationActor, new AggregationRequest(), uiNotifier);

            system.AwaitTermination();
        }
    }

    #region Contracts

    public enum RetrieverTypes
    {
        Cpu,
        Memory,
        Hdd,
        NetworkSpeed
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

        public RetrieverResponse(float usage, string unit, RetrieverTypes type)
        {
            Usage = usage;
            Unit = unit;
            Type = type;
        }
    }

    public class AggregationResponse
    {
        public Dictionary<RetrieverTypes, AggreagtionDetails> Stats { get; set; }

        public AggregationResponse()
        {
            Stats = new Dictionary<RetrieverTypes, AggreagtionDetails>();
        }
    }

    public class AggreagtionDetails
    {
        public RetrieverTypes Type { get; set; }

        public float Avg { get; set; }

        public float Min { get; set; }

        public float Max { get; set; }
    }

    public class AggregationRequest
    {
    }

    #endregion Contracts

    #region actors

    public class UiNotifierActor : ReceiveActor
    {
        private static Dictionary<RetrieverTypes, RetrieverResponse> _dashboard = new Dictionary<RetrieverTypes, RetrieverResponse>();
        private static Dictionary<RetrieverTypes, AggreagtionDetails> _aggregation = new Dictionary<RetrieverTypes, AggreagtionDetails>();

        public UiNotifierActor()
        {
            Receive<RetrieverResponse>((response) =>
            {
                NotifyDashboard(response);

                Context.ActorSelection("../Aggregation").Tell(response);
                Context.ActorSelection("../Logger").Tell(response);
            });

            Receive<AggregationResponse>((response) =>
            {
                _aggregation = response.Stats;
                Context.ActorSelection("../Logger").Tell(response);
            });
        }

        public static void DrawDashboard()
        {
            var result = new StringBuilder();
            result.AppendLine(string.Empty);
            result.AppendLine("======================= AGGREGATED ========================");
            foreach (var item in _aggregation)
            {
                var avg = item.Value.Avg.ToString("n2");
                var min = item.Value.Min.ToString("n2");
                var max = item.Value.Max.ToString("n2");

                result.AppendLine(string.Format("[{0,12}] - Avg: {1,7} - Max: {2,7} ", item.Key, avg, max));
            }

            result.AppendLine(string.Empty);
            result.AppendLine("======================= REALTIME ==========================");
            result.AppendLine(string.Empty);
            foreach (var dashboardItem in _dashboard)
            {
                var dotNumber = (int)(dashboardItem.Value.Usage * 0.5);
                var usage = dashboardItem.Value.Usage.ToString("n2");
                var percentage = string.Format("{0,5} {1}", usage, dashboardItem.Value.Unit);

                result.AppendLine(string.Format("[{0,12}]: {1,50} {2}", dashboardItem.Key, ".".Repeat(dotNumber), percentage));
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

    public class AggregationActor : ReceiveActor
    {
        private List<RetrieverResponse> _responses = new List<RetrieverResponse>();

        public AggregationActor()
        {
            Receive<RetrieverResponse>((response) =>
            {
                _responses.Add(response);
            });

            Receive<AggregationRequest>((request) =>
            {
                var responesToAggregate = _responses.ToArray();

                var result = new AggregationResponse();

                foreach (RetrieverTypes type in Enum.GetValues(typeof(RetrieverTypes)))
                {
                    var usagesForType = responesToAggregate.Where(m => m.Type == type).ToList();
                    result.Stats.Add(type, new AggreagtionDetails()
                    {
                        Type = type,
                        Avg = usagesForType.Any() ? usagesForType.Average(m => m.Usage) : 0,
                        Max = usagesForType.Any() ? usagesForType.Max(m => m.Usage) : 0
                    });
                }

                Sender.Tell(result);
            });
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

                case RetrieverTypes.NetworkSpeed:
                    return new PingRetreiver();

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
            Int64 phav = PerformanceInfo.GetPhysicalAvailableMemoryInMiB();
            Int64 tot = PerformanceInfo.GetTotalMemoryInMiB();
            decimal percentFree = ((decimal)phav / (decimal)tot) * 100;
            int percentOccupied = (int)(100 - percentFree);

            return new RetrieverResponse(percentOccupied, "%", type);
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

    public class PingRetreiver : IResourceRetriever
    {
        public RetrieverResponse GetUsage(RetrieverTypes type)
        {
            Ping pingSender = new Ping();
            IPAddress address = IPAddress.Parse("8.8.8.8");
            PingReply reply = pingSender.Send(address);

            if (reply.Status == IPStatus.Success)
            {
                return new RetrieverResponse(reply.RoundtripTime, "ms", RetrieverTypes.NetworkSpeed);
            }

            return new RetrieverResponse(0, "ms", RetrieverTypes.NetworkSpeed);
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

    public static class PerformanceInfo
    {
        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPerformanceInfo([Out] out PerformanceInformation PerformanceInformation, [In] int Size);

        [StructLayout(LayoutKind.Sequential)]
        public struct PerformanceInformation
        {
            public int Size;
            public IntPtr CommitTotal;
            public IntPtr CommitLimit;
            public IntPtr CommitPeak;
            public IntPtr PhysicalTotal;
            public IntPtr PhysicalAvailable;
            public IntPtr SystemCache;
            public IntPtr KernelTotal;
            public IntPtr KernelPaged;
            public IntPtr KernelNonPaged;
            public IntPtr PageSize;
            public int HandlesCount;
            public int ProcessCount;
            public int ThreadCount;
        }

        public static Int64 GetPhysicalAvailableMemoryInMiB()
        {
            PerformanceInformation pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToInt64((pi.PhysicalAvailable.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            }
            else
            {
                return -1;
            }

        }

        public static Int64 GetTotalMemoryInMiB()
        {
            PerformanceInformation pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToInt64((pi.PhysicalTotal.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            }
            else
            {
                return -1;
            }

        }
    }


    #endregion Other
}