using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRater.Model
{
    class RatableGame : RatableObjectCompletable
    {
        private Platform platform;
        public Platform Platform
        {
            get { return platform; }
            set { platform = value; }
        }

        private Platform platformPlayedOn;
        public Platform PlatformPlayedOn
        {
            get { return platformPlayedOn; }
            set { platformPlayedOn = value; }
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
            base.RestoreFromRepresentation(sr);
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "platform":
                        platform = sr.GetISavable<Platform>(key);
                        break;
                    case "platformPlayedOn":
                        platformPlayedOn = sr.GetISavable<Platform>(key);
                        break;
                    case "completionCriteria":
                        completionCriteria = sr.GetValue(key);
                        break;
                    case "completionComment":
                        completionComment = sr.GetValue(key);
                        break;
                    default:
                        Console.WriteLine("RatableGame.cs RestoreFromRepresentation: unrecognized key " + key);
                        break;
                }
            }
        }
    }
}
