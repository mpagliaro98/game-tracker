using RatableTracker.Exceptions;
using RatableTracker.LoadSave;
using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RatableTracker.Model
{
    public abstract class TrackerObjectBase : SaveDeleteObject, IDisposable
    {
        public static int MaxLengthName => 200;

        public string Name { get; set; } = "";

        public override UniqueID UniqueID { get; protected set; } = UniqueID.NewID();

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
        }

        public virtual void InitAdditionalResources()
        {
            AddEventHandlers();
        }

        protected virtual void AddEventHandlers() { }

        public virtual void Dispose()
        {
            RemoveEventHandlers();
        }

        protected virtual void RemoveEventHandlers() { }

        public override SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = base.LoadIntoRepresentation();
            sr.SaveValue("UniqueID", new ValueContainer(UniqueID));
            sr.SaveValue("Name", new ValueContainer(Name));
            return sr;
        }

        public override void RestoreFromRepresentation(SavableRepresentation sr)
        {
            base.RestoreFromRepresentation(sr);
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "UniqueID":
                        UniqueID = sr.GetValue(key).GetUniqueID();
                        break;
                    case "Name":
                        Name = sr.GetValue(key).GetString();
                        break;
                    default:
                        break;
                }
            }
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
