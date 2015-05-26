using System;
using System.Collections.Generic;
using System.Diagnostics;
using Akka.Actor;

namespace ChartApp.Actors
{
    public class PerformanceCounterActor:UntypedActor
    {
        private string _seriesName;
        private Func<PerformanceCounter> _performanceCounterGenerator;
        private PerformanceCounter _performanceCounter;
        private HashSet<IActorRef> _subscriptions;
        private ICancelable _cancelPublishing;
        private readonly TimeSpan _refreshInterval = TimeSpan.FromMilliseconds(250);

        public PerformanceCounterActor(string seriesName, Func<PerformanceCounter> performanceCounterGenerator )
        {
            _seriesName = seriesName;
            _performanceCounterGenerator = performanceCounterGenerator;
            _subscriptions = new HashSet<IActorRef>();
            _cancelPublishing = new Cancelable(Context.System.Scheduler);
        }

        #region Actor Lifecycle Methods

        protected override void PreStart()
        {
            _performanceCounter = _performanceCounterGenerator();
            Context.System.Scheduler.ScheduleTellRepeatedly(_refreshInterval, _refreshInterval,
                Self, new GatherMetrics(), Self);
        }

        protected override void PostStop()
        {
            try
            {
                _cancelPublishing.Cancel();
                _performanceCounter.Dispose();
            }
            catch 
            {}
            finally
            {
                base.PostStop();
            }
        }

        #endregion
        protected override void OnReceive(object message)
        {
            if (message is GatherMetrics)
            {
                var metric = new Metric(_seriesName, _performanceCounter.NextValue());
                foreach (var sub in _subscriptions)
                {
                    sub.Tell(metric );
                }
            }

            else if (message is SubscribeCounter)
            {
                var sc = message as SubscribeCounter;
                _subscriptions.Add(sc.Subscriber);
            }

            else if (message is UnsubscribeCounter)
            {
                var uc = message as UnsubscribeCounter;
                _subscriptions.Remove(uc.Subscriber);
            }
        }
    }
}
