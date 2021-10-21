using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using RatableTracker.Framework.Interfaces;
using RatableTracker.Framework.LoadSave;

namespace RatableTracker.Framework
{
    public class Status : ISavable, IReferable
    {
        public string Name { get; set; } = "";
        public Color Color { get; set; } = new Color();

        private Guid referenceKey = Guid.NewGuid();
        public Guid ReferenceKey => referenceKey;

        public Status() { }

        public virtual SavableRepresentation<T> LoadIntoRepresentation<T>() where T : IValueContainer<T>, new()
        {
            SavableRepresentation<T> sr = new SavableRepresentation<T>();
            sr.SaveValue("referenceKey", referenceKey);
            sr.SaveValue("name", Name);
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
                        referenceKey = sr.GetGuid(key);
                        break;
                    case "name":
                        Name = sr.GetString(key);
                        break;
                    case "color":
                        Color = sr.GetColor(key);
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine(GetType().Name + " RestoreFromRepresentation: unrecognized key " + key);
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
                Status o = (Status)obj;
                return !ReferenceKey.Equals(Guid.Empty) && ReferenceKey.Equals(o.ReferenceKey);
            }
        }
    }
}
