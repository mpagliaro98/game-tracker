using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Interfaces;

namespace RatableTracker.Framework.ObjectHierarchy
{
    public interface IStatus
    {
        ObjectReference RefStatus { get; }
        void SetStatus<T>(T obj) where T : Status, IReferable;
        void RemoveStatus();
    }
}
