using RatableTracker.Rework.ListManipulation;
using RatableTracker.Rework.Model;
using RatableTracker.Rework.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Rework
{
    public class SortGames : SortRatedObjectStatusCategorical
    {
        public const int SORT_Platform = 100;
        public const int SORT_PlatformPlayedOn = 101;
        public const int SORT_ReleaseDate = 102;
        public const int SORT_AcquiredOn = 103;
        public const int SORT_StartedOn = 104;
        public const int SORT_FinishedOn = 105;

        protected readonly new SettingsGame settings;

        public SortGames(SettingsGame settings) : base(settings) { }

        protected override Func<RankedObject, object> GetSortFunction(int sortMethod, TrackerModule module)
        {
            Func<RankedObject, object> sortFunction = base.GetSortFunction(sortMethod, module);
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
                    sortFunction = obj => ((GameObject)obj).FinishedOn;
                    break;
            }
            return sortFunction;
        }
    }
}
