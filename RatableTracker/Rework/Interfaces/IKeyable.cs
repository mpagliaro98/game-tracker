using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Interfaces
{
    public interface IKeyable
    {
        UniqueID UniqueID { get; }
        int GetHashCode();
        bool Equals(object obj);
    }
}
