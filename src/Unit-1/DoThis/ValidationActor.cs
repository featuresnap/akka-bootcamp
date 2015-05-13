using System;
using Akka.Actor;

namespace WinTail
{
    public class ValidationActor:UntypedActor
    {
        private IActorRef _consoleWriter;

        public ValidationActor(IActorRef consoleWriter)
        {
            _consoleWriter = consoleWriter;
        }

        protected override void OnReceive(object message)
        {
            var msg = message as string;

            if (string.IsNullOrEmpty(msg))
            {
                _consoleWriter.Tell(new Messages.NullInputError("Please input at least one character."));
            }
            else
            {
                var valid = IsValidInput(msg);

                if (valid)
                {
                    _consoleWriter.Tell(new Messages.InputSuccess("Thank you. Input was valid."));
                    
                }
                else
                {
                    _consoleWriter.Tell(new Messages.ValidationError("Input was invalid. Must contain even number of characters."));
                }
            }

            Sender.Tell(new Messages.ContinueProcessing());
        }

        private bool IsValidInput(string message)
        {
            return message.Length % 2 == 0;
        }
    }
}