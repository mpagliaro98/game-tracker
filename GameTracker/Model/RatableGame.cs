using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.ObjectHierarchy;
using RatableTracker.Framework.LoadSave;
using RatableTracker.Framework.Interfaces;
using RatableTracker.Framework;

namespace GameTracker.Model
{
    public class RatableGame : RatableObjectStatusCategorical
    {
        private ObjectReference platform = new ObjectReference();
        public ObjectReference RefPlatform => platform;

        private ObjectReference platformPlayedOn = new ObjectReference();
        public ObjectReference RefPlatformPlayedOn => platformPlayedOn;

        public string CompletionCriteria { get; set; } = "";

        public string CompletionComment { get; set; } = "";

        public RatableGame() : base() { }

        public override SavableRepresentation<T> LoadIntoRepresentation<T>()
        {
            SavableRepresentation<T> sr = base.LoadIntoRepresentation<T>();
            sr.SaveValue("platform", platform);
            sr.SaveValue("platformPlayedOn", platformPlayedOn);
            sr.SaveValue("completionCriteria", CompletionCriteria);
            sr.SaveValue("completionComment", CompletionComment);
            return sr;
        }

        public override void RestoreFromRepresentation<T>(SavableRepresentation<T> sr)
        {
            base.RestoreFromRepresentation(sr);
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "platform":
                        platform = sr.GetISavable<ObjectReference>(key);
                        break;
                    case "platformPlayedOn":
                        platformPlayedOn = sr.GetISavable<ObjectReference>(key);
                        break;
                    case "completionCriteria":
                        CompletionCriteria = sr.GetString(key);
                        break;
                    case "completionComment":
                        CompletionComment = sr.GetString(key);
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine(GetType().Name + " RestoreFromRepresentation: unrecognized key " + key);
                        break;
                }
            }
        }

        public void SetPlatform<T>(T obj) where T : Platform, IReferable
        {
            platform.SetReference(obj);
        }

        public void RemovePlatform()
        {
            platform.ClearReference();
        }

        public void SetPlatformPlayedOn<T>(T obj) where T : Platform, IReferable
        {
            platformPlayedOn.SetReference(obj);
        }

        public void RemovePlatformPlayedOn()
        {
            platformPlayedOn.ClearReference();
        }
    }
}
