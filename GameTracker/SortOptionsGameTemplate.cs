using RatableTracker.Framework.ModuleHierarchy;
using RatableTracker.Framework.ObjectHierarchy;
using RatableTracker.Framework;
using RatableTracker.List_Manipulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameTracker.Model;

namespace GameTracker
{
    public class SortOptionsGameTemplate<TListedObj, TModule, TRange, TSettings, TStatus, TRatingCat> : SortOptionsRatableObjStatusCategorical<TListedObj, TModule, TRange, TSettings, TStatus, TRatingCat>
        where TListedObj : RatableGame, new()
        where TModule : RatingModuleGameTemplate<TListedObj, TRange, TSettings, TStatus, TRatingCat>
        where TRange : ScoreRange, new()
        where TSettings : SettingsScore, new()
        where TStatus : CompletionStatus, new()
        where TRatingCat : RatingCategory, new()
    {
        public const int SORT_Platform = 100;
        public const int SORT_PlatformPlayedOn = 101;
        public const int SORT_ReleaseDate = 102;
        public const int SORT_AcquiredOn = 103;
        public const int SORT_StartedOn = 104;
        public const int SORT_FinishedOn = 105;

        public SortOptionsGameTemplate(int sortMethod, SortMode sortMode) : base(sortMethod, sortMode) { }

        protected override Func<TListedObj, object> GetSortFunction(int sortMethod, TModule rm)
        {
            Func<TListedObj, object> sortFunction = base.GetSortFunction(sortMethod, rm);
            switch (sortMethod)
            {
                case SORT_Platform:
                    sortFunction = game => game.RefPlatform.HasReference() ? rm.FindPlatform(game.RefPlatform).Name : "";
                    break;
                case SORT_PlatformPlayedOn:
                    sortFunction = game => game.RefPlatformPlayedOn.HasReference() ? rm.FindPlatform(game.RefPlatformPlayedOn).Name : "";
                    break;
                case SORT_ReleaseDate:
                    sortFunction = game => game.ReleaseDate;
                    break;
                case SORT_AcquiredOn:
                    sortFunction = game => game.AcquiredOn;
                    break;
                case SORT_StartedOn:
                    sortFunction = game => game.StartedOn;
                    break;
                case SORT_FinishedOn:
                    sortFunction = game => game.FinishedOn;
                    break;
            }
            return sortFunction;
        }
    }
}
