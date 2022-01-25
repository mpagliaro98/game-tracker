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
        public static int MaxLengthName => 200;

        public string Name { get; set; } = "";

        private ObjectReference scoreRelationship = new ObjectReference();
        public ObjectReference RefScoreRelationship => scoreRelationship;

        public IEnumerable<double> ValueList { get; set; } = new List<double>();

        public Color Color { get; set; } = new Color();

        private ObjectReference referenceKey = new ObjectReference(true);
        public ObjectReference ReferenceKey => referenceKey;

        public ScoreRange() { }

        public virtual SavableRepresentation<T> LoadIntoRepresentation<T>() where T : IValueContainer<T>, new()
        {
            SavableRepresentation<T> sr = new SavableRepresentation<T>();
            sr.SaveValue("referenceKey", referenceKey);
            sr.SaveValue("name", Name);
            sr.SaveValue("scoreRelationship", scoreRelationship);
            sr.SaveList("valueList", ValueList);
            sr.SaveValue("color", Color);
            return sr;
        }

        public virtual void RestoreFromRepresentation<T>(SavableRepresentation<T> sr) where T : IValueContainer<T>, new()
        {
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "referenceKey":
                        referenceKey = sr.GetISavable<ObjectReference>(key);
                        break;
                    case "name":
                        Name = sr.GetString(key);
                        break;
                    case "scoreRelationship":
                        scoreRelationship = sr.GetISavable<ObjectReference>(key);
                        break;
                    case "valueList":
                        ValueList = sr.GetListOfType<double>(key);
                        break;
                    case "color":
                        Color = sr.GetColor(key);
                        break;
                    default:
                        break;
                }
            }
        }

        public void SetScoreRelationship<T>(T obj) where T : ScoreRelationship, IReferable
        {
            scoreRelationship.SetReference(obj);
        }

        public void OverwriteReferenceKey(IReferable orig)
        {
            if (orig is ScoreRange origRange)
                referenceKey = origRange.referenceKey;
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
