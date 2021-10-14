using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Interfaces;
using RatableTracker.Framework.LoadSave;

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
        public string ScoreRelationshipName
        {
            get { return scoreRelationshipName; }
            set { scoreRelationshipName = value; }
        }

        private IEnumerable<int> valueList;

        private Guid referenceKey = Guid.NewGuid();
        public Guid ReferenceKey
        {
            get { return referenceKey; }
        }

        private RatingModule parentModule;
        
        public ScoreRange()
        {
            referenceKey = Guid.NewGuid();
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

        public RatingModule GetParentModule()
        {
            return parentModule;
        }

        public void SetParentModule(RatingModule parentModule)
        {
            this.parentModule = parentModule;
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
