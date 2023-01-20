using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.LoadSave;
using RatableTracker.Rework.Model;
using RatableTracker.Rework.Modules;
using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.ObjAddOns
{
    public class RatingCategory : SavableObject, IKeyable
    {
        public static int MaxLengthName => 200;
        public static int MaxLengthComment => 4000;

        public string Name { get; set; } = "";
        public string Comment { get; set; } = "";
        public double Weight { get; protected set; } = 1.0;

        public UniqueID UniqueID { get; private set; } = UniqueID.NewID();

        protected readonly CategoryExtensionModule module;

        public RatingCategory(CategoryExtensionModule module)
        {
            this.module = module;
        }

        public virtual void Validate()
        {
            // TODO unique exceptions
            if (Name == "")
                throw new Exception("A name is required");
            if (Name.Length > MaxLengthName)
                throw new Exception("Name cannot be longer than " + MaxLengthName.ToString() + " characters");
            if (Comment.Length > MaxLengthComment)
                throw new Exception("Comment cannot be longer than " + MaxLengthComment.ToString() + " characters");
        }

        public void Save()
        {
            module.SaveRatingCategory(this);
        }

        public virtual void PostSave() { }

        public void Delete(TrackerModule module)
        {
            this.module.DeleteRatingCategory(this, module);
        }

        public virtual void PostDelete() { }

        public virtual bool RemoveReferenceToObject(IKeyable obj, Type type)
        {
            return false;
        }

        public override SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = base.LoadIntoRepresentation();
            sr.SaveValue("UniqueID", new ValueContainer(UniqueID));
            sr.SaveValue("Name", new ValueContainer(Name));
            sr.SaveValue("Comment", new ValueContainer(Comment));
            sr.SaveValue("Weight", new ValueContainer(Weight));
            return sr;
        }

        public override void RestoreFromRepresentation(SavableRepresentation sr)
        {
            base.RestoreFromRepresentation(sr);
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "UniqueID":
                        UniqueID = sr.GetValue(key).GetUniqueID();
                        break;
                    case "Name":
                        Name = sr.GetValue(key).GetString();
                        break;
                    case "Comment":
                        Comment = sr.GetValue(key).GetString();
                        break;
                    case "Weight":
                        Weight = sr.GetValue(key).GetDouble();
                        break;
                    default:
                        break;
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is RatingCategory)) return false;
            RatingCategory other = (RatingCategory)obj;
            return UniqueID.Equals(other.UniqueID);
        }

        public override int GetHashCode()
        {
            return UniqueID.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
