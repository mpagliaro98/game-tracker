using RatableTracker.ListManipulation;
using RatableTracker.Model;
using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker
{
    public class SortPlatforms : SortBase<Platform>
    {
        public const int SORT_Name = 1;
        public const int SORT_NumGames = 2;
        public const int SORT_Average = 3;
        public const int SORT_Highest = 4;
        public const int SORT_Lowest = 5;
        public const int SORT_PercentFinished = 6;
        public const int SORT_Release = 7;
        public const int SORT_Acquired = 8;

        public new GameModule Module { get { return (GameModule)base.Module; } set { base.Module = value; } }
        public new SettingsGame Settings { get { return (SettingsGame)base.Settings; } set { base.Settings = value; } }

        public SortPlatforms() : base() { }

        public SortPlatforms(GameModule module, SettingsGame settings) : base(module, settings) { }

        protected override Func<Platform, object> GetSortFunction(int sortMethod)
        {
            Func<Platform, object> sortFunction = base.GetSortFunction(sortMethod);
            switch (sortMethod)
            {
                case SORT_Name:
                    sortFunction = platform => platform.Name.CleanForSorting();
                    break;
                case SORT_NumGames:
                    sortFunction = platform => Module.GetGamesOnPlatform(platform, Settings).Count();
                    break;
                case SORT_Average:
                    sortFunction = platform => Module.GetFinishedGamesOnPlatform(platform, Settings).Select(obj => obj.ScoreDisplay).AverageIfEmpty(Settings.MinScore);
                    break;
                case SORT_Highest:
                    sortFunction = platform => Module.GetFinishedGamesOnPlatform(platform, Settings).Select(obj => obj.ScoreDisplay).Max();
                    break;
                case SORT_Lowest:
                    sortFunction = platform => Module.GetFinishedGamesOnPlatform(platform, Settings).Select(obj => obj.ScoreDisplay).Min();
                    break;
                case SORT_PercentFinished:
                    sortFunction = platform => Module.GetProportionGamesFinishedByPlatform(platform, Settings);
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

        protected override Func<Platform, object> DefaultSort()
        {
            return obj => obj.Name.CleanForSorting();
        }
    }
}
