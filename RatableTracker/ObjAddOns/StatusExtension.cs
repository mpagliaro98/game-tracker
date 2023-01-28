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
    public class StatusExtension : ExtensionBase
    {
        private UniqueID _status = UniqueID.BlankID();
        public Status Status
        {
            get
            {
                if (!_status.HasValue()) return null;
                return Util.Util.FindObjectInList(Module.GetStatusList(), _status);
            }
            set
            {
                _status = value == null ? UniqueID.BlankID() : value.UniqueID;
            }
        }

        protected new StatusExtensionModule Module => (StatusExtensionModule)base.Module;
        public new RankedObject BaseObject { get { return (RankedObject)base.BaseObject; } internal set { base.BaseObject = value; } }

        public StatusExtension(StatusExtensionModule module, Settings settings) : base(module, settings) { }

        public StatusExtension(StatusExtension copyFrom) : base(copyFrom)
        {
            _status = UniqueID.Copy(copyFrom._status);
        }

        private void OnStatusDeleted(object sender, Events.StatusDeleteArgs args)
        {
            if (_status.Equals(args.DeletedObject.UniqueID))
            {
                Status = null;
                BaseObject.SaveWithoutValidation((TrackerModule)Module.BaseModule, args.Connection);
            }
        }

        protected override void AddEventHandlers()
        {
            Module.StatusDeleted += OnStatusDeleted;
        }

        protected override void RemoveEventHandlers()
        {
            Module.StatusDeleted -= OnStatusDeleted;
        }

        public override void LoadIntoRepresentation(ref SavableRepresentation sr)
        {
            base.LoadIntoRepresentation(ref sr);
            sr.SaveValue("Status", new ValueContainer(_status));
        }

        public override void RestoreFromRepresentation(SavableRepresentation sr)
        {
            base.RestoreFromRepresentation(sr);
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
