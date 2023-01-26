using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Exceptions
{
    public class NoDataFoundException : Exception
    {
        public NoDataFoundException() : base() { }

        public NoDataFoundException(string message) : base(message) { }

        public NoDataFoundException(string message, Exception inner) : base(message, inner) { }
    }
}
