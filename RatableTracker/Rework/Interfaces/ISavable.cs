using RatableTracker.Rework.LoadSave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Interfaces
{
    public interface ISavable
    {
        void LoadIntoRepresentation(ref SavableRepresentation<ValueContainer> sr);
        void RestoreFromRepresentation(SavableRepresentation<ValueContainer> sr);
    }
}
