using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Framework.Interfaces
{
    public interface IReferable
    {
        Guid ReferenceKey { get; }
    }
}
