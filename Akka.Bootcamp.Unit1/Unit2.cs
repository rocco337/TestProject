using Akka.Actor;
using Akka.Bootcamp.Unit1;
using Akka.Util.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;

namespace Akka.Bootcamp
{
    public class Unit2
    {
        private IActorRef _chartActor;
        private readonly AtomicCounter _seriesCounter = new AtomicCounter(1);

        public Unit2()
        {
            ActorSystem ChartActors = ActorSystem.Create("ChartActors");

            var sysChart = new Chart();

            _chartActor = ChartActors.ActorOf(Props.Create(() => new ChartingActor(sysChart)), "charting");
            var series = new Series();

            _chartActor.Tell(new ChartingActor.InitializeChart(new Dictionary<string, Series>()
            {
                { series.Name, series}
            }));
        }
    }

    public class Series
    {
        public string Name { get; set; }
    }

    public class Chart
    {
        public List<Series> Series { get; set; }
    }

    public class ChartingActor : UntypedActor
    {
        #region Messages

        public class InitializeChart
        {
            public InitializeChart(Dictionary<string, Series> initialSeries)
            {
                InitialSeries = initialSeries;
            }

            public Dictionary<string, Series> InitialSeries { get; private set; }
        }

        #endregion Messages

        private readonly Chart _chart;
        private Dictionary<string, Series> _seriesIndex;

        public ChartingActor(Chart chart)
            : this(chart, new Dictionary<string, Series>())
        {
        }

        public ChartingActor(Chart chart, Dictionary<string, Series> seriesIndex)
        {
            _chart = chart;
            _seriesIndex = seriesIndex;
        }

        protected override void OnReceive(object message)
        {
            if (message is InitializeChart)
            {
                var ic = message as InitializeChart;
                HandleInitialize(ic);
            }
        }

        #region Individual Message Type Handlers

        private void HandleInitialize(InitializeChart ic)
        {
            if (ic.InitialSeries != null)
            {
                //swap the two series out
                _seriesIndex = ic.InitialSeries;
            }

            //delete any existing series
            _chart.Series.Clear();

            //attempt to render the initial chart
            if (_seriesIndex.Any())
            {
                foreach (var series in _seriesIndex)
                {
                    //force both the chart and the internal index to use the same names
                    series.Value.Name = series.Key;
                    _chart.Series.Add(series.Value);
                }
            }
        }

        #endregion Individual Message Type Handlers
    }
}