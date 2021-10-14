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
        protected abstract IEnumerable<T> LoadISavableList<T>() where T : ISavable, new();
        protected abstract T LoadISavable<T>() where T : ISavable, new();
        protected abstract void SaveISavableList<T>(IEnumerable<T> list) where T : ISavable;
        protected abstract void SaveISavable<T>(T obj) where T : ISavable;
        
        public virtual IEnumerable<RatableObject> LoadRatableObjects(RatingModule parentModule)
        {
            return LoadListParent<RatableObject>(parentModule);
        }

        public virtual IEnumerable<ScoreRange> LoadScoreRanges(RatingModule parentModule)
        {
            return LoadListParent<ScoreRange>(parentModule);
        }

        public virtual IEnumerable<RatingCategory> LoadRatingCategories(RatingModule parentModule)
        {
            return LoadListParent<RatingCategory>(parentModule);
        }

        public virtual Settings LoadSettings(RatingModule parentModule)
        {
            return LoadObjectParent<Settings>(parentModule);
        }

        public virtual void SaveRatableObjects(IEnumerable<RatableObject> ratableObjects)
        {
            SaveListParent(ratableObjects);
        }

        public virtual void SaveScoreRanges(IEnumerable<ScoreRange> scoreRanges)
        {
            SaveListParent(scoreRanges);
        }

        public virtual void SaveRatingCategories(IEnumerable<RatingCategory> ratingCategories)
        {
            SaveListParent(ratingCategories);
        }

        public virtual void SaveSettings(Settings settings)
        {
            SaveObjectParent(settings);
        }

        protected virtual IEnumerable<T> LoadListParent<T>(RatingModule parentModule) where T : ISavable, new()
        {
            var loadedList = LoadISavableList<T>();
            SetParentModule(loadedList, parentModule);
            return loadedList;
        }

        protected virtual T LoadObjectParent<T>(RatingModule parentModule) where T : ISavable, new()
        {
            var loaded = LoadISavable<T>();
            SetParentModule(loaded, parentModule);
            return loaded;
        }

        protected virtual void SaveListParent<T>(IEnumerable<T> list) where T : ISavable
        {
            SaveISavableList(list);
        }

        protected virtual void SaveObjectParent<T>(T obj) where T : ISavable
        {
            SaveISavable(obj);
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
                moduleAccess.SetParentModule(parentModule);
            }
        }
    }
}
