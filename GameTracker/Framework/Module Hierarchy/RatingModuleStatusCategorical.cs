﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.ObjectHierarchy;
using RatableTracker.Framework.Global;

namespace RatableTracker.Framework.ModuleHierarchy
{
    public abstract class RatingModuleStatusCategorical<TListedObj, TRange, TSettings, TStatus, TRatingCat>
        : RatingModuleStatus<TListedObj, TRange, TSettings, TStatus>, IModuleCategorical<TRatingCat>
        where TListedObj : RatableObjectStatusCategorical
        where TRange : ScoreRange
        where TSettings : SettingsScore
        where TStatus : Status
        where TRatingCat : RatingCategory
    {
        protected IEnumerable<TRatingCat> ratingCategories;
        public IEnumerable<TRatingCat> RatingCategories
        {
            get { return ratingCategories; }
        }

        public virtual int LimitRatingCategories => 10;

        public override void Init()
        {
            LoadRatingCategories();
            base.Init();
        }

        public abstract void LoadRatingCategories();
        public abstract void SaveRatingCategories();

        public override double GetScoreOfObject(TListedObj ro)
        {
            if (ro.IgnoreCategories)
            {
                return base.GetScoreOfObject(ro);
            }
            else
            {
                double total = 0;
                double sumOfWeights = SumOfWeights(ro);
                foreach (RatingCategoryValue categoryValue in ro.CategoryValues)
                {
                    TRatingCat cat = FindRatingCategory(categoryValue.RefRatingCategory);
                    double categoryWeight = cat.Weight;
                    total += (categoryWeight / sumOfWeights) * categoryValue.PointValue;
                }
                return total;
            }
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

        public TRatingCat FindRatingCategory(ObjectReference objectKey)
        {
            return FindObject(ratingCategories, objectKey);
        }

        protected double SumOfWeights(TListedObj ro)
        {
            double sum = 0;
            foreach (RatingCategoryValue rcv in ro.CategoryValues)
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
    }
}
