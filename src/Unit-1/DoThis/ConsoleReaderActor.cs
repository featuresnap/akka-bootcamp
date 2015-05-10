using System;
using Akka.Actor;

namespace WinTail
{
    /// <summary>
    /// Actor responsible for reading FROM the console. 
    /// Also responsible for calling <see cref="ActorSystem.Shutdown"/>.
    /// </summary>
    public class ConsoleReaderActor : UntypedActor
    {
        public const string ExitCommand = "exit";
        private IActorRef _consoleWriterActor;

        public ConsoleReaderActor(IActorRef consoleWriterActor)
        {
            _consoleWriterActor = consoleWriterActor;
        }

        protected override void OnReceive(object message)
        {
            var consoleInput = Console.ReadLine();
            if (IsExitCommand(consoleInput))
            {
                Context.System.Shutdown();
                return;
            }

            _consoleWriterActor.Tell(consoleInput);
            Self.Tell("continue");
        }

        private static bool IsExitCommand(string consoleInput)
        {
            return !string.IsNullOrEmpty(consoleInput) && String.Equals(consoleInput, ExitCommand, StringComparison.OrdinalIgnoreCase);
        }
    }
}