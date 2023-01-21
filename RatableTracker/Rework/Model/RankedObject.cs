using RatableTracker.Rework.Exceptions;
using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.LoadSave;
using RatableTracker.Rework.Modules;
using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Model
{
    public class RankedObject : SavableObject, IKeyable
    {
        public static int MaxLengthName => 200;
        public static int MaxLengthComment => 10000;

        public string Name { get; set; } = "";
        public string Comment { get; set; } = "";
        public virtual int Rank
        {
            get
            {
                IList<RankedObject> rankedObjects = module.GetModelObjectList();
                return rankedObjects.IndexOf(this) + 1;
            }
        }

        public virtual double Score
        {
            get { return Rank; }
        }

        public UniqueID UniqueID { get; private set; } = UniqueID.NewID();

        protected readonly Settings settings;
        protected readonly TrackerModule module;

        public RankedObject(Settings settings, TrackerModule module)
        {
            this.settings = settings;
            this.module = module;
        }

        public void Validate()
        {
            try
            {
                ValidateFields();
            }
            catch (ValidationException e)
            {
                module.Logger?.Log(e.GetType().Name + ": " + e.Message + " - invalid value: " + e.InvalidValue.ToString());
                throw;
            }
            PostValidate();
        }

        protected virtual void ValidateFields()
        {
            if (Name == "")
                throw new ValidationException("A name is required", Name);
            if (Name.Length > MaxLengthName)
                throw new ValidationException("Name cannot be longer than " + MaxLengthName.ToString() + " characters", Name);
            if (Comment.Length > MaxLengthComment)
                throw new ValidationException("Comment cannot be longer than " + MaxLengthComment.ToString() + " characters", Comment);
        }

        protected virtual void PostValidate() { }

        public void Save()
        {
            module.SaveModelObject(this);
        }

        public virtual void PostSave() { }

        public void Delete()
        {
            module.DeleteModelObject(this);
        }

        public virtual void PostDelete() { }

        public virtual bool RemoveReferenceToObject(IKeyable obj, Type type)
        {
            return false;
        }

        public void MoveUpOneRank()
        {
            module.Logger?.Log("MoveUpOneRank - " + UniqueID.ToString());
            if (Rank <= 1)
            {
                try
                {
                    throw new InvalidObjectStateException("Cannot raise rank any further");
                }
                catch (InvalidObjectStateException e)
                {
                    module.Logger?.Log(e.GetType().Name + ": " + e.Message);
                    throw;
                }
            }
            ChangeRank(Rank - 1);
        }

        public void MoveDownOneRank()
        {
            module.Logger?.Log("MoveDownOneRank - " + UniqueID.ToString());
            if (Rank >= module.TotalNumModelObjects())
            {
                try
                {
                    throw new InvalidObjectStateException("Cannot lower rank any further");
                }
                catch (InvalidObjectStateException e)
                {
                    module.Logger?.Log(e.GetType().Name + ": " + e.Message);
                    throw;
                }
            }
            ChangeRank(Rank + 1);
        }

        public void ChangeRank(int newRank)
        {
            module.Logger?.Log("ChangeRank - " + UniqueID.ToString());
            if (newRank < 1 || newRank > module.TotalNumModelObjects())
            {
                try
                {
                    throw new InvalidObjectStateException("Rank out of range: " + newRank.ToString());
                }
                catch (InvalidObjectStateException e)
                {
                    module.Logger?.Log(e.GetType().Name + ": " + e.Message);
                    throw;
                }
            }
            module.ChangeModelObjectPositionInList(this, newRank - 1);
        }

        public virtual void ApplySettingsChanges(Settings settings) { }

        public override SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = base.LoadIntoRepresentation();
            sr.SaveValue("UniqueID", new ValueContainer(UniqueID));
            sr.SaveValue("Name", new ValueContainer(Name));
            sr.SaveValue("Comment", new ValueContainer(Comment));
            sr.SaveValue("Rank", new ValueContainer(Rank));
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
                    default:
                        break;
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is RankedObject)) return false;
            RankedObject other = (RankedObject)obj;
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
