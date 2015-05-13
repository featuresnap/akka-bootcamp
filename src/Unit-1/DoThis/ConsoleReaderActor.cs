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

        private IActorRef _consoleWriterActor;

        public ConsoleReaderActor(IActorRef consoleWriterActor)
        {
            _consoleWriterActor = consoleWriterActor;
        }

        protected override void OnReceive(object message)
        {
            if (IsStartCommand(message)) {DoPrintInstructions(); }

            GetAndValidateInput();

            Self.Tell(new Messages.ContinueProcessing());
        }

        private void GetAndValidateInput()
        {
            var consoleInput = Console.ReadLine();
            if (string.IsNullOrEmpty(consoleInput))
            {
                _consoleWriterActor.Tell(new Messages.NullInputError("Please enter one or more characters."));
            }
            else if(IsExitCommand(consoleInput)) 
            {
                HandleExit();
            }
            else
            {
                var valid = IsValid(consoleInput);
                if (valid)
                {
                    _consoleWriterActor.Tell(new Messages.InputSuccess("Thank you. Input was valid."));
                }
                else
                {
                    _consoleWriterActor.Tell(new Messages.ValidationError("Sorry. Input was not valid."));
                }
            }
            
        }

        private void HandleExit()
        {
            Context.System.Shutdown();
        }

        private bool IsValid(string input)
        {
            return input.Length%2 == 0;
        }

        private void DoPrintInstructions()
        {
            _consoleWriterActor.Tell(new Messages.PrintInstructions());
        }

        private bool IsStartCommand(object message)
        {
            return StartCommand.Equals(message);
        }

        private static bool IsExitCommand(string consoleInput)
        {
            return !string.IsNullOrEmpty(consoleInput) && String.Equals(consoleInput, ExitCommand, StringComparison.OrdinalIgnoreCase);
        }
    }
}