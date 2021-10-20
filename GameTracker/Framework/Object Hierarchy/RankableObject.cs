using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Interfaces;
using RatableTracker.Framework.LoadSave;
using RatableTracker.Framework.Global;

namespace RatableTracker.Framework.ObjectHierarchy
{
    public class RankableObject : ISavable, IReferable
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

        private Guid referenceKey = Guid.NewGuid();
        public Guid ReferenceKey
        {
            get { return referenceKey; }
        }

        public RankableObject() { }

        public virtual SavableRepresentation<T> LoadIntoRepresentation<T>() where T : IValueContainer<T>, new()
        {
            SavableRepresentation<T> sr = new SavableRepresentation<T>();
            sr.SaveValue("referenceKey", referenceKey);
            sr.SaveValue("name", name);
            sr.SaveValue("comment", comment);
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
                        name = sr.GetString(key);
                        break;
                    case "comment":
                        comment = sr.GetString(key);
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
                RankableObject o = (RankableObject)obj;
                return !ReferenceKey.Equals(Guid.Empty) && ReferenceKey.Equals(o.ReferenceKey);
            }
        }
    }
}
