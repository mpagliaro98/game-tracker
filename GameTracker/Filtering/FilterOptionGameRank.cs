using RatableTracker.ListManipulation.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Filtering
{
    [FilterOption(typeof(GameObject))]
    public class FilterOptionGameRank : FilterOptionNumericBase<GameObject>
    {
        public override string Name => "Rank Overall";
        public override FilterNumberFormat NumberFormat => FilterNumberFormat.Integer;

        public FilterOptionGameRank() : base() { }

        protected override double GetComparisonValue(GameObject obj)
        {
            return ((GameModule)Module).GetRankOfScore(obj.ScoreDisplay, (SettingsGame)Settings);
        }
    }
}
