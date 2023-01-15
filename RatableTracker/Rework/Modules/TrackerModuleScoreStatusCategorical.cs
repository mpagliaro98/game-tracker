using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.LoadSave;
using RatableTracker.Rework.ObjAddOns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Modules
{
    public class TrackerModuleScoreStatusCategorical : TrackerModuleScoreStatuses
    {
        private readonly CategoryExtensionModule _categoryExtension;
        public CategoryExtensionModule CategoryExtension { get { return _categoryExtension; } }

        public TrackerModuleScoreStatusCategorical(ILoadSaveMethod loadSave) : base(loadSave)
        {
            _categoryExtension = new CategoryExtensionModule(this);
        }
    }
}
