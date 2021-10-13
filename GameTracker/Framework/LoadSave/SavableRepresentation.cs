using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Interfaces;
using RatableTracker.Framework.Global;

namespace RatableTracker.Framework.LoadSave
{
    public partial class SavableRepresentation
    {
        private IDictionary<string, IValueContainer> values;

        public SavableRepresentation()
        {
            values = new Dictionary<string, IValueContainer>();
        }

        public void SaveValue(string key, string value)
        {
            IValueContainer vc = new ValueContainer(value);
            values.Add(key, vc);
        }

        public void SaveValue(string key, int value)
        {
            IValueContainer vc = new ValueContainer(value.ToString());
            values.Add(key, vc);
        }

        public void SaveValue(string key, bool value)
        {
            IValueContainer vc = new ValueContainer(value.ToString());
            values.Add(key, vc);
        }

        public void SaveValue(string key, double value)
        {
            IValueContainer vc = new ValueContainer(value.ToString());
            values.Add(key, vc);
        }

        public void SaveValue(string key, ISavable obj)
        {
            IValueContainer vc = new ValueContainer(obj);
            values.Add(key, vc);
        }

        public void SaveValue(string key, SavableRepresentation sr)
        {
            IValueContainer vc = new ValueContainer(sr);
            values.Add(key, vc);
        }

        public void SaveList(string key, IEnumerable<string> values)
        {
            IValueContainer vc = new ValueContainer(values);
            this.values.Add(key, vc);
        }

        public void SaveList(string key, IEnumerable<int> values)
        {
            IEnumerable<string> list = Util.ConvertListToStringList(values);
            IValueContainer vc = new ValueContainer(list);
            this.values.Add(key, vc);
        }

        public void SaveList(string key, IEnumerable<double> values)
        {
            IEnumerable<string> list = Util.ConvertListToStringList(values);
            IValueContainer vc = new ValueContainer(list);
            this.values.Add(key, vc);
        }

        public void SaveList(string key, IEnumerable<ISavable> values)
        {
            IValueContainer vc = new ValueContainer(values);
            this.values.Add(key, vc);
        }

        public void SaveList(string key, IEnumerable<SavableRepresentation> values)
        {
            IValueContainer vc = new ValueContainer(values);
            this.values.Add(key, vc);
        }

        public string GetString(string key)
        {
            return values[key].GetContentString();
        }

        public int GetInt(string key)
        {
            return int.Parse(values[key].GetContentString());
        }

        public bool GetBool(string key)
        {
            return bool.Parse(values[key].GetContentString());
        }

        public double GetDouble(string key)
        {
            return double.Parse(values[key].GetContentString());
        }

        public T GetISavable<T>(string key) where T : ISavable, new()
        {
            return values[key].GetContentISavable<T>();
        }

        public SavableRepresentation GetSavableRepresentation(string key)
        {
            return values[key].GetContentSavableRepresentation();
        }

        public IEnumerable<string> GetStringList(string key)
        {
            return values[key].GetContentStringList();
        }

        public IEnumerable<T> GetListOfISavable<T>(string key) where T : ISavable, new()
        {
            return values[key].GetContentISavableList<T>();
        }

        public IEnumerable<SavableRepresentation> GetSavableRepresentationList(string key)
        {
            return values[key].GetContentSavableRepresentationList();
        }

        public IEnumerable<T> GetListOfType<T>(string key)
        {
            List<T> result = new List<T>();
            IEnumerable<object> list = GetStringList(key);
            foreach (object obj in list)
            {
                T t = (T)obj;
                result.Add(t);
            }
            return result;
        }

        public bool HasValue(string key)
        {
            return values.ContainsKey(key) && values[key].HasValue();
        }

        public bool IsValueAList(string key)
        {
            return values.ContainsKey(key) && values[key].HasValue() && values[key].IsValueAList();
        }

        public ICollection<string> GetAllSavedKeys()
        {
            return values.Keys;
        }
    }
}
