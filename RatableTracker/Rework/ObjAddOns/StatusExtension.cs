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
        private UniqueID _status = new UniqueID(false);
        public Status Status
        {
            get
            {
                if (!_status.HasValue()) return null;
                return Util.Util.FindObjectInList(module.GetStatusList(), _status);
            }
            set
            {
                _status = value.UniqueID;
            }
        }

        private readonly StatusExtensionModule module;
        private readonly RankedObject obj;

        public StatusExtension(StatusExtensionModule module, RankedObject obj)
        {
            this.module = module;
            this.obj = obj;
        }

        public virtual void Validate()
        {

        }

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
