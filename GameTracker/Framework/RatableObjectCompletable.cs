using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.LoadSave;

namespace RatableTracker.Framework
{
    public class RatableObjectCompletable : RatableObject
    {
        private ObjectReference completionStatus = new ObjectReference();
        public CompletionStatus CompletionStatus
        {
            get
            {
                return completionStatus.HasReference() ? ((RatingModuleCompletable)GetParentModule()).FindCompletionStatus(completionStatus) : null;
            }
            set { completionStatus.SetReference(value); }
        }

        public RatableObjectCompletable() : base() { }

        public override SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = base.LoadIntoRepresentation();
            sr.SaveValue("completionStatus", completionStatus);
            return sr;
        }

        public override void RestoreFromRepresentation(SavableRepresentation sr)
        {
            if (sr == null) return;
            base.RestoreFromRepresentation(sr);
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "completionStatus":
                        completionStatus = sr.GetISavable<ObjectReference>(key);
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine("RatableObjectCompletable.cs RestoreFromRepresentation: unrecognized key " + key);
                        break;
                }
            }
        }

        public void RemoveCompletionStatus()
        {
            completionStatus.ClearReference();
        }
    }
}
