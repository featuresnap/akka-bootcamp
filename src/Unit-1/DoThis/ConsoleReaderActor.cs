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
        public const string StartCommand = "start";

        private IActorRef _validationActor;

        public ConsoleReaderActor(IActorRef validationActor)
        {
            _validationActor = validationActor;
        }

        protected override void OnReceive(object message)
        {
            if (IsStartCommand(message)) { DoPrintInstructions(); }

            GetAndValidateInput();
        }

        private void GetAndValidateInput()
        {
            var consoleInput = Console.ReadLine();

            if (IsExitCommand(consoleInput))
            {
                Context.System.Shutdown();
            }
            else
            {
                _validationActor.Tell(consoleInput);
            }
        }

        private void DoPrintInstructions()
        {
            Console.WriteLine("Enter the uri of a log file on disk.\n");
        }

        private bool IsStartCommand(object message)
        {
            return StartCommand.Equals(message);
        }

        private static bool IsExitCommand(string consoleInput)
        {
            return !String.IsNullOrEmpty(consoleInput) && String.Equals(consoleInput, ExitCommand, StringComparison.OrdinalIgnoreCase);
        }
    }
}