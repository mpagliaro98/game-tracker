using RatableTracker.ListManipulation.Filtering;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Filtering
{
    [FilterOption(typeof(GameObject))]
    public class FilterOptionGamePlatform : FilterOptionListBase<GameObject>
    {
        public override string Name => "Platform";
        public override List<KeyValuePair<UniqueID, string>> ListValues => new List<KeyValuePair<UniqueID, string>>()
        {
            new KeyValuePair<UniqueID, string>(UniqueID.BlankID(), "No Platform")
        }.Concat(((GameModule)Module).GetPlatformList((SettingsGame)Settings).OrderBy(p => p.Name.CleanForSorting()).Select(p => new KeyValuePair<UniqueID, string>(p.UniqueID, p.Name))).ToList();

        public override Func<GameObject, bool> GenerateFilterExpression()
        {
            return (obj) => FilterID.HasValue() ? obj.PlatformEffective != null && obj.PlatformEffective.UniqueID.Equals(FilterID) : obj.PlatformEffective == null;
        }
    }
}
