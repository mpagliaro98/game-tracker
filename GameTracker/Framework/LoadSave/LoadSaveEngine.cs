using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Interfaces;

namespace RatableTracker.Framework.LoadSave
{
    public abstract class LoadSaveEngine
    {
        protected static LoadSaveIdentifier ID_RATABLEOBJECTS = new LoadSaveIdentifier("RatableObjects");
        protected static LoadSaveIdentifier ID_SCORERANGES = new LoadSaveIdentifier("ScoreRanges");
        protected static LoadSaveIdentifier ID_RATINGCATEGORIES = new LoadSaveIdentifier("RatingCategories");
        protected static LoadSaveIdentifier ID_SETTINGS = new LoadSaveIdentifier("Settings");

        protected abstract IEnumerable<T> LoadISavableList<T>(LoadSaveIdentifier id) where T : ISavable, new();
        protected abstract T LoadISavable<T>(LoadSaveIdentifier id) where T : ISavable, new();
        protected abstract void SaveISavableList<T>(IEnumerable<T> list, LoadSaveIdentifier id) where T : ISavable;
        protected abstract void SaveISavable<T>(T obj, LoadSaveIdentifier id) where T : ISavable;
        
        public virtual IEnumerable<RatableObject> LoadRatableObjects(RatingModule parentModule)
        {
            return LoadListParent<RatableObject>(parentModule, ID_RATABLEOBJECTS);
        }

        public virtual IEnumerable<ScoreRange> LoadScoreRanges(RatingModule parentModule)
        {
            return LoadListParent<ScoreRange>(parentModule, ID_SCORERANGES);
        }

        public virtual IEnumerable<RatingCategory> LoadRatingCategories(RatingModule parentModule)
        {
            return LoadListParent<RatingCategory>(parentModule, ID_RATINGCATEGORIES);
        }

        public virtual Settings LoadSettings(RatingModule parentModule)
        {
            return LoadObjectParent<Settings>(parentModule, ID_SETTINGS);
        }

        public virtual void SaveRatableObjects(IEnumerable<RatableObject> ratableObjects)
        {
            SaveListParent(ratableObjects, ID_RATABLEOBJECTS);
        }

        public virtual void SaveScoreRanges(IEnumerable<ScoreRange> scoreRanges)
        {
            SaveListParent(scoreRanges, ID_SCORERANGES);
        }

        public virtual void SaveRatingCategories(IEnumerable<RatingCategory> ratingCategories)
        {
            SaveListParent(ratingCategories, ID_RATINGCATEGORIES);
        }

        public virtual void SaveSettings(Settings settings)
        {
            SaveObjectParent(settings, ID_SETTINGS);
        }

        protected virtual IEnumerable<T> LoadListParent<T>(RatingModule parentModule, LoadSaveIdentifier id) where T : ISavable, new()
        {
            var loadedList = LoadISavableList<T>(id);
            SetParentModule(loadedList, parentModule);
            return loadedList;
        }

        protected virtual T LoadObjectParent<T>(RatingModule parentModule, LoadSaveIdentifier id) where T : ISavable, new()
        {
            var loaded = LoadISavable<T>(id);
            SetParentModule(loaded, parentModule);
            return loaded;
        }

        protected virtual void SaveListParent<T>(IEnumerable<T> list, LoadSaveIdentifier id) where T : ISavable
        {
            SaveISavableList(list, id);
        }

        protected virtual void SaveObjectParent<T>(T obj, LoadSaveIdentifier id) where T : ISavable
        {
            SaveISavable(obj, id);
        }

        protected void SetParentModule<T>(IEnumerable<T> list, RatingModule parentModule)
        {
            foreach (T obj in list)
            {
                SetParentModule(obj, parentModule);
            }
        }

        protected void SetParentModule<T>(T obj, RatingModule parentModule)
        {
            if (obj is IModuleAccess moduleAccess)
            {
                moduleAccess.ParentModule = parentModule;
            }
        }
    }
}
