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
        public ObjectReference RefPlatform
        {
            get { return platform; }
        }

        private ObjectReference platformPlayedOn = new ObjectReference();
        public ObjectReference RefPlatformPlayedOn
        {
            get { return platformPlayedOn; }
        }

        private string completionCriteria = "";
        public string CompletionCriteria
        {
            get { return completionCriteria; }
            set { completionCriteria = value; }
        }

        private string completionComment = "";
        public string CompletionComment
        {
            get { return completionComment; }
            set { completionComment = value; }
        }

        public RatableGame() : base() { }

        public override SavableRepresentation<T> LoadIntoRepresentation<T>()
        {
            SavableRepresentation<T> sr = base.LoadIntoRepresentation<T>();
            sr.SaveValue("platform", platform);
            sr.SaveValue("platformPlayedOn", platformPlayedOn);
            sr.SaveValue("completionCriteria", completionCriteria);
            sr.SaveValue("completionComment", completionComment);
            return sr;
        }

        public override void RestoreFromRepresentation<T>(SavableRepresentation<T> sr)
        {
            if (sr == null) return;
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
                        completionCriteria = sr.GetString(key);
                        break;
                    case "completionComment":
                        completionComment = sr.GetString(key);
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine("RatableGame.cs RestoreFromRepresentation: unrecognized key " + key);
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
