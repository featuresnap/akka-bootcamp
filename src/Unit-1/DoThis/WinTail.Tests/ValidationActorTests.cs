using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Dispatch.SysMsg;
using Akka.TestKit;
using Akka.TestKit.NUnit;
using NUnit.Framework;

namespace WinTail.Tests
{
    [TestFixture]
    public class ValidationActorTests:TestKit
    {
        private IActorRef _consoleWriter;
        private IActorRef _validationActor;

        [SetUp]
        public void SetUp()
        {
            _consoleWriter = TestActor;
            _validationActor = Sys.ActorOf(Props.Create<ValidationActor>(_consoleWriter));
        }


        [TestCase(null)]
        [TestCase("")]
        public void EmptyInput_Sends_NullInputError_To_ConsoleWriterActor(string input)
        {
            _validationActor.Tell(input);
         
            ExpectMsg<Messages.NullInputError>();
        }

        [Test]
        public void ValidInput_Sends_InputSuccess_To_ConsoleWriterActor()
        {
            var validInput = "abcd";
            _validationActor.Tell(validInput);
            ExpectMsg<Messages.InputSuccess>();
        }

        [Test]
        public void InvalidInput_Sends_ValidationError_To_ConsoleWriterActor()
        {
            var invalidInput = "x";
            _validationActor.Tell(invalidInput);
            ExpectMsg<Messages.ValidationError>();
        }

        //[Test]
        //public async Task  ValidationActor_Tells_Sender_ContinueProcessing()
        //{
        //    var input = "f";

        //    var senderResponse =  await _validationActor.Ask("Foo", TimeSpan.FromSeconds(3.0));

        //    Assert.That(senderResponse, Is.InstanceOf<Messages.ContinueProcessing>());
        //}





    }
}
