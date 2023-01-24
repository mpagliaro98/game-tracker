using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Exceptions
{
    public class ListManipulationException : ValidationException
    {
        public ListManipulationException() : base() { }

        public ListManipulationException(string message) : base(message) { }

        public ListManipulationException(string message, object invalidValue) : base(message, invalidValue) { }

        public ListManipulationException(string message, object invalidValue, Exception inner) : base(message, invalidValue, inner) { }
    }
}
