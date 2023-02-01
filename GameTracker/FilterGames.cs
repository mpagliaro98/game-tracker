using Newtonsoft.Json;
using RatableTracker.ListManipulation;
using RatableTracker.Model;
using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GameTracker
{
    [Serializable]
    public class FilterGames : FilterRatedObjectStatusCategorical
    {
        public bool ShowCompilations { get; set; } = false;

        private UniqueID _platform = UniqueID.BlankID();
        [XmlIgnore]
        [JsonIgnore]
        public Platform Platform
        {
            get
            {
                if (!_platform.HasValue()) return null;
                return RatableTracker.Util.Util.FindObjectInList(Module.GetPlatformList(), _platform);
            }
            set
            {
                _platform = value == null ? UniqueID.BlankID() : value.UniqueID;
            }
        }

        [XmlIgnore][JsonIgnore] public new GameModule Module { get { return (GameModule)base.Module; } set { base.Module = value; } }
        [XmlIgnore][JsonIgnore] public new SettingsGame Settings { get { return (SettingsGame)base.Settings; } set { base.Settings = value; } }

        public FilterGames() : base() { }

        public FilterGames(GameModule module, SettingsGame settings) : base(module, settings) { }

        protected override IList<RankedObject> ApplyFiltering(IList<RankedObject> list)
        {
            IList<RankedObject> newList = base.ApplyFiltering(list);
            if (ShowCompilations)
                newList = newList.OfType<GameObject>().Where(obj => !obj.IsPartOfCompilation || obj.IsCompilation).Cast<RankedObject>().ToList();
            else
                newList = newList.OfType<GameObject>().Where(obj => !obj.IsCompilation).Cast<RankedObject>().ToList();
            if (Platform != null)
                newList = newList.OfType<GameObject>().Where(obj => obj.Platform != null && obj.Platform.Equals(Platform)).Cast<RankedObject>().ToList();
            return newList;
        }
    }
}
