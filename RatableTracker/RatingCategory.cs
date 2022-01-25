using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Interfaces;
using RatableTracker.Framework.LoadSave;

namespace RatableTracker.Framework
{
    public class RatingCategory : ISavable, IReferable
    {
        public static int MaxLengthName => 200;
        public static int MaxLengthComment => 4000;

        public string Name { get; set; } = "";

        public string Comment { get; set; } = "";

        protected double weight = 1.0;
        public double Weight => weight;

        private ObjectReference referenceKey = new ObjectReference(true);
        public ObjectReference ReferenceKey => referenceKey;

        public RatingCategory() { }

        public virtual SavableRepresentation<T> LoadIntoRepresentation<T>() where T : IValueContainer<T>, new()
        {
            SavableRepresentation<T> sr = new SavableRepresentation<T>();
            sr.SaveValue("referenceKey", referenceKey);
            sr.SaveValue("name", Name);
            sr.SaveValue("comment", Comment);
            sr.SaveValue("weight", weight);
            return sr;
        }

        public virtual void RestoreFromRepresentation<T>(SavableRepresentation<T> sr) where T : IValueContainer<T>, new()
        {
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "referenceKey":
                        referenceKey = sr.GetISavable<ObjectReference>(key);
                        break;
                    case "name":
                        Name = sr.GetString(key);
                        break;
                    case "comment":
                        Comment = sr.GetString(key);
                        break;
                    case "weight":
                        weight = sr.GetDouble(key);
                        break;
                    default:
                        break;
                }
            }
        }

        public void OverwriteReferenceKey(IReferable orig)
        {
            if (orig is RatingCategory origCategory)
            referenceKey = origCategory.referenceKey;
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
                RatingCategory o = (RatingCategory)obj;
                return !ReferenceKey.Equals(Guid.Empty) && ReferenceKey.Equals(o.ReferenceKey);
            }
        }
    }
}
