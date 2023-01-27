using RatableTracker.Interfaces;
using RatableTracker.LoadSave;
using RatableTracker.ObjAddOns;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Modules
{
    public class TrackerModuleScoreStatusCategorical : TrackerModuleScoreStatuses, IModuleCategorical
    {
        public CategoryExtensionModule CategoryExtension { get; private set; }

        protected new ILoadSaveHandler<ILoadSaveMethodScoreStatusCategorical> _loadSave => (ILoadSaveHandler<ILoadSaveMethodScoreStatusCategorical>)base._loadSave;

        public TrackerModuleScoreStatusCategorical(ILoadSaveHandler<ILoadSaveMethodScoreStatusCategorical> loadSave) : this(loadSave, new Logger()) { }

        public TrackerModuleScoreStatusCategorical(ILoadSaveHandler<ILoadSaveMethodScoreStatusCategorical> loadSave, Logger logger) : this(loadSave, new StatusExtensionModule(loadSave, logger), logger) { }

        public TrackerModuleScoreStatusCategorical(ILoadSaveHandler<ILoadSaveMethodScoreStatusCategorical> loadSave, StatusExtensionModule statusExtension) : this(loadSave, statusExtension, new Logger()) { }

        public TrackerModuleScoreStatusCategorical(ILoadSaveHandler<ILoadSaveMethodScoreStatusCategorical> loadSave, StatusExtensionModule statusExtension, Logger logger) : this(loadSave, statusExtension, new CategoryExtensionModule(loadSave, logger), logger) { }

        public TrackerModuleScoreStatusCategorical(ILoadSaveHandler<ILoadSaveMethodScoreStatusCategorical> loadSave, CategoryExtensionModule categoryExtension) : this(loadSave, categoryExtension, new Logger()) { }

        public TrackerModuleScoreStatusCategorical(ILoadSaveHandler<ILoadSaveMethodScoreStatusCategorical> loadSave, CategoryExtensionModule categoryExtension, Logger logger) : this(loadSave, new StatusExtensionModule(loadSave, logger), categoryExtension, logger) { }

        public TrackerModuleScoreStatusCategorical(ILoadSaveHandler<ILoadSaveMethodScoreStatusCategorical> loadSave, StatusExtensionModule statusExtension, CategoryExtensionModule categoryExtension) : this(loadSave, statusExtension, categoryExtension, new Logger()) { }

        public TrackerModuleScoreStatusCategorical(ILoadSaveHandler<ILoadSaveMethodScoreStatusCategorical> loadSave, StatusExtensionModule statusExtension, CategoryExtensionModule categoryExtension, Logger logger) : base(loadSave, statusExtension, logger)
        {
            CategoryExtension = categoryExtension;
            CategoryExtension.BaseModule = this;
        }

        public override void LoadData(Settings settings)
        {
            base.LoadData(settings);
            try
            {
                CategoryExtension.LoadData((SettingsScore)settings);
            }
            catch (InvalidCastException e)
            {
                throw new InvalidCastException("Settings for loading categories must be of type SettingsScore or more derived", e);
            }
        }

        public void TransferToNewModule(TrackerModuleScoreStatusCategorical newModule, SettingsScore settings)
        {
            using (var connCurrent = _loadSave.NewConnection())
            {
                using (var connNew = newModule._loadSave.NewConnection())
                {
                    TransferToNewModule(connCurrent, connNew, settings);
                }
            }
        }

        protected virtual void TransferToNewModule(ILoadSaveMethodScoreStatusCategorical connCurrent, ILoadSaveMethodScoreStatusCategorical connNew, SettingsScore settings)
        {
            base.TransferToNewModule(connCurrent, connNew, settings);
            connNew.SaveAllCategories(connCurrent.LoadCategories(CategoryExtension, settings));
        }

        public override void ApplySettingsChanges(Settings settings, ILoadSaveMethod conn)
        {
            base.ApplySettingsChanges(settings, conn);
            CategoryExtension.ApplySettingsChanges(settings, this, conn as ILoadSaveMethodCategoryExtension);
        }
    }
}
