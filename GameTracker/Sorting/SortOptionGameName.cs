using RatableTracker.ListManipulation.Sorting;
using RatableTracker.Util;

namespace GameTracker.Sorting;

[SortOption(typeof(GameObject))]
public class SortOptionGameName : SortOptionSimpleBase<GameObject>
{
    public override string Name => "Name";

    public SortOptionGameName() : base() { }

    protected override object GetSortValue(GameObject obj)
    {
        return obj.DisplayName.CleanForSorting();
    }
}