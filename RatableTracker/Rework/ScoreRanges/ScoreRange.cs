using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.LoadSave;
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
    public class ScoreRange : IKeyable, ISavable
    {
        public static int MaxLengthName => 200;

        public string Name { get; set; } = "";
        public IEnumerable<double> ValueList { get; set; } = new List<double>();
        public Color Color { get; set; } = new Color();

        private UniqueID _scoreRelationship = new UniqueID(false);
        public ScoreRelationship ScoreRelationship
        {
            get
            {
                if (!_scoreRelationship.HasValue()) return null;
                return TrackerModule.FindObjectInList(module.GetScoreRelationshipList(), _scoreRelationship);
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

        public virtual SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = new SavableRepresentation();
            sr.SaveValue("TypeName", new ValueContainer(GetType().Name));
            sr.SaveValue("UniqueID", new ValueContainer(UniqueID));
            sr.SaveValue("Name", new ValueContainer(Name));
            sr.SaveValue("ValueList", new ValueContainer(ValueList));
            sr.SaveValue("Color", new ValueContainer(Color));
            sr.SaveValue("ScoreRelationship", new ValueContainer(_scoreRelationship));
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
                    case "ValueList":
                        ValueList = sr.GetValue(key).GetDoubleList();
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
