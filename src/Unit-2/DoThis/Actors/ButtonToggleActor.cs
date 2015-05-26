using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Akka.Actor;

namespace ChartApp.Actors
{
    public class ButtonToggleActor:UntypedActor
    {

        #region Messages

        public class Toggle
        {
        }

        #endregion

        private CounterType _myCounterType;
        private bool _isToggledOn;
        private readonly Button _myButton;
        private readonly IActorRef _coordinatorActor;

        public ButtonToggleActor(IActorRef coordinatorActor, Button myButton,
            CounterType myCounterType, bool isToggledOn =false)
        {
            _coordinatorActor = coordinatorActor;
            _myButton = myButton;
            _myCounterType = myCounterType;
            _isToggledOn = isToggledOn;
        }
        protected override void OnReceive(object message)
        {
            if (message is Toggle && _isToggledOn)
            {
                _coordinatorActor.Tell(new PerformanceCounterCoordinatorActor.Unwatch(_myCounterType));
                FlipToggle();
            }

            else if (message is Toggle && !_isToggledOn)
            {
                _coordinatorActor.Tell(new PerformanceCounterCoordinatorActor.Watch(_myCounterType));
                FlipToggle();
            }

            else { Unhandled(message);}
        }

        private void FlipToggle()
        {
            _isToggledOn = !_isToggledOn;
            _myButton.Text = string.Format("{0} ({1})", _myCounterType.ToString().ToUpperInvariant(),
                _isToggledOn ? "ON" : "OFF");

        }
    }
}
