using RatableTracker.Exceptions;
using RatableTracker.Interfaces;
using RatableTracker.Model;
using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace RatableTracker.ListManipulation.Filtering
{
    public enum FilterType : byte
    {
        Text = 1,
        List = 2,
        Date = 3,
        Boolean = 4,
        Numeric = 5
    }

    public abstract class FilterOptionBase : IFilterOption
    {
        public TrackerModule Module { get; set; }
        public Settings Settings { get; set; }

        public abstract string Name { get; }
        public abstract FilterType FilterType { get; }

        public FilterOptionBase() { }

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
            if (obj is not FilterOptionBase other) return false;
            return GetType().Equals(other.GetType()) && Name == other.Name;
        }

        protected internal abstract void ValidateFilterValues(object filterValues);

        protected internal virtual void SetNonSerializableFields(TrackerModule module, Settings settings)
        {
            Module = module;
            Settings = settings;
        }

        public virtual IList<IFilterOption> InstantiateManually()
        {
            throw new NotImplementedException();
        }

        protected internal virtual void SerializeExtraInformation(XmlWriter writer)
        {

        }

        protected internal virtual void DeserializeExtraInformation(XmlReader reader)
        {

        }

        public FilterOptionBase Copy()
        {
            // write this filter option to an XML string
            var temp = new FilterSegment() { FilterOption = this };
            string copyXML;
            var serializer = new XmlSerializer(typeof(FilterSegment));
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, temp);
                copyXML = writer.ToString();
            }

            // convert that XML string back into a new instance of this object
            using (var reader = new StringReader(copyXML))
            {
                temp = (FilterSegment)serializer.Deserialize(reader);
            }
            FilterOptionBase copyObject = temp.FilterOption;
            copyObject.SetNonSerializableFields(Module, Settings);
            return copyObject;
        }
    }
}
