using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Interfaces;
using RatableTracker.Framework.LoadSave;
using RatableTracker.Framework.Global;

namespace RatableTracker.Framework
{
    public class RatableObject : ISavable, IModuleAccess, IReferable
    {
        private string name = "";
        public string Name {
            get { return name; }
            set { name = value; }
        }

        private string comment = "";
        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }

        private IEnumerable<RatingCategoryValue> categoryValues;
        public IEnumerable<RatingCategoryValue> CategoryValues
        {
            get { return categoryValues; }
        }

        private bool ignoreCategories = false;
        public bool IgnoreCategories
        {
            get { return ignoreCategories; }
            set { ignoreCategories = value; }
        }

        private double finalScoreManual = 0;
        public double FinalScore
        {
            get
            {
                if (IgnoreCategories)
                {
                    return finalScoreManual;
                }
                else
                {
                    double total = 0;
                    double sumOfWeights = SumOfWeights();
                    foreach (RatingCategoryValue categoryValue in categoryValues)
                    {
                        double categoryWeight = categoryValue.RatingCategory.Weight;
                        total += (categoryWeight / sumOfWeights) * categoryValue.PointValue;
                    }
                    return total;
                }
            }
        }

        private Guid referenceKey = Guid.NewGuid();
        public Guid ReferenceKey
        {
            get { return referenceKey; }
        }

        private RatingModule parentModule;
        public RatingModule ParentModule
        {
            get { return parentModule; }
            set { parentModule = value; }
        }

        public RatableObject() { }

        public virtual SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = new SavableRepresentation();
            sr.SaveValue("referenceKey", referenceKey);
            sr.SaveValue("name", name);
            sr.SaveValue("comment", comment);
            sr.SaveList("categoryValues", categoryValues);
            sr.SaveValue("ignoreCategories", ignoreCategories);
            sr.SaveValue("finalScoreManual", finalScoreManual);
            return sr;
        }

        public virtual void RestoreFromRepresentation(SavableRepresentation sr)
        {
            if (sr == null) return;
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "referenceKey":
                        referenceKey = sr.GetGuid(key);
                        break;
                    case "name":
                        name = sr.GetString(key);
                        break;
                    case "comment":
                        comment = sr.GetString(key);
                        break;
                    case "categoryValues":
                        categoryValues = sr.GetListOfISavable<RatingCategoryValue>(key);
                        break;
                    case "ignoreCategories":
                        ignoreCategories = sr.GetBool(key);
                        break;
                    case "finalScoreManual":
                        finalScoreManual = sr.GetDouble(key);
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine("RatableObject.cs RestoreFromRepresentation: unrecognized key " + key);
                        break;
                }
            }
        }

        public double SumOfWeights()
        {
            double sum = 0;
            foreach (RatingCategoryValue rcv in CategoryValues)
            {
                sum += rcv.RatingCategory.Weight;
            }
            return sum;
        }

        public void SetManualFinalScore(double val)
        {
            finalScoreManual = val;
        }

        public void UpdateRatingCategoryValues(Func<RatingCategoryValue, bool> where, Action<RatingCategoryValue> action)
        {
            Util.UpdateInListOnCondition(categoryValues, where, action);
        }

        public void DeleteRatingCategoryValues(Predicate<RatingCategoryValue> where)
        {
            Util.DeleteFromListOnCondition(ref categoryValues, where);
        }

        public override int GetHashCode()
        {
            return ReferenceKey.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                RatableObject o = (RatableObject)obj;
                return !ReferenceKey.Equals(Guid.Empty) && ReferenceKey.Equals(o.ReferenceKey);
            }
        }
    }
}
