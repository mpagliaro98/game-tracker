using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.ListManipulation.Filtering
{
    public interface IFilterOption
    {
        TrackerModule Module { get; set; }
        Settings Settings { get; set; }
        string Name { get; }
        FilterType FilterType { get; }
        IList<IFilterOption> InstantiateManually();
    }
}
