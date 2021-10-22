using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Framework.Exceptions
{
    class ScoreOutOfRangeException : Exception
    {
        public ScoreOutOfRangeException()
        {
        }

        public ScoreOutOfRangeException(string message) : base(message)
        {
        }

        public ScoreOutOfRangeException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
