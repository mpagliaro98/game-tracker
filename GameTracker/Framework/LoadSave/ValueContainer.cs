using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Interfaces;

namespace RatableTracker.Framework.LoadSave
{
    public partial class ValueContainer : IValueContainer
    {
        private string valueString = null;
        private SavableRepresentation valueSR = null;
        private IEnumerable<string> valueStringList = null;
        private IEnumerable<SavableRepresentation> valueSRList = null;

        public ValueContainer(string val)
        {
            SetContent(val);
        }

        public ValueContainer(ISavable val)
        {
            SetContent(val);
        }

        public ValueContainer(SavableRepresentation val)
        {
            SetContent(val);
        }

        public ValueContainer(IEnumerable<string> val)
        {
            SetContent(val);
        }

        public ValueContainer(IEnumerable<ISavable> val)
        {
            SetContent(val);
        }

        public ValueContainer(IEnumerable<SavableRepresentation> val)
        {
            SetContent(val);
        }

        public void SetContent(string val)
        {
            valueString = val;
        }

        public void SetContent(ISavable val)
        {
            SavableRepresentation sr = val?.LoadIntoRepresentation();
            valueSR = sr;
        }

        public void SetContent(SavableRepresentation val)
        {
            valueSR = val;
        }

        public void SetContent(IEnumerable<string> val)
        {
            valueStringList = val;
        }

        public void SetContent(IEnumerable<ISavable> val)
        {
            LinkedList<SavableRepresentation> list = new LinkedList<SavableRepresentation>();
            foreach (ISavable value in val)
            {
                list.AddLast(value.LoadIntoRepresentation());
            }
            valueSRList = list;
        }

        public void SetContent(IEnumerable<SavableRepresentation> val)
        {
            valueSRList = val;
        }

        public string GetContentString()
        {
            return valueString;
        }

        public T GetContentISavable<T>() where T : ISavable, new()
        {
            SavableRepresentation sr = valueSR;
            T t = new T();
            t.RestoreFromRepresentation(sr);
            return t;
        }

        public SavableRepresentation GetContentSavableRepresentation()
        {
            return valueSR;
        }

        public IEnumerable<string> GetContentStringList()
        {
            return valueStringList;
        }

        public IEnumerable<T> GetContentISavableList<T>() where T : ISavable, new()
        {
            List<T> result = new List<T>();
            IEnumerable<SavableRepresentation> list = GetContentSavableRepresentationList();
            foreach (SavableRepresentation sr in list)
            {
                T t = new T();
                t.RestoreFromRepresentation(sr);
                result.Add(t);
            }
            return result;
        }

        public IEnumerable<SavableRepresentation> GetContentSavableRepresentationList()
        {
            return valueSRList;
        }

        public bool HasValue()
        {
            return valueString != null || valueSR != null || valueStringList != null || valueSRList != null;
        }

        public bool IsValueAList()
        {
            return valueStringList != null || valueSRList != null;
        }

        public bool IsValueObjectList()
        {
            return valueSRList != null;
        }

        public bool IsValueObject()
        {
            return valueSR != null;
        }
    }
}
