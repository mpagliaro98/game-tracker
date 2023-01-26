using RatableTracker.Exceptions;
using RatableTracker.Interfaces;
using RatableTracker.LoadSave;
using RatableTracker.Model;
using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.ObjAddOns
{
    public class RatingCategory : SaveDeleteObject
    {
        public static int MaxLengthName => 200;
        public static int MaxLengthComment => 4000;

        public string Name { get; set; } = "";
        public string Comment { get; set; } = "";
        public double Weight { get; protected set; } = 1.0;

        public override UniqueID UniqueID { get; protected set; } = UniqueID.NewID();

        protected readonly CategoryExtensionModule module;
        protected readonly SettingsScore settings;

        public RatingCategory(CategoryExtensionModule module, SettingsScore settings)
        {
            this.module = module;
            this.settings = settings;
        }

        public RatingCategory(RatingCategory copyFrom)
        {
            module = copyFrom.module;
            settings = copyFrom.settings;
            UniqueID = UniqueID.Copy(copyFrom.UniqueID);
            Name = copyFrom.Name;
            Comment = copyFrom.Comment;
            Weight = copyFrom.Weight;
        }

        protected override void ValidateFields()
        {
            base.ValidateFields();
            if (Name == "")
                throw new ValidationException("A name is required", Name);
            if (Name.Length > MaxLengthName)
                throw new ValidationException("Name cannot be longer than " + MaxLengthName.ToString() + " characters", Name);
            if (Comment.Length > MaxLengthComment)
                throw new ValidationException("Comment cannot be longer than " + MaxLengthComment.ToString() + " characters", Comment);
        }

        protected override bool SaveObjectToModule(TrackerModule module, ILoadSaveMethod conn)
        {
            return this.module.SaveRatingCategory(this, module, (ILoadSaveMethodCategoryExtension)conn);
        }

        protected override void PostSave(TrackerModule module, bool isNew)
        {
            base.PostSave(module, isNew);
            if (isNew)
                this.module.AddCategoryValueToAllModelObjects(module, settings, this);
        }

        protected override void DeleteObjectFromModule(TrackerModule module, ILoadSaveMethod conn)
        {
            this.module.DeleteRatingCategory(this, module, (ILoadSaveMethodCategoryExtension)conn);
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
