using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Exceptions
{
    public class ValidationException : Exception
    {
        public object InvalidValue { get; private set; } = new object();

        public ValidationException() : base() { }

        public ValidationException(string message) : base(message) { }

        public ValidationException(string message, object invalidValue) : base(message)
        {
            InvalidValue = invalidValue;
        }

        public ValidationException(string message, Exception inner) : base(message, inner) { }

        public ValidationException(string message, object invalidValue, Exception inner) : base(message, inner)
        {
            InvalidValue = invalidValue;
        }
    }
}
