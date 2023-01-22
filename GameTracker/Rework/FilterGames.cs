using RatableTracker.Rework.ListManipulation;
using RatableTracker.Rework.Model;
using RatableTracker.Rework.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Rework
{
    public class FilterGames : FilterRatedObjectStatusCategorical
    {
        public bool ShowCompilations { get; set; } = false;

        protected readonly new SettingsGame settings;

        public FilterGames(SettingsGame settings) : base(settings) { }

        protected override IList<RankedObject> ApplyFiltering(IList<RankedObject> list, TrackerModule module)
        {
            IList<RankedObject> newList = base.ApplyFiltering(list, module);
            if (ShowCompilations)
                newList = newList.Where(obj => !((GameObject)obj).IsPartOfCompilation || ((GameObject)obj).IsCompilation).ToList();
            else
                newList = newList.Where(obj => !((GameObject)obj).IsCompilation).ToList();
            return newList;
        }
    }
}
