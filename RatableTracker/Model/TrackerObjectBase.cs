using RatableTracker.Exceptions;
using RatableTracker.Interfaces;
using RatableTracker.LoadSave;
using RatableTracker.Modules;
using RatableTracker.ObjAddOns;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RatableTracker.Model
{
    public abstract class TrackerObjectBase : SaveDeleteObject, IDisposable, IKeyable
    {
        public static int MaxLengthName => 200;

        [Savable()] public string Name { get; set; } = "";

        [Savable()] public UniqueID UniqueID { get; protected set; } = UniqueID.NewID();

        protected Settings Settings { get; init; }
        protected TrackerModule Module { get; init; }

        public TrackerObjectBase(Settings settings, TrackerModule module)
        {
            this.Settings = settings;
            this.Module = module;
        }

        public TrackerObjectBase(TrackerObjectBase copyFrom) : this(copyFrom.Settings, copyFrom.Module)
        {
            UniqueID = UniqueID.Copy(copyFrom.UniqueID);
            Name = copyFrom.Name;
        }

        protected override void ValidateFields()
        {
            base.ValidateFields();
            if (Name == "")
                throw new ValidationException("A name is required", Name);
            if (Name.Length > MaxLengthName)
                throw new ValidationException("Name cannot be longer than " + MaxLengthName.ToString() + " characters", Name);

            foreach (var extension in GetExtensions())
            {
                extension.ValidateFields();
            }
        }

        public virtual void InitAdditionalResources()
        {
            AddEventHandlers();

            foreach (var extension in GetExtensions())
            {
                extension.InitAdditionalResources();
            }
        }

        protected virtual void AddEventHandlers() { }

        public virtual void Dispose()
        {
            RemoveEventHandlers();

            foreach (var extension in GetExtensions())
            {
                extension.Dispose();
            }
        }

        protected virtual void RemoveEventHandlers() { }

        public sealed override SavableRepresentation LoadIntoRepresentation()
        {
            var sr = base.LoadIntoRepresentation();

            // automatically load in data from extensions
            foreach (var extension in GetExtensions())
            {
                LoadSavableFieldsIntoRepresentation(extension, ref sr, extension.LoadHandleManually);
            }

            return sr;
        }

        public sealed override void RestoreFromRepresentation(SavableRepresentation sr)
        {
            base.RestoreFromRepresentation(sr);

            // automatically restore data to extensions
            foreach (var extension in GetExtensions())
            {
                RestoreSavableFieldsFromRepresentation(extension, sr, extension.RestoreHandleManually);
            }
        }

        private IEnumerable<ExtensionBase> GetExtensions()
        {
            List<ExtensionBase> list = new List<ExtensionBase>();
            var properties = GetType().GetProperties().Where(prop => prop.IsDefined(typeof(TrackerObjectExtensionAttribute), false));
            foreach (var prop in properties)
            {
                Attribute[] attrs = Attribute.GetCustomAttributes(prop);
                foreach (Attribute attr in attrs)
                {
                    if (attr is TrackerObjectExtensionAttribute extAttr)
                    {
                        ExtensionBase extension;
                        try
                        {
                            extension = (ExtensionBase)prop.GetValue(this);
                        }
                        catch (InvalidCastException ex)
                        {
                            throw new InvalidCastException("The TrackerObjectExtensionAttribute can only be applied to properties that extend ExtensionBase", ex);
                        }
                        list.Add(extension);
                    }
                }
            }
            return list;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is TrackerObjectBase)) return false;
            TrackerObjectBase other = (TrackerObjectBase)obj;
            return UniqueID.Equals(other.UniqueID);
        }

        public override int GetHashCode()
        {
            return UniqueID.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
