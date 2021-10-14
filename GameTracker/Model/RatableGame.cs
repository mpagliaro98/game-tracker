using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework;
using RatableTracker.Framework.LoadSave;

namespace GameTracker.Model
{
    public class RatableGame : RatableObjectCompletable
    {
        private ObjectReference platform = new ObjectReference();
        public Platform Platform
        {
            get
            {
                return platform.HasReference() ? ((RatingModuleGame)GetParentModule()).FindPlatform(platform) : null;
            }
            set { platform.SetReference(value); }
        }

        private ObjectReference platformPlayedOn = new ObjectReference();
        public Platform PlatformPlayedOn
        {
            get
            {
                return platformPlayedOn.HasReference() ? ((RatingModuleGame)GetParentModule()).FindPlatform(platformPlayedOn) : null;
            }
            set { platform.SetReference(value); }
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

        public override SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = base.LoadIntoRepresentation();
            sr.SaveValue("platform", platform);
            sr.SaveValue("platformPlayedOn", platformPlayedOn);
            sr.SaveValue("completionCriteria", completionCriteria);
            sr.SaveValue("completionComment", completionComment);
            return sr;
        }

        public override void RestoreFromRepresentation(SavableRepresentation sr)
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

        public void RemovePlatform()
        {
            platform.ClearReference();
        }

        public void RemovePlatformPlayedOn()
        {
            platformPlayedOn.ClearReference();
        }
    }
}
