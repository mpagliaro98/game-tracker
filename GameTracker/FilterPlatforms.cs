using RatableTracker.ListManipulation;
using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker
{
    public class FilterPlatforms : FilterBase<Platform>
    {
        public new GameModule Module { get { return (GameModule)base.Module; } set { base.Module = value; } }
        public new SettingsGame Settings { get { return (SettingsGame)base.Settings; } set { base.Settings = value; } }

        public FilterPlatforms() : base() { }

        public FilterPlatforms(GameModule module, SettingsGame settings) : base(module, settings) { }

        protected override IList<Platform> ApplyFiltering(IList<Platform> list)
        {
            return base.ApplyFiltering(list);
        }
    }
}
