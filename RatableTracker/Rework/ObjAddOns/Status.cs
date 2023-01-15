using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.LoadSave;
using RatableTracker.Rework.Model;
using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RatableTracker.Rework.ObjAddOns
{
    public class Status : IKeyable, ISavable
    {
        public static int MaxLengthName => 200;

        public string Name { get; set; } = "";
        public Color Color { get; set; } = new Color();

        private UniqueID _uniqueID = new UniqueID();
        public UniqueID UniqueID { get { return _uniqueID; } }

        protected readonly StatusExtensionModule module;

        public Status(StatusExtensionModule module)
        {
            this.module = module;
        }

        public virtual void Validate()
        {
            // TODO unique exceptions
            if (Name == "")
                throw new Exception("A name is required");
            if (Name.Length > MaxLengthName)
                throw new Exception("Name cannot be longer than " + MaxLengthName.ToString() + " characters");
        }

        public void Save()
        {
            // TODO validate and save
        }

        public virtual SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = new SavableRepresentation();
            sr.SaveValue("TypeName", new ValueContainer(GetType().Name));
            sr.SaveValue("UniqueID", new ValueContainer(UniqueID));
            sr.SaveValue("Name", new ValueContainer(Name));
            sr.SaveValue("Color", new ValueContainer(Color));
            return sr;
        }

        public virtual void RestoreFromRepresentation(SavableRepresentation sr)
        {
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "UniqueID":
                        _uniqueID = sr.GetValue(key).GetUniqueID();
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
