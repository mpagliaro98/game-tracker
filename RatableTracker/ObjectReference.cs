using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Interfaces;
using RatableTracker.Framework.LoadSave;

namespace RatableTracker.Framework
{
    public class ObjectReference : ISavable
    {
        private Guid objectKey = Guid.Empty;
        public Guid ObjectKey => objectKey;

        public ObjectReference() { }

        public ObjectReference(bool init = false)
        {
            if (init) objectKey = Guid.NewGuid();
        }

        public ObjectReference(Guid key)
        {
            objectKey = key;
        }

        public ObjectReference(IReferable referable)
        {
            SetReference(referable);
        }

        public SavableRepresentation<T> LoadIntoRepresentation<T>() where T : IValueContainer<T>, new()
        {
            SavableRepresentation<T> sr = new SavableRepresentation<T>();
            sr.SaveValue("objectKey", objectKey);
            return sr;
        }

        public void RestoreFromRepresentation<T>(SavableRepresentation<T> sr) where T : IValueContainer<T>, new()
        {
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "objectKey":
                        objectKey = sr.GetGuid(key);
                        break;
                    default:
                        break;
                }
            }
        }

        public bool IsReferencedObject(IReferable obj)
        {
            return Equals(obj.ReferenceKey);
        }

        public void SetReference(IReferable obj)
        {
            objectKey = obj.ReferenceKey.ObjectKey;
        }

        public void ClearReference()
        {
            objectKey = Guid.Empty;
        }

        public bool HasReference()
        {
            return !objectKey.Equals(Guid.Empty);
        }

        public override int GetHashCode()
        {
            return objectKey.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(ObjectReference)) return false;
            ObjectReference key = (ObjectReference)obj;
            return objectKey.Equals(key.objectKey);
        }

        public override string ToString()
        {
            return ObjectKey.ToString("N");
        }

        public static explicit operator ObjectReference(string str)
        {
            if (str == null) return null;
            Guid guid = new Guid(str);
            return new ObjectReference(guid);
        }
    }
}
