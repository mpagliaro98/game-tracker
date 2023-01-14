using GameTracker.Model;
using RatableTracker.Framework;
using RatableTracker.List_Manipulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker
{
    public class SortOptionsPlatformTemplate<TListedObj, TModule, TRange, TSettings, TStatus, TRatingCat>
        where TListedObj : RatableGame, new()
        where TModule : RatingModuleGameTemplate<TListedObj, TRange, TSettings, TStatus, TRatingCat>
        where TRange : ScoreRange, new()
        where TSettings : SettingsScore, new()
        where TStatus : CompletionStatus, new()
        where TRatingCat : RatingCategory, new()
    {
        public const int SORT_None = 0;
        public const int SORT_Name = 1;
        public const int SORT_NumGames = 2;
        public const int SORT_Average = 3;
        public const int SORT_Highest = 4;
        public const int SORT_Lowest = 5;
        public const int SORT_PercentFinished = 6;
        public const int SORT_Release = 7;
        public const int SORT_Acquired = 8;

        private readonly int SortMethod = SORT_None;
        private readonly SortMode SortMode = SortMode.ASCENDING;

        public SortOptionsPlatformTemplate(int sortMethod, SortMode sortMode)
        {
            SortMethod = sortMethod;
            SortMode = sortMode;
        }

        public IEnumerable<Platform> ApplySorting(IEnumerable<Platform> list, TModule rm)
        {
            Func<Platform, object> sortFunction = GetSortFunction(SortMethod, rm);
            if (sortFunction == null)
            {
                if (SortMode == SortMode.DESCENDING)
                    list = list.Reverse();
                return list;
            }

            if (SortMode == SortMode.ASCENDING)
                return list.OrderBy(platform => platform.Name.ToLower().StartsWith("the ") ? platform.Name.Substring(4) : platform.Name).OrderBy(sortFunction);
            else if (SortMode == SortMode.DESCENDING)
                return list.OrderBy(platform => platform.Name.ToLower().StartsWith("the ") ? platform.Name.Substring(4) : platform.Name).OrderByDescending(sortFunction);
            else
                throw new Exception("Unhandled sort mode");
        }

        protected virtual Func<Platform, object> GetSortFunction(int sortMethod, TModule rm)
        {
            Func<Platform, object> sortFunction = null;
            switch (sortMethod)
            {
                case SORT_Name:
                    sortFunction = platform => platform.Name.ToLower().StartsWith("the ") ? platform.Name.Substring(4) : platform.Name;
                    break;
                case SORT_NumGames:
                    sortFunction = platform => rm.GetNumGamesByPlatform(platform);
                    break;
                case SORT_Average:
                    sortFunction = platform => rm.GetAverageScoreOfGamesByPlatform(platform);
                    break;
                case SORT_Highest:
                    sortFunction = platform => rm.GetHighestScoreFromGamesByPlatform(platform);
                    break;
                case SORT_Lowest:
                    sortFunction = platform => rm.GetLowestScoreFromGamesByPlatform(platform);
                    break;
                case SORT_PercentFinished:
                    sortFunction = platform => rm.GetPercentageGamesFinishedByPlatform(platform);
                    break;
                case SORT_Release:
                    sortFunction = platform => platform.ReleaseYear;
                    break;
                case SORT_Acquired:
                    sortFunction = platform => platform.AcquiredYear;
                    break;
            }
            return sortFunction;
        }
    }
}
