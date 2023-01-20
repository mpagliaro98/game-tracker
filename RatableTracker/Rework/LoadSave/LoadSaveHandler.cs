using RatableTracker.Rework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.LoadSave
{
    public sealed class LoadSaveHandler<T> : ILoadSaveHandler<T> where T : ILoadSaveMethod
    {
        public delegate T CreateLoadSaveInstance();
        private readonly CreateLoadSaveInstance _createMethod;

        public LoadSaveHandler(CreateLoadSaveInstance createMethod)
        {
            _createMethod = createMethod;
        }

        public T NewConnection()
        {
            return _createMethod();
        }
    }
}
