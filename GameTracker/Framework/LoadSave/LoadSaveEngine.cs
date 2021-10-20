using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Interfaces;

namespace RatableTracker.Framework.LoadSave
{
    public abstract class LoadSaveEngine<TRatableObj, TRatingCat>
        where TRatableObj : RatableObject, ISavable, new()
        where TRatingCat : RatingCategory, ISavable, new()
    {
        protected static LoadSaveIdentifier ID_RATABLEOBJECTS = new LoadSaveIdentifier("RatableObjects");
        protected static LoadSaveIdentifier ID_SCORERANGES = new LoadSaveIdentifier("ScoreRanges");
        protected static LoadSaveIdentifier ID_RATINGCATEGORIES = new LoadSaveIdentifier("RatingCategories");
        protected static LoadSaveIdentifier ID_SETTINGS = new LoadSaveIdentifier("Settings");

        protected abstract IEnumerable<T> LoadISavableList<T>(LoadSaveIdentifier id) where T : ISavable, new();
        protected abstract T LoadISavable<T>(LoadSaveIdentifier id) where T : ISavable, new();
        protected abstract void SaveISavableList<T>(IEnumerable<T> list, LoadSaveIdentifier id) where T : ISavable;
        protected abstract void SaveISavable<T>(T obj, LoadSaveIdentifier id) where T : ISavable;
        
        public virtual IEnumerable<TRatableObj> LoadRatableObjects(RatingModule<TRatableObj, TRatingCat> parentModule)
        {
            return LoadListParent<TRatableObj>(parentModule, ID_RATABLEOBJECTS);
        }

        public virtual IEnumerable<ScoreRange> LoadScoreRanges(RatingModule<TRatableObj, TRatingCat> parentModule)
        {
            return LoadListParent<ScoreRange>(parentModule, ID_SCORERANGES);
        }

        public virtual IEnumerable<TRatingCat> LoadRatingCategories(RatingModule<TRatableObj, TRatingCat> parentModule)
        {
            return LoadListParent<TRatingCat>(parentModule, ID_RATINGCATEGORIES);
        }

        public virtual Settings LoadSettings(RatingModule<TRatableObj, TRatingCat> parentModule)
        {
            return LoadObjectParent<Settings>(parentModule, ID_SETTINGS);
        }

        public virtual void SaveRatableObjects(IEnumerable<TRatableObj> ratableObjects)
        {
            SaveListParent(ratableObjects, ID_RATABLEOBJECTS);
        }

        public virtual void SaveScoreRanges(IEnumerable<ScoreRange> scoreRanges)
        {
            SaveListParent(scoreRanges, ID_SCORERANGES);
        }

        public virtual void SaveRatingCategories(IEnumerable<TRatingCat> ratingCategories)
        {
            SaveListParent(ratingCategories, ID_RATINGCATEGORIES);
        }

        public virtual void SaveSettings(Settings settings)
        {
            SaveObjectParent(settings, ID_SETTINGS);
        }

        protected virtual IEnumerable<T> LoadListParent<T>(RatingModule<TRatableObj, TRatingCat> parentModule, LoadSaveIdentifier id) where T : ISavable, new()
        {
            return LoadISavableList<T>(id);
        }

        protected virtual T LoadObjectParent<T>(RatingModule<TRatableObj, TRatingCat> parentModule, LoadSaveIdentifier id) where T : ISavable, new()
        {
            return LoadISavable<T>(id);
        }

        protected virtual void SaveListParent<T>(IEnumerable<T> list, LoadSaveIdentifier id) where T : ISavable
        {
            SaveISavableList(list, id);
        }

        protected virtual void SaveObjectParent<T>(T obj, LoadSaveIdentifier id) where T : ISavable
        {
            SaveISavable(obj, id);
        }
    }
}
