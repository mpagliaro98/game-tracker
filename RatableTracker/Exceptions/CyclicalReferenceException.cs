using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Exceptions
{
    public class CyclicalReferenceException : Exception
    {
        public CyclicalReferenceException() : base() { }

        public CyclicalReferenceException(string message) : base(message) { }

        public CyclicalReferenceException(string message, Exception inner) : base(message, inner) { }
    }
}
