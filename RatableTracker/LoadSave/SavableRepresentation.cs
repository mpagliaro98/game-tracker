using RatableTracker.Interfaces;
using RatableTracker.LoadSave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.LoadSave
{
    public class SavableRepresentation
    {
        protected readonly IDictionary<string, ValueContainer> values;

        public SavableRepresentation()
        {
            values = new Dictionary<string, ValueContainer>();
        }

        public void SaveValue(string key, ValueContainer value)
        {
            values[key] = value;
        }

        public ValueContainer GetValue(string key)
        {
            return values[key];
        }

        public IEnumerable<string> GetAllSavedKeys()
        {
            return values.Keys;
        }
    }
}
