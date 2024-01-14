using RatableTracker.ListManipulation.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Filtering;

[FilterOption(typeof(GameObject))]
public class FilterOptionGameDLC : FilterOptionBooleanBase<GameObject>
{
    public override string Name => "DLC";
    public override string DisplayText => "Only DLC";

    public override Func<GameObject, bool> GenerateFilterExpression()
    {
        return (obj) => obj.IsDLC;
    }
}