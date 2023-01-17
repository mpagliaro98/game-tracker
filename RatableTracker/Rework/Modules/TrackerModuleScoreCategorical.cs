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
    public class TrackerModuleScoreCategorical : TrackerModuleScores
    {
        private readonly CategoryExtensionModule _categoryExtension;
        public CategoryExtensionModule CategoryExtension { get { return _categoryExtension; } }

        protected readonly new ILoadSaveHandler<ILoadSaveMethodScoreCategorical> _loadSave;

        public TrackerModuleScoreCategorical(ILoadSaveHandler<ILoadSaveMethodScoreCategorical> loadSave) : this(loadSave, new CategoryExtensionModule(loadSave)) { }

        public TrackerModuleScoreCategorical(ILoadSaveHandler<ILoadSaveMethodScoreCategorical> loadSave, CategoryExtensionModule categoryExtension) : base(loadSave)
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
