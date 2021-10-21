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
    public partial class SavableRepresentation<TValCont>
        where TValCont : IValueContainer<TValCont>, new()
    {
        private readonly IDictionary<string, IValueContainer<TValCont>> values;

        public SavableRepresentation()
        {
            values = new Dictionary<string, IValueContainer<TValCont>>();
        }

        public void SaveValue(string key, string value)
        {
            IValueContainer<TValCont> vc = new TValCont();
            vc.SetContent(value);
            values.Add(key, vc);
        }

        public void SaveValue(string key, int value)
        {
            IValueContainer<TValCont> vc = new TValCont();
            vc.SetContent(value.ToString());
            values.Add(key, vc);
        }

        public void SaveValue(string key, bool value)
        {
            IValueContainer<TValCont> vc = new TValCont();
            vc.SetContent(value.ToString());
            values.Add(key, vc);
        }

        public void SaveValue(string key, double value)
        {
            IValueContainer<TValCont> vc = new TValCont();
            vc.SetContent(value.ToString());
            values.Add(key, vc);
        }

        public void SaveValue(string key, Guid value)
        {
            IValueContainer<TValCont> vc = new TValCont();
            vc.SetContent(value.ToString());
            values.Add(key, vc);
        }

        public void SaveValue(string key, Color value)
        {
            IValueContainer<TValCont> vc = new TValCont();
            vc.SetContent(value.ToArgb().ToString());
            values.Add(key, vc);
        }

        public void SaveValue(string key, DateTime value)
        {
            IValueContainer<TValCont> vc = new TValCont();
            vc.SetContent(value.ToString());
            values.Add(key, vc);
        }

        public void SaveValue(string key, ISavable obj)
        {
            IValueContainer<TValCont> vc = new TValCont();
            vc.SetContent(obj);
            values.Add(key, vc);
        }

        public void SaveValue(string key, SavableRepresentation<TValCont> sr)
        {
            IValueContainer<TValCont> vc = new TValCont();
            vc.SetContent(sr);
            values.Add(key, vc);
        }

        public void SaveList(string key, IEnumerable<string> values)
        {
            IValueContainer<TValCont> vc = new TValCont();
            vc.SetContent(values);
            this.values.Add(key, vc);
        }

        public void SaveList(string key, IEnumerable<int> values)
        {
            IEnumerable<string> list = Util.ConvertListToStringList(values);
            IValueContainer<TValCont> vc = new TValCont();
            vc.SetContent(list);
            this.values.Add(key, vc);
        }

        public void SaveList(string key, IEnumerable<double> values)
        {
            IEnumerable<string> list = Util.ConvertListToStringList(values);
            IValueContainer<TValCont> vc = new TValCont();
            vc.SetContent(list);
            this.values.Add(key, vc);
        }

        public void SaveList(string key, IEnumerable<ISavable> values)
        {
            IValueContainer<TValCont> vc = new TValCont();
            vc.SetContent(values);
            this.values.Add(key, vc);
        }

        public void SaveList(string key, IEnumerable<SavableRepresentation<TValCont>> values)
        {
            IValueContainer<TValCont> vc = new TValCont();
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

        public DateTime GetDateTime(string key)
        {
            return DateTime.Parse(values[key].GetContentString());
        }

        public T GetISavable<T>(string key) where T : ISavable, new()
        {
            return values[key].GetContentISavable<T>();
        }

        public SavableRepresentation<TValCont> GetSavableRepresentation(string key)
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

        public IEnumerable<SavableRepresentation<TValCont>> GetSavableRepresentationList(string key)
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
