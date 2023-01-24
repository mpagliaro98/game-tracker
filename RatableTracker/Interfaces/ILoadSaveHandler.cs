using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Interfaces
{
    // covariant generic interface needed to the module constructors can use derived ILoadSaveMethod types
    public interface ILoadSaveHandler<out T> where T : ILoadSaveMethod
    {
        T NewConnection();
    }
}
