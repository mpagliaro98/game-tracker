using RatableTracker.Rework.Exceptions;
using RatableTracker.Rework.Model;
using RatableTracker.Rework.Modules;
using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.ListManipulation
{
    public class FilterRankedObjects
    {
        protected readonly Settings settings;

        public FilterRankedObjects(Settings settings)
        {
            this.settings = settings;
        }

        public IList<RankedObject> ApplyFilters(IList<RankedObject> list, TrackerModule module)
        {
            try
            {
                return ApplyFiltering(list, module);
            }
            catch (InvalidCastException e)
            {
                throw new ListManipulationException(GetType().Name + " - Chosen filtering requires all objects in list be a more specific derived type (" + e.GetType().Name + ": " + e.Message + ")");
            }
        }

        protected virtual IList<RankedObject> ApplyFiltering(IList<RankedObject> list, TrackerModule module)
        {
            return list;
        }
    }
}
