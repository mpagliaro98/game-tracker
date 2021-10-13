using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Model
{
    public class ScoreRange : ISavable, IModuleAccess
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

        private RatingModule parentModule;
        
        public ScoreRange() { }

        public SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = new SavableRepresentation();
            sr.SaveValue("name", name);
            sr.SaveValue("scoreRelationshipName", scoreRelationshipName);
            sr.SaveListGeneric("valueList", valueList);
            return sr;
        }

        public void RestoreFromRepresentation(SavableRepresentation sr)
        {
            if (sr == null) return;
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "name":
                        name = sr.GetValue(key);
                        break;
                    case "scoreRelationshipName":
                        scoreRelationshipName = sr.GetValue(key);
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
    }
}
