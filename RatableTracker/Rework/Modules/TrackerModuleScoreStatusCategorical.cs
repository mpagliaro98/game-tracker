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

        protected readonly new ILoadSaveHandler<ILoadSaveMethodScoreStatusCategorical> _loadSave;

        public TrackerModuleScoreStatusCategorical(ILoadSaveHandler<ILoadSaveMethodScoreStatusCategorical> loadSave) : this(loadSave, new StatusExtensionModule(loadSave)) { }

        public TrackerModuleScoreStatusCategorical(ILoadSaveHandler<ILoadSaveMethodScoreStatusCategorical> loadSave, StatusExtensionModule statusExtension) : this(loadSave, statusExtension, new CategoryExtensionModule(loadSave)) { }

        public TrackerModuleScoreStatusCategorical(ILoadSaveHandler<ILoadSaveMethodScoreStatusCategorical> loadSave, StatusExtensionModule statusExtension, CategoryExtensionModule categoryExtension) : base(loadSave, statusExtension)
        {
            _categoryExtension = categoryExtension;
        }

        public override void Init()
        {
            base.Init();
            CategoryExtension.Init();
        }
    }
}
