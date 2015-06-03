using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace Akka.Bootcamp.Unit1
{
    public class ProccessUsage
    {
        public ProccessUsage()
        {
            var system = ActorSystem.Create("ProcessUsageSystem");

            system.ActorOf<Logger>("Logger");
            system.ActorOf<AggregationActor>("Aggregation");

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

        public override string ToString()
        {
            return string.Format("{0}: {1} {2}", Type, Usage, Unit);
        }

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

        public float Value { get; set; }
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

            foreach (var item in _aggregation)
            {
                result.AppendLine(string.Format("[{0}]:{1}", item.Key, item.Value.Value));
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

                if (_responses.Count > 50)
                {
                    var result = new AggregationResponse();

                    foreach (RetrieverTypes type in Enum.GetValues(typeof(RetrieverTypes)))
                    {
                        result.Stats.Add(type, new AggreagtionDetails()
                        {
                            Type = type,
                            Value = _responses.Where(m => m.Type == type).Average(m => m.Usage)
                        });
                    }
                    _responses.Clear();
                    Sender.Tell(result);
                }
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

    #endregion Other
}