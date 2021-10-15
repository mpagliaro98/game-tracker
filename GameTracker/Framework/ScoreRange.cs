using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Interfaces;
using RatableTracker.Framework.LoadSave;
using RatableTracker.Framework.ScoreRelationships;

namespace RatableTracker.Framework
{
    public class ScoreRange : ISavable, IModuleAccess, IReferable
    {
        private string name = "";
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string scoreRelationshipName = "";
        public ScoreRelationship ScoreRelationship
        {
            get { return parentModule.FindScoreRelationship(scoreRelationshipName); }
            set { scoreRelationshipName = value.Name; }
        }

        private IEnumerable<int> valueList;
        public IEnumerable<int> ValueList
        {
            get { return valueList; }
            set { valueList = value; }
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

        public ScoreRange() { }

        public ScoreRange(RatingModule parentModule, string name, IEnumerable<int> valueList, ScoreRelationship sr)
        {
            this.parentModule = parentModule;
            this.name = name;
            this.valueList = valueList;
            scoreRelationshipName = sr.Name;
        }

        public SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = new SavableRepresentation();
            sr.SaveValue("referenceKey", referenceKey);
            sr.SaveValue("name", name);
            sr.SaveValue("scoreRelationshipName", scoreRelationshipName);
            sr.SaveList("valueList", valueList);
            return sr;
        }

        public void RestoreFromRepresentation(SavableRepresentation sr)
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
                    case "scoreRelationshipName":
                        scoreRelationshipName = sr.GetString(key);
                        break;
                    case "valueList":
                        valueList = sr.GetListOfType<int>(key);
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine("ScoreRange.cs RestoreFromRepresentation: unrecognized key " + key);
                        break;
                }
            }
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
                ScoreRange o = (ScoreRange)obj;
                return !ReferenceKey.Equals(Guid.Empty) && ReferenceKey.Equals(o.ReferenceKey);
            }
        }
    }
}
