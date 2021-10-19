using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
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

        public virtual IValueContainer ProvideValueContainer()
        {
            return new ValueContainer();
        }

        public void SaveValue(string key, string value)
        {
            IValueContainer vc = ProvideValueContainer();
            vc.SetContent(value);
            values.Add(key, vc);
        }

        public void SaveValue(string key, int value)
        {
            IValueContainer vc = ProvideValueContainer();
            vc.SetContent(value.ToString());
            values.Add(key, vc);
        }

        public void SaveValue(string key, bool value)
        {
            IValueContainer vc = ProvideValueContainer();
            vc.SetContent(value.ToString());
            values.Add(key, vc);
        }

        public void SaveValue(string key, double value)
        {
            IValueContainer vc = ProvideValueContainer();
            vc.SetContent(value.ToString());
            values.Add(key, vc);
        }

        public void SaveValue(string key, Guid value)
        {
            IValueContainer vc = ProvideValueContainer();
            vc.SetContent(value.ToString());
            values.Add(key, vc);
        }

        public void SaveValue(string key, Color value)
        {
            IValueContainer vc = ProvideValueContainer();
            vc.SetContent(value.ToArgb().ToString());
            values.Add(key, vc);
        }

        public void SaveValue(string key, ISavable obj)
        {
            IValueContainer vc = ProvideValueContainer();
            vc.SetContent(obj);
            values.Add(key, vc);
        }

        public void SaveValue(string key, SavableRepresentation sr)
        {
            IValueContainer vc = ProvideValueContainer();
            vc.SetContent(sr);
            values.Add(key, vc);
        }

        public void SaveList(string key, IEnumerable<string> values)
        {
            IValueContainer vc = ProvideValueContainer();
            vc.SetContent(values);
            this.values.Add(key, vc);
        }

        public void SaveList(string key, IEnumerable<int> values)
        {
            IEnumerable<string> list = Util.ConvertListToStringList(values);
            IValueContainer vc = ProvideValueContainer();
            vc.SetContent(list);
            this.values.Add(key, vc);
        }

        public void SaveList(string key, IEnumerable<double> values)
        {
            IEnumerable<string> list = Util.ConvertListToStringList(values);
            IValueContainer vc = ProvideValueContainer();
            vc.SetContent(list);
            this.values.Add(key, vc);
        }

        public void SaveList(string key, IEnumerable<ISavable> values)
        {
            IValueContainer vc = ProvideValueContainer();
            vc.SetContent(values);
            this.values.Add(key, vc);
        }

        public void SaveList(string key, IEnumerable<SavableRepresentation> values)
        {
            IValueContainer vc = ProvideValueContainer();
            vc.SetContent(values);
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

        public Guid GetGuid(string key)
        {
            return Guid.Parse(values[key].GetContentString());
        }

        public Color GetColor(string key)
        {
            return Color.FromArgb(int.Parse(values[key].GetContentString()));
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
            IEnumerable<string> list = GetStringList(key);
            foreach (string obj in list)
            {
                T t = TConverter.ChangeType<T>(obj);
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
