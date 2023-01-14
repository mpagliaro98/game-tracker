using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.ObjectHierarchy;
using RatableTracker.Framework.Global;
using RatableTracker.Framework.Exceptions;
using RatableTracker.Framework.Interfaces;

namespace RatableTracker.Framework.ModuleHierarchy
{
    public abstract class RatingModuleStatusCategorical<TListedObj, TRange, TSettings, TStatus, TRatingCat>
        : RatingModuleStatus<TListedObj, TRange, TSettings, TStatus>, IModuleCategorical<TListedObj, TRatingCat>
        where TListedObj : RatableObjectStatusCategorical
        where TRange : ScoreRange
        where TSettings : SettingsScore, new()
        where TStatus : Status
        where TRatingCat : RatingCategory
    {
        protected IEnumerable<TRatingCat> ratingCategories = new List<TRatingCat>();
        public IEnumerable<TRatingCat> RatingCategories => ratingCategories;

        public virtual int LimitRatingCategories => 10;

        public override void Init()
        {
            LoadRatingCategories();
            base.Init();
        }

        public override async Task InitAsync()
        {
            await LoadRatingCategoriesAsync();
            await base.InitAsync();
        }

        public abstract void LoadRatingCategories();
        public abstract Task LoadRatingCategoriesAsync();
        public abstract void SaveRatingCategories();
        public abstract Task SaveRatingCategoriesAsync();

        public override double GetScoreOfObject(TListedObj obj)
        {
            if (obj.IgnoreCategories)
            {
                return base.GetScoreOfObject(obj);
            }
            else
            {
                return GetScoreOfCategoryValues(obj.CategoryValues);
            }
        }

        public virtual double GetScoreOfCategory(TListedObj obj, TRatingCat cat)
        {
            foreach (RatingCategoryValue val in obj.CategoryValues)
            {
                if (val.RefRatingCategory.IsReferencedObject(cat))
                {
                    return val.PointValue;
                }
            }
            return Settings.MinScore;
        }

        public virtual double GetScoreOfCategoryValues(IEnumerable<RatingCategoryValue> categoryValues)
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
            base.ScaleScoreOfObject(obj, oldRange, newRange, minRangeOld, minRangeNew);
            foreach (RatingCategoryValue rcv in obj.CategoryValues)
            {
                if (oldRange == 0)
                    rcv.PointValue = minRangeNew;
                else
                    rcv.PointValue = ((rcv.PointValue - minRangeOld) * newRange / oldRange) + minRangeNew;
            }
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

        public TRatingCat FindRatingCategory(ObjectReference objectKey)
        {
            return FindObject(ratingCategories, objectKey);
        }

        public void AddRatingCategory(TRatingCat obj)
        {
            ValidateRatingCategory(obj);
            AddToList(ref ratingCategories, SaveRatingCategories, obj, LimitRatingCategories);
        }

        public void UpdateRatingCategory(TRatingCat obj, TRatingCat orig)
        {
            ValidateRatingCategory(obj);
            UpdateInList(ref ratingCategories, SaveRatingCategories, obj, orig);
        }

        public void DeleteRatingCategory(TRatingCat obj)
        {
            DeleteFromList(ref ratingCategories, SaveRatingCategories, obj);
            listedObjs.ForEach(ro => ro.DeleteRatingCategoryValues(rcv => rcv.RefRatingCategory.IsReferencedObject(obj)));
            if (GlobalSettings.Autosave) SaveListedObjects();
        }

        public override void ValidateListedObject(TListedObj obj)
        {
            base.ValidateListedObject(obj);
            ValidateCategoryScores(obj.CategoryValues);
                
        }

        public virtual void ValidateCategoryScores(IEnumerable<RatingCategoryValue> vals)
        {
            foreach (RatingCategoryValue val in vals)
            {
                try
                {
                    ValidateScore(val.PointValue);
                }
                catch (ValidationException e)
                {
                    TRatingCat cat = FindRatingCategory(val.RefRatingCategory);
                    throw new ValidationException(e.Message + " (" + cat.Name + ")");
                }
            }
        }

        public virtual void ValidateRatingCategory(TRatingCat obj)
        {
            if (obj.Name == "")
                throw new ValidationException("A name is required");
            if (obj.Name.Length > RatingCategory.MaxLengthName)
                throw new ValidationException("Name cannot be longer than " + RatingCategory.MaxLengthName.ToString() + " characters");
            if (obj.Comment.Length > RatingCategory.MaxLengthComment)
                throw new ValidationException("Comment cannot be longer than " + RatingCategory.MaxLengthComment.ToString() + " characters");
        }
    }
}
