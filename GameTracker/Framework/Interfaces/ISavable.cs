using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.LoadSave;

namespace RatableTracker.Framework.Interfaces
{
    public interface ISavable
    {
        SavableRepresentation<T> LoadIntoRepresentation<T>() where T : IValueContainer<T>, new();
        void RestoreFromRepresentation<T>(SavableRepresentation<T> sr) where T : IValueContainer<T>, new();
    }
}
