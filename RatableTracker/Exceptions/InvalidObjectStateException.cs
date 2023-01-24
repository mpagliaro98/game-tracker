using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Exceptions
{
    public class InvalidObjectStateException : Exception
    {
        public InvalidObjectStateException() : base() { }

        public InvalidObjectStateException(string message) : base(message) { }

        public InvalidObjectStateException(string message, Exception inner) : base(message, inner) { }
    }
}
