using RatableTracker.Rework.LoadSave;
using RatableTracker.Rework.ObjAddOns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Rework
{
    public class StatusGame : Status
    {
        public bool UseAsFinished { get; set; } = false;
        public bool ExcludeFromStats { get; set; } = false;

        public override bool HideScoreOfModelObject
        {
            get { return !UseAsFinished; }
        }

        public override bool ExcludeModelObjectFromStats
        {
            get { return ExcludeFromStats; }
        }

        public StatusGame(StatusExtensionModule module) : base(module) { }

        public override SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = base.LoadIntoRepresentation();
            sr.SaveValue("UseAsFinished", new ValueContainer(UseAsFinished));
            sr.SaveValue("ExcludeFromStats", new ValueContainer(ExcludeFromStats));
            return sr;
        }

        public override void RestoreFromRepresentation(SavableRepresentation sr)
        {
            base.RestoreFromRepresentation(sr);
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "UseAsFinished":
                        UseAsFinished = sr.GetValue(key).GetBool();
                        break;
                    case "ExcludeFromStats":
                        ExcludeFromStats = sr.GetValue(key).GetBool();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
