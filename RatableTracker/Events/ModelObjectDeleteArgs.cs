using RatableTracker.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Events
{
    public class ModelObjectDeleteArgs : ObjectDeleteArgs
    {
        public ILoadSaveMethod Connection { get; private set; } = null;

        public ModelObjectDeleteArgs(IKeyable deleted, Type type, ILoadSaveMethod connection) : base(deleted, type)
        {
            Connection = connection;
        }
    }
}
