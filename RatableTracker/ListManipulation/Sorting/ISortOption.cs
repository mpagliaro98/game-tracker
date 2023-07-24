using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.ListManipulation.Sorting
{
    public interface ISortOption
    {
        TrackerModule Module { get; set; }
        Settings Settings { get; set; }
        string Name { get; }
        IList<ISortOption> InstantiateManually();
    }
}
