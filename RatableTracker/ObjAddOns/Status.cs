using RatableTracker.Exceptions;
using RatableTracker.Interfaces;
using RatableTracker.LoadSave;
using RatableTracker.Model;
using RatableTracker.Modules;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RatableTracker.ObjAddOns
{
    public class Status : SaveDeleteObject, IDisposable
    {
        public static int MaxLengthName => 200;

        public string Name { get; set; } = "";
        public Color Color { get; set; } = new Color();

        public virtual bool HideScoreOfModelObject { get { return false; } }
        public virtual bool ExcludeModelObjectFromStats { get { return false; } }

        public override UniqueID UniqueID { get; protected set; } = UniqueID.NewID();

        protected readonly StatusExtensionModule module;

        public Status(StatusExtensionModule module)
        {
            this.module = module;
        }

        public Status(Status copyFrom) : this(copyFrom.module)
        {
            UniqueID = UniqueID.Copy(copyFrom.UniqueID);
            Name = copyFrom.Name;
            Color = copyFrom.Color;
        }

        protected override void ValidateFields()
        {
            base.ValidateFields();
            if (Name == "")
                throw new ValidationException("A name is required", Name);
            if (Name.Length > MaxLengthName)
                throw new ValidationException("Name cannot be longer than " + MaxLengthName.ToString() + " characters", Name);
        }

        protected override bool SaveObjectToModule(TrackerModule module, ILoadSaveMethod conn)
        {
            return this.module.SaveStatus(this, module, (ILoadSaveMethodStatusExtension)conn);
        }

        protected override void DeleteObjectFromModule(TrackerModule module, ILoadSaveMethod conn)
        {
            this.module.DeleteStatus(this, module, (ILoadSaveMethodStatusExtension)conn);
        }

        public void Dispose()
        {
            RemoveEventHandlers();
        }

        protected virtual void RemoveEventHandlers() { }

        public override SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = base.LoadIntoRepresentation();
            sr.SaveValue("UniqueID", new ValueContainer(UniqueID));
            sr.SaveValue("Name", new ValueContainer(Name));
            sr.SaveValue("Color", new ValueContainer(Color));
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
                    case "Color":
                        Color = sr.GetValue(key).GetColor();
                        break;
                    default:
                        break;
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is Status)) return false;
            Status other = (Status)obj;
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
