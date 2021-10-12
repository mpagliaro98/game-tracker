using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Model
{
    public class RatableObjectCompletable : RatableObject
    {
        private CompletionStatus completionStatus;
        public CompletionStatus CompletionStatus
        {
            get { return completionStatus; }
            set { completionStatus = value; }
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
            base.RestoreFromRepresentation(sr);
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "completionStatus":
                        completionStatus = sr.GetISavable<CompletionStatus>(key);
                        break;
                    default:
                        Console.WriteLine("RatableObjectCompletable.cs RestoreFromRepresentation: unrecognized key " + key);
                        break;
                }
            }
        }
    }
}
