using RatableTracker.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Events
{
    public class SettingsChangeArgs
    {
        public Type SettingsType { get; init; } = null;
        public ILoadSaveMethod Connection { get; init; } = null;

        public SettingsChangeArgs(Type type, ILoadSaveMethod connection)
        {
            SettingsType = type;
            Connection = connection;
        }
    }
}
