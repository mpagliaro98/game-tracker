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

        public void TransferToNewModule(TrackerModuleScoreCategorical newModule)
        {
            using (var connCurrent = _loadSave.NewConnection())
            {
                using (var connNew = newModule._loadSave.NewConnection())
                {
                    base.TransferToNewModule(connCurrent, connNew);
                    TransferToNewModule(connCurrent, connNew);
                }
            }
        }

        protected virtual void TransferToNewModule(ILoadSaveMethodCategoryExtension connCurrent, ILoadSaveMethodCategoryExtension connNew)
        {
            connNew.SaveAllCategories(connCurrent.LoadCategories());
        }
    }
}
