using RatableTracker.Rework.LoadSave;
using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RatableTracker.Rework.Model
{
    public abstract class SavableObject
    {
        public virtual SavableRepresentation LoadIntoRepresentation()
        {
            return new SavableRepresentation();
        }

        public virtual void RestoreFromRepresentation(SavableRepresentation sr)
        {
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    default:
                        break;
                }
            }
        }
    }
}
