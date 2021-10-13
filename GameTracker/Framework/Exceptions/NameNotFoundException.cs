using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Framework.Exceptions
{
    public class NameNotFoundException : Exception
    {
        public NameNotFoundException()
        {
        }

        public NameNotFoundException(string message) : base(message)
        {
        }

        public NameNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
