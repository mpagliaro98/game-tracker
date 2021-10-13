using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Model
{
    public class RatingCategory : ISavable
    {
        private string name = "";
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string comment = "";
        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }

        protected double weight = 1.0;
        public double Weight
        {
            get { return weight; }
        }

        public RatingCategory() { }

        public RatingCategory(string name, string comment)
        {
            Name = name;
            Comment = comment;
        }

        public virtual SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = new SavableRepresentation();
            sr.SaveValue("name", name);
            sr.SaveValue("comment", comment);
            sr.SaveValue("weight", weight);
            return sr;
        }

        public virtual void RestoreFromRepresentation(SavableRepresentation sr)
        {
            if (sr == null) return;
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "name":
                        name = sr.GetString(key);
                        break;
                    case "comment":
                        comment = sr.GetString(key);
                        break;
                    case "weight":
                        weight = sr.GetDouble(key);
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine("RatingCategory.cs RestoreFromRepresentation: unrecognized key " + key);
                        break;
                }
            }
        }
    }
}
