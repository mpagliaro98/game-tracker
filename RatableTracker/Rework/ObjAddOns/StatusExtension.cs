using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.LoadSave;
using RatableTracker.Rework.Model;
using RatableTracker.Rework.Modules;
using RatableTracker.Rework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RatableTracker.Rework.ObjAddOns
{
    public class StatusExtension
    {
        private UniqueID _status = UniqueID.BlankID();
        public Status Status
        {
            get
            {
                if (!_status.HasValue()) return null;
                return Util.Util.FindObjectInList(module.GetStatusList(), _status);
            }
            set
            {
                _status = value == null ? UniqueID.BlankID() : value.UniqueID;
            }
        }

        protected readonly StatusExtensionModule module;

        public StatusExtension(StatusExtensionModule module)
        {
            this.module = module;
        }

        public virtual void ValidateFields()
        {

        }

        public virtual bool RemoveReferenceToObject(IKeyable obj, Type type)
        {
            if (type == typeof(Status))
            {
                if (obj.Equals(Status))
                {
                    Status = null;
                    return true;
                }
            }
            return false;
        }

        public virtual void ApplySettingsChanges(Settings settings) { }

        public virtual void LoadIntoRepresentation(ref SavableRepresentation sr)
        {
            sr.SaveValue("Status", new ValueContainer(_status));
        }

        public virtual void RestoreFromRepresentation(SavableRepresentation sr)
        {
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "Status":
                        _status = sr.GetValue(key).GetUniqueID();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
