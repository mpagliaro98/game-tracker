using RatableTracker.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Events
{
    public class ScoreRangeDeleteArgs : ObjectDeleteArgs
    {
        public ILoadSaveMethodScores Connection { get; private set; } = null;

        public ScoreRangeDeleteArgs(IKeyable deleted, Type type, ILoadSaveMethodScores connection) : base(deleted, type)
        {
            Connection = connection;
        }
    }
}
