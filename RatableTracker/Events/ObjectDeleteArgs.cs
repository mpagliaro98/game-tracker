using RatableTracker.Interfaces;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Events
{
    public abstract class ObjectDeleteArgs
    {
        public IKeyable DeletedObject { get; private set; } = null;
        public Type ObjectType { get; private set; } = null;

        public ObjectDeleteArgs(IKeyable deleted, Type type)
        {
            DeletedObject = deleted;
            ObjectType = type;
        }
    }
}
