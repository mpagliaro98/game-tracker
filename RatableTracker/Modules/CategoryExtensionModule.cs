using RatableTracker.Events;
using RatableTracker.Exceptions;
using RatableTracker.Interfaces;
using RatableTracker.Model;
using RatableTracker.ObjAddOns;
using RatableTracker.Util;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Modules
{
    public class CategoryExtensionModule : ModuleExtensionBase
    {
        public virtual int LimitRatingCategories => 10;

        private IList<RatingCategory> _ratingCategories = new List<RatingCategory>();
        protected IList<RatingCategory> RatingCategories { get { return _ratingCategories; } private set { _ratingCategories = value; } }

        public delegate void RatingCategoryDeleteHandler(object sender, RatingCategoryDeleteArgs args);
        public event RatingCategoryDeleteHandler RatingCategoryDeleted;

        protected new ILoadSaveHandler<ILoadSaveMethodCategoryExtension> LoadSave => (ILoadSaveHandler<ILoadSaveMethodCategoryExtension>)base.LoadSave;
        public new IModuleCategorical BaseModule { get { return (IModuleCategorical)base.BaseModule; } internal set { base.BaseModule = (ModuleBase)value; } }

        public CategoryExtensionModule(ILoadSaveHandler<ILoadSaveMethodCategoryExtension> loadSave) : base(loadSave) { }

        public CategoryExtensionModule(ILoadSaveHandler<ILoadSaveMethodCategoryExtension> loadSave, Logger logger) : base(loadSave, logger) { }

        public virtual void LoadData(SettingsScore settings)
        {
            LoadTrackerObjectList(ref _ratingCategories, (conn) => ((ILoadSaveMethodCategoryExtension)conn).LoadCategories(BaseModule, settings));
        }

        public IList<RatingCategory> GetRatingCategoryList()
        {
            return GetTrackerObjectList(RatingCategories, null, null);
        }

        public int TotalNumRatingCategories()
        {
            return RatingCategories.Count;
        }

        internal bool SaveRatingCategory(RatingCategory ratingCategory, TrackerModule module, ILoadSaveMethodCategoryExtension conn)
        {
            return SaveTrackerObject(ratingCategory, ref _ratingCategories, LimitRatingCategories, conn.SaveOneCategory);
        }

        internal void DeleteRatingCategory(RatingCategory ratingCategory, TrackerModule module, ILoadSaveMethodCategoryExtension conn)
        {
            DeleteTrackerObject(ratingCategory, ref _ratingCategories, conn.DeleteOneCategory,
                (obj) => RatingCategoryDeleted?.Invoke(this, new RatingCategoryDeleteArgs(obj, obj.GetType(), conn)), () => RatingCategoryDeleted == null ? 0 : RatingCategoryDeleted.GetInvocationList().Length);
        }

        public double SumOfCategoryWeights()
        {
            return RatingCategories.Select(cat => cat.Weight).Sum();
        }

        internal void AddCategoryValueToAllModelObjects(TrackerModule module, SettingsScore settings, RatingCategory category, ILoadSaveMethodCategoryExtension conn)
        {
            foreach (RankedObject obj in module.GetModelObjectList())
            {
                if (obj is IModelObjectCategorical objCat)
                {
                    objCat.CategoryExtension.CategoryValuesManual.Add(new CategoryValue(this, settings, category));
                    conn.SaveOneModelObject(obj);
                }
            }
        }

        public double GetTotalScoreFromCategoryValues(IList<CategoryValue> categoryValues)
        {
            double sumOfWeights = SumOfCategoryWeights();
            double total = categoryValues.Select(cv => cv.RatingCategory.Weight / sumOfWeights * cv.PointValue).Sum();
            return total;
        }
    }
}
