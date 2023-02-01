using RatableTracker.ListManipulation;
using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GameTracker
{
    [Serializable]
    public class FilterPlatforms : FilterBase<Platform>
    {
        [XmlIgnore][JsonIgnore] public new GameModule Module { get { return (GameModule)base.Module; } set { base.Module = value; } }
        [XmlIgnore][JsonIgnore] public new SettingsGame Settings { get { return (SettingsGame)base.Settings; } set { base.Settings = value; } }

        public FilterPlatforms() : base() { }

        public FilterPlatforms(GameModule module, SettingsGame settings) : base(module, settings) { }

        protected override IList<Platform> ApplyFiltering(IList<Platform> list)
        {
            return base.ApplyFiltering(list);
        }
    }
}
