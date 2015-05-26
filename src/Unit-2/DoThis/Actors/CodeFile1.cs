using System.IO;
using Akka.Actor;

namespace ChartApp.Actors
{

    #region Reporting

    public class GatherMetrics
    {
    }

    public class Metric
    {
        public readonly string Series;
        public readonly float CounterValue;

        public Metric(string series, float counterValue)
        {
            CounterValue = counterValue;
            Series = series;
        }
    }

    #endregion

    #region Performance Counter Management

    public enum CounterType
    {
        Cpu,
        Memory,
        Disk
    }

    public class SubscribeCounter
    {
        public readonly CounterType Counter;
        public readonly IActorRef Subscriber;

        public SubscribeCounter(CounterType counter, IActorRef subscriber)
        {
            Subscriber = subscriber;
            Counter = counter;
        }
    }
    public class UnsubscribeCounter
    {
        public readonly CounterType Counter;
        public readonly IActorRef Subscriber;

        public UnsubscribeCounter(CounterType counter, IActorRef subscriber)
        {
            Subscriber = subscriber;
            Counter = counter;
        }
    }

    #endregion
}