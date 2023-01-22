using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.ListManipulation
{
    public class FilterRatedObjects : FilterRankedObjects
    {
        protected readonly new SettingsScore settings;

        public FilterRatedObjects(SettingsScore settings) : base(settings) { }
    }
}
