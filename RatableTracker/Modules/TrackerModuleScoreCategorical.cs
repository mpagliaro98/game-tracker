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
    public class TrackerModuleScoreCategorical : TrackerModuleScores, IModuleCategorical
    {
        public CategoryExtensionModule CategoryExtension { get; private set; }

        protected new ILoadSaveHandler<ILoadSaveMethodScoreCategorical> _loadSave => (ILoadSaveHandler<ILoadSaveMethodScoreCategorical>)base._loadSave;

        public TrackerModuleScoreCategorical(ILoadSaveHandler<ILoadSaveMethodScoreCategorical> loadSave) : this(loadSave, new Logger()) { }

        public TrackerModuleScoreCategorical(ILoadSaveHandler<ILoadSaveMethodScoreCategorical> loadSave, Logger logger) : this(loadSave, new CategoryExtensionModule(loadSave, logger), logger) { }

        public TrackerModuleScoreCategorical(ILoadSaveHandler<ILoadSaveMethodScoreCategorical> loadSave, CategoryExtensionModule categoryExtension) : this(loadSave, categoryExtension, new Logger()) { }

        public TrackerModuleScoreCategorical(ILoadSaveHandler<ILoadSaveMethodScoreCategorical> loadSave, CategoryExtensionModule categoryExtension, Logger logger) : base(loadSave, logger)
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

        public void TransferToNewModule(TrackerModuleScoreCategorical newModule, SettingsScore settings)
        {
            using (var connCurrent = _loadSave.NewConnection())
            {
                using (var connNew = newModule._loadSave.NewConnection())
                {
                    TransferToNewModule(connCurrent, connNew, settings);
                }
            }
        }

        protected virtual void TransferToNewModule(ILoadSaveMethodScoreCategorical connCurrent, ILoadSaveMethodScoreCategorical connNew, SettingsScore settings)
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
