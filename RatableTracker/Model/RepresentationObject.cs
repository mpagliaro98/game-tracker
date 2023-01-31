using RatableTracker.LoadSave;
using RatableTracker.ObjAddOns;
using RatableTracker.Util;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RatableTracker.Model
{
    public abstract class RepresentationObject
    {
        public const string TYPENAME_KEY = "TypeName";

        protected delegate void ManualLoadHandler(ref SavableRepresentation sr, string key);
        protected delegate void ManualRestoreHandler(SavableRepresentation sr, string key);

        public virtual SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = new SavableRepresentation();
            sr.SaveValue(TYPENAME_KEY, new ValueContainer(GetType().Name));
            LoadSavableFieldsIntoRepresentation(this, ref sr, LoadHandleManually);
            return sr;
        }

        protected void LoadSavableFieldsIntoRepresentation(object obj, ref SavableRepresentation sr, ManualLoadHandler manual)
        {
            var properties = obj.GetType().GetAllProperties().Where(prop => prop.IsDefined(typeof(SavableAttribute), false));
            LoadMemberValuesIntoRepresentation(ref sr, properties, (prop) => prop.PropertyType, (prop) => prop.GetValue(obj), manual);
            var fields = obj.GetType().GetAllFields().Where(field => field.IsDefined(typeof(SavableAttribute), false));
            LoadMemberValuesIntoRepresentation(ref sr, fields, (field) => field.FieldType, (field) => field.GetValue(obj), manual);
        }

        private void LoadMemberValuesIntoRepresentation<T>(ref SavableRepresentation sr, IEnumerable<T> members, Func<T, Type> getType, Func<T, object> getVal, ManualLoadHandler manual) where T : MemberInfo
        {
            foreach (var member in members)
            {
                Attribute[] attrs = Attribute.GetCustomAttributes(member);
                foreach (Attribute attr in attrs)
                {
                    if (attr is SavableAttribute savable)
                    {
                        string key = savable.Key;

                        if (savable.HandleLoadManually)
                        {
                            manual(ref sr, key);
                            continue;
                        }

                        // if using a dervied ValueContainer, override this to use it and its supported types
                        Type memberType = getType(member);
                        LoadHandleTypes(memberType, ref sr, key, getVal(member));
                    }
                }
            }
        }

        protected virtual void LoadHandleTypes(Type memberType, ref SavableRepresentation sr, string key, object value)
        {
            if (memberType.Equals(typeof(string)))
                sr.SaveValue(key, new ValueContainer((string)value));
            else if (memberType.Equals(typeof(int)))
                sr.SaveValue(key, new ValueContainer((int)value));
            else if (memberType.Equals(typeof(bool)))
                sr.SaveValue(key, new ValueContainer((bool)value));
            else if (memberType.Equals(typeof(double)))
                sr.SaveValue(key, new ValueContainer((double)value));
            else if (memberType.Equals(typeof(Guid)))
                sr.SaveValue(key, new ValueContainer((Guid)value));
            else if (memberType.Equals(typeof(Color)))
                sr.SaveValue(key, new ValueContainer((Color)value));
            else if (memberType.Equals(typeof(DateTime)))
                sr.SaveValue(key, new ValueContainer((DateTime)value));
            else if (memberType.Equals(typeof(UniqueID)))
                sr.SaveValue(key, new ValueContainer((UniqueID)value));
            else if (memberType.Equals(typeof(RepresentationObject)))
                throw new NotSupportedException("Type " + memberType.Name + " must be handled manually");
            else if (memberType.Equals(typeof(SavableRepresentation)))
                sr.SaveValue(key, new ValueContainer((SavableRepresentation)value));
            else if (memberType.Equals(typeof(IEnumerable<string>)) || memberType.Equals(typeof(IList<string>)))
                sr.SaveValue(key, new ValueContainer((IEnumerable<string>)value));
            else if (memberType.Equals(typeof(IEnumerable<int>)) || memberType.Equals(typeof(IList<int>)))
                sr.SaveValue(key, new ValueContainer((IEnumerable<int>)value));
            else if (memberType.Equals(typeof(IEnumerable<double>)) || memberType.Equals(typeof(IList<double>)))
                sr.SaveValue(key, new ValueContainer((IEnumerable<double>)value));
            else if (memberType.Equals(typeof(IEnumerable<RepresentationObject>)) || memberType.Equals(typeof(IList<RepresentationObject>)))
                throw new NotSupportedException("Type " + memberType.Name + " must be handled manually");
            else if (memberType.Equals(typeof(IEnumerable<SavableRepresentation>)) || memberType.Equals(typeof(IList<SavableRepresentation>)))
                sr.SaveValue(key, new ValueContainer((IEnumerable<SavableRepresentation>)value));
            else
                throw new NotSupportedException("Type not supported - " + memberType.Name);
        }

        protected virtual void LoadHandleManually(ref SavableRepresentation sr, string key) { }

        public virtual void RestoreFromRepresentation(SavableRepresentation sr)
        {
            RestoreSavableFieldsFromRepresentation(this, sr, RestoreHandleManually);
        }

        protected void RestoreSavableFieldsFromRepresentation(object obj, SavableRepresentation sr, ManualRestoreHandler manual)
        {
            var properties = obj.GetType().GetAllProperties().Where(prop => prop.IsDefined(typeof(SavableAttribute), false));
            RestoreMemberValuesFromRepresentation(sr, properties, (prop) => prop.PropertyType, (prop, val) => prop.SetValue(obj, val), manual);
            var fields = obj.GetType().GetAllFields().Where(field => field.IsDefined(typeof(SavableAttribute), false));
            RestoreMemberValuesFromRepresentation(sr, fields, (field) => field.FieldType, (field, val) => field.SetValue(obj, val), manual);
        }

        private void RestoreMemberValuesFromRepresentation<T>(SavableRepresentation sr, IEnumerable<T> members, Func<T, Type> getType, Action<T, object> setVal, ManualRestoreHandler manual) where T : MemberInfo
        {
            foreach (var member in members)
            {
                Attribute[] attrs = Attribute.GetCustomAttributes(member);
                foreach (Attribute attr in attrs)
                {
                    if (attr is SavableAttribute savable)
                    {
                        if (savable.SaveOnly)
                            continue;

                        string key = savable.Key;

                        if (!sr.HasKey(key))
                            continue;

                        if (savable.HandleRestoreManually)
                        {
                            manual(sr, key);
                            continue;
                        }

                        ValueContainer value = sr.GetValue(key);
                        Type memberType = getType(member);

                        // if using a dervied ValueContainer, override this to use it and its supported types
                        RestoreHandleTypes(memberType, ref sr, member, value, setVal);
                    }
                }
            }
        }

        protected virtual void RestoreHandleTypes<T>(Type memberType, ref SavableRepresentation sr, T member, ValueContainer value, Action<T, object> setVal) where T : MemberInfo
        {
            if (memberType.Equals(typeof(string)))
                setVal(member, value.GetString());
            else if (memberType.Equals(typeof(int)))
                setVal(member, value.GetInt());
            else if (memberType.Equals(typeof(bool)))
                setVal(member, value.GetBool());
            else if (memberType.Equals(typeof(double)))
                setVal(member, value.GetDouble());
            else if (memberType.Equals(typeof(Guid)))
                setVal(member, value.GetGuid());
            else if (memberType.Equals(typeof(Color)))
                setVal(member, value.GetColor());
            else if (memberType.Equals(typeof(DateTime)))
                setVal(member, value.GetDateTime());
            else if (memberType.Equals(typeof(UniqueID)))
                setVal(member, value.GetUniqueID());
            else if (memberType.Equals(typeof(RepresentationObject)))
                throw new NotSupportedException("Type " + memberType.Name + " must be handled manually");
            else if (memberType.Equals(typeof(SavableRepresentation)))
                setVal(member, value.GetSavableRepresentation());
            else if (memberType.Equals(typeof(IEnumerable<string>)) || memberType.Equals(typeof(IList<string>)))
                setVal(member, value.GetStringList());
            else if (memberType.Equals(typeof(IEnumerable<int>)) || memberType.Equals(typeof(IList<int>)))
                setVal(member, value.GetIntList());
            else if (memberType.Equals(typeof(IEnumerable<double>)) || memberType.Equals(typeof(IList<double>)))
                setVal(member, value.GetDoubleList());
            else if (memberType.Equals(typeof(IEnumerable<RepresentationObject>)) || memberType.Equals(typeof(IList<RepresentationObject>)))
                throw new NotSupportedException("Type " + memberType.Name + " must be handled manually");
            else if (memberType.Equals(typeof(IEnumerable<SavableRepresentation>)) || memberType.Equals(typeof(IList<SavableRepresentation>)))
                setVal(member, value.GetSavableRepresentationList());
            else
                throw new NotSupportedException("Type not supported - " + memberType.Name);
        }

        protected virtual void RestoreHandleManually(SavableRepresentation sr, string key) { }
    }
}
