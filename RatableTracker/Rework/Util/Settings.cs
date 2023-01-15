using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.LoadSave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Util
{
    public class Settings : ISavable
    {
        public Settings() { }

        public virtual void LoadIntoRepresentation(ref SavableRepresentation<ValueContainer> sr)
        {
            // TODO load into representation (use attributes?)
        }

        public virtual void RestoreFromRepresentation(SavableRepresentation<ValueContainer> sr)
        {
            // TODO get from representation (use attributes?)
        }
    }
}
