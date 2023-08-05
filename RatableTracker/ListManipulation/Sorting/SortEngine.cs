using RatableTracker.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml;
using System.Xml.Serialization;
using RatableTracker.Modules;
using RatableTracker.Util;

namespace RatableTracker.ListManipulation.Sorting
{
    public enum SortMode
    {
        Ascending = 0,
        Descending = 1
    }

    [Serializable]
    public class SortEngine : IXmlSerializable
    {
        [XmlIgnore] public TrackerModule Module { get; set; }
        [XmlIgnore] public Settings Settings { get; set; }

        [XmlIgnore] public SortOptionBase DefaultSortOption { get; set; } = null;
        public SortOptionBase SortOption { get; set; } = null;
        public SortMode SortMode { get; set; } = SortMode.Ascending;

        public IList<T> ApplySorting<T>(IList<T> list)
        {
            try
            {
                if (SortOption == null)
                {
                    if (SortMode == SortMode.Descending)
                        list = list.Reverse().ToList();
                    return list;
                }
                ISortOptionAction<T> action = (ISortOptionAction<T>)SortOption;
                var expr = action.GenerateSortExpression();
                if (SortMode == SortMode.Descending)
                    return ApplyDefaultSorting(list).OrderByDescending(expr).ToList();
                else
                    return ApplyDefaultSorting(list).OrderBy(expr).ToList();
            }
            catch (InvalidCastException ex)
            {
                throw new ListManipulationException("Sort option \"" + SortOption.Name + "\" does not implement the ISortOptionAction interface, or an invalid type is being used", null, ex);
            }
        }

        private IList<T> ApplyDefaultSorting<T>(IList<T> list)
        {
            if (DefaultSortOption == null)
                return list;
            else
                return list.OrderBy(((ISortOptionAction<T>)DefaultSortOption).GenerateSortExpression()).ToList();
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            // custom deserialization to handle loading of SortOption
            if (reader.ReadToDescendant(nameof(SortOption)))
            {
                if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == nameof(SortOption))
                {
                    bool isNull = bool.Parse(reader.GetAttribute("Null"));
                    if (isNull)
                    {
                        SortOption = null;
                    }
                    else
                    {
                        string typeName = reader.GetAttribute("Type");
                        string assemblyName = reader.GetAttribute("Assembly");
                        Type type = Type.GetType(typeName + ", " + assemblyName);
                        SortOption = (SortOptionBase)Activator.CreateInstance(type);
                        SortOption.SetNonSerializableFields(Module, Settings);
                        SortOption.DeserializeExtraInformation(reader);
                    }
                }
                reader.Read();
                if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == nameof(SortMode))
                {
                    SortMode = Enum.Parse<SortMode>(reader.ReadElementContentAsString());
                }
                reader.Read();
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            // custom serialization to handle saving of SortOption
            writer.WriteStartElement(nameof(SortOption));
            writer.WriteAttributeString("Type", SortOption?.GetType().FullName);
            writer.WriteAttributeString("Assembly", SortOption?.GetType().Assembly.GetName().Name);
            writer.WriteAttributeString("Null", SortOption == null ? "True" : "False");
            SortOption?.SerializeExtraInformation(writer);
            writer.WriteEndElement();

            writer.WriteElementString(nameof(SortMode), SortMode.ToString());
        }

        public void SetNonSerializableFields(TrackerModule module, Settings settings, SortOptionBase defaultSort)
        {
            Module = module;
            Settings = settings;
            DefaultSortOption = defaultSort;
            SortOption?.SetNonSerializableFields(module, settings);
        }

        public static IList<ISortOption> GetSortOptionList<T>(TrackerModule module, Settings settings, ICollection<Type> exclude = null)
        {
            // use reflection to find and instantiate all objects with SortOptionAttribute and InAutoList=true with the type of T or an ancestor
            exclude ??= new List<Type>();
            Type currentType = typeof(T);
            var listTypes = new List<Type>();
            var listTypesManual = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var t in assembly.GetTypes())
                {
                    var attributes = t.GetCustomAttributes(typeof(SortOptionAttribute), false);
                    if (attributes != null && attributes.Length > 0)
                    {
                        foreach (var attr in attributes.Cast<SortOptionAttribute>())
                        {
                            if (attr.InAutoList)
                            {
                                if (exclude.Contains(t))
                                    continue;
                                if (currentType.Equals(attr.ExpectedType) || currentType.IsSubclassOf(attr.ExpectedType) || currentType.GetInterfaces().Contains(attr.ExpectedType))
                                {
                                    if (attr.InstantiateManually)
                                        listTypesManual.Add(t);
                                    else
                                        listTypes.Add(t);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            var listOptions = new List<ISortOption>();
            foreach (var t in listTypes)
            {
                ISortOption option = (ISortOption)Activator.CreateInstance(t);
                option.Module = module;
                option.Settings = settings;
                listOptions.Add(option);
            }
            foreach (var t in listTypesManual)
            {
                ISortOption option = (ISortOption)Activator.CreateInstance(t);
                option.Module = module;
                option.Settings = settings;
                listOptions.AddRange(option.InstantiateManually());
            }
            return listOptions.OrderBy(op => op.Name).ToList();
        }
    }
}
