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
    public class FilterBase<T>
    {
        public TrackerModule Module { get; set; }
        public Settings Settings { get; set; }

        public FilterBase()
        {
            Module = null;
            Settings = null;
        }

        public FilterBase(TrackerModule module, Settings settings)
        {
            Module = module;
            Settings = settings;
        }

        public IList<T> ApplyFilters(IList<T> list)
        {
            try
            {
                return ApplyFiltering(list);
            }
            catch (InvalidCastException e)
            {
                throw new ListManipulationException(GetType().Name + " - Chosen filtering requires all objects in list be a more specific derived type (" + e.GetType().Name + ": " + e.Message + ")");
            }
        }

        protected virtual IList<T> ApplyFiltering(IList<T> list)
        {
            return list;
        }
    }
}
