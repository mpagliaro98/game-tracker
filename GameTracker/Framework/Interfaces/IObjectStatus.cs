using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Framework.Interfaces
{
    public interface IObjectStatus
    {
        ObjectReference RefStatus { get; }
        void SetStatus<T>(T obj) where T : Status, IReferable;
        void RemoveStatus();
    }
}
