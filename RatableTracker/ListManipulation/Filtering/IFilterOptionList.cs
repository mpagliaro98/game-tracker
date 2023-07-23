using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.ListManipulation.Filtering
{
    public interface IFilterOptionList : IFilterOption
    {
        List<KeyValuePair<UniqueID, string>> ListValues { get; }
    }
}
