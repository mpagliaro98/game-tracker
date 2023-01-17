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

        private UniqueID _uniqueID = new UniqueID();
        public UniqueID UniqueID { get { return _uniqueID; } }

        protected readonly Settings settings;
        protected readonly TrackerModule module;

        public RankedObject(Settings settings, TrackerModule module)
        {
            this.settings = settings;
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
            module.SaveModelObject(this);
        }

        public void Delete()
        {
            module.DeleteModelObject(this);
        }

        public void MoveUpOneRank()
        {
            // TODO throw unique exception
            if (Rank <= 1)
                throw new Exception("Cannot raise rank any further");
            ChangeRank(Rank - 1);
        }

        public void MoveDownOneRank()
        {
            // TODO throw unique exception
            if (Rank >= module.TotalNumModelObjects())
                throw new Exception("Cannot lower rank any further");
            ChangeRank(Rank + 1);
        }

        public void ChangeRank(int newRank)
        {
            // TODO throw unique exception
            if (newRank < 1 || newRank > module.TotalNumModelObjects())
                throw new Exception("Rank out of range");
            module.ChangeModelObjectPositionInList(this, newRank - 1);
        }

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
                        _uniqueID = sr.GetValue(key).GetUniqueID();
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
