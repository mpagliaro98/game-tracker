using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using RatableTracker.Framework.Interfaces;
using RatableTracker.Framework.LoadSave;
using RatableTracker.Framework.ScoreRelationships;

namespace RatableTracker.Framework
{
    public class ScoreRange : ISavable, IReferable
    {
        private string name = "";
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private ObjectReference scoreRelationship = new ObjectReference();
        public ObjectReference RefScoreRelationship
        {
            get { return scoreRelationship; }
        }

        private IEnumerable<double> valueList = new List<double>();
        public IEnumerable<double> ValueList
        {
            get { return valueList; }
            set { valueList = value; }
        }

        private Color color = new Color();
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        private Guid referenceKey = Guid.NewGuid();
        public Guid ReferenceKey
        {
            get { return referenceKey; }
        }

        public ScoreRange() { }

        public virtual SavableRepresentation<T> LoadIntoRepresentation<T>() where T : IValueContainer<T>, new()
        {
            SavableRepresentation<T> sr = new SavableRepresentation<T>();
            sr.SaveValue("referenceKey", referenceKey);
            sr.SaveValue("name", name);
            sr.SaveValue("scoreRelationship", scoreRelationship);
            sr.SaveList("valueList", valueList);
            sr.SaveValue("color", color);
            return sr;
        }

        public virtual void RestoreFromRepresentation<T>(SavableRepresentation<T> sr) where T : IValueContainer<T>, new()
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
                    case "scoreRelationship":
                        scoreRelationship = sr.GetISavable<ObjectReference>(key);
                        break;
                    case "valueList":
                        valueList = sr.GetListOfType<double>(key);
                        break;
                    case "color":
                        color = sr.GetColor(key);
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine("ScoreRange.cs RestoreFromRepresentation: unrecognized key " + key);
                        break;
                }
            }
        }

        public void SetScoreRelationship<T>(T obj) where T : ScoreRelationship, IReferable
        {
            scoreRelationship.SetReference(obj);
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
