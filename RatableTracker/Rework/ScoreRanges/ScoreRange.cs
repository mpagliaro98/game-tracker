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
using System.Xml.Linq;

namespace RatableTracker.Rework.ScoreRanges
{
    public class ScoreRange : SavableObject, IKeyable
    {
        public static int MaxLengthName => 200;

        public string Name { get; set; } = "";
        public IList<double> ValueList { get; set; } = new List<double>();
        public Color Color { get; set; } = new Color();

        private UniqueID _scoreRelationship = new UniqueID(false);
        public ScoreRelationship ScoreRelationship
        {
            get
            {
                if (!_scoreRelationship.HasValue()) return null;
                return Util.Util.FindObjectInList(module.GetScoreRelationshipList(), _scoreRelationship);
            }
            set
            {
                _scoreRelationship = value.UniqueID;
            }
        }

        private UniqueID _uniqueID = new UniqueID();
        public UniqueID UniqueID { get { return _uniqueID; } }

        protected readonly TrackerModuleScores module;

        public ScoreRange(TrackerModuleScores module)
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
            ScoreRelationship sr = ScoreRelationship;
            if (sr == null)
                throw new Exception("A score relationship is required");
            if (sr.NumValuesRequired != ValueList.Count())
                throw new Exception("The " + sr.Name + " relationship requires " + sr.NumValuesRequired.ToString() + " values, but " + ValueList.Count().ToString() + " were given");
        }

        public void Save()
        {
            module.SaveScoreRange(this);
        }

        public void Delete()
        {
            module.DeleteScoreRange(this);
        }

        public virtual bool RemoveReferenceToObject(IKeyable obj, Type type)
        {
            return false;
        }

        public override SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = base.LoadIntoRepresentation();
            sr.SaveValue("UniqueID", new ValueContainer(UniqueID));
            sr.SaveValue("Name", new ValueContainer(Name));
            sr.SaveValue("ValueList", new ValueContainer(ValueList));
            sr.SaveValue("Color", new ValueContainer(Color));
            sr.SaveValue("ScoreRelationship", new ValueContainer(_scoreRelationship));
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
                    case "ValueList":
                        ValueList = sr.GetValue(key).GetDoubleList().ToList();
                        break;
                    case "Color":
                        Color = sr.GetValue(key).GetColor();
                        break;
                    case "ScoreRelationship":
                        _scoreRelationship = sr.GetValue(key).GetUniqueID();
                        break;
                    default:
                        break;
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is ScoreRange)) return false;
            ScoreRange other = (ScoreRange)obj;
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
