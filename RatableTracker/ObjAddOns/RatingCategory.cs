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
    public class RatingCategory : TrackerObjectBase
    {
        public static int MaxLengthComment => 4000;

        public string Comment { get; set; } = "";
        public double Weight { get; protected set; } = 1.0;

        protected new IModuleCategorical module => (IModuleCategorical)base.Module;
        protected new SettingsScore settings => (SettingsScore)base.Settings;

        public RatingCategory(IModuleCategorical module, SettingsScore settings) : base(settings, (TrackerModuleScores)module) { }

        public RatingCategory(RatingCategory copyFrom) : base(copyFrom)
        {
            Comment = copyFrom.Comment;
            Weight = copyFrom.Weight;
        }

        protected override void ValidateFields()
        {
            base.ValidateFields();
            if (Comment.Length > MaxLengthComment)
                throw new ValidationException("Comment cannot be longer than " + MaxLengthComment.ToString() + " characters", Comment);
        }

        protected override bool SaveObjectToModule(TrackerModule module, ILoadSaveMethod conn)
        {
            return this.module.CategoryExtension.SaveRatingCategory(this, module, (ILoadSaveMethodCategoryExtension)conn);
        }

        protected override void PostSave(TrackerModule module, bool isNew, ILoadSaveMethod conn)
        {
            base.PostSave(module, isNew, conn);
            if (isNew)
                this.module.CategoryExtension.AddCategoryValueToAllModelObjects(module, settings, this, conn as ILoadSaveMethodCategoryExtension);
        }

        protected override void DeleteObjectFromModule(TrackerModule module, ILoadSaveMethod conn)
        {
            this.module.CategoryExtension.DeleteRatingCategory(this, module, (ILoadSaveMethodCategoryExtension)conn);
        }

        public override SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = base.LoadIntoRepresentation();
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
    }
}
