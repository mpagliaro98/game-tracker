using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.ObjectHierarchy;
using RatableTracker.Framework.Global;
using RatableTracker.Framework.Exceptions;

namespace RatableTracker.Framework.ModuleHierarchy
{
    public abstract class RatingModuleCategorical<TListedObj, TRange, TSettings, TRatingCat>
        : RatingModule<TListedObj, TRange, TSettings>, IModuleCategorical<TListedObj, TRatingCat>
        where TListedObj : RatableObjectCategorical
        where TRange : ScoreRange
        where TSettings : SettingsScore
        where TRatingCat : RatingCategory
    {
        protected IEnumerable<TRatingCat> ratingCategories;
        public IEnumerable<TRatingCat> RatingCategories => ratingCategories;

        public virtual int LimitRatingCategories => 10;

        public override void Init()
        {
            LoadRatingCategories();
            base.Init();
        }

        public abstract void LoadRatingCategories();
        public abstract void SaveRatingCategories();

        public override double GetScoreOfObject(TListedObj obj)
        {
            if (obj.IgnoreCategories)
            {
                return base.GetScoreOfObject(obj);
            }
            else
            {
                double total = 0;
                double sumOfWeights = SumOfWeights(obj.CategoryValues);
                foreach (RatingCategoryValue categoryValue in obj.CategoryValues)
                {
                    TRatingCat cat = FindRatingCategory(categoryValue.RefRatingCategory);
                    double categoryWeight = cat.Weight;
                    total += (categoryWeight / sumOfWeights) * categoryValue.PointValue;
                }
                return total;
            }
        }

        public virtual double SimulateScoreOfObject(IEnumerable<RatingCategoryValue> categoryValues)
        {
            double total = 0;
            double sumOfWeights = SumOfWeights(categoryValues);
            foreach (RatingCategoryValue categoryValue in categoryValues)
            {
                TRatingCat cat = FindRatingCategory(categoryValue.RefRatingCategory);
                double categoryWeight = cat.Weight;
                total += (categoryWeight / sumOfWeights) * categoryValue.PointValue;
            }
            return total;
        }

        protected override void ScaleScoreOfObject(TListedObj obj, double oldRange, double newRange, double minRangeOld, double minRangeNew)
        {
            if (obj.IgnoreCategories)
            {
                base.ScaleScoreOfObject(obj, oldRange, newRange, minRangeOld, minRangeNew);
            }
            else
            {
                foreach (RatingCategoryValue rcv in obj.CategoryValues)
                {
                    if (oldRange == 0)
                        rcv.PointValue = minRangeNew;
                    else
                        rcv.PointValue = ((rcv.PointValue - minRangeOld) * newRange / oldRange) + minRangeNew;
                }
            }
        }

        public virtual bool ValidateCategoryScores(IEnumerable<RatingCategoryValue> vals)
        {
            foreach (RatingCategoryValue val in vals)
            {
                if (val.PointValue < Settings.MinScore || val.PointValue > Settings.MaxScore)
                    return false;
            }
            return true;
        }

        public virtual void SetCategoryValuesForObject(TListedObj obj, IEnumerable<RatingCategoryValue> vals)
        {
            if (!ValidateCategoryScores(vals))
                throw new ScoreOutOfRangeException();
            obj.CategoryValues = vals;
        }

        public TRatingCat FindRatingCategory(ObjectReference objectKey)
        {
            return FindObject(ratingCategories, objectKey);
        }

        protected double SumOfWeights(IEnumerable<RatingCategoryValue> categoryValues)
        {
            double sum = 0;
            foreach (RatingCategoryValue rcv in categoryValues)
            {
                TRatingCat cat = FindRatingCategory(rcv.RefRatingCategory);
                sum += cat.Weight;
            }
            return sum;
        }

        public void AddRatingCategory(TRatingCat obj)
        {
            AddToList(ref ratingCategories, SaveRatingCategories, obj, LimitRatingCategories);
        }

        public void UpdateRatingCategory(TRatingCat obj, TRatingCat orig)
        {
            UpdateInList(ref ratingCategories, SaveRatingCategories, obj, orig);
        }

        public void DeleteRatingCategory(TRatingCat obj)
        {
            DeleteFromList(ref ratingCategories, SaveRatingCategories, obj);
            listedObjs.ForEach(ro => ro.DeleteRatingCategoryValues(rcv => rcv.RefRatingCategory.IsReferencedObject(obj)));
            if (GlobalSettings.Autosave) SaveListedObjects();
        }

        public void SortRatingCategories<TField>(Func<TRatingCat, TField> keySelector, SortMode mode = SortMode.ASCENDING)
        {
            SortList(ref ratingCategories, keySelector, mode);
        }
    }
}
