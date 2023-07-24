using RatableTracker.ListManipulation.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Filtering
{
    [FilterOption(typeof(GameObject))]
    public class FilterOptionGameRankInPlatform : FilterOptionNumericBase<GameObject>
    {
        public override string Name => "Rank within Platform";
        public override FilterNumberFormat NumberFormat => FilterNumberFormat.Integer;

        public FilterOptionGameRankInPlatform() : base() { }

        protected override double GetComparisonValue(GameObject obj)
        {
            return obj.Platform == null ? 0 : ((GameModule)Module).GetRankOfScoreByPlatform(obj.ScoreDisplay, obj.Platform, (SettingsGame)Settings);
        }
    }
}
