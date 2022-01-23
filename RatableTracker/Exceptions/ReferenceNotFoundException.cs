using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Framework.Exceptions
{
    public class ReferenceNotFoundException : Exception
    {
        public ReferenceNotFoundException()
        {
        }

        public ReferenceNotFoundException(string message) : base(message)
        {
        }

        public ReferenceNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
