using RatableTracker.Model;
using RatableTracker.Modules;
using RatableTracker.ScoreRanges;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RatableTracker.ListManipulation.Filtering;

[FilterOption(typeof(RatedObject))]
public class FilterOptionModelRange : FilterOptionListBase<RatedObject>
{
    public override string Name => "Score Range";
    public override List<KeyValuePair<UniqueID, string>> ListValues => new List<KeyValuePair<UniqueID, string>>()
        {
            new KeyValuePair<UniqueID, string>(UniqueID.BlankID(), "Does not fit a Score Range")
        }.Concat(((TrackerModuleScores)Module).GetScoreRangeList().OrderBy(p => p.Name.CleanForSorting()).Select(p => new KeyValuePair<UniqueID, string>(p.UniqueID, p.Name))).ToList();

    public override Func<RatedObject, bool> GenerateFilterExpression()
    {
        return (obj) => FilterID.HasValue() ? obj.ScoreRangeDisplay != null && obj.ScoreRangeDisplay.UniqueID.Equals(FilterID) : obj.ScoreRangeDisplay == null;
    }
}
