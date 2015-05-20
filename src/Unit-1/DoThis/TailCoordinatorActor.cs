using System;
using System.Threading;
using Akka.Actor;

namespace WinTail
{
    public class TailCoordinatorActor:UntypedActor
    {
        #region Messages
        public class StartTail
        {
            public readonly string FileUri;
            public readonly IActorRef ReporterActor;

            public StartTail(string fileUri, IActorRef reporterActor)
            {
                ReporterActor = reporterActor;
                FileUri = fileUri;
            }
        }

        public class StopTail
        {
            public readonly string FilePath;

            public StopTail(string filePath)
            {
                FilePath = filePath;
            }
        }

        #endregion

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(10, 
                TimeSpan.FromSeconds(30),
                x =>
                {
                    if (x is ArithmeticException) { return Directive.Resume;}
                    if (x is NotSupportedException) { return Directive.Stop;}
                    return Directive.Restart;
                });
        }

        protected override void OnReceive(object message)
        {
            if (message is StartTail)
            {
                var msg = message as StartTail;
                Context.ActorOf(Props.Create(() => new TailActor(msg.ReporterActor, msg.FileUri)));
            }
        }
    }
}