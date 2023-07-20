using RatableTracker.ListManipulation;
using RatableTracker.Model;
using RatableTracker.Modules;
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
    public class SortGames : SortRatedObjectStatusCategorical
    {
        public const int SORT_Platform = 100;
        public const int SORT_PlatformPlayedOn = 101;
        public const int SORT_ReleaseDate = 102;
        public const int SORT_AcquiredOn = 103;
        public const int SORT_StartedOn = 104;
        public const int SORT_FinishedOn = 105;

        [XmlIgnore][JsonIgnore] public new GameModule Module { get { return (GameModule)base.Module; } set { base.Module = value; } }
        [XmlIgnore][JsonIgnore] public new SettingsGame Settings { get { return (SettingsGame)base.Settings; } set { base.Settings = value; } }

        public SortGames() : base() { }

        public SortGames(GameModule module, SettingsGame settings) : base(module, settings) { }

        protected override Func<RankedObject, object> GetSortFunction(int sortMethod)
        {
            Func<RankedObject, object> sortFunction = base.GetSortFunction(sortMethod);
            switch (sortMethod)
            {
                case SORT_Platform:
                    sortFunction = obj => ((GameObject)obj).Platform == null ? "" : ((GameObject)obj).Platform.Name;
                    break;
                case SORT_PlatformPlayedOn:
                    sortFunction = obj => ((GameObject)obj).PlatformPlayedOn == null ? "" : ((GameObject)obj).PlatformPlayedOn.Name;
                    break;
                case SORT_ReleaseDate:
                    sortFunction = obj => ((GameObject)obj).ReleaseDate;
                    break;
                case SORT_AcquiredOn:
                    sortFunction = obj => ((GameObject)obj).AcquiredOn;
                    break;
                case SORT_StartedOn:
                    sortFunction = obj => ((GameObject)obj).StartedOn;
                    break;
                case SORT_FinishedOn:
                    sortFunction = obj => ((GameObject)obj).IsUnfinishable ? ((GameObject)obj).StartedOn : ((GameObject)obj).FinishedOn;
                    break;
            }
            return sortFunction;
        }
    }
}
