using RatableTracker.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Events
{
    public class StatusDeleteArgs : ObjectDeleteArgs
    {
        public ILoadSaveMethodStatusExtension Connection { get; private set; } = null;

        public StatusDeleteArgs(IKeyable deleted, Type type, ILoadSaveMethodStatusExtension connection) : base(deleted, type)
        {
            Connection = connection;
        }
    }
}
