using RatableTracker.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.LoadSave
{
    public sealed class LoadSaveHandler<T> : ILoadSaveHandler<T> where T : ILoadSaveMethod
    {
        private readonly Func<T> _createMethod;

        public LoadSaveHandler(Func<T> createMethod)
        {
            _createMethod = createMethod;
        }

        public T NewConnection()
        {
            return _createMethod();
        }
    }
}
