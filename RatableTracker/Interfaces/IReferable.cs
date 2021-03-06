using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Framework.Interfaces
{
    public interface IReferable
    {
        ObjectReference ReferenceKey { get; }
        void OverwriteReferenceKey(IReferable orig);
        int GetHashCode();
        bool Equals(object obj);
    }
}
