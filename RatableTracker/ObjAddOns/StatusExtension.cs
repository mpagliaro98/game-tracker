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
    public class StatusExtension : IDisposable
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
        public RankedObject BaseObject { get; internal set; }

        public StatusExtension(StatusExtensionModule module)
        {
            this.module = module;
            this.module.StatusDeleted += OnStatusDeleted;
        }

        public StatusExtension(StatusExtension copyFrom) : this(copyFrom.module)
        {
            _status = UniqueID.Copy(copyFrom._status);
        }

        public virtual void ValidateFields() { }

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

        private void OnStatusDeleted(object sender, Events.StatusDeleteArgs args)
        {
            if (_status.Equals(args.DeletedObject.UniqueID))
            {
                Status = null;
                BaseObject.Save((TrackerModule)module.BaseModule, args.Connection);
            }
        }

        public virtual void ApplySettingsChanges(Settings settings) { }

        public void Dispose()
        {
            RemoveEventHandlers();
        }

        protected virtual void RemoveEventHandlers()
        {
            module.StatusDeleted -= OnStatusDeleted;
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
