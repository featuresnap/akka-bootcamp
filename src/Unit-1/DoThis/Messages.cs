using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTail
{
    public class Messages
    {
        #region Neutral / System Messages

        public class ContinueProcessing{}

        public class PrintInstructions{}
 
        #endregion

        #region Success Messages

        public class InputSuccess
        {
            public string Reason { get; private set; }

            public InputSuccess(string reason)
            {
                Reason = reason;
            }
        }

        #endregion

        #region Failure Messages
        /// <summary>
        /// Base class for failure messages.
        /// </summary>
        public abstract class InputError
        {
            public string Reason { get; private set; }

            public InputError(string reason)
            {
                Reason = reason;
            }
        }
        /// <summary>
        /// User provided blank input.
        /// </summary>
        public class NullInputError : InputError
        {
            public NullInputError(string reason) : base(reason) { }
        }

        /// <summary>
        /// User provided invalid input.
        /// </summary>
        public class ValidationError : InputError
        {
            public ValidationError(string reason) : base(reason) { }
        }

        #endregion
    }
}
