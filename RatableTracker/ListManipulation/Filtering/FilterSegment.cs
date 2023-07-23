using RatableTracker.Exceptions;
using RatableTracker.Model;
using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace RatableTracker.ListManipulation.Filtering
{
    [Serializable]
    public sealed class FilterSegment : IXmlSerializable
    {
        [XmlIgnore] public TrackerModule Module { get; set; }
        [XmlIgnore] public Settings Settings { get; set; }

        [XmlIgnore] public FilterOptionBase FilterOption { get; set; } = null;
        public object FilterValues { get; set; } = null;
        public bool Negate { get; set; } = false;

        public FilterSegment() { }

        public Func<T, bool> GetFilterExpression<T>()
        {
            try
            {
                FilterOption.ValidateFilterValues(FilterValues);
                IFilterOptionAction<T> action = (IFilterOptionAction<T>)FilterOption;
                var expr = action.GenerateFilterExpression();
                if (Negate)
                    return (obj) => !expr(obj);
                else
                    return expr;
            }
            catch (InvalidCastException ex)
            {
                throw new ListManipulationException("Filter option \"" + FilterOption.Name + "\" does not implement the IFilterOptionAction interface, or an invalid type is being used", null, ex);
            }
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            // custom deserialization to handle loading of FilterOption
            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == nameof(FilterSegment))
            {
                if (reader.ReadToDescendant(nameof(FilterOption)))
                {
                    if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == nameof(FilterOption))
                    {
                        string typeName = reader.GetAttribute("Type");
                        string assemblyName = reader.GetAttribute("Assembly");
                        Type type = Type.GetType(typeName + ", " + assemblyName);
                        FilterOption = (FilterOptionBase)Activator.CreateInstance(type);
                        FilterOption.SetNonSerializableFields(Module, Settings);
                        FilterOption.DeserializeExtraInformation(reader);
                    }
                    reader.Read();
                    if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == nameof(FilterValues))
                    {
                        var overrides = new XmlAttributeOverrides();
                        overrides.Add(typeof(object), new XmlAttributes { XmlRoot = new XmlRootAttribute(nameof(FilterValues)) });
                        var serializer = new XmlSerializer(typeof(object), overrides, new[] { typeof(List<string>) }, null, null);
                        FilterValues = serializer.Deserialize(reader);
                    }
                    if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == nameof(Negate))
                    {
                        Negate = bool.Parse(reader.ReadElementContentAsString());
                    }
                    reader.Read();
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            // custom serialization to handle saving of FilterOption
            writer.WriteStartElement(nameof(FilterOption));
            writer.WriteAttributeString("Type", FilterOption.GetType().FullName);
            writer.WriteAttributeString("Assembly", FilterOption.GetType().Assembly.GetName().Name);
            FilterOption.SerializeExtraInformation(writer);
            writer.WriteEndElement();

            var overrides = new XmlAttributeOverrides();
            overrides.Add(typeof(object), new XmlAttributes { XmlRoot = new XmlRootAttribute(nameof(FilterValues)) });
            var serializer = new XmlSerializer(typeof(object), overrides, new[] { typeof(List<string>) }, null, null);
            serializer.Serialize(writer, FilterValues);

            writer.WriteElementString(nameof(Negate), Negate.ToString());
        }

        public void SetNonSerializableFields(TrackerModule module, Settings settings)
        {
            Module = module;
            Settings = settings;
            FilterOption?.SetNonSerializableFields(module, settings);
        }
    }
}
