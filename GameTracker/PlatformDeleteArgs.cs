using RatableTracker.Events;
using RatableTracker.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker
{
    public class PlatformDeleteArgs : ObjectDeleteArgs
    {
        public ILoadSaveMethodGame Connection { get; private set; } = null;

        public PlatformDeleteArgs(IKeyable deleted, Type type, ILoadSaveMethodGame connection) : base(deleted, type)
        {
            Connection = connection;
        }
    }
}
