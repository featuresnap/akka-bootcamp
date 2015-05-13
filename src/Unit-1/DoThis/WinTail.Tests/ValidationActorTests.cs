using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.TestKit.NUnit;
using NUnit.Framework;

namespace WinTail.Tests
{
    [TestFixture]
    public class ValidationActorTests:TestKit
    {
        [Test]
        public void CanCreateValidationActor()
        {
            var actor = Sys.ActorOf(Props.Create(() => new ValidationActor(TestActor)));
        }

        [TestCase(null)]
        [TestCase("")]
        public void EmptyInput_Sends_NullInputError_To_ConsoleWriterActor(string input)
        {
            var consoleWriter = TestActor;
            var actor = Sys.ActorOf(Props.Create<ValidationActor>(consoleWriter));
            actor.Tell(input);
            ExpectMsg<Messages.NullInputError>();
        }

        
       

    }
}
