using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Framework.Exceptions
{
    class ExceededLimitException : Exception
    {
        public ExceededLimitException()
        {
        }

        public ExceededLimitException(string message) : base(message)
        {
        }

        public ExceededLimitException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
