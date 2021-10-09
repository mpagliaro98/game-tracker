using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRater.Model
{
    class IncompleteScoreRangeException : Exception
    {
        public IncompleteScoreRangeException()
        {
        }

        public IncompleteScoreRangeException(string message) : base(message)
        {
        }

        public IncompleteScoreRangeException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
