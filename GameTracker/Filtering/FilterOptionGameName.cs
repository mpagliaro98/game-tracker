using RatableTracker.ListManipulation.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Filtering;

[FilterOption(typeof(GameObject))]
public class FilterOptionGameName : FilterOptionTextBase<GameObject>
{
    public override string Name => "Name";

    public FilterOptionGameName() : base() { }

    protected override string GetComparisonValue(GameObject obj)
    {
        return obj.DisplayName;
    }
}