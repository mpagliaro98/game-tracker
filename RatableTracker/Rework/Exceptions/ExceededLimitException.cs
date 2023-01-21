using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Exceptions
{
    public class ExceededLimitException : Exception
    {
        public ExceededLimitException() : base() { }

        public ExceededLimitException(string message) : base(message) { }

        public ExceededLimitException(string message, Exception inner) : base(message, inner) { }
    }
}
