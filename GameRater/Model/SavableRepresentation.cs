using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRater.Model
{
    class SavableRepresentation
    {
        private class ValueContainer
        {
            public string valueString;
            public SavableRepresentation valueSR;
            public IEnumerable<string> valueStringList;
            public IEnumerable<SavableRepresentation> valueSRList;
        }

        private IDictionary<string, ValueContainer> values;

        public SavableRepresentation()
        {
            values = new Dictionary<string, ValueContainer>();
        }

        public void SaveValue(string key, string value)
        {
            ValueContainer vc = new ValueContainer();
            vc.valueString = value;
            values.Add(key, vc);
        }

        public void SaveValue(string key, ISavable obj)
        {
            SavableRepresentation sr = obj.LoadIntoRepresentation();
            ValueContainer vc = new ValueContainer();
            vc.valueSR = sr;
            values.Add(key, vc);
        }

        public void SaveList<T>(string key, IEnumerable<T> values)
        {
            LinkedList<string> list = new LinkedList<string>();
            foreach (T value in values)
            {
                list.AddLast(value.ToString());
            }
            ValueContainer vc = new ValueContainer();
            vc.valueStringList = list;
            this.values.Add(key, vc);
        }

        public void SaveList(string key, IEnumerable<ISavable> values)
        {
            LinkedList<SavableRepresentation> list = new LinkedList<SavableRepresentation>();
            foreach (ISavable value in values)
            {
                list.AddLast(value.LoadIntoRepresentation());
            }
            ValueContainer vc = new ValueContainer();
            vc.valueSRList = list;
            this.values.Add(key, vc);
        }

        public void SaveList(string key, IEnumerable<SavableRepresentation> values)
        {
            ValueContainer vc = new ValueContainer();
            vc.valueSRList = values;
            this.values.Add(key, vc);
        }

        public string GetValue(string key)
        {
            return values[key].valueString;
        }

        public T GetISavable<T>(string key) where T : ISavable, new()
        {
            SavableRepresentation sr = values[key].valueSR;
            T t = new T();
            t.RestoreFromRepresentation(sr);
            return t;
        }

        public IEnumerable<string> GetList(string key)
        {
            return values[key].valueStringList;
        }

        public IEnumerable<SavableRepresentation> GetSRList(string key)
        {
            return values[key].valueSRList;
        }

        public ICollection<string> GetAllSavedKeys()
        {
            return values.Keys;
        }

        public bool IsValueAList(string key)
        {
            return values[key].valueStringList != null || values[key].valueSRList != null;
        }

        public IEnumerable<T> GetListOfType<T>(string key)
        {
            IEnumerable<T> result = new List<T>();
            IEnumerable<object> list = GetList(key);
            foreach (object obj in list)
            {
                T t = (T)obj;
                result = result.Append(t).ToList();
            }
            return result;
        }

        public IEnumerable<T> GetListOfISavable<T>(string key) where T : ISavable, new()
        {
            IEnumerable<T> result = new List<T>();
            IEnumerable<SavableRepresentation> list = GetSRList(key);
            foreach (SavableRepresentation sr in list)
            {
                T t = new T();
                t.RestoreFromRepresentation(sr);
                result = result.Append(t).ToList();
            }
            return result;
        }
    }
}
