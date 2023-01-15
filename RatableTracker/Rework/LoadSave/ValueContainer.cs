using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Rework.Interfaces;

namespace RatableTracker.Rework.LoadSave
{
    public class ValueContainer
    {
        private string valueString = null;
        private SavableRepresentation<ValueContainer> valueSR = null;
        private IEnumerable<string> valueStringList = null;
        private IEnumerable<SavableRepresentation<ValueContainer>> valueSRList = null;

        public ValueContainer() { }

        public void SetContent(string val)
        {
            valueString = val;
        }

        //public void SetContent(ISavable val)
        //{
        //    var sr = new SavableRepresentation<ValueContainer>();
        //    val?.LoadIntoRepresentation(ref sr);
        //    valueSR = sr;
        //}

        //public void SetContent(SavableRepresentation<ValueContainer> val)
        //{
        //    valueSR = val;
        //}

        public void SetContent(IEnumerable<string> val)
        {
            valueStringList = val;
        }

        //public void SetContent(IEnumerable<ISavable> val)
        //{
        //    LinkedList<SavableRepresentation<TInner>> list = new LinkedList<SavableRepresentation<TInner>>();
        //    foreach (ISavable value in val)
        //    {
        //        list.AddLast(value.LoadIntoRepresentation<TInner>());
        //    }
        //    valueSRList = list;
        //}

        //public void SetContent(IEnumerable<SavableRepresentation<TInner>> val)
        //{
        //    valueSRList = val;
        //}

        public string GetContentString()
        {
            return valueString;
        }

        //public T GetContentISavable<T>() where T : ISavable, new()
        //{
        //    SavableRepresentation<TInner> sr = valueSR;
        //    T t = new T();
        //    if (sr != null) t.RestoreFromRepresentation(sr);
        //    return t;
        //}

        //public SavableRepresentation<TInner> GetContentSavableRepresentation()
        //{
        //    return valueSR;
        //}

        public IEnumerable<string> GetContentStringList()
        {
            return valueStringList;
        }

        //public IEnumerable<T> GetContentISavableList<T>() where T : ISavable, new()
        //{
        //    List<T> result = new List<T>();
        //    IEnumerable<SavableRepresentation<TInner>> list = GetContentSavableRepresentationList();
        //    foreach (SavableRepresentation<TInner> sr in list)
        //    {
        //        T t = new T();
        //        if (sr != null) t.RestoreFromRepresentation(sr);
        //        result.Add(t);
        //    }
        //    return result;
        //}

        //public IEnumerable<SavableRepresentation<TInner>> GetContentSavableRepresentationList()
        //{
        //    return valueSRList;
        //}

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
