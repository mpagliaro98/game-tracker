using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Framework.Interfaces
{
    public partial interface IValueContainer<TValCont>
        where TValCont : IValueContainer<TValCont>, new()
    {
        string ConvertValueToJSON();
    }
}
