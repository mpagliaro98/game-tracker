using RatableTracker.Rework.ListManipulation;
using RatableTracker.Rework.Modules;
using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Rework
{
    public class FilterPlatforms : FilterBase<Platform>
    {
        public new GameModule Module { get; set; }
        public new SettingsGame Settings { get; set; }

        public FilterPlatforms() : base() { }

        public FilterPlatforms(GameModule module, SettingsGame settings) : base(module, settings) { }

        protected override IList<Platform> ApplyFiltering(IList<Platform> list)
        {
            return base.ApplyFiltering(list);
        }
    }
}
