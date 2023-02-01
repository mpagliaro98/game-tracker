using RatableTracker.Exceptions;
using RatableTracker.Model;
using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RatableTracker.ListManipulation
{
    [Serializable]
    public class FilterBase<T>
    {
        [XmlIgnore][JsonIgnore] public TrackerModule Module { get; set; }
        [XmlIgnore][JsonIgnore] public Settings Settings { get; set; }

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
