using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.LoadSave;
using RatableTracker.Rework.ObjAddOns;
using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Modules
{
    public class TrackerModuleScoreStatusCategorical : TrackerModuleScoreStatuses, IModuleCategorical
    {
        public CategoryExtensionModule CategoryExtension { get; private set; }

        protected readonly new ILoadSaveHandler<ILoadSaveMethodScoreStatusCategorical> _loadSave;

        public TrackerModuleScoreStatusCategorical(ILoadSaveHandler<ILoadSaveMethodScoreStatusCategorical> loadSave, ILogger logger = null) : this(loadSave, new StatusExtensionModule(loadSave, logger), logger) { }

        public TrackerModuleScoreStatusCategorical(ILoadSaveHandler<ILoadSaveMethodScoreStatusCategorical> loadSave, StatusExtensionModule statusExtension, ILogger logger = null) : this(loadSave, statusExtension, new CategoryExtensionModule(loadSave, logger), logger) { }

        public TrackerModuleScoreStatusCategorical(ILoadSaveHandler<ILoadSaveMethodScoreStatusCategorical> loadSave, CategoryExtensionModule categoryExtension, ILogger logger = null) : this(loadSave, new StatusExtensionModule(loadSave, logger), categoryExtension, logger) { }

        public TrackerModuleScoreStatusCategorical(ILoadSaveHandler<ILoadSaveMethodScoreStatusCategorical> loadSave, StatusExtensionModule statusExtension, CategoryExtensionModule categoryExtension, ILogger logger = null) : base(loadSave, statusExtension, logger)
        {
            CategoryExtension = categoryExtension;
        }

        public override void LoadData(Settings settings)
        {
            base.LoadData(settings);
            CategoryExtension.LoadData();
        }

        public void TransferToNewModule(TrackerModuleScoreStatusCategorical newModule, Settings settings)
        {
            using (var connCurrent = _loadSave.NewConnection())
            {
                using (var connNew = newModule._loadSave.NewConnection())
                {
                    TransferToNewModule(connCurrent, connNew, settings);
                }
            }
        }

        protected virtual void TransferToNewModule(ILoadSaveMethodCategoryExtension connCurrent, ILoadSaveMethodCategoryExtension connNew, Settings settings)
        {
            base.TransferToNewModule(connCurrent, connNew, settings);
            connNew.SaveAllCategories(connCurrent.LoadCategories(CategoryExtension));
        }

        public override void RemoveReferencesToObject(IKeyable obj, Type type)
        {
            base.RemoveReferencesToObject(obj, type);
            CategoryExtension.RemoveReferencesToObject(obj, type);
        }

        public override void ApplySettingsChanges(Settings settings)
        {
            base.ApplySettingsChanges(settings);
            CategoryExtension.ApplySettingsChanges(settings, this);
        }
    }
}
