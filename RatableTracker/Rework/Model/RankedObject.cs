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
    public class RankedObject : IKeyable, ISavable
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
            // TODO figure out saving architecture
        }

        public void MoveUpOneRank()
        {
            // TODO move position in list and save
        }

        public void MoveDownOneRank()
        {
            // TODO move position in list and save
        }

        public void ChangeRank(int newRank)
        {
            // TODO change rank and save
        }

        public virtual SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = new SavableRepresentation();
            sr.SaveValue("TypeName", new ValueContainer(GetType().Name));
            sr.SaveValue("UniqueID", new ValueContainer(UniqueID));
            sr.SaveValue("Name", new ValueContainer(Name));
            sr.SaveValue("Comment", new ValueContainer(Comment));
            sr.SaveValue("Rank", new ValueContainer(Rank));
            return sr;
        }

        public virtual void RestoreFromRepresentation(SavableRepresentation sr)
        {
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
