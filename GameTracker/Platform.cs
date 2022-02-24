using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using RatableTracker.Framework.Interfaces;
using RatableTracker.Framework.LoadSave;
using RatableTracker.Framework;

namespace GameTracker.Model
{
    public class Platform : ISavable, IReferable
    {
        public static int MaxLengthName => 200;
        public static int MaxLengthAbbreviation => 10;

        public string Name { get; set; } = "";

        public string Abbreviation { get; set; } = "";

        public int ReleaseYear { get; set; } = 0;

        public int AcquiredYear { get; set; } = 0;

        private ObjectReference referenceKey = new ObjectReference(true);
        public ObjectReference ReferenceKey => referenceKey;

        public Color Color { get; set; } = new Color();

        public Platform() { }

        public SavableRepresentation<T> LoadIntoRepresentation<T>() where T : IValueContainer<T>, new()
        {
            SavableRepresentation<T> sr = new SavableRepresentation<T>();
            sr.SaveValue("referenceKey", referenceKey);
            sr.SaveValue("name", Name);
            sr.SaveValue("color", Color);
            sr.SaveValue("releaseYear", ReleaseYear);
            sr.SaveValue("acquiredYear", AcquiredYear);
            sr.SaveValue("abbreviation", Abbreviation);
            return sr;
        }

        public void RestoreFromRepresentation<T>(SavableRepresentation<T> sr) where T : IValueContainer<T>, new()
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
                    case "color":
                        Color = sr.GetColor(key);
                        break;
                    case "releaseYear":
                        ReleaseYear = sr.GetInt(key);
                        break;
                    case "acquiredYear":
                        AcquiredYear = sr.GetInt(key);
                        break;
                    case "abbreviation":
                        Abbreviation = sr.GetString(key);
                        break;
                    default:
                        break;
                }
            }
        }

        public void OverwriteReferenceKey(IReferable orig)
        {
            if (orig is Platform origPlatform)
                referenceKey = origPlatform.referenceKey;
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
                Platform o = (Platform)obj;
                return !ReferenceKey.Equals(Guid.Empty) && ReferenceKey.Equals(o.ReferenceKey);
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
