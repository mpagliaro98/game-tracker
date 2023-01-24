using RatableTracker.Interfaces;
using RatableTracker.Model;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.LoadSave
{
    public class ValueContainer
    {
        protected string valueString = null;
        protected SavableRepresentation valueSR = null;
        protected IEnumerable<string> valueStringList = null;
        protected IEnumerable<SavableRepresentation> valueSRList = null;

        public ValueContainer(string val)
        {
            SetContent(val);
        }

        public ValueContainer(int val)
        {
            SetContent(val);
        }

        public ValueContainer(bool val)
        {
            SetContent(val);
        }

        public ValueContainer(double val)
        {
            SetContent(val);
        }

        public ValueContainer(Guid val)
        {
            SetContent(val);
        }

        public ValueContainer(Color val)
        {
            SetContent(val);
        }

        public ValueContainer(DateTime val)
        {
            SetContent(val);
        }

        public ValueContainer(UniqueID val)
        {
            SetContent(val);
        }

        public ValueContainer(RepresentationObject val)
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

        public ValueContainer(IEnumerable<int> val)
        {
            SetContent(val);
        }

        public ValueContainer(IEnumerable<double> val)
        {
            SetContent(val);
        }

        public ValueContainer(IEnumerable<RepresentationObject> val)
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

        public void SetContent(int val)
        {
            SetContent(val.ToString());
        }

        public void SetContent(bool val)
        {
            SetContent(val.ToString());
        }

        public void SetContent(double val)
        {
            SetContent(val.ToString());
        }

        public void SetContent(Guid val)
        {
            SetContent(val.ToString());
        }

        public void SetContent(Color val)
        {
            SetContent(val.ToArgb().ToString());
        }

        public void SetContent(DateTime val)
        {
            SetContent(val.ToString());
        }

        public void SetContent(UniqueID val)
        {
            SetContent(val.ToString());
        }

        public void SetContent(RepresentationObject savable)
        {
            SetContent(savable.LoadIntoRepresentation());
        }

        public void SetContent(SavableRepresentation val)
        {
            valueSR = val;
        }

        public void SetContent(IEnumerable<string> val)
        {
            valueStringList = val;
        }

        public void SetContent(IEnumerable<int> val)
        {
            IEnumerable<string> list = Util.Util.ConvertListToStringList(val);
            SetContent(list);
        }

        public void SetContent(IEnumerable<double> val)
        {
            IEnumerable<string> list = Util.Util.ConvertListToStringList(val);
            SetContent(list);
        }

        public void SetContent(IEnumerable<RepresentationObject> val)
        {
            LinkedList<SavableRepresentation> list = new LinkedList<SavableRepresentation>();
            foreach (RepresentationObject value in val)
            {
                var sr = value?.LoadIntoRepresentation();
                list.AddLast(sr);
            }
            SetContent(list);
        }

        public void SetContent(IEnumerable<SavableRepresentation> val)
        {
            valueSRList = val;
        }

        public string GetString()
        {
            return valueString;
        }

        public int GetInt()
        {
            return Convert.ToInt32(GetString());
        }

        public bool GetBool()
        {
            return Convert.ToBoolean(GetString());
        }

        public double GetDouble()
        {
            return Convert.ToDouble(GetString());
        }

        public Guid GetGuid()
        {
            return Guid.Parse(GetString());
        }

        public Color GetColor()
        {
            return Color.FromArgb(GetInt());
        }

        public DateTime GetDateTime()
        {
            return DateTime.Parse(GetString());
        }

        public UniqueID GetUniqueID()
        {
            return UniqueID.Parse(GetString());
        }

        public RepresentationObject GetRepresentationObject(Func<RepresentationObject> initSavable)
        {
            RepresentationObject savable = initSavable();
            savable?.RestoreFromRepresentation(GetSavableRepresentation());
            return savable;
        }

        public SavableRepresentation GetSavableRepresentation()
        {
            return valueSR;
        }

        public IEnumerable<string> GetStringList()
        {
            return valueStringList;
        }

        public IEnumerable<int> GetIntList()
        {
            return GetStringList().Select((s) => Convert.ToInt32(s));
        }

        public IEnumerable<double> GetDoubleList()
        {
            return GetStringList().Select((s) => Convert.ToDouble(s));
        }

        public IEnumerable<T> GetRepresentationObjectList<T>(Func<T> initSavable) where T : RepresentationObject
        {
            LinkedList<T> list = new LinkedList<T>();
            foreach (SavableRepresentation sr in GetSavableRepresentationList())
            {
                T savable = initSavable();
                savable?.RestoreFromRepresentation(sr);
                list.AddLast(savable);
            }
            return list;
        }

        public IEnumerable<SavableRepresentation> GetSavableRepresentationList()
        {
            return valueSRList;
        }

        public virtual bool HasValue()
        {
            return valueString != null || valueSR != null || valueStringList != null || valueSRList != null;
        }

        public virtual bool IsValueAList()
        {
            return valueStringList != null || valueSRList != null;
        }

        public virtual bool IsValueObjectList()
        {
            return valueSRList != null;
        }

        public virtual bool IsValueObject()
        {
            return valueSR != null;
        }
    }
}
