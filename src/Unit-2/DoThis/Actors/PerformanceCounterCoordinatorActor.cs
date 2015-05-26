using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;
using Akka.Actor;

namespace ChartApp.Actors
{
    public class PerformanceCounterCoordinatorActor : ReceiveActor
    {
        #region Messages

        public class Watch
        {
            public readonly CounterType Counter;

            public Watch(CounterType counter)
            {
                Counter = counter;
            }
        }

        public class Unwatch
        {
            public readonly CounterType Counter;

            public Unwatch(CounterType counter)
            {
                Counter = counter;
            }
        }

        #endregion

        #region Counter Generators
        private static readonly Dictionary<CounterType, Func<PerformanceCounter>> _counterGenerators =
            new Dictionary<CounterType, Func<PerformanceCounter>>
        {
            {CounterType.Cpu, ()=> new PerformanceCounter("Processor", "% Processor Time", "_Total", true)},
            {CounterType.Memory, ()=> new PerformanceCounter("Memory", "% Committed Bytes In Use", true)},
            {CounterType.Disk, () => new PerformanceCounter("LogicalDisk", "% Disk Time","_Total", true)}
        };

        public static readonly Dictionary<CounterType, Func<Series>> _counterSeries = new Dictionary<CounterType, Func<Series>>
        {
            {CounterType.Cpu, () => new Series(CounterType.Cpu.ToString()){ChartType = SeriesChartType.SplineArea, Color = Color.DarkGreen}},
            {CounterType.Memory, () => new Series(CounterType.Memory.ToString()){ChartType = SeriesChartType.FastLine, Color = Color.MediumBlue}},
            {CounterType.Disk, () => new Series(CounterType.Disk.ToString()){ChartType = SeriesChartType.SplineArea, Color=Color.DarkRed}}
        };
        #endregion

        private Dictionary<CounterType, IActorRef> _counterActors;

        private IActorRef _chartingActor;

        public PerformanceCounterCoordinatorActor(IActorRef chartingActor)
            : this(chartingActor, new Dictionary<CounterType, IActorRef>())
        {

        }

        public PerformanceCounterCoordinatorActor(IActorRef chartingActor, Dictionary<CounterType, IActorRef> counterActors)
        {
            _chartingActor = chartingActor;
            _counterActors = counterActors;

            Receive<Watch>(watch =>
            {
                if (!_counterActors.ContainsKey(watch.Counter))
                {
                    var counterActor = Context.ActorOf(Props.Create(() =>
                         new PerformanceCounterActor(watch.Counter.ToString(),
                                _counterGenerators[watch.Counter])));
                    _counterActors[watch.Counter] = counterActor;
                }

                _chartingActor.Tell(new ChartingActor.AddSeries(_counterSeries[watch.Counter]()));
                _counterActors[watch.Counter].Tell(new SubscribeCounter(watch.Counter, _chartingActor));
            });

            Receive<Unwatch>(unwatch =>
            {
                if (!_counterActors.ContainsKey(unwatch.Counter))
                {
                    return;
                }

                _counterActors[unwatch.Counter].Tell(new UnsubscribeCounter(unwatch.Counter, _chartingActor));
                _chartingActor.Tell(new ChartingActor.RemoveSeries(unwatch.Counter.ToString()));
            
            });
        }
    }
}
