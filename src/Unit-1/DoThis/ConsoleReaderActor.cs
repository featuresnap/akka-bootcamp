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
            if (IsStartCommand(message)) {DoPrintInstructions(); }

            GetAndValidateInput();
        }

        private void GetAndValidateInput()
        {
            var consoleInput = Console.ReadLine();

            if(IsExitCommand(consoleInput))
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
            Console.WriteLine("Write whatever you want into the console!");
            Console.Write("Some lines will appear as");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(" red ");
            Console.ResetColor();
            Console.Write(" and others will appear as");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" green! ");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Type 'exit' to quit this application at any time.\n");
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