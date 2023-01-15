using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.LoadSave;
using RatableTracker.Rework.Model;
using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.ObjAddOns
{
    public class Status : IKeyable, ISavable
    {
        public static int MaxLengthName => 200;

        public string Name { get; set; } = "";
        public Color Color { get; set; } = new Color();

        public UniqueID UniqueID => new UniqueID();

        public Status() { }

        public virtual void LoadIntoRepresentation(ref SavableRepresentation<ValueContainer> sr)
        {
            // TODO load into representation (use attributes?)
        }

        public virtual void RestoreFromRepresentation(SavableRepresentation<ValueContainer> sr)
        {
            // TODO get from representation (use attributes?)
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is Status)) return false;
            Status other = (Status)obj;
            return UniqueID.Equals(other.UniqueID);
        }

        public override int GetHashCode()
        {
            return UniqueID.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
