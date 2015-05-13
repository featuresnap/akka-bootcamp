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
            _consoleWriter.Tell(new Messages.NullInputError("Please input at least one character."));
            
        }
    }
}