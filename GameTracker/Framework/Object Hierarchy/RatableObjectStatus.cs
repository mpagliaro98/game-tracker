using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.LoadSave;
using RatableTracker.Framework.Interfaces;

namespace RatableTracker.Framework.ObjectHierarchy
{
    public class RatableObjectStatus : RatableObject, IObjectStatus
    {
        private ObjectReference status = new ObjectReference();
        public ObjectReference RefStatus => status;

        public RatableObjectStatus() : base() { }

        public override SavableRepresentation<T> LoadIntoRepresentation<T>()
        {
            SavableRepresentation<T> sr = base.LoadIntoRepresentation<T>();
            sr.SaveValue("status", status);
            return sr;
        }

        public override void RestoreFromRepresentation<T>(SavableRepresentation<T> sr)
        {
            base.RestoreFromRepresentation(sr);
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "status":
                        status = sr.GetISavable<ObjectReference>(key);
                        break;
                    default:
                        break;
                }
            }
        }

        public void SetStatus<T>(T obj) where T : Status, IReferable
        {
            status.SetReference(obj);
        }

        public void RemoveStatus()
        {
            status.ClearReference();
        }
    }
}
