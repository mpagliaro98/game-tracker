using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RatableTracker.ListManipulation.Sorting
{
    public abstract class SortOptionBase : ISortOption
    {
        public TrackerModule Module { get; set; }
        public Settings Settings { get; set; }

        public abstract string Name { get; }

        public SortOptionBase() { }

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is not SortOptionBase other) return false;
            return GetType().Equals(other.GetType()) && Name == other.Name;
        }

        protected internal virtual void SetNonSerializableFields(TrackerModule module, Settings settings)
        {
            Module = module;
            Settings = settings;
        }

        public virtual IList<ISortOption> InstantiateManually()
        {
            throw new NotImplementedException();
        }

        protected internal virtual void SerializeExtraInformation(XmlWriter writer)
        {

        }

        protected internal virtual void DeserializeExtraInformation(XmlReader reader)
        {

        }
    }
}
