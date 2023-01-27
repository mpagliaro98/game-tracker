using RatableTracker.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Events
{
    public class RatingCategoryDeleteArgs : ObjectDeleteArgs
    {
        public ILoadSaveMethodCategoryExtension Connection { get; private set; } = null;

        public RatingCategoryDeleteArgs(IKeyable deleted, Type type, ILoadSaveMethodCategoryExtension connection) : base(deleted, type)
        {
            Connection = connection;
        }
    }
}
