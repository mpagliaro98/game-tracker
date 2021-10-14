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
        public Guid ObjectKey
        {
            get { return objectKey; }
        }

        public ObjectReference() { }

        public ObjectReference(IReferable referable)
        {
            SetReference(referable);
        }

        public SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = new SavableRepresentation();
            sr.SaveValue("objectKey", objectKey);
            return sr;
        }

        public void RestoreFromRepresentation(SavableRepresentation sr)
        {
            if (sr == null) return;
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "objectKey":
                        objectKey = sr.GetGuid(key);
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine("ObjectReference.cs RestoreFromRepresentation: unrecognized key " + key);
                        break;
                }
            }
        }

        public bool IsReferencedObject(IReferable obj)
        {
            return obj.ReferenceKey.Equals(ObjectKey);
        }

        public void SetReference(IReferable obj)
        {
            objectKey = obj.ReferenceKey;
        }
    }
}
